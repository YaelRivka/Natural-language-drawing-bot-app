using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System; // נדרש עבור ArgumentNullException ו-Console

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PromptController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _geminiApiKey;

        private const string GeminiBaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

        public PromptController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _geminiApiKey = _configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini:ApiKey not found in configuration.");
        }

     
        public class PromptRequest
        {
            public string Prompt { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult<List<DrawingCommand>>> GenerateDrawing([FromBody] PromptRequest request)
        {
            var description = request.Prompt;
            if (string.IsNullOrWhiteSpace(description))
            {
                return BadRequest("תיאור הציור אינו יכול להיות ריק.");
            }

            string jsonResponse = "";
            try
            {
                // דוגמת JSON קצרה עבור DrawingCommand - חשובה כדי להדריך את ג'מיני לפורמט הפלט.
                string drawingCommandExampleJson = "[{\"Shape\": \"rectangle\", \"X\": 10, \"Y\": 20, \"Width\": 50, \"Height\": 30, \"Color\": \"red\"}]";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new
                                {
                                    // הפרומפט עודכן כדי לבקש במפורש לא לעטוף ב-Markdown
                                   text = $"Convert the following drawing description to a JSON list of DrawingCommand objects. " +
       $"DrawingCommand objects should have properties like Shape (string), X (int), Y (int), Radius (int), Width (int), Height (int), Color (string), From (int[]), To (int[]). " +
       $"For the description: '{description}'. " +
       $"Return only the JSON array, without any additional text, explanations, or Markdown code block delimiters. " +
       $"**Use only 'rectangle', 'circle','line', and 'triangle' shapes. Do not use 'path' shape.** " + // **התוספת החדשה**
       $"Example JSON output: {drawingCommandExampleJson}"
                                }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 2048
                    }
                };

                var jsonRequest = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { WriteIndented = true });
                var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                var requestUrl = $"{GeminiBaseUrl}?key={_geminiApiKey}";

                Console.WriteLine($"Sending request to URL: {requestUrl}");
                Console.WriteLine($"Request Body: {jsonRequest}");

                var response = await _httpClient.PostAsync(requestUrl, content);

                response.EnsureSuccessStatusCode();

                jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Received raw response JSON from Gemini: {jsonResponse}");

                // פרסור התשובה המלאה של ג'מיני למודל GeminiApiResponse
                var geminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (geminiResponse?.Candidates == null || geminiResponse.Candidates.Count == 0 ||
                    geminiResponse.Candidates[0].Content?.Parts == null || geminiResponse.Candidates[0].Content.Parts.Count == 0)
                {
                    return StatusCode(500, "תשובה לא צפויה מ-API של ג'מיני: לא נמצא תוכן.");
                }

                // **החלק הקריטי: חילוץ וניקוי המחרוזת של פקודות הציור**
                var rawTextFromGemini = geminiResponse.Candidates[0].Content.Parts[0].Text;

                if (string.IsNullOrWhiteSpace(rawTextFromGemini))
                {
                    return StatusCode(500, "API של ג'מיני החזיר תוכן ריק עבור פקודות הציור.");
                }

                string drawingCommandsJsonString = rawTextFromGemini.Trim();

                // הסרת קידומת Markdown: '```json' או '```'
                if (drawingCommandsJsonString.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
                {
                    drawingCommandsJsonString = drawingCommandsJsonString.Substring("```json".Length).Trim();
                }
                else if (drawingCommandsJsonString.StartsWith("```", StringComparison.OrdinalIgnoreCase)) // לטפל במקרה שבו זה רק ```
                {
                    drawingCommandsJsonString = drawingCommandsJsonString.Substring("```".Length).Trim();
                }

                // הסרת סיומת Markdown: '```'
                if (drawingCommandsJsonString.EndsWith("```", StringComparison.OrdinalIgnoreCase))
                {
                    drawingCommandsJsonString = drawingCommandsJsonString.Substring(0, drawingCommandsJsonString.Length - "```".Length).Trim();
                }

                Console.WriteLine($"Cleaned Drawing Commands JSON string for deserialization: {drawingCommandsJsonString}");

                // המרה של המחרוזת הנקייה לרשימת DrawingCommand
                var drawingCommands = JsonSerializer.Deserialize<List<DrawingCommand>>(drawingCommandsJsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Ok(drawingCommands);
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"HttpRequestException: {ex.Message}. Status Code: {ex.StatusCode}.");
                return StatusCode(500, $"שגיאה בקריאה ל-API חיצוני: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"JsonException: {ex.Message}. Problematic JSON (full raw response): {jsonResponse}");
                if (!string.IsNullOrWhiteSpace(jsonResponse))
                {
                    try
                    {
                        var tempGeminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        var extractedText = tempGeminiResponse?.Candidates?[0]?.Content?.Parts?[0]?.Text;
                        if (!string.IsNullOrWhiteSpace(extractedText))
                        {
                            Console.Error.WriteLine($"Attempted to deserialize this extracted string: {extractedText}");
                        }
                    }
                    catch { /* איגנור שגיאות, רק מנסים לקבל מידע לדיבוג */ }
                }
                return StatusCode(500, $"שגיאה בעיבוד התשובה מ-API: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"General Exception: {ex.Message}");
                return StatusCode(500, $"אירעה שגיאה כללית: {ex.Message}");
            }
        }
    }
}
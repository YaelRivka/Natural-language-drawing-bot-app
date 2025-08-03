using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System; // ���� ���� ArgumentNullException �-Console

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PromptController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _geminiApiKey;

        private const string GeminiBaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        public PromptController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _geminiApiKey = _configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini:ApiKey not found in configuration.");
        }


     

        [HttpPost]
        public async Task<ActionResult<List<DrawingCommand>>> GenerateDrawing([FromBody] PromptRequest request)
        {
            var description = request.Prompt;
            var previousDrawingJson = request.PreviousDrawing != null && request.PreviousDrawing.Count > 0
             ? JsonSerializer.Serialize(request.PreviousDrawing): "[]";
            if (string.IsNullOrWhiteSpace(description))
            {
                return BadRequest("����� ����� ���� ���� ����� ���.");
            }

            string jsonResponse = "";
            try
            {
                // ����� JSON ���� ���� DrawingCommand - ����� ��� ������ �� �'���� ������ ����.
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
                                    text = $"����� ����� ���: {previousDrawingJson}.\\n\" +\r\n      " +
                                    $" $\"���� �� ����� �����:" +
                                    $"��� �� ����� ����� ��� ������ JSON �� �������� `DrawingCommand`. �������� `DrawingCommand` ������ ����� �������� ���� `Shape` (������, ���� ����� �� 'rectangle', 'circle', 'line' �� 'triangle'), `X` (���� ���, ����� �����), `Y` (���� ���, ����� ����), `Radius` (���� ���, ����� ���� �����), `Width` (���� ���, ���� ���� ���� ������), `Height` (���� ���, ���� ���� ���� ������), `Color` (������, �� �� ���), `From` (���� �� ��� ������ ����� [X, Y] ������ ����� �� ��), `To` (���� �� ��� ������ ����� [X, Y] ������ ���� �� ��). " +
                                           $"���� ������: '{description}'. " +
                                           $"���� �� �� ���� �-JSON, ��� ���� ����, ������ �� ������ �� ���� ��� Markdown. " +
                                           $" " +
                                           $"**������ ������� ������ ������ ��������� ����������, ��� ����� �������� �����:** " +
                                           $" " +
                                           $"1.  **����� ���� ���� ����� ������ �������:** " +
                                           $"    * **��� �� ������ �� ��� ��� ��� ����� (����) ����� ����� �������������.** �� ���� �� �� �������� ����� ��� �� ����� ����. " +
                                           $"    * ���� �� ����� ������. �� ����� ����, ����� ����� ��� ���� �� ����������� X �-Y. " +
                                           $"    * **����������� X ������ ����� ����� �� �-0 �� 480, ������������ Y ����� �� �-0 �� 380.** (���� ������ ��� ����� ����� ����� ���). " +
                                           $" " +
                                           $"2.  **����� ����, ���������� ����� ��� ����:** " +
                                           $"    * **��� �� ������ �������� ��������, ������ ���� ����� ���� �����.** ������� ��� '���' ������ ����� ���� ������ (Y ���� ����, ���� �-0), '���' �� '����' ���� ������ (Y ���� ����, ���� �-380). " +
                                           $"    * **�� ������� ����� ����� �� ����� (��� ���, ��, ���, ������) ���� ����� ����� �� ����� ������ ���� �� ���� ����� ���� '�����' (������, ����).** �� ����� ���������� ���� ������, ��� �� �� �� ����� (����, ��� �� �����). " +
                                           $" " +
                                           $"3.  **���������� �����:** " +
                                           $"    * **��� ����� ������ �������������� ������� ���� ���������� ����� ����� ����� ���� ����� ����.** ������, ��� ����� ����� ����� ����� �� �� ����� ����� �� ������������. ��� ���� ����� ����� ���� ���� ��� �� ����. " +
                                           $"    * **�� ����� ����� ����� ��� ���� ����� �� ������ ��� ������� �� �� ����.** " +
                                           $" " +
                                           $"4.  **����� ����� ������:** " +
                                           $"    * **���� ���� �� ������ �������� ������ ������.** �� ������ ���� ��� ������ (������, '������ �����'), ����� ���� �� ���� �������� ��������. " +
                                           $"    * �� �� ���� ���, ��� ����� �������� ���������� �������� �������� (������, ��� ����, ��� �����). " +
                                           $" " +
                                           $"5.  **��������� �� ������� ����� (����� �� ����� ����):** " +
                                           $"    * ���� ����� ������ ����� ��� ����� ����, ��� �� ������ ���� ����� ������ ���� �������� ���� �����. ������, �� �� ��� '��� ����', ��� ����� '������ ���', ���� ���� ����� �� ���� ����� ������ ���� �� ���� ���� ����. **�� ���� �� ����� �������� ������.** " +
                                           $" " +
                                           $"6.  **������ ������� ������:** " +
                                           $"    **����� ����� �-JSON ������ ���� ��������� ������ ��� ������ ���. ���� �� �������� (X, Y) ��� ���� ���� ����� ���� �� ����� ��� ���� ������, �� ���� �� ����� ������ ������������ �� ����� ��������.** " +
                                           $" " +
                                           $"    * **����� �'���':** " +
                                           $"        ```json " +
                                           $"        [ " +
                                           $"          {{\"Shape\": \"rectangle\", \"X\": 200, \"Y\": 250, \"Width\": 100, \"Height\": 80, \"Color\": \"brown\"}}, " +
                                           $"          {{\"Shape\": \"triangle\", \"X\": 235, \"Y\": 170, \"Width\": 100, \"Height\": 80, \"Color\": \"red\"}}, " +
                                           $"          {{\"Shape\": \"rectangle\", \"X\": 235, \"Y\": 290, \"Width\": 30, \"Height\": 40, \"Color\": \"darkgray\"}}, " +
                                           $"          {{\"Shape\": \"circle\", \"X\": 270, \"Y\": 270, \"Radius\": 15, \"Color\": \"lightblue\"}} " +
                                           $"        ] " +
                                           $"        ``` " +
                                           $" " +
                                           $"    * **����� �'���':** " +
                                           $"        ```json " +
                                           $"        [ " +
                                           $"          {{\"Shape\": \"circle\", \"X\": 250, \"Y\": 200, \"Radius\": 20, \"Color\": \"peachpuff\"}}, " +
                                           $"          {{\"Shape\": \"rectangle\", \"X\": 240, \"Y\": 220, \"Width\": 20, \"Height\": 60, \"Color\": \"blue\"}}, " +
                                           $"          {{\"Shape\": \"line\", \"From\": [240, 280], \"To\": [230, 320], \"Color\": \"black\"}}, " +
                                           $"          {{\"Shape\": \"line\", \"From\": [260, 280], \"To\": [270, 320], \"Color\": \"black\"}}, " +
                                           $"          {{\"Shape\": \"line\", \"From\": [240, 230], \"To\": [210, 250], \"Color\": \"black\"}}, " +
                                           $"          {{\"Shape\": \"line\", \"From\": [260, 230], \"To\": [290, 250], \"Color\": \"black\"}} " +
                                           $"        ] " +
                                           $"        ``` " +
                                           $" " +
                                           $"    * **����� �'��':** " +
                                           $"        ```json " +
                                           $"        [ " +
                                           $"          {{\"Shape\": \"rectangle\", \"X\": 100, \"Y\": 280, \"Width\": 30, \"Height\": 70, \"Color\": \"saddlebrown\"}}, " +
                                           $"          {{\"Shape\": \"circle\", \"X\": 115, \"Y\": 250, \"Radius\": 40, \"Color\": \"forestgreen\"}} " +
                                           $"        ] " +
                                           $"        ``` " +
                                           $" " +
                                           $"    * **����� �'������':** " +
                                           $"        ```json " +
                                           $"        [ " +
                                           $"          {{\"Shape\": \"rectangle\", \"X\": 100, \"Y\": 300, \"Width\": 120, \"Height\": 40, \"Color\": \"gray\"}}, " +
                                           $"          {{\"Shape\": \"circle\", \"X\": 125, \"Y\": 340, \"Radius\": 15, \"Color\": \"black\"}}, " +
                                           $"          {{\"Shape\": \"circle\", \"X\": 195, \"Y\": 340, \"Radius\": 15, \"Color\": \"black\"}} " +
                                           $"        ] " +
                                           $"        ``` " +
                                           $" " +
                                           $"    * **����� �'���':** " +
                                           $"        ```json " +
                                           $"        [ " +
                                           $"          {{\"Shape\": \"circle\", \"X\": 400, \"Y\": 80, \"Radius\": 30, \"Color\": \"gold\"}} " +
                                           $"        ] " +
                                           $"        ``` " +
                                           $" " +
                                           $"    * **����� �'���':** " +
                                           $"        ```json " +
                                           $"        [ " +
                                           $"          {{\"Shape\": \"rectangle\", \"X\": 0, \"Y\": 350, \"Width\": 500, \"Height\": 50, \"Color\": \"limegreen\"}} " +
                                           $"        ] " +
                                           $"        ``` " +
                                           $" " +
                                           $"    * **�� �������� ������ ���� ���� ��������� �������:** " +
                                           $"        **��� �� �������� ������ �������� ����� (rectangle, circle, line, triangle). ��� ���� ����� ������, �� ���������� ������, ��������� ������ ������� �� �������� ���� ������. ���� �� ����� ���� �� ��� ����� ������ ������ �������.** " +
                                           $" " +
                                           $"����� ��� JSON �����: {drawingCommandExampleJson}"
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

                // ����� ������ ����� �� �'���� ����� GeminiApiResponse
                var geminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (geminiResponse?.Candidates == null || geminiResponse.Candidates.Count == 0 ||
                    geminiResponse.Candidates[0].Content?.Parts == null || geminiResponse.Candidates[0].Content.Parts.Count == 0)
                {
                    return StatusCode(500, "����� �� ����� �-API �� �'����: �� ���� ����.");
                }

                // **���� ������: ����� ������ ������� �� ������ �����**
                var rawTextFromGemini = geminiResponse.Candidates[0].Content.Parts[0].Text;

                if (string.IsNullOrWhiteSpace(rawTextFromGemini))
                {
                    return StatusCode(500, "API �� �'���� ����� ���� ��� ���� ������ �����.");
                }

                string drawingCommandsJsonString = rawTextFromGemini.Trim();

                // ���� ������ Markdown: '```json' �� '```'
                if (drawingCommandsJsonString.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
                {
                    drawingCommandsJsonString = drawingCommandsJsonString.Substring("```json".Length).Trim();
                }
                else if (drawingCommandsJsonString.StartsWith("```", StringComparison.OrdinalIgnoreCase)) // ���� ����� ��� �� �� ```
                {
                    drawingCommandsJsonString = drawingCommandsJsonString.Substring("```".Length).Trim();
                }

                // ���� ����� Markdown: '```'
                if (drawingCommandsJsonString.EndsWith("```", StringComparison.OrdinalIgnoreCase))
                {
                    drawingCommandsJsonString = drawingCommandsJsonString.Substring(0, drawingCommandsJsonString.Length - "```".Length).Trim();
                }

                Console.WriteLine($"Cleaned Drawing Commands JSON string for deserialization: {drawingCommandsJsonString}");

                // ���� �� ������� ������ ������ DrawingCommand
                var drawingCommands = JsonSerializer.Deserialize<List<DrawingCommand>>(drawingCommandsJsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Ok(drawingCommands);
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"HttpRequestException: {ex.Message}. Status Code: {ex.StatusCode}.");
                return StatusCode(500, $"����� ������ �-API ������: {ex.Message}");
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
                    catch { /* ������ ������, �� ����� ���� ���� ������ */ }
                }
                return StatusCode(500, $"����� ������ ������ �-API: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"General Exception: {ex.Message}");
                return StatusCode(500, $"����� ����� �����: {ex.Message}");
            }
        }
    }
}
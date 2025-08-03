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
                                    text = $"הציור הקיים הוא: {previousDrawingJson}.\\n\" +\r\n      " +
                                    $" $\"הוסף את הבקשה החדשה:" +
                                    $"המר את תיאור הציור הבא לרשימת JSON של אובייקטי `DrawingCommand`. אובייקטי `DrawingCommand` צריכים לכלול מאפיינים כגון `Shape` (מחרוזת, יכול להיות רק 'rectangle', 'circle', 'line' או 'triangle'), `X` (מספר שלם, מיקום אופקי), `Y` (מספר שלם, מיקום אנכי), `Radius` (מספר שלם, רדיוס עבור עיגול), `Width` (מספר שלם, רוחב עבור מלבן ומשולש), `Height` (מספר שלם, גובה עבור מלבן ומשולש), `Color` (מחרוזת, שם של צבע), `From` (מערך של שני מספרים שלמים [X, Y] לנקודת התחלה של קו), `To` (מערך של שני מספרים שלמים [X, Y] לנקודת סיום של קו). " +
                                           $"עבור התיאור: '{description}'. " +
                                           $"החזר רק את מערך ה-JSON, ללא טקסט נוסף, הסברים או תחומים של בלוק קוד Markdown. " +
                                           $" " +
                                           $"**הנחיות קפדניות ליצירת ציורים מציאותיים וקוהרנטיים, תוך שימוש בדוגמאות הבאות:** " +
                                           $" " +
                                           $"1.  **שימוש מרבי בשטח הקנבס ופיזור אלמנטים:** " +
                                           $"    * **פזר את הצורות על פני רוב שטח הציור (קנבס) באופן אסתטי ופרופורציונלי.** אל תרכז את כל האלמנטים בפינה אחת או במרכז בלבד. " +
                                           $"    * חשוב על הציור כמכלול. אם הקנבס גדול, השתמש בטווח רחב יותר של קואורדינטות X ו-Y. " +
                                           $"    * **קואורדינטות X צריכות להיות בטווח של כ-0 עד 480, וקואורדינטות Y בטווח של כ-0 עד 380.** (התאם טווחים אלו לגודל הקנבס בפועל שלך). " +
                                           $" " +
                                           $"2.  **מיקום לוגי, קומפוזיציה עקבית וקו קרקע:** " +
                                           $"    * **מקם את הצורות במיקומים הגיוניים, ברורים וללא חפיפה בלתי רצויה.** אלמנטים כמו 'שמש' צריכים להיות בחלק העליון (Y נמוך יותר, קרוב ל-0), 'דשא' או 'קרקע' בחלק התחתון (Y גבוה יותר, קרוב ל-380). " +
                                           $"    * **כל אובייקט שצריך להיות על הקרקע (כמו אדם, עץ, בית, מכונית) חייב להיות ממוקם כך שחלקו התחתון נוגע או נמצא בסמוך לשטח 'הקרקע' (לדוגמה, הדשא).** אל תגרום לאובייקטים לרחף באוויר, אלא אם כן זה מתאים (למשל, שמש או ציפור). " +
                                           $" " +
                                           $"3.  **פרופורציות וגודל:** " +
                                           $"    * **צור צורות בגדלים פרופורציונליים וסבירים ביחס לאובייקטים אחרים בציור וביחס לשטח הקנבס כולו.** לדוגמה, שמש צריכה להיות גדולה מספיק אך לא ענקית באופן לא פרופורציונלי. אדם צריך להיות בגובה סביר ביחס לעץ או לבית. " +
                                           $"    * **אל תיצור צורות קטנות מדי שקשה לראות או גדולות מדי שתופסות את כל המסך.** " +
                                           $" " +
                                           $"4.  **שימוש מדויק בצבעים:** " +
                                           $"    * **הקפד תמיד על הצבעים המפורטים בתיאור המשתמש.** אם המשתמש מבקש צבע ספציפי (לדוגמה, 'מכונית שחורה'), השתמש בצבע זה עבור האובייקט הרלוונטי. " +
                                           $"    * אם לא צוין צבע, בחר צבעים הגיוניים ומציאותיים המתאימים לאובייקט (לדוגמה, דשא ירוק, שמש צהובה). " +
                                           $" " +
                                           $"5.  **אינטגרציה של אלמנטים חדשים (במקרה של בקשות המשך):** " +
                                           $"    * כאשר מתבקש להוסיף אלמנט חדש לציור קיים, מקם את האלמנט החדש באופן הגיוני ביחס לאלמנטים שכבר צוירו. לדוגמה, אם יש כבר 'דשא ושמש', ואז מתבקש 'להוסיף אדם', האדם צריך לעמוד על הדשא ופונה לכיוון השמש או ביחס סביר אליה. **אל תמחק או תתעלם מאלמנטים קיימים.** " +
                                           $" " +
                                           $"6.  **ספריית דוגמאות מובנות:** " +
                                           $"    **השתמש במבנה ה-JSON המדויק עבור אובייקטים נפוצים כפי שמפורט כאן. התאם את המיקומים (X, Y) כדי למקם אותם באופן לוגי על הקנבס לפי בקשת המשתמש, אך שמור על המבנה הפנימי והפרופורציות של רכיבי האובייקט.** " +
                                           $" " +
                                           $"    * **דוגמה ל'בית':** " +
                                           $"        ```json " +
                                           $"        [ " +
                                           $"          {{\"Shape\": \"rectangle\", \"X\": 200, \"Y\": 250, \"Width\": 100, \"Height\": 80, \"Color\": \"brown\"}}, " +
                                           $"          {{\"Shape\": \"triangle\", \"X\": 235, \"Y\": 170, \"Width\": 100, \"Height\": 80, \"Color\": \"red\"}}, " +
                                           $"          {{\"Shape\": \"rectangle\", \"X\": 235, \"Y\": 290, \"Width\": 30, \"Height\": 40, \"Color\": \"darkgray\"}}, " +
                                           $"          {{\"Shape\": \"circle\", \"X\": 270, \"Y\": 270, \"Radius\": 15, \"Color\": \"lightblue\"}} " +
                                           $"        ] " +
                                           $"        ``` " +
                                           $" " +
                                           $"    * **דוגמה ל'אדם':** " +
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
                                           $"    * **דוגמה ל'עץ':** " +
                                           $"        ```json " +
                                           $"        [ " +
                                           $"          {{\"Shape\": \"rectangle\", \"X\": 100, \"Y\": 280, \"Width\": 30, \"Height\": 70, \"Color\": \"saddlebrown\"}}, " +
                                           $"          {{\"Shape\": \"circle\", \"X\": 115, \"Y\": 250, \"Radius\": 40, \"Color\": \"forestgreen\"}} " +
                                           $"        ] " +
                                           $"        ``` " +
                                           $" " +
                                           $"    * **דוגמה ל'מכונית':** " +
                                           $"        ```json " +
                                           $"        [ " +
                                           $"          {{\"Shape\": \"rectangle\", \"X\": 100, \"Y\": 300, \"Width\": 120, \"Height\": 40, \"Color\": \"gray\"}}, " +
                                           $"          {{\"Shape\": \"circle\", \"X\": 125, \"Y\": 340, \"Radius\": 15, \"Color\": \"black\"}}, " +
                                           $"          {{\"Shape\": \"circle\", \"X\": 195, \"Y\": 340, \"Radius\": 15, \"Color\": \"black\"}} " +
                                           $"        ] " +
                                           $"        ``` " +
                                           $" " +
                                           $"    * **דוגמה ל'שמש':** " +
                                           $"        ```json " +
                                           $"        [ " +
                                           $"          {{\"Shape\": \"circle\", \"X\": 400, \"Y\": 80, \"Radius\": 30, \"Color\": \"gold\"}} " +
                                           $"        ] " +
                                           $"        ``` " +
                                           $" " +
                                           $"    * **דוגמה ל'דשא':** " +
                                           $"        ```json " +
                                           $"        [ " +
                                           $"          {{\"Shape\": \"rectangle\", \"X\": 0, \"Y\": 350, \"Width\": 500, \"Height\": 50, \"Color\": \"limegreen\"}} " +
                                           $"        ] " +
                                           $"        ``` " +
                                           $" " +
                                           $"    * **אם האובייקט המבוקש אינו באחת מהדוגמאות המובנות:** " +
                                           $"        **פרק את האובייקט לצורות הבסיסיות ביותר (rectangle, circle, line, triangle). צור אותן באופן הגיוני, עם פרופורציות נכונות, ובמיקומים יחסיים שיוצרים את האובייקט השלם והמובן. הקפד על פיזור נכון על פני הקנבס ושימוש בצבעים מתאימים.** " +
                                           $" " +
                                           $"דוגמת פלט JSON כללית: {drawingCommandExampleJson}"
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
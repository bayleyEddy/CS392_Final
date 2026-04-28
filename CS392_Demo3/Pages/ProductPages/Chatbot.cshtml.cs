using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using CS392_Demo3.Models;
using CS392_Demo3.Services;
using CS392_Demo3.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CS392_Demo3.Pages.ProductPages
{
    public class ChatbotModel : PageModel
    {
        private readonly MongoDBService _mongo;
        private readonly AIService _ai;
        private readonly ILogger<ChatbotModel> _logger;

        public ChatbotModel(MongoDBService mongo, AIService ai, ILogger<ChatbotModel> logger)
        {
            _mongo = mongo;
            _ai = ai;
            _logger = logger;
        }

        [BindProperty]
        public string UserQuestion { get; set; }

        public List<ChatMessage> ChatHistory { get; set; } = new();

        public async Task OnGetAsync()
        {
            ChatHistory = HttpContext.Session.GetObject<List<ChatMessage>>("chat") ?? new();
        }

        private string ExtractJsonObject(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "{}";

            raw = raw.Replace("```json", "")
                     .Replace("```", "")
                     .Trim();

            int start = raw.IndexOf('{');
            int end = raw.LastIndexOf('}');

            if (start >= 0 && end > start)
                return raw.Substring(start, end - start + 1);

            return "{}"; // safe fallback
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ChatHistory = HttpContext.Session.GetObject<List<ChatMessage>>("chat") ?? new();

            if (string.IsNullOrWhiteSpace(UserQuestion))
            {
                ModelState.AddModelError(string.Empty, "Please enter a question.");
                return Page();
            }

            ChatHistory.Add(new ChatMessage("user", UserQuestion));

            try
            {
                // STEP 1 — Extract intent JSON
                var intentJson = await _ai.ExtractIntentAsync(UserQuestion);
                _logger.LogInformation("RAW INTENT JSON: " + intentJson);

                var cleaned = ExtractJsonObject(intentJson);

                // ⭐ Case-insensitive JSON parsing
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                ProductIntent intent = null;

                try
                {
                    intent = JsonSerializer.Deserialize<ProductIntent>(cleaned, options);

                    if (intent != null && intent.Criteria == null)
                        intent.Criteria = new ProductCriteria();
                }
                catch
                {
                    _logger.LogError("Failed to deserialize cleaned JSON: " + cleaned);
                }

                if (intent == null || intent.Criteria == null)
                {
                    ChatHistory.Add(new ChatMessage("ai",
                        "I couldn't understand your request. Try asking:\n" +
                        "• What food is in stock\n" +
                        "• Show me electronics under $500\n" +
                        "• What products are available"));

                    HttpContext.Session.SetObject("chat", ChatHistory);
                    return Page();
                }

                var results = await _mongo.FilterProductsAsync(
                    intent.Criteria.Category,
                    intent.Criteria.MaxPrice,
                    intent.Criteria.InStock
                );

                _logger.LogInformation($"FILTER CATEGORY: {intent.Criteria.Category}");
                _logger.LogInformation($"RESULT COUNT: {results.Count}");

                // STEP 3 — Generate AI answer
                var aiResponse = await _ai.GenerateProductAnswerAsync(UserQuestion, results);

                ChatHistory.Add(new ChatMessage("ai", aiResponse));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Chatbot failed.");
                ChatHistory.Add(new ChatMessage("ai", "Sorry, I couldn't process that request."));
            }

            HttpContext.Session.SetObject("chat", ChatHistory);
            return Page();
        }
    }

    public class ChatMessage
    {
        public string Role { get; set; }
        public string Text { get; set; }

        public ChatMessage() { }

        public ChatMessage(string role, string text)
        {
            Role = role;
            Text = text;
        }
    }

    public class ProductIntent
    {
        public string Action { get; set; }
        public ProductCriteria Criteria { get; set; }
    }

    public class ProductCriteria
    {
        public string Category { get; set; }
        public double? MaxPrice { get; set; }
        public bool? InStock { get; set; }
    }
}

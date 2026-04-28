using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CS392_Demo3.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace CS392_Demo3.Services
{
    public class AIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIService> _logger;
        private readonly string _apiKey;
        private readonly string _model;

        public AIService(HttpClient httpClient, IConfiguration config, ILogger<AIService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            var aiSettings = config.GetSection("AISettings");
            _apiKey = aiSettings["ApiKey"];
            _model = aiSettings["Model"] ?? "gemini-2.5-flash";

            var baseAddress = aiSettings["BaseAddress"]
                ?? "https://generativelanguage.googleapis.com/v1beta/";

            _httpClient.BaseAddress = new Uri(baseAddress);
        }


        private static string ExtractGeminiText(string respText, ILogger logger)
        {
            try
            {
                using var doc = JsonDocument.Parse(respText);

                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text ?? "";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to parse Gemini response JSON: {Response}", respText);
                return "";
            }
        }

        private string ExtractJsonObject(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return raw;
            // remove code fences
            raw = Regex.Replace(raw, @"```(?:json)?", "", RegexOptions.IgnoreCase);
            int brace = 0;
            int start = -1;
            for (int i = 0; i < raw.Length; i++)
            {
                if (raw[i] == '{') { if (start == -1) start = i; brace++; }
                else if (raw[i] == '}') { brace--; if (brace == 0 && start != -1) return raw.Substring(start, i - start + 1); }
            }
            return raw.Trim();
        }


        public async Task<string> AskGeminiAsync(string prompt)
        {
            var payload = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var endpoint = $"models/{_model}:generateContent?key={_apiKey}";

            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var resp = await _httpClient.PostAsync(endpoint, content);
            var respText = await resp.Content.ReadAsStringAsync();

            _logger.LogDebug("Gemini response (status {Status}): {Resp}", resp.StatusCode, respText);
            if (!resp.IsSuccessStatusCode) { _logger.LogError("Gemini API error: {Status}", resp.StatusCode); return respText; }

            return ExtractGeminiText(respText, _logger);
        }


        public async Task<string> ExtractIntentAsync(string question)
        {
            var prompt = $@"
Output ONLY valid JSON and NOTHING else. No explanation, no markdown, no code fences.

Example (use exactly this shape; return null for irrelevant fields):
{{
  ""action"": ""filter"",
  ""criteria"": {{
    ""category"": null,
    ""maxPrice"": null,
    ""inStock"": null
  }}
}}

    User question: {question}
";

            return await AskGeminiAsync(prompt);
        }


        public async Task<string> GenerateProductAnswerAsync(string question, List<Product> products)
        {
            var productList = JsonSerializer.Serialize(products);

            var prompt = $@"
User question: {question}

Matching products (JSON list):
{productList}

Write a clear, helpful natural-language answer for the user.
If the list is empty, say that no matching products were found.
";

            return await AskGeminiAsync(prompt);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using MoodSongRecommendations.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoodSongRecommendations.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new IndexModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetSongRecommendations(string mod)
        {
            string result;
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "API_KEY");

                var request = new OpenAIRequest
                {
                    Model = "text-davinci-003",
                    Prompt = mod + "moda en uygun şarkıları listeler misin?",
                    MaxTokens = 256,
                    Temperature = 0.7f
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.openai.com/v1/completions", content);
                var resjson = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = JsonSerializer.Deserialize<OpenAIErrorResponse>(resjson);
                    throw new System.Exception(errorResponse.Error.Message);
                }
                var data = JsonSerializer.Deserialize<OpenAIResponse>(resjson);
                result = data.choices[0].text;

            }
            catch (Exception ex)
            {
                result = $"An error occurred: {ex.Message}";
                throw;
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetYoutubeSong(string songName)
        {
            var result=Json("");
            try
            {
                HttpClient client = new HttpClient();
                var parameter = new Dictionary<string, string>
                {
                    ["key"] = "API_KEY",
                    ["q"] = songName,
                    ["part"] = "snippet",
                    ["maxResults"] = "1",
                    ["type"] = "video",
                    ["videoEmbeddable"] = "true"
                };
                var baseUrl = "https://www.googleapis.com/youtube/v3/search?";
                var url=MakeUrlWithQuery(baseUrl, parameter);
                 
                var response = await client.GetStringAsync(url);
                 if (response != null)
                {
                    var data = System.Text.Json.JsonSerializer.Deserialize<YoutubeAPIResponse>(response);
                    
                    var list = data.items.ToList();

                    result =Json(list);
                }
            }
            catch (Exception ex)
            {
                string message = $"An error occurred: {ex.Message}";
                result= Json(message);
                throw;
            }
            return Json(result);
        }

        private static string MakeUrlWithQuery(string baseUrl,
            Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException(nameof(baseUrl));

            if (parameters == null || parameters.Count() == 0)
                return baseUrl;

            return parameters.Aggregate(baseUrl,
                (accumulated, kvp) => string.Format($"{accumulated}{kvp.Key}={kvp.Value}&"));

        }
    }
}
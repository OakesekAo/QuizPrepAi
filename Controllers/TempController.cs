using QuizPrepAi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using QuizPrepAi.Models.Settings;
using Microsoft.Extensions.Options;
using OpenAI_API.Completions;
using OpenAI_API;

namespace QuizPrepAi.Controllers
{
    public class TempController : Controller
    {
        private readonly ILogger<TempController> _logger;
        private readonly AppSettings _appSettings;
        private readonly StringBuilder _quizMessage = new();

        public TempController(ILogger<TempController> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public IActionResult Index()
        {
	        return View();
        }

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
		public async Task<ActionResult> ChatGPTResponse(string userMessage) 
        {

			_quizMessage.Append($"User: {userMessage}\nChatGPT: ");
			string apiKey = _appSettings.QuizPrepAiSettings.OpenAiAPIKey;

			// api endpoint
			//string endpoint = "https://api.openai.com/v1/chat/completions";

			// setup client
			var client = new OpenAIAPI(apiKey);

			//var message = new Message() { Role = "user", Content = userMessage };//Role = "user",

			// setup request
			var requestData = new CompletionRequest()
			{
				Model = "text-davinci-002",
				MaxTokens = 1000,
				//Prompt = message.Content,
			};

			// send request
			var response = await client.Completions.CreateCompletionAsync(requestData.Prompt,requestData.Model,requestData.MaxTokens);

			if (response is null)
			{
				ErrorMessage("No response was returned by the API");
				return View("Index");
			}

			var completion = response.Completions[0];
			//var responseMessage = new Message() { Role = "ai", Content = completion.Text };

			// add response to list
			//_quizMessage.Append(responseMessage.Content.Trim());
			ViewBag.Message = _quizMessage;

			return View("Index");





			//         _quizMessage.Append($"User: {userMessage}\nChatGPT: ");
			//      string apiKey = _appSettings.QuizPrepAiSettings.OpenAiAPIKey;

			//      // api endpoint
			//      string endpoint = "https://api.openai.com/v1/chat/completions";

			//      // List of messages from api
			//      List<Message> messages = new List<Message>();

			//      // HttpClient setup
			//      var httpClient = new HttpClient();

			//      var message = new Message() {Role = "user", Content = userMessage };//Role = "user",

			//         // add message to list
			//         messages.Add(message);

			//      // setup request
			//      var requestData = new Request()
			//      {
			//       ModelId = "gpt-3.5-turbo",
			//       Messages = messages
			//      };

			//      // setup token
			//      httpClient.DefaultRequestHeaders.Clear();
			//      httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

			//// send request			
			//      using var response =  httpClient.PostAsJsonAsync(endpoint, requestData).Result;

			//      if (!response.IsSuccessStatusCode)
			//      {
			//       ErrorMessage($"{(int)response.StatusCode} {response.StatusCode}");
			//       return View("Index");
			//      }
			//ResponseData? responseData = response.Content.ReadFromJsonAsync<ResponseData>().Result;

			//var choices = responseData?.Choices ?? new List<Choice>();
			//if (choices.Count == 0)
			//{
			//	ErrorMessage("No choices were returned by the API");
			//	return View("Index");
			//}
			//var choice = choices[0];
			//var responseMessage = choice.Message;

			//// add response to list
			//messages.Add(responseMessage);
			//         _quizMessage.Append(responseMessage.Content.Trim());
			//         ViewBag.Message = _quizMessage;
			//return View("Index");
		}

		private void ErrorMessage(string error)
		{
            _quizMessage.Clear();
            _quizMessage.Append(error);
			ViewBag.Message = _quizMessage;
		}
    }
}
using QuizPrepAi.Models;
using QuizPrepAi.Models.JsonModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using QuizPrepAi.Models.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using OpenAI_API.Completions;
using OpenAI_API;

public class Temp2Controller : Controller
{
    private readonly HttpClient _httpClient;
	private readonly AppSettings _appSettings;

	public Temp2Controller(IOptions<AppSettings> appSettings)
	{

		_appSettings = appSettings.Value;
		_httpClient = new HttpClient();
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _appSettings.QuizPrepAiSettings.OpenAiAPIKey);
		_httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
	}

	public IActionResult Index()
    {
        return View(new MyViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> GenerateQuestions(MyViewModel model)
    {

        string userMessageGenerateQuestion = $"Generate 1 question for: {model.Prompt}";

        string apiKey = _appSettings.QuizPrepAiSettings.OpenAiAPIKey;
        var client = new OpenAIAPI(apiKey);

        var message = new Message() { Role = "user", Content = userMessageGenerateQuestion };//Role = "user",

        // setup request
        var requestData = new CompletionRequest()
        {
            Model = "text-davinci-002",
            MaxTokens = 1000,
            Prompt = message.Content,
        };

        // send request
        var response = await client.Completions.CreateCompletionAsync(requestData.Prompt, requestData.Model, requestData.MaxTokens);


        //string endpoint = "https://api.openai.com/v1/chat/completions";
        //var response = await _httpClient.PostAsJsonAsync(endpoint, requestBody);
        var jsonResponse = response.Completions.ToList();

        var questions = new List<Question>();

        foreach (var choice in jsonResponse)
        {
            var parts = choice.Text.TrimStart('\n');
            var questionText = parts[2];
            var correctAnswer = parts[3];
            var incorrectAnswers = parts.Skip(3).Take(4).ToList();
            var explanation = parts.Skip(7).FirstOrDefault() ?? "";

            var question = new Question
            {
                Text = questionText.TrimEnd('?'),
                CorrectAnswer = correctAnswer,
                IncorrectAnswers = incorrectAnswers,
                Explanation = explanation
            };

            questions.Add(question);
        }

        model.Questions = questions;

        return View("Index", model);
    }
}

public class MyViewModel
{
    public string Prompt { get; set; }
    public List<Question> Questions { get; set; }
}

public class Question
{
    public string Text { get; set; }
    public string CorrectAnswer { get; set; }
    public List<string> IncorrectAnswers { get; set; }
    public string Explanation { get; set; }
}

public class OpenAICompletionResponse
{
    public List<OpenAICompletionChoice> choices { get; set; }
}

public class OpenAICompletionChoice
{
public string text { get; set; }
public double? logprobs { get; set; }
public double? finish_reason { get; set; }
}

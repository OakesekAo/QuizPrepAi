using QuizPrepAi.Models;
using QuizPrepAi.Models.JsonModels;
using QuizPrepAi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using QuizPrepAi.Models.Settings;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Completions;
using System.Diagnostics;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace QuizPrepAi.Services
{
    public class QuizService : IQuizService
    {
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _httpClient;
        private readonly IQPAPIService _QPapiService;

        private readonly StringBuilder _quizMessage = new();

        public QuizService(IOptions<AppSettings> appSettings, IHttpClientFactory httpClient, IQPAPIService qPapiService)
        {
            _appSettings = appSettings.Value;
            _httpClient = httpClient;
            _QPapiService = qPapiService;
        }

        public async Task<QuizModel> GenerateQuiz(string topic)
        {
            var questions = await GenerateQuestions(topic);
            var quiz = new QuizModel
            {
                Questions = questions
            };
            return quiz;
        }


        private async Task<List<QuestionModel>> GenerateQuestions(string topic)
        {
            var prompt = $"Generate {20} multiple-choice questions on the topic of {topic}";
            var apiResponse = await _QPapiService.GenerateContent(prompt);
            var questionPrompts = ExtractQuestionPrompts(apiResponse);
            var questions = new List<QuestionModel>();
            foreach (var questionPrompt in questionPrompts)
            {
                var incorrectAnswers = await GenerateIncorrectAnswers(questionPrompt);
                var correctAnswer = await GenerateCorrectAnswer(questionPrompt);
                var question = new QuestionModel
                {
                    Prompt = questionPrompt,
                    Answers = ShuffleAnswers(new List<string> { correctAnswer }.Concat(incorrectAnswers).ToList()),
                    CorrectAnswer = correctAnswer
                };
                questions.Add(question);
            }
            return questions;
        }





        public async Task<ICollection<string>> GetExplanation(string question)
        {
            var prompt = $"Provide an explanation for the following question: {question}\nExplanation:";
            var apiResponse = await _QPapiService.GenerateContent(prompt);
            var answers = ExtractAnswers(apiResponse);
            if (answers.Count > 0)
            {
                return new List<string> { answers[0] };
            }
            else
            {
                throw new Exception("Failed to generate correct answer");
            }
        }

        public async Task<ICollection<string>> GetStudyGuide(string question)
        {
            var prompt = $"Provide a study guide for the following question: {question}\nStudy guide:";
            var apiResponse = await _QPapiService.GenerateContent(prompt);
            var answers = ExtractAnswers(apiResponse);
            if (answers.Count > 0)
            {
                return new List<string> { answers[0] };
            }
            else
            {
                throw new Exception("Failed to generate correct answer");
            }
        }





        private async Task<List<string>> GenerateIncorrectAnswers(string questionPrompt)
        {
            var prompt = $"Generate {3} incorrect answers for the question: {questionPrompt}.";
            var apiResponse = await _QPapiService.GenerateContent(prompt);
            return ExtractAnswers(apiResponse);
        }

        private async Task<string> GenerateCorrectAnswer(string questionPrompt)
        {
            var prompt = $"Generate a correct answer for the question: {questionPrompt}.";
            var apiResponse = await _QPapiService.GenerateContent(prompt);
            var answers = ExtractAnswers(apiResponse);
            if (answers.Count > 0)
            {
                return answers[0];
            }
            else
            {
                throw new Exception("Failed to generate correct answer");
            }

        }

        //public async Task<string> GenerateText(string prompt)
        //{
        //    var messages = new List<Message>
        //        {
        //            new Message {Role = "user", Content = prompt}
        //        };

        //    var client = new OpenAIAPI(_appSettings.QuizPrepAiSettings.OpenAiAPIKey);

        //    var requestData = new Request
        //    {
        //        ModelId = "text-ada-001",
        //        Messages = messages
        //    };

        //    try
        //    {
        //        var response = await client.Completions.CreateCompletionAsync(
        //            new CompletionRequest(requestData.Messages.ToString(), requestData.ModelId));

        //        //var responseData = await response.Completions.;

        //        if (response?.Completions?.Count > 0)
        //        {
        //            var choice = response.Completions[0];
        //            return choice.Text;
        //        }

        //        // No response from the API
        //        // ErrorMessage("No response was returned by the API");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle errors or exceptions
        //        // ErrorMessage(ex.Message);
        //    }

        //    return null;
        //}


        //public async Task<string> GenerateText(string prompt)
        //{


        //    //Assemble the full request uri string
        //    var query = $"{_appSettings.OpenAiSettings.BaseUrl}/completions/";

        //    var queryParams = new Dictionary<string, string>()
        //    {
        //        {"api_key", _appSettings.QuizPrepAiSettings.OpenAiAPIKey },
        //        {"prompt", prompt }
        //    };

        //    var requestUri = QueryHelpers.AddQueryString(query, queryParams);

        //    using var client = _httpClient.CreateClient();
        //    var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        //    var response = await client.SendAsync(request);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var result = await response.Content.ReadAsStringAsync();
        //        return result;
        //    }
        //    else
        //    {
        //        throw new Exception("Failed to generate text");
        //    }
        //}


        //private async Task<string> GenerateText2(string prompt)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_appSettings.QuizPrepAiSettings.OpenAiAPIKey}");
        //        client.DefaultRequestHeaders.Add("Access-Control-Request-Method", "POST");


        //        var response = await client.PostAsync("https://api.chatgpt.com/v1/generate", new StringContent($"{{\"prompt\": \"{prompt}\"}}"));

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var result = await response.Content.ReadAsStringAsync();
        //            return result;
        //        }
        //        else
        //        {
        //            throw new Exception("Failed to generate text");
        //        }
        //    }
        //}

        private List<string> ExtractQuestionPrompts(string apiResponse)
        {
            //need to check string format and find out what to use to strip it
            var responseJson = JsonConvert.DeserializeObject<ChatGptApiResponse>(apiResponse);
            var questionPrompts = new List<string>();
            foreach (var completion in responseJson.Completions)
            {
                var text = completion.Text.Trim();
                if (text.EndsWith("?"))
                {
                    questionPrompts.Add(text);
                }
            }
            return questionPrompts;
        }

        private List<string> ExtractAnswers(string apiResponse)
        {
            var responseJson = JsonConvert.DeserializeObject<ChatGptApiResponse>(apiResponse);
            var answers = new List<string>();
            foreach (var completion in responseJson.Completions)
            {
                var text = completion.Text.Trim();
                if (!string.IsNullOrEmpty(text))
                {
                    answers.Add(text);
                }
            }
            return answers;
        }


        private List<string> ShuffleAnswers(List<string> answers)
        {
            var rng = new Random();
            int n = answers.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = answers[k];
                answers[k] = answers[n];
                answers[n] = value;
            }
            return answers;
        }



        public class ChatGptApiResponse
        {
            public List<ChatGptCompletion> Completions { get; set; }
        }

        public class ChatGptCompletion
        {
            public string Text { get; set; }
        }


    }
}

﻿using QuizPrepAi.Models;
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

namespace QuizPrepAi.Services
{
    public class QuizService : IQuizService
    {
        private AppSettings _appSettings;
        private readonly IHttpClientFactory _httpClient;

        public QuizService(IOptions<AppSettings> appSettings, IHttpClientFactory httpClient)
        {
            _appSettings = appSettings.Value;
            _httpClient = httpClient;
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
        public async Task<ICollection<string>> GetExplanation(string question)
        {
            var prompt = $"Provide an explanation for the following question: {question}\nExplanation:";
            var apiResponse = await GenerateText(prompt);
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
            var apiResponse = await GenerateText(prompt);
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




        private async Task<List<QuestionModel>> GenerateQuestions(string topic)
        {
            var prompt = $"Generate {20} multiple-choice questions on the topic of {topic}";
            var apiResponse = await GenerateText(prompt);
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

        private async Task<List<string>> GenerateIncorrectAnswers(string questionPrompt)
        {
            var prompt = $"Generate {3} incorrect answers for the question: {questionPrompt}.";
            var apiResponse = await GenerateText(prompt);
            return ExtractAnswers(apiResponse);
        }

        private async Task<string> GenerateCorrectAnswer(string questionPrompt)
        {
            var prompt = $"Generate a correct answer for the question: {questionPrompt}.";
            var apiResponse = await GenerateText(prompt);
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

        public async Task<string> GenerateText(string prompt)
        {
            var client = new OpenAIAPI(_appSettings.QuizPrepAiSettings.OpenAiAPIKey);

            var completionsRequest = new CompletionRequest
            {
                Model = _appSettings.OpenAiSettings.ModelId,
                Prompt = prompt,
                MaxTokens = _appSettings.OpenAiSettings.MaxTokens,
                Temperature = _appSettings.OpenAiSettings.Temperature,
                TopP = _appSettings.OpenAiSettings.TopP
            };

            try
            {
                var completions = await client.Completions.CreateCompletionAsync(completionsRequest.Prompt,completionsRequest.Model);
   
                // var quiz = new QuizModel
                // {
                //     Questions = new List<QuestionModel>
                //     {
                //         new QuestionModel { CorrectAnswer = completions.Completions[0].Text }
                //     }
                // };
                

                //not enough reponses for quiz...?
                return completions.Completions[0].ToString();
            }
                

            catch (Exception ex)
            {
                throw new Exception("Failed to generate text", ex);
            }
        }


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
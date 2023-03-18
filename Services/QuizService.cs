﻿using QuizPrepAi.Models;
using QuizPrepAi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QuizPrepAi.Services
{
    public class QuizService : IQuizService
    {
        private readonly string _chatGPTApiKey;

        public QuizService(string chatGPTApiKey)
        {
            _chatGPTApiKey = chatGPTApiKey;
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
            if(answers.Count > 0)
            {
                return answers[0];
            }
            else
            {
                throw new Exception("Failed to generate correct answer");
            }

        }

        private async Task<string> GenerateText(string prompt)
        {
            using(var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_chatGPTApiKey}");


                var response = await client.PostAsync("https://api.chatgpt.com/v1/generate", new StringContent($"{{\"prompt\": \"{prompt}\"}}"));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    throw new Exception("Failed to generate text");
                }
            }
        }

        private List<string> ExtractQuestionPrompts(string apiResponse)
        {
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

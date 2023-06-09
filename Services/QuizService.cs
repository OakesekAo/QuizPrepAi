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
using System.Diagnostics;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuizPrepAi.Data;

namespace QuizPrepAi.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _httpClient;
        private readonly IQPAPIService _QPapiService;

        private readonly StringBuilder _quizMessage = new();

        public QuizService(IOptions<AppSettings> appSettings, IHttpClientFactory httpClient, IQPAPIService qPapiService, ApplicationDbContext context)
        {
            _appSettings = appSettings.Value;
            _httpClient = httpClient;
            _QPapiService = qPapiService;
            _context = context;
        }

        public async Task<Quiz> GenerateQuiz(string topic)
        {
            Quiz quiz = new Quiz(); // Initialize the QuizModel object

            var questionBlob = await GenerateQuizBlob(topic);

            quiz = ParseQuestions(questionBlob);

            return quiz;
        }

        public Quiz ParseQuestions(string questionBlob)
        {
            // Initialize the list of questions
            List<Question> questions = new List<Question>();

            // Split the questionBlob into individual questions
            string[] questionStrings = questionBlob.Split(new string[] { "\r\n\n\n", "\n\n" }, StringSplitOptions.None);


            foreach (string questionString in questionStrings)
            {
                // Skip empty entries
                if (string.IsNullOrWhiteSpace(questionString)) continue;

                // Extract the question, answer options, and correct answer
                string[] lines = questionString.Split("\n");
                //string question = lines[0].Substring(3); // Remove "Q1. ", "Q2. ", etc.
                string question = lines[0];
                if (question.Length > 3)
                {
                    question = question.Substring(3); // Remove "Q1. ", "Q2. ", etc.
                }
                else
                {
                    question = ""; // Or whatever default value you prefer
                }

                List<string> answers = new List<string>();
                string correctAnswer = "";
                foreach (string line in lines.Skip(1))
                {
                    if (line.StartsWith("A. "))
                    {
                        correctAnswer = line.Substring(3);
                        answers.Add(correctAnswer);
                    }
                    else if (line.StartsWith("B. ")) answers.Add(line.Substring(3));
                    else if (line.StartsWith("C. ")) answers.Add(line.Substring(3));
                    else if (line.StartsWith("D. ")) answers.Add(line.Substring(3));
                    else if (line.StartsWith("Correct Answer: ")) correctAnswer = line.Substring(17);
                }


                // Shuffle the answers
                answers = ShuffleAnswers(answers);

                // Create a new QuestionModel and add it to the list of questions
                Question questionModel = new Question
                {
                    SingleQuestion = question,
                    Answers = answers,
                    CorrectAnswer = correctAnswer
                };
                questions.Add(questionModel);
            }

            // Update the total number of questions
            int totalQuestions = questions.Count;

            // Create a new QuizModel and return it
            Quiz quizModel = new Quiz
            {
                //Topic = "",--
                Question = questions,
                TotalQuestions = totalQuestions
            };
            return quizModel;
        }







        private async Task<string> GenerateQuizBlob(string topic)
        {
            Question questions = new();
            var prompt = $"Generate 3 questions on the topic of {topic} with one correct answer and three incorrect answers in the following format: \n\nQuiz question. \n the first answer. this will be the correct answer(do not include this sentence). \n second answer, this will be an incorrect answer(do not include this sentence).\n third answer, this will be an incorrect answer(do not include this sentence). \n fourth answer, this will be an incorrect answer(do not include this sentence).\n\n Use that format for all 5 questions";
            var apiResponse = await _QPapiService.GenerateContent(prompt);
            return apiResponse;
        }

        //private async Task<string> GenerateQuizBlob(string topic)
        //{
        //    var tasks = new List<Task<string>>();
        //    for (int i = 0; i < 5; i++)
        //    {
        //        tasks.Add(GenerateQuizQuestions(topic));
        //    }

        //    await Task.WhenAll(tasks);

        //    var quizBlobBuilder = new StringBuilder();
        //    foreach (var task in tasks)
        //    {
        //        var result = task.Result.Replace("\r\n\n\n", "\n\n"); // replace "\r\n\n\n" with "\n\n"
        //        quizBlobBuilder.AppendLine(result);
        //    }

        //    return quizBlobBuilder.ToString();
        //}

        //private async Task<string> GenerateQuizQuestions(string topic)
        //{
        //    Question questions = new();
        //    var prompt = $"Generate one question on the topic of {topic} with one correct answer and three incorrect answers in the following format: \n\nQuiz question. \n the first answer. this will be the correct answer(do not include this sentence). \n second answer, this will be an incorrect answer(do not include this sentence).\n third answer, this will be an incorrect answer(do not include this sentence). \n fourth answer, this will be an incorrect answer(do not include this sentence).\n\n Use that format for all 5 questions";
        //    var apiResponse = await _QPapiService.GenerateContent(prompt);
        //    return apiResponse;
        //}





        private async Task<string> GenerateQuestions(string topic)
        {
            Question questions = new();
            var prompt = $"Generate 5 questions on the topic of {topic}";
            var apiResponse = await _QPapiService.GenerateContent(prompt); 
            return apiResponse;
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
            var prompt = $"Generate one correct answer for the question: {questionPrompt}.";
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


        private List<string> ExtractQuestionPrompts(string apiResponse)
        {
            // Split the response string into an array of strings
            string[] prompts = apiResponse.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Remove any leading numbers and dots from each question prompt
            for (int i = 0; i < prompts.Length; i++)
            {
                prompts[i] = prompts[i].Substring(prompts[i].IndexOf('.') + 2);
            }

            // Return the list of question prompts
            return prompts.ToList();
        }

        private List<string> ExtractAnswers(string apiResponse)
        {
            // Split the response string into an array of strings
            string[] answers = apiResponse.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Remove any leading or trailing whitespace from each answer
            for (int i = 0; i < answers.Length; i++)
            {
                answers[i] = answers[i].Trim();
            }

            // Return the list of answers
            return answers.ToList();
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

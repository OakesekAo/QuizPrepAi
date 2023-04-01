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
            QuizModel quiz = new QuizModel(); // Initialize the QuizModel object

            var questionBlob = await GenerateQuestions(topic);
            List<string> questionPrompts = ExtractQuestionPrompts(questionBlob);

            foreach (string prompt in questionPrompts)
            {
                // Create a new QuestionModel object for each question
                QuestionModel question = new QuestionModel();

                // Set the question prompt
                question.Question = prompt;

                // Get the correct answer for the question
                string correctAnswer = await GenerateCorrectAnswer(prompt);
                question.CorrectAnswer = correctAnswer;

                // Get a list of incorrect answers for the question
                List<string> incorrectAnswers = await GenerateIncorrectAnswers(prompt);
                question.Answers = incorrectAnswers;
                question.Answers.Insert(0, correctAnswer); // Add the correct answer to the list of possible answers

                // Add the question to the quiz
                quiz.Questions.Add(question);
            }

            return quiz;
        }



        private async Task<string> GenerateQuestions(string topic)
        {
            QuestionModel questions = new();
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

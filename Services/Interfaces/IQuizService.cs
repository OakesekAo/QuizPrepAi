﻿using QuizPrepAi.Models;

namespace QuizPrepAi.Services.Interfaces
{
    public interface IQuizService
    {
        public Task<QuizModel> GenerateQuiz(string topic);
        public Task<ICollection<string>> GetStudyGuide(string question);
        public Task<ICollection<string>> GetExplanation(string question);
    }
}

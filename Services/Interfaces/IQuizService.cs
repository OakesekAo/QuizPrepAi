using QuizPrepAi.Models;

namespace QuizPrepAi.Services.Interfaces
{
    public interface IQuizService
    {
        Task<QuizModel> GenerateQuiz(string topic);
        public Task<string> GetStudyGuide(string question);
        public Task<string> GetExplanation(string question);
    }
}

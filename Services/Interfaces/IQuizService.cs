using QuizPrepAi.Models;

namespace QuizPrepAi.Services.Interfaces
{
    public interface IQuizService
    {
        Task<QuizModel> GenerateQuiz(string topic);
    }
}

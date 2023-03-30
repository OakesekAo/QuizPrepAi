using QuizPrepAi.Models;

namespace QuizPrepAi.Services.Interfaces
{
    public interface IQPAPIService
    {
        public Task<string> GenerateContent(string query);
    }
}

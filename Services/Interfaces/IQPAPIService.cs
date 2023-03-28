using QuizPrepAi.Models;

namespace QuizPrepAi.Services.Interfaces
{
    public interface IQPAPIService
    {
        public Task<List<string>> GenerateContent(QPRequestModel generateRequestModel);
    }
}

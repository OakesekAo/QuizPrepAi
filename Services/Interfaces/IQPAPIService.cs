namespace QuizPrepAi.Services.Interfaces
{
    public interface IQPAPIService
    {
        public Task<List<string>> GenerateContent(AIGenerateRequestModel generateRequestModel)
    }
}

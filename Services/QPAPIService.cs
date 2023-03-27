using Microsoft.Extensions.Options;
using OpenAI_API;
using QuizPrepAi.Models;
using QuizPrepAi.Models.Settings;
using QuizPrepAi.Services.Interfaces;
using System.Text;

namespace QuizPrepAi.Services
{
    public class QPAPIService : IQPAPIService
    {

        private readonly AppSettings _appSettings;

        public QPAPIService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<List<string>> GenerateContent(QPRequestModel generateRequestModel)
        {
            var apiKey = _appSettings.QuizPrepAiSettings.OpenAiAPIKey;
            var apiModel = _appSettings.OpenAiSettings.ModelId;

            List<string> resultList = new List<string>();
            string returnString = "";
            OpenAIAPI api = new OpenAIAPI(new APIAuthentication(apiKey));
            var completionRequest = new OpenAI_API.Completions.CompletionRequest()
            {
                Prompt = generateRequestModel.Prompt,
                Model = apiModel,
                Temperature = 0.5,
                MaxTokens = 300,
                TopP = 1.0,
                FrequencyPenalty = 0.0,
                PresencePenalty = 0.0,
                NumChoicesPerPrompt = 1

            };
            var result = await api.Completions.CreateCompletionsAsync(completionRequest, 1);
            foreach (var choice in result.Completions)
            {
                returnString = choice.Text;
                resultList.Add(choice.Text);
            }
            return resultList;
        }
    }
}

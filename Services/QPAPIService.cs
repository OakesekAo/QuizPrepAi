using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Completions;
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

        public async Task<string> GenerateContent(string query)
        {
            //init result
            string outputResult = "";
            var apiKey = _appSettings.QuizPrepAiSettings.OpenAiAPIKey;
            var apiModel = _appSettings.OpenAiSettings.ModelId;


            OpenAIAPI api = new OpenAIAPI(new APIAuthentication(apiKey));

            CompletionRequest completionRequest = new CompletionRequest()
            {
                user = "user",
                Prompt = query,
                Model = apiModel,
                Temperature = 0.5,
                MaxTokens = 300,
                TopP = 1.0,
                FrequencyPenalty = 0.0,
                PresencePenalty = 0.0,
                NumChoicesPerPrompt = 1

            };


            var completions = await api.Completions.CreateCompletionsAsync(completionRequest, 1);
            foreach (var completion in completions.Completions)
            {
                outputResult += completion.Text;
            }

            return outputResult;
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Completions;
using OpenAI_API.Models;
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
            var apiKey = _appSettings.OpenAiAPIKey;




            OpenAIAPI api = new OpenAIAPI(new APIAuthentication(apiKey));
            //var result = await api.Completions.CreateCompletionAsync("One Two Three One Two", temperature: 0.1);

            CompletionRequest completionRequest = new CompletionRequest()
            {
                user = "user",
                Prompt = query,
                Temperature = 0.1,
                MaxTokens = 2000,
                TopP = 0.1,
                FrequencyPenalty = 0.0,
                PresencePenalty = 0.0,
                NumChoicesPerPrompt = 1

            };


            var completions = await api.Completions.CreateCompletionAsync(completionRequest);
            foreach (var completion in completions.Completions)
            {
                outputResult += completion.Text;
            }

            return outputResult;
        }
    }
}

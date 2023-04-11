namespace QuizPrepAi.Models.Settings
{
    public class AppSettings
    {
        public string OpenAiAPIKey { get; set; } 
        public QuizPrepAiSettings QuizPrepAiSettings { get; set; }
        public OpenAiSettings OpenAiSettings { get; set; }

        // Parameterless constructor required for dependency injection
        //public AppSettings()
        //{
        //}

        //public AppSettings(IConfiguration configuration)
        //{
        //    // Check if the OpenAI API key environment variable is set
        //    var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        //    if (!string.IsNullOrEmpty(openAiApiKey))
        //    {
        //        // Use the environment variable value if it is set
        //        OpenAiAPIKey = openAiApiKey;
        //    }
        //    else
        //    {
        //        // Otherwise, use the value from the secrets.json file
        //        OpenAiAPIKey = configuration.GetValue<string>("OpenAiAPIKey");
        //    }

        //    // Initialize other settings properties
        //    QuizPrepAiSettings = configuration.GetSection("QuizPrepAiSettings").Get<QuizPrepAiSettings>();
        //    OpenAiSettings = configuration.GetSection("OpenAiSettings").Get<OpenAiSettings>();
        //}
    }
}

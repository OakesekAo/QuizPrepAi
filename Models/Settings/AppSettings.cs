namespace QuizPrepAi.Models.Settings
{
    public class AppSettings
    {
        public string OpenAiAPIKey { get; set; }
        public QuizPrepAiSettings QuizPrepAiSettings { get; set; }
        public OpenAiSettings OpenAiSettings { get; set; }
    }
}

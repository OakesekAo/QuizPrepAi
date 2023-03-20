namespace QuizPrepAi.Models.Settings
{
    public class OpenAiSettings
    {
        public string BaseUrl { get; set; }
    }

    public class QueryOPtions
    {
        public string Language { get; set; }
        public string AppendToResponse { get; set; }
    }
}

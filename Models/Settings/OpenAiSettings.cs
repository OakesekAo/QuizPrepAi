﻿namespace QuizPrepAi.Models.Settings
{
    public class OpenAiSettings
    {
        public string? BaseUrl { get; set; }
        public string? ModelId { get; set; }
        public int MaxTokens { get; set; }
        public double Temperature { get; set; }
        public double TopP { get; set; }
    }

    public class QueryOPtions
    {
        public string Language { get; set; }
        public string AppendToResponse { get; set; }
            
    }
}

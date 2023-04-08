using Microsoft.Extensions.Primitives;

namespace QuizPrepAi.Models
{
    public class Question
    {
        //This needs to hold question, answer and possible inccornct answers, linked to explaination model possibly
        public int Id { get; set; }
        public string? QPUserId { get; set; }
        public string SingleQuestion { get; set; }
        //question answers from API
        public List<string> Answers { get; set; }
        public string CorrectAnswer { get; set; }

        //moved to own model
        //public StringValues UserAnswer { get; internal set; }
        //virtuals
        public virtual QPUser? QPUser { get; set; }
    }
}

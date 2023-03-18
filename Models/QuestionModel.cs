namespace QuizPrepAi.Models
{
    public class QuestionModel
    {
        public string Prompt { get; set; }
        public List<string> Answers { get; set; }
        public string CorrectAnswer { get; set; }
    }
}

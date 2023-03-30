namespace QuizPrepAi.Models
{
    public class QuestionModel
    {
        //This needs to hold question, answer and possible inccornct answers, linked to explaination model possibly
        public string Prompt { get; set; }
        public List<string> Answers { get; set; }
        public string CorrectAnswer { get; set; }
    }
}

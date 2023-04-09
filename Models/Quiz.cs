namespace QuizPrepAi.Models
{
    public class Quiz
    {
        public int Id { get; set; }

        public string? QPUserId { get; set; }

        public string Topic { get; set; }
        //public List<Question> SingleQuestionId { get; set; }
        public int TotalQuestions { get; set; } 
        public int CorrectAnswers { get; set; }
        //public List<UserAnswer> AnswerId { get; set; }

        //virtuals
        public virtual QPUser? User { get; set; }
        public virtual List<UserAnswer> SelectedAnswer { get; set; }
        public virtual List<Question> Question { get; set;}

    }
}

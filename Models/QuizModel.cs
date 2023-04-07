namespace QuizPrepAi.Models
{
    public class QuizModel
    {
        public int Id { get; set; }

        public string? QPUserId { get; set; }

        public string Topic { get; set; }
        public List<QuestionModel> Questions { get; set; }
        public int TotalQuestions { get; set; } 
        public int CorrectAnswers { get; set; }
        public List<string> UserAnswers { get; set; }

        //virtuals
        public virtual QPUser? QPUser { get; set; }
        public virtual QuestionModel Question { get; set;}

    }
}

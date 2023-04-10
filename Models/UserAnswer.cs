namespace QuizPrepAi.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public string? QPUserId { get; set; }
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public string SelectedAnswer { get; set; }
        //virtuals
        public virtual Quiz Quiz { get; set; }
        public virtual Question Question { get; set; }
        public virtual QPUser? User { get; set; }
    }
}

namespace QuizPrepAi.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public string? QPUserId { get; set; }
        public string Answer { get; set; }
        //virtuals
        public virtual QPUser? QPUser { get; set; }
    }
}

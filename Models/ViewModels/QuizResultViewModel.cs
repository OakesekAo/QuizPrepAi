namespace QuizPrepAi.Models.ViewModels
{
    public class QuizResultViewModel
    {
        public string QuizTitle { get; set; }
        public int TotalQuestions { get; set; }
        public int Score { get; set; }
        public IList<UserAnswer> UserAnswers { get; set; }
    }
}

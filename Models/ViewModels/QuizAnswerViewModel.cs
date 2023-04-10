namespace QuizPrepAi.Models.ViewModels
{
    public class QuizAnswerViewModel
    {
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public IList<string> Answers { get; set; }
        public int TotalQuestions { get; set; }
        public int QuestionNumber { get; set; }
        public string SelectedAnswer { get; set; }
    }
}

namespace QuizPrepAi.Models
{
    public class QuizModel
    {
        public string Topic { get; set; }
        public List<QuestionModel> Questions { get; set; }
        public int TotalQuestions { get; set; } 
        public int CorrectAnswers { get; set; }
        public List<string> UserAnswers { get; set; }

        //public QuizModel()
        //{
        //    Questions = new List<QuestionModel>();
        //    _totalQuestions = Questions.Count;
        //    _correctAnswers = 0;
        //}


    }
}

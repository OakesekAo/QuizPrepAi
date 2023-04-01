namespace QuizPrepAi.Models
{
    public class QuizModel
    {
        private int _totalQuestions;
        private int _correctAnswers;

        public List<QuestionModel> Questions { get; set; }
        public int TotalQuestions { get { return _totalQuestions; } }
        public int CorrectAnswers { get { return _correctAnswers; } }
        public List<string> UserAnswers { get; set; }

        public QuizModel()
        {
            Questions = new List<QuestionModel>();
            _totalQuestions = Questions.Count;
            _correctAnswers = 0;
        }

        private int CalculateScore()
        {
            if (Questions == null || UserAnswers == null || Questions.Count != UserAnswers.Count)
            {
                return 0;
            }

            int score = 0;

            for (int i = 0; i < Questions.Count; i++)
            {
                if (Questions[i].CorrectAnswer == UserAnswers[i])
                {
                    score++;
                }
            }

            return score;
        }

        public void ValidateAnswers()
        {
            if (Questions != null && UserAnswers != null && Questions.Count == UserAnswers.Count)
            {
                for (int i = 0; i < Questions.Count; i++)
                {
                    Questions[i].UserAnswer = UserAnswers[i];
                }
            }
        }

    }
}

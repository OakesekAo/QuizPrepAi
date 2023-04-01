using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizPrepAi.Models;
using QuizPrepAi.Services.Interfaces;

namespace QuizPrepAi.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string quizText)
        {

            var quiz = await _quizService.GenerateQuiz(quizText);
            return View(quiz);

            //var quiz = _quizService.GenerateQuiz(quizText);
            //ViewBag.Answer = quiz;
            //ViewBag.Text = quizText;
            //return View();
        }

        [HttpPost]
        public IActionResult Quiz(QuizModel quiz)
        {
            // Validate the user's answers and calculate the results
            int correctAnswers = 0;
            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                var question = quiz.Questions[i];
                var userAnswer = Request.Form["answer-" + i];
                question.UserAnswer = userAnswer;

                if (userAnswer == question.CorrectAnswer)
                {
                    correctAnswers++;
                }
            }

            quiz.ValidateAnswers();

            // Redirect to the results page
            return RedirectToAction("Results", quiz);
        }

        [HttpGet]
        public IActionResult Results(QuizModel quiz)
        {
            return View(quiz);
        }



        [HttpGet]
        public IActionResult Explanation(int questionId)
        {
            //TODO fix to string
            string question = questionId.ToString();
            // Call the service to get the explanation for the given question id
            var explanation = _quizService.GetExplanation(question);
            // Return the explanation as JSON
            return Json(new { explanation });
        }

        [HttpGet]
        public IActionResult StudyGuide(int questionId)
        {
            //TODO fix to string
            string question = questionId.ToString();
            // Call the service to get the study guide for the given question id
            var studyGuide = _quizService.GetStudyGuide(question);

            // Return the study guide as JSON
            return Json(new { studyGuide });
        }

    }


}

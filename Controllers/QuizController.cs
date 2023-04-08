using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        //[HttpPost]
        //public async Task<IActionResult> Index(string quizText)
        //{

        //    return RedirectToAction("Quiz");

        //    //var quiz = _quizService.GenerateQuiz(quizText);
        //    //ViewBag.Answer = quiz;
        //    //ViewBag.Text = quizText;
        //    //return View();
        //}

        [HttpPost]
        public async Task<IActionResult> Quiz(string quizText)
        {
            var quiz = await _quizService.GenerateQuiz(quizText);

            // TODO See if this still needs to be calculated
            //quiz.TotalQuestions = quiz.Questions.Count;
            return View(quiz);
        }

        //[HttpGet]
        public IActionResult Results(string quizModel, Quiz quiz)
        {
            quiz = JsonConvert.DeserializeObject<Quiz>(quizModel);
            //refactor
            //quiz.CorrectAnswers = quiz.TotalQuestions.Count(q => q.UserAnswer == q.CorrectAnswer);
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

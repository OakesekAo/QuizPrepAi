using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizPrepAi.Models;
using QuizPrepAi.Services.Interfaces;

namespace QuizPrepAi.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;
        private readonly IQPAPIService _openaiService;

        public QuizController(IQuizService quizService, IQPAPIService openaiService)
        {
            _quizService = quizService;
            _openaiService = openaiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string quizText)
        {

            var quiz = _openaiService.GenerateContent(quizText);
            ViewBag.Answer = quiz;
            ViewBag.Text = quizText;
            return View();
        }

        [HttpPost]
        public IActionResult Quiz(QuizModel quiz)
        {
            // Validate the user's answers and calculate the results
            // ...

            // Redirect to the results page
            return RedirectToAction("Results");
        }

        [HttpGet]
        public IActionResult Results()
        {
            return View();
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

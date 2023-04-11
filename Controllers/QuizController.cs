using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuizPrepAi.Data;
using QuizPrepAi.Models;
using QuizPrepAi.Models.ViewModels;
using QuizPrepAi.Services.Interfaces;

namespace QuizPrepAi.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<QPUser> _userManager;

        public QuizController(IQuizService quizService, ApplicationDbContext context, UserManager<QPUser> userManager)
        {
            _quizService = quizService;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetupQuiz(string quizText)
        {


            var quiz = await _quizService.GenerateQuiz(quizText);
            quiz.Topic = quizText;
            // save quiz to the DB
            _context.Quiz.Add(quiz);
            await _context.SaveChangesAsync();

            // send first question with possible answers to the view
            var questionIndex = 0;
            var question = quiz.Question[questionIndex];
            var model = new QuizAnswerViewModel
            {
                QuizId = quiz.Id,
                QuestionId = question.Id,
                Question = question.SingleQuestion,
                Answers = question.Answers,
                TotalQuestions = quiz.TotalQuestions,
                QuestionNumber = questionIndex + 1
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Quiz(int quizId, int questionId, int totalQuestions, string selectedAnswer)
        {
            // retrieve the current quiz and question
            var quiz = await _context.Quiz.Include(q => q.Question).FirstOrDefaultAsync(q => q.Id == quizId);
            var question = quiz.Question.FirstOrDefault(q => q.Id == questionId);
            // save the user's answer to the database
            _context.UserAnswer.Add(new UserAnswer
            {
                QuizId = quizId,
                QuestionId = questionId,
                SelectedAnswer = selectedAnswer
            });
            await _context.SaveChangesAsync();

            // if there are more questions, send the next one to the view
            var questionIndex = quiz.Question.IndexOf(question);
            if (questionIndex < totalQuestions - 1)
            {
                var nextQuestion = quiz.Question[questionIndex + 1];
                var model = new QuizAnswerViewModel
                {
                    QuizId = quizId,
                    QuestionId = nextQuestion.Id,
                    Question = nextQuestion.SingleQuestion,
                    Answers = nextQuestion.Answers,
                    TotalQuestions = totalQuestions,
                    QuestionNumber = questionIndex + 2
                };

                return View(model);
            }
            // otherwise, redirect to the results action
            return RedirectToAction("Results", new { quizId });
        }

        public async Task<IActionResult> Results(int quizId)
        {
            // retrieve the quiz and the user's answers
            var quiz = await _context.Quiz.Include(q => q.Question).FirstOrDefaultAsync(q => q.Id == quizId);
            var userAnswers = await _context.UserAnswer.Where(a => a.QuizId == quizId).ToListAsync();
            // calculate the user's score
            var score = userAnswers.Count(a => a.SelectedAnswer == a.Question.CorrectAnswer);

            // create the view model
            var model = new QuizResultViewModel
            {
                QuizTitle = "Quiz",
                TotalQuestions = quiz.TotalQuestions,
                Score = score,
                UserAnswers = userAnswers
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> QuizList()
        {
            // retrieve all quizzes from the DB
            var quizzes = await _context.Quiz.ToListAsync();
            return View(quizzes);
        }

        [HttpGet]
        public async Task<IActionResult> LoadQuiz(int quizId)
        {
            // retrieve the quiz from the DB
            var quiz = await _context.Quiz.Include(q => q.Question).FirstOrDefaultAsync(q => q.Id == quizId);
            // send the first question with possible answers to the view
            var questionIndex = 0;
            var question = quiz.Question[questionIndex];
            var model = new QuizAnswerViewModel
            {
                QuizId = quiz.Id,
                QuestionId = question.Id,
                Question = question.SingleQuestion,
                Answers = question.Answers,
                TotalQuestions = quiz.TotalQuestions,
                QuestionNumber = questionIndex + 1
            };
            return View("Quiz", model);
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

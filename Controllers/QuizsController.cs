using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizPrepAi.Data;
using QuizPrepAi.Models;

namespace QuizPrepAi.Controllers
{
    public class QuizsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuizsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: Quizs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Quiz.Include(q => q.User);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize]
        // GET: Quizs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Quiz == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quiz
                .Include(q => q.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        [Authorize]
        // GET: Quizs/Create
        public IActionResult Create()
        {
            ViewData["QPUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        [Authorize]
        // POST: Quizs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,QPUserId,Topic,TotalQuestions,CorrectAnswers")] Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quiz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QPUserId"] = new SelectList(_context.Users, "Id", "Id", quiz.QPUserId);
            return View(quiz);
        }

        [Authorize]
        // GET: Quizs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Quiz == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quiz.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }
            ViewData["QPUserId"] = new SelectList(_context.Users, "Id", "Id", quiz.QPUserId);
            return View(quiz);
        }

        [Authorize]
        // POST: Quizs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,QPUserId,Topic,TotalQuestions,CorrectAnswers")] Quiz quiz)
        {
            if (id != quiz.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quiz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizExists(quiz.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["QPUserId"] = new SelectList(_context.Users, "Id", "Id", quiz.QPUserId);
            return View(quiz);
        }

        [Authorize]
        // GET: Quizs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Quiz == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quiz
                .Include(q => q.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        [Authorize]
        // POST: Quizs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Quiz == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Quiz'  is null.");
            }
            var quiz = await _context.Quiz.FindAsync(id);
            if (quiz != null)
            {
                _context.Quiz.Remove(quiz);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        private bool QuizExists(int id)
        {
          return (_context.Quiz?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

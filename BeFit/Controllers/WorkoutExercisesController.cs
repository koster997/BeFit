using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Identity;

namespace BeFit.Controllers
{
    public class WorkoutExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkoutExercisesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: WorkoutExercises
        public async Task<IActionResult> Index()
        {
            var exercises = _context.WorkoutExercises
                .Include(w => w.ExerciseType)
                .Include(w => w.WorkoutSession)
                .Include(w => w.ApplicationUser)
                .OrderByDescending(w => w.WorkoutSessionId);

            return View(await exercises.ToListAsync());
        }

        // GET: WorkoutExercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutExercise = await _context.WorkoutExercises
                .Include(w => w.ExerciseType)
                .Include(w => w.WorkoutSession)
                .Include(w => w.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workoutExercise == null)
            {
                return NotFound();
            }

            return View(workoutExercise);
        }

        // GET: WorkoutExercises/Create
        public IActionResult Create()
        {
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name");
            ViewData["WorkoutSessionId"] = new SelectList(_context.WorkoutSessions, "Id", "StartTime");
            return View();
        }

        // POST: WorkoutExercises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExerciseTypeId,WorkoutSessionId,Weight,Sets,Repetitions")] WorkoutExercise workoutExercise)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User); // pobiera zalogowanego użytkownika
                workoutExercise.ApplicationUserId = user?.Id;     // przypisuje jego ID

                _context.Add(workoutExercise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", workoutExercise.ExerciseTypeId);
            ViewData["WorkoutSessionId"] = new SelectList(_context.WorkoutSessions, "Id", "StartTime", workoutExercise.WorkoutSessionId);
            return View(workoutExercise);
        }

        // GET: WorkoutExercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutExercise = await _context.WorkoutExercises.FindAsync(id);
            if (workoutExercise == null)
            {
                return NotFound();
            }

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", workoutExercise.ExerciseTypeId);
            ViewData["WorkoutSessionId"] = new SelectList(_context.WorkoutSessions, "Id", "StartTime", workoutExercise.WorkoutSessionId);
            return View(workoutExercise);
        }

        // POST: WorkoutExercises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExerciseTypeId,WorkoutSessionId,Weight,Sets,Repetitions,ApplicationUserId")] WorkoutExercise workoutExercise)
        {
            if (id != workoutExercise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workoutExercise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutExerciseExists(workoutExercise.Id))
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

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", workoutExercise.ExerciseTypeId);
            ViewData["WorkoutSessionId"] = new SelectList(_context.WorkoutSessions, "Id", "StartTime", workoutExercise.WorkoutSessionId);
            return View(workoutExercise);
        }

        // GET: WorkoutExercises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutExercise = await _context.WorkoutExercises
                .Include(w => w.ExerciseType)
                .Include(w => w.WorkoutSession)
                .Include(w => w.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workoutExercise == null)
            {
                return NotFound();
            }

            return View(workoutExercise);
        }

        // POST: WorkoutExercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workoutExercise = await _context.WorkoutExercises.FindAsync(id);
            if (workoutExercise != null)
            {
                _context.WorkoutExercises.Remove(workoutExercise);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutExerciseExists(int id)
        {
            return _context.WorkoutExercises.Any(e => e.Id == id);
        }
    }
}
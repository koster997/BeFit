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
using Microsoft.AspNetCore.Authorization;

namespace BeFit.Controllers
{
    [Authorize]
    public class WorkoutSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkoutSessionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: WorkoutSessions
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var sessions = await _context.WorkoutSessions
                .Where(w => w.ApplicationUserId == user.Id)
                .Include(w => w.ApplicationUser)
                .OrderByDescending(w => w.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        // GET: WorkoutSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutSession = await _context.WorkoutSessions
                .Include(w => w.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workoutSession == null)
            {
                return NotFound();
            }

            return View(workoutSession);
        }

        // GET: WorkoutSessions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkoutSessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartTime,EndTime")] WorkoutSession workoutSession)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User); // pobiera aktualnie zalogowanego użytkownika
                workoutSession.ApplicationUserId = user?.Id;     // przypisuje jego ID do sesji

                _context.Add(workoutSession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workoutSession);
        }

        // GET: WorkoutSessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutSession = await _context.WorkoutSessions.FindAsync(id);
            if (workoutSession == null)
            {
                return NotFound();
            }
            return View(workoutSession);
        }

        // POST: WorkoutSessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartTime,EndTime,ApplicationUserId")] WorkoutSession workoutSession)
        {
            if (id != workoutSession.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workoutSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutSessionExists(workoutSession.Id))
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
            return View(workoutSession);
        }

        // GET: WorkoutSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutSession = await _context.WorkoutSessions
                .Include(w => w.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workoutSession == null)
            {
                return NotFound();
            }

            return View(workoutSession);
        }

        // POST: WorkoutSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workoutSession = await _context.WorkoutSessions.FindAsync(id);
            if (workoutSession != null)
            {
                _context.WorkoutSessions.Remove(workoutSession);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutSessionExists(int id)
        {
            return _context.WorkoutSessions.Any(e => e.Id == id);
        }
    }
}
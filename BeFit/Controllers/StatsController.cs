using Microsoft.AspNetCore.Mvc;
using BeFit.Data;
using BeFit.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BeFit.Controllers
{
    [Authorize]
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StatsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var fourWeeksAgo = DateTime.Now.AddDays(-28);
            var user = await _userManager.GetUserAsync(User);

            var stats = await _context.WorkoutExercises
                .Where(we => we.WorkoutSession.StartTime >= fourWeeksAgo
                             && we.ApplicationUserId == user.Id)  // filtr: tylko dane zalogowanego
                .GroupBy(we => we.ExerciseType.Name)
                .Select(g => new ExerciseStats
                {
                    ExerciseName = g.Key,
                    Count = g.Count(),
                    TotalRepetitions = g.Sum(x => x.Sets * x.Repetitions),
                    AvgWeight = g.Average(x => x.Weight),
                    MaxWeight = g.Max(x => x.Weight)
                })
                .ToListAsync();

            return View(stats);
        }
    }
}
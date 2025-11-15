using Microsoft.AspNetCore.Mvc;
using BeFit.Data;
using BeFit.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BeFit.Controllers
{
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var fourWeeksAgo = DateTime.Now.AddDays(-28);

            var stats = await _context.WorkoutExercises
                .Where(we => we.WorkoutSession.StartTime >= fourWeeksAgo)
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
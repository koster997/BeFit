using System;
using System.Linq;
using System.Threading.Tasks;
using BeFit.Data;
using BeFit.Models;
using BeFit.Services;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Pages;

public partial class StatsPage : ContentPage
{
    private readonly BeFitDbContext _context;
    private readonly AuthService _auth;

    public StatsPage()
    {
        InitializeComponent();
        _context = ServiceHelper.GetService<BeFitDbContext>();
        _auth = ServiceHelper.GetService<AuthService>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStats();
    }

    private async Task LoadStats()
    {
        if (!_auth.IsLoggedIn)
        {
            InfoLabel.Text = "Musisz byæ zalogowany, aby zobaczyæ swoje statystyki.";
            StatsList.ItemsSource = null;
            return;
        }

        var userId = _auth.CurrentUserId!;
        InfoLabel.Text = $"Statystyki dla u¿ytkownika: {_auth.CurrentUser!.Username}";

        var fourWeeksAgo = DateTime.Now.AddDays(-28);

        var query = await _context.PerformedExercises
            .Include(e => e.ExerciseType)
            .Include(e => e.TrainingSession)
            .Where(e => e.TrainingSession != null
                        && e.TrainingSession.UserId == userId
                        && e.TrainingSession.StartTime >= fourWeeksAgo)
            .ToListAsync();

        var stats = query
            .GroupBy(e => e.ExerciseType)
            .Where(g => g.Key != null)
            .Select(g => new ExerciseStats
            {
                ExerciseName = g.Key!.Name,
                TimesPerformed = g.Count(),
                TotalReps = g.Sum(x => x.Sets * x.Reps),
                AverageWeight = g.Average(x => x.Weight),
                MaxWeight = g.Max(x => x.Weight)
            })
            .OrderBy(s => s.ExerciseName)
            .ToList();

        StatsList.ItemsSource = stats;
    }
}
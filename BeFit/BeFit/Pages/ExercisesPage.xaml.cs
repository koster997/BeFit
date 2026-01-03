using System;
using System.Linq;
using System.Threading.Tasks;
using BeFit.Data;
using BeFit.Models;
using BeFit.Services;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Pages;

public partial class ExercisesPage : ContentPage
{
    private readonly BeFitDbContext _context;
    private readonly AuthService _auth;

    public ExercisesPage()
    {
        InitializeComponent();
        _context = ServiceHelper.GetService<BeFitDbContext>();
        _auth = ServiceHelper.GetService<AuthService>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshData();
    }

    private async Task RefreshData()
    {
        if (!_auth.IsLoggedIn)
        {
            InfoLabel.Text = "Musisz byæ zalogowany, aby przegl¹daæ swoje æwiczenia.";
            AddExerciseButton.IsEnabled = false;
            ExercisesList.ItemsSource = null;
            return;
        }

        InfoLabel.Text = $"Zalogowany u¿ytkownik: {_auth.CurrentUser!.Username}";
        AddExerciseButton.IsEnabled = true;

        var userId = _auth.CurrentUserId!;

        var data = await _context.PerformedExercises
            .Include(e => e.ExerciseType)
            .Include(e => e.TrainingSession)
            .Where(e => e.TrainingSession != null && e.TrainingSession.UserId == userId)
            .OrderByDescending(e => e.TrainingSession!.StartTime)
            .ToListAsync();

        ExercisesList.ItemsSource = data;
    }

    private async void OnAddExerciseClicked(object sender, EventArgs e)
    {
        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Musisz byæ zalogowany, aby dodawaæ æwiczenia.", "OK");
            return;
        }

        await Navigation.PushAsync(new ExerciseEditPage());
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Musisz byæ zalogowany, aby edytowaæ æwiczenia.", "OK");
            return;
        }

        if (sender is Button btn && btn.CommandParameter is int id)
        {
            var exercise = await _context.PerformedExercises
                .Include(e => e.TrainingSession)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exercise == null || exercise.TrainingSession == null || exercise.TrainingSession.UserId != _auth.CurrentUserId)
            {
                await DisplayAlert("B³¹d", "Nie mo¿esz edytowaæ æwiczeñ innego u¿ytkownika.", "OK");
                return;
            }

            await Navigation.PushAsync(new ExerciseEditPage(id));
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Musisz byæ zalogowany, aby usuwaæ æwiczenia.", "OK");
            return;
        }

        if (sender is not Button btn || btn.CommandParameter is not int id)
            return;

        var exercise = await _context.PerformedExercises
            .Include(e => e.TrainingSession)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (exercise == null || exercise.TrainingSession == null || exercise.TrainingSession.UserId != _auth.CurrentUserId)
        {
            await DisplayAlert("B³¹d", "Nie mo¿esz usuwaæ æwiczeñ innego u¿ytkownika.", "OK");
            return;
        }

        var confirm = await DisplayAlert("Potwierdzenie usuniêcia",
            "Czy na pewno chcesz usun¹æ to wykonane æwiczenie?",
            "Tak", "Nie");

        if (!confirm) return;

        _context.PerformedExercises.Remove(exercise);
        await _context.SaveChangesAsync();
        await RefreshData();
    }
}
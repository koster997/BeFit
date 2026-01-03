using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BeFit.Data;
using BeFit.Models;
using BeFit.Services;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Pages;

public partial class ExerciseEditPage : ContentPage
{
    private readonly BeFitDbContext _context;
    private readonly AuthService _auth;
    private int _exerciseId;
    private bool _isLoaded;

    private List<TrainingSession> _sessions = new();
    private List<ExerciseType> _exerciseTypes = new();

    public ExerciseEditPage() : this(0)
    {
    }

    public ExerciseEditPage(int exerciseId)
    {
        InitializeComponent();
        _context = ServiceHelper.GetService<BeFitDbContext>();
        _auth = ServiceHelper.GetService<AuthService>();
        _exerciseId = exerciseId;

        TitleLabel.Text = _exerciseId == 0
            ? "Nowe wykonane æwiczenie"
            : "Edycja wykonanego æwiczenia";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Musisz byæ zalogowany, aby edytowaæ æwiczenia.", "OK");
            await Navigation.PopAsync();
            return;
        }

        if (_isLoaded) return;

        var userId = _auth.CurrentUserId!;

        _sessions = await _context.TrainingSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();

        if (_sessions.Count == 0)
        {
            await DisplayAlert("Brak sesji", "Najpierw utwórz sesjê treningow¹, aby móc dodawaæ æwiczenia.", "OK");
            await Navigation.PopAsync();
            return;
        }

        SessionPicker.ItemsSource = _sessions;

        _exerciseTypes = await _context.ExerciseTypes
            .OrderBy(e => e.Name)
            .ToListAsync();

        ExerciseTypePicker.ItemsSource = _exerciseTypes;

        if (_exerciseId != 0)
        {
            var exercise = await _context.PerformedExercises
                .Include(e => e.TrainingSession)
                .Include(e => e.ExerciseType)
                .FirstOrDefaultAsync(e => e.Id == _exerciseId);

            if (exercise == null || exercise.TrainingSession == null || exercise.TrainingSession.UserId != userId)
            {
                await DisplayAlert("B³¹d", "Nie mo¿esz edytowaæ æwiczenia innego u¿ytkownika.", "OK");
                await Navigation.PopAsync();
                return;
            }

            SessionPicker.SelectedItem = _sessions.FirstOrDefault(s => s.Id == exercise.TrainingSessionId);
            ExerciseTypePicker.SelectedItem = _exerciseTypes.FirstOrDefault(t => t.Id == exercise.ExerciseTypeId);

            WeightEntry.Text = exercise.Weight.ToString(CultureInfo.InvariantCulture);
            SetsEntry.Text = exercise.Sets.ToString();
            RepsEntry.Text = exercise.Reps.ToString();
            NotesEditor.Text = exercise.Notes;
        }

        _isLoaded = true;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Musisz byæ zalogowany, aby zapisywaæ æwiczenia.", "OK");
            return;
        }

        if (SessionPicker.SelectedItem is not TrainingSession selectedSession)
        {
            await DisplayAlert("B³¹d", "Wybierz sesjê treningow¹.", "OK");
            return;
        }

        if (selectedSession.UserId != _auth.CurrentUserId)
        {
            await DisplayAlert("B³¹d", "Nie mo¿esz przypisaæ æwiczenia do sesji innego u¿ytkownika.", "OK");
            return;
        }

        if (ExerciseTypePicker.SelectedItem is not ExerciseType selectedType)
        {
            await DisplayAlert("B³¹d", "Wybierz typ æwiczenia.", "OK");
            return;
        }

        if (!double.TryParse(WeightEntry.Text?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double weight))
        {
            await DisplayAlert("B³¹d", "Podaj poprawne obci¹¿enie (liczba).", "OK");
            return;
        }

        if (weight < 0 || weight > 2000)
        {
            await DisplayAlert("B³¹d", "Obci¹¿enie musi byæ w zakresie od 0 do 2000 kg.", "OK");
            return;
        }

        if (!int.TryParse(SetsEntry.Text, out int sets))
        {
            await DisplayAlert("B³¹d", "Podaj poprawn¹ liczbê serii (liczba ca³kowita).", "OK");
            return;
        }

        if (sets < 1 || sets > 50)
        {
            await DisplayAlert("B³¹d", "Liczba serii musi byæ w zakresie od 1 do 50.", "OK");
            return;
        }

        if (!int.TryParse(RepsEntry.Text, out int reps))
        {
            await DisplayAlert("B³¹d", "Podaj poprawn¹ liczbê powtórzeñ (liczba ca³kowita).", "OK");
            return;
        }

        if (reps < 1 || reps > 200)
        {
            await DisplayAlert("B³¹d", "Liczba powtórzeñ musi byæ w zakresie od 1 do 200.", "OK");
            return;
        }

        var notes = NotesEditor.Text?.Trim();
        if (notes != null && notes.Length > 500)
        {
            await DisplayAlert("B³¹d", "Notatki mog¹ mieæ maksymalnie 500 znaków.", "OK");
            return;
        }

        PerformedExercise exercise;

        if (_exerciseId == 0)
        {
            exercise = new PerformedExercise();
            _context.PerformedExercises.Add(exercise);
        }
        else
        {
            exercise = await _context.PerformedExercises
                .Include(e => e.TrainingSession)
                .FirstOrDefaultAsync(e => e.Id == _exerciseId)
                ?? new PerformedExercise();

            if (exercise.Id == 0)
            {
                _context.PerformedExercises.Add(exercise);
            }
            else if (exercise.TrainingSession == null || exercise.TrainingSession.UserId != _auth.CurrentUserId)
            {
                await DisplayAlert("B³¹d", "Nie mo¿esz edytowaæ æwiczenia innego u¿ytkownika.", "OK");
                return;
            }
        }

        exercise.TrainingSessionId = selectedSession.Id;
        exercise.ExerciseTypeId = selectedType.Id;
        exercise.Weight = weight;
        exercise.Sets = sets;
        exercise.Reps = reps;
        exercise.Notes = string.IsNullOrEmpty(notes) ? null : notes;

        await _context.SaveChangesAsync();

        await DisplayAlert("Sukces", "Æwiczenie zosta³o zapisane.", "OK");
        await Navigation.PopAsync();
    }
}
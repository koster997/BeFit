using System.Globalization;
using System.Linq;
using BeFitMob.Models;
using BeFitMob.Services;

namespace BeFitMob.Pages;

public partial class ExercisesPage : ContentPage
{
    private readonly DataService _data;
    private readonly AuthService _auth;

    private int _editingId = 0;

    private List<TrainingSession> _sessions = new();
    private List<ExerciseType> _exerciseTypes = new();

    public ExercisesPage()
    {
        InitializeComponent();
        _data = ServiceHelper.GetService<DataService>();
        _auth = ServiceHelper.GetService<AuthService>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateInfo();
        LoadPickers();
        LoadList();
        ClearForm();
    }

    private void UpdateInfo()
    {
        if (!_auth.IsLoggedIn)
        {
            InfoLabel.Text = "Musisz siê zalogowaæ, aby dodawaæ i edytowaæ æwiczenia.";
        }
        else
        {
            InfoLabel.Text = $"Zalogowany u¿ytkownik: {_auth.CurrentUser!.Username}";
        }
    }

    private void LoadPickers()
    {
        if (!_auth.IsLoggedIn)
        {
            SessionPicker.ItemsSource = null;
            ExerciseTypePicker.ItemsSource = null;
            return;
        }

        _sessions = _data.GetSessionsForUser(_auth.CurrentUserId!).OrderByDescending(s => s.StartTime).ToList();
        _exerciseTypes = _data.GetExerciseTypes().ToList();

        SessionPicker.ItemsSource = _sessions;
        ExerciseTypePicker.ItemsSource = _exerciseTypes;
    }

    private void LoadList()
    {
        if (!_auth.IsLoggedIn)
        {
            ExercisesList.ItemsSource = null;
            return;
        }

        ExercisesList.ItemsSource = _data.GetExercisesForUser(_auth.CurrentUserId!);
    }

    private void ClearForm()
    {
        _editingId = 0;
        SessionPicker.SelectedItem = null;
        ExerciseTypePicker.SelectedItem = null;
        WeightEntry.Text = string.Empty;
        SetsEntry.Text = string.Empty;
        RepsEntry.Text = string.Empty;
        NotesEditor.Text = string.Empty;
    }

    private async void OnNewClicked(object sender, EventArgs e)
    {
        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Zaloguj siê, aby dodawaæ æwiczenia.", "OK");
            return;
        }

        ClearForm();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Zaloguj siê, aby zapisaæ æwiczenie.", "OK");
            return;
        }

        if (SessionPicker.SelectedItem is not TrainingSession selectedSession)
        {
            await DisplayAlert("B³¹d", "Wybierz sesjê treningow¹.", "OK");
            return;
        }

        if (ExerciseTypePicker.SelectedItem is not ExerciseType selectedType)
        {
            await DisplayAlert("B³¹d", "Wybierz typ æwiczenia.", "OK");
            return;
        }

        if (!double.TryParse(WeightEntry.Text?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var weight))
        {
            await DisplayAlert("B³¹d", "Podaj poprawne obci¹¿enie (liczba).", "OK");
            return;
        }
        if (weight < 0 || weight > 2000)
        {
            await DisplayAlert("B³¹d", "Obci¹¿enie musi byæ w zakresie 0–2000 kg.", "OK");
            return;
        }

        if (!int.TryParse(SetsEntry.Text, out var sets))
        {
            await DisplayAlert("B³¹d", "Podaj poprawn¹ liczbê serii (liczba ca³kowita).", "OK");
            return;
        }
        if (sets < 1 || sets > 50)
        {
            await DisplayAlert("B³¹d", "Liczba serii musi byæ w zakresie 1–50.", "OK");
            return;
        }

        if (!int.TryParse(RepsEntry.Text, out var reps))
        {
            await DisplayAlert("B³¹d", "Podaj poprawn¹ liczbê powtórzeñ (liczba ca³kowita).", "OK");
            return;
        }
        if (reps < 1 || reps > 200)
        {
            await DisplayAlert("B³¹d", "Liczba powtórzeñ musi byæ w zakresie 1–200.", "OK");
            return;
        }

        var notes = NotesEditor.Text?.Trim();
        if (!string.IsNullOrEmpty(notes) && notes.Length > 500)
        {
            await DisplayAlert("B³¹d", "Notatki mog¹ mieæ maksymalnie 500 znaków.", "OK");
            return;
        }

        if (_editingId == 0)
        {
            var ex = new PerformedExercise
            {
                TrainingSessionId = selectedSession.Id,
                ExerciseTypeId = selectedType.Id,
                Weight = weight,
                Sets = sets,
                Reps = reps,
                Notes = string.IsNullOrEmpty(notes) ? null : notes
            };

            var added = _data.AddExercise(ex, _auth.CurrentUserId!);
            if (added == null)
            {
                await DisplayAlert("B³¹d", "Nie mo¿esz przypisaæ æwiczenia do sesji innego u¿ytkownika.", "OK");
                return;
            }

            await DisplayAlert("Sukces", "Dodano nowe æwiczenie.", "OK");
        }
        else
        {
            var existing = _data.GetExerciseForUserById(_editingId, _auth.CurrentUserId!);
            if (existing == null)
            {
                await DisplayAlert("B³¹d", "Nie mo¿esz edytowaæ æwiczenia innego u¿ytkownika.", "OK");
                return;
            }

            existing.TrainingSessionId = selectedSession.Id;
            existing.ExerciseTypeId = selectedType.Id;
            existing.Weight = weight;
            existing.Sets = sets;
            existing.Reps = reps;
            existing.Notes = string.IsNullOrEmpty(notes) ? null : notes;

            var ok = _data.UpdateExercise(existing, _auth.CurrentUserId!);
            if (!ok)
            {
                await DisplayAlert("B³¹d", "Nie mo¿esz edytowaæ æwiczenia innego u¿ytkownika.", "OK");
                return;
            }

            await DisplayAlert("Sukces", "Zapisano zmiany æwiczenia.", "OK");
        }

        LoadList();
        ClearForm();
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Zaloguj siê, aby edytowaæ æwiczenia.", "OK");
            return;
        }

        if (sender is Button btn && btn.CommandParameter is int id)
        {
            var ex = _data.GetExerciseForUserById(id, _auth.CurrentUserId!);
            if (ex == null || ex.TrainingSession == null || ex.ExerciseType == null)
            {
                await DisplayAlert("B³¹d", "Nie mo¿esz edytowaæ æwiczenia innego u¿ytkownika.", "OK");
                return;
            }

            _editingId = ex.Id;

            SessionPicker.SelectedItem = _sessions.FirstOrDefault(s => s.Id == ex.TrainingSessionId);
            ExerciseTypePicker.SelectedItem = _exerciseTypes.FirstOrDefault(t => t.Id == ex.ExerciseTypeId);

            WeightEntry.Text = ex.Weight.ToString(CultureInfo.InvariantCulture);
            SetsEntry.Text = ex.Sets.ToString();
            RepsEntry.Text = ex.Reps.ToString();
            NotesEditor.Text = ex.Notes;
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Zaloguj siê, aby usuwaæ æwiczenia.", "OK");
            return;
        }

        if (sender is Button btn && btn.CommandParameter is int id)
        {
            var confirm = await DisplayAlert("Potwierdzenie",
                "Czy na pewno chcesz usun¹æ to æwiczenie?",
                "Tak", "Nie");
            if (!confirm) return;

            var ok = _data.DeleteExercise(id, _auth.CurrentUserId!);
            if (!ok)
            {
                await DisplayAlert("B³¹d", "Nie mo¿esz usuwaæ æwiczenia innego u¿ytkownika.", "OK");
                return;
            }

            LoadList();
            if (_editingId == id)
                ClearForm();
        }
    }
}
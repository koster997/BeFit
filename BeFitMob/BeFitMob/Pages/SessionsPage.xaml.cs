using BeFitMob.Models;
using BeFitMob.Services;

namespace BeFitMob.Pages;

public partial class SessionsPage : ContentPage
{
    private readonly DataService _data;
    private readonly AuthService _auth;

    private int _editingId = 0;

    public SessionsPage()
    {
        InitializeComponent();
        _data = ServiceHelper.GetService<DataService>();
        _auth = ServiceHelper.GetService<AuthService>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateInfo();
        LoadList();
        SetupDefaultDates();
    }

    private void UpdateInfo()
    {
        if (!_auth.IsLoggedIn)
        {
            InfoLabel.Text = "Musisz siê zalogowaæ, aby tworzyæ i edytowaæ sesje treningowe.";
        }
        else
        {
            InfoLabel.Text = $"Zalogowany u¿ytkownik: {_auth.CurrentUser!.Username}";
        }
    }

    private void LoadList()
    {
        if (!_auth.IsLoggedIn)
        {
            SessionsList.ItemsSource = null;
            return;
        }

        SessionsList.ItemsSource = _data.GetSessionsForUser(_auth.CurrentUserId!);
    }

    private void SetupDefaultDates()
    {
        var now = DateTime.Now;
        StartDatePicker.Date = now.Date;
        StartTimePicker.Time = now.TimeOfDay;
        EndDatePicker.Date = now.Date;
        EndTimePicker.Time = now.TimeOfDay.Add(TimeSpan.FromHours(1));
        _editingId = 0;
    }

    private async void OnNewClicked(object sender, EventArgs e)
    {
        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Zaloguj siê, aby tworzyæ sesje.", "OK");
            return;
        }

        SetupDefaultDates();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Zaloguj siê, aby zapisaæ sesjê.", "OK");
            return;
        }

        var start = StartDatePicker.Date.Add(StartTimePicker.Time);
        var end = EndDatePicker.Date.Add(EndTimePicker.Time);

        if (end <= start)
        {
            await DisplayAlert("B³¹d", "Zakoñczenie musi byæ póŸniej ni¿ rozpoczêcie.", "OK");
            return;
        }

        if (_editingId == 0)
        {
            var session = new TrainingSession
            {
                StartTime = start,
                EndTime = end
            };
            _data.AddSession(session, _auth.CurrentUserId!);
            await DisplayAlert("Sukces", "Dodano now¹ sesjê.", "OK");
        }
        else
        {
            var session = _data.GetSessionForUserById(_editingId, _auth.CurrentUserId!);
            if (session == null)
            {
                await DisplayAlert("B³¹d", "Nie znaleziono edytowanej sesji.", "OK");
                return;
            }

            session.StartTime = start;
            session.EndTime = end;
            _data.UpdateSession(session, _auth.CurrentUserId!);
            await DisplayAlert("Sukces", "Zapisano zmiany sesji.", "OK");
        }

        LoadList();
        _editingId = 0;
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Zaloguj siê, aby edytowaæ sesje.", "OK");
            return;
        }

        if (sender is Button btn && btn.CommandParameter is int id)
        {
            var session = _data.GetSessionForUserById(id, _auth.CurrentUserId!);
            if (session == null)
            {
                await DisplayAlert("B³¹d", "Nie mo¿esz edytowaæ sesji innego u¿ytkownika.", "OK");
                return;
            }

            _editingId = session.Id;
            StartDatePicker.Date = session.StartTime.Date;
            StartTimePicker.Time = session.StartTime.TimeOfDay;
            EndDatePicker.Date = session.EndTime.Date;
            EndTimePicker.Time = session.EndTime.TimeOfDay;
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Zaloguj siê, aby usuwaæ sesje.", "OK");
            return;
        }

        if (sender is Button btn && btn.CommandParameter is int id)
        {
            var confirm = await DisplayAlert("Potwierdzenie",
                "Czy na pewno chcesz usun¹æ tê sesjê (wraz z æwiczeniami)?",
                "Tak", "Nie");
            if (!confirm) return;

            var ok = _data.DeleteSession(id, _auth.CurrentUserId!);
            if (!ok)
            {
                await DisplayAlert("B³¹d", "Nie mo¿esz usuwaæ sesji innego u¿ytkownika.", "OK");
                return;
            }

            LoadList();
            if (_editingId == id)
                _editingId = 0;
        }
    }
}
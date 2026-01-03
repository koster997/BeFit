using System;
using BeFit.Data;
using BeFit.Models;
using BeFit.Services;

namespace BeFit.Pages;

public partial class SessionEditPage : ContentPage
{
    private readonly BeFitDbContext _context;
    private readonly AuthService _auth;
    private int _sessionId;
    private bool _isLoaded;

    public SessionEditPage() : this(0)
    {
    }

    public SessionEditPage(int sessionId)
    {
        InitializeComponent();
        _context = ServiceHelper.GetService<BeFitDbContext>();
        _auth = ServiceHelper.GetService<AuthService>();
        _sessionId = sessionId;

        TitleLabel.Text = _sessionId == 0
            ? "Nowa sesja treningowa"
            : "Edycja sesji treningowej";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Musisz byæ zalogowany, aby edytowaæ sesje treningowe.", "OK");
            await Navigation.PopAsync();
            return;
        }

        if (_sessionId != 0 && !_isLoaded)
        {
            var session = await _context.TrainingSessions.FindAsync(_sessionId);
            if (session == null || session.UserId != _auth.CurrentUserId)
            {
                await DisplayAlert("B³¹d", "Nie mo¿esz edytowaæ sesji innego u¿ytkownika.", "OK");
                await Navigation.PopAsync();
                return;
            }

            StartDatePicker.Date = session.StartTime.Date;
            StartTimePicker.Time = session.StartTime.TimeOfDay;

            EndDatePicker.Date = session.EndTime.Date;
            EndTimePicker.Time = session.EndTime.TimeOfDay;

            _isLoaded = true;
        }
        else if (_sessionId == 0 && !_isLoaded)
        {
            var now = DateTime.Now;
            StartDatePicker.Date = now.Date;
            StartTimePicker.Time = now.TimeOfDay;
            EndDatePicker.Date = now.Date;
            EndTimePicker.Time = now.TimeOfDay.Add(TimeSpan.FromHours(1));
            _isLoaded = true;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!_auth.IsLoggedIn)
        {
            await DisplayAlert("Brak uprawnieñ", "Musisz byæ zalogowany, aby zapisywaæ sesje.", "OK");
            return;
        }

        var start = StartDatePicker.Date.Add(StartTimePicker.Time);
        var end = EndDatePicker.Date.Add(EndTimePicker.Time);

        if (end <= start)
        {
            await DisplayAlert("B³¹d", "Data i godzina zakoñczenia musz¹ byæ póŸniejsze ni¿ rozpoczêcia.", "OK");
            return;
        }

        TrainingSession session;

        if (_sessionId == 0)
        {
            session = new TrainingSession
            {
                UserId = _auth.CurrentUserId!
            };
            _context.TrainingSessions.Add(session);
        }
        else
        {
            session = await _context.TrainingSessions.FindAsync(_sessionId)
                      ?? new TrainingSession { UserId = _auth.CurrentUserId! };

            if (session.Id == 0)
            {
                _context.TrainingSessions.Add(session);
            }
            else if (session.UserId != _auth.CurrentUserId)
            {
                await DisplayAlert("B³¹d", "Nie mo¿esz edytowaæ sesji innego u¿ytkownika.", "OK");
                return;
            }
        }

        session.StartTime = start;
        session.EndTime = end;

        await _context.SaveChangesAsync();

        await DisplayAlert("Sukces", "Sesja zosta³a zapisana.", "OK");
        await Navigation.PopAsync();
    }
}
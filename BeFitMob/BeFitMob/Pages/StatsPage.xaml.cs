using BeFitMob.Services;

namespace BeFitMob.Pages;

public partial class StatsPage : ContentPage
{
    private readonly DataService _data;
    private readonly AuthService _auth;

    public StatsPage()
    {
        InitializeComponent();
        _data = ServiceHelper.GetService<DataService>();
        _auth = ServiceHelper.GetService<AuthService>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadStats();
    }

    private void LoadStats()
    {
        if (!_auth.IsLoggedIn)
        {
            InfoLabel.Text = "Musisz siê zalogowaæ, aby zobaczyæ swoje statystyki.";
            StatsList.ItemsSource = null;
            return;
        }

        InfoLabel.Text = $"Statystyki dla u¿ytkownika: {_auth.CurrentUser!.Username}";

        var stats = _data.GetStatsForUser(_auth.CurrentUserId!);
        StatsList.ItemsSource = stats;
    }
}
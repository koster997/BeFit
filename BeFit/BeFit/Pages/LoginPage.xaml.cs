using System;
using BeFit.Services;

namespace BeFit.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _auth;

    public LoginPage()
    {
        InitializeComponent();
        _auth = ServiceHelper.GetService<AuthService>();
        UpdateStatus();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text?.Trim() ?? string.Empty;
        var password = PasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("B³¹d", "Podaj nazwê u¿ytkownika i has³o.", "OK");
            return;
        }

        var ok = _auth.Login(username, password);
        if (!ok)
        {
            await DisplayAlert("B³¹d logowania", "Niepoprawny login lub has³o.", "OK");
        }
        else
        {
            await DisplayAlert("Sukces", $"Zalogowano jako {_auth.CurrentUser!.Username} ({_auth.CurrentUser.Role}).", "OK");
        }

        UpdateStatus();
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        _auth.Logout();
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        if (_auth.IsLoggedIn)
        {
            StatusLabel.Text = $"Zalogowany u¿ytkownik: {_auth.CurrentUser!.Username}, rola: {_auth.CurrentUser.Role}";
        }
        else
        {
            StatusLabel.Text = "Brak zalogowanego u¿ytkownika. Jako goœæ mo¿esz tylko przegl¹daæ typy æwiczeñ.";
        }
    }
}
using BeFitMob.Models;
using BeFitMob.Services;

namespace BeFitMob.Pages;

public partial class ExerciseTypesPage : ContentPage
{
    private readonly DataService _data;
    private readonly AuthService _auth;

    private int _editingId = 0; // 0 = nowy

    public ExerciseTypesPage()
    {
        InitializeComponent();
        _data = ServiceHelper.GetService<DataService>();
        _auth = ServiceHelper.GetService<AuthService>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadList();
        UpdateInfo();
        ClearForm();
    }

    private void UpdateInfo()
    {
        if (_auth.IsAdmin)
        {
            InfoLabel.Text = "Administrator – mo¿esz dodawaæ, edytowaæ i usuwaæ typy æwiczeñ.";
        }
        else if (_auth.IsLoggedIn)
        {
            InfoLabel.Text = "Zwyk³y u¿ytkownik – mo¿esz tylko przegl¹daæ typy æwiczeñ.";
        }
        else
        {
            InfoLabel.Text = "Goœæ – mo¿esz tylko przegl¹daæ typy æwiczeñ.";
        }
    }

    private void LoadList()
    {
        ExerciseTypesList.ItemsSource = _data.GetExerciseTypes();
    }

    private void ClearForm()
    {
        _editingId = 0;
        NameEntry.Text = string.Empty;
        DescriptionEditor.Text = string.Empty;
    }

    private async void OnNewClicked(object sender, EventArgs e)
    {
        if (!_auth.IsAdmin)
        {
            await DisplayAlert("Brak uprawnieñ", "Tylko administrator mo¿e dodawaæ typy æwiczeñ.", "OK");
            return;
        }

        ClearForm();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!_auth.IsAdmin)
        {
            await DisplayAlert("Brak uprawnieñ", "Tylko administrator mo¿e zapisywaæ typy æwiczeñ.", "OK");
            return;
        }

        var name = NameEntry.Text?.Trim() ?? string.Empty;
        var desc = DescriptionEditor.Text?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("B³¹d", "Nazwa æwiczenia jest wymagana.", "OK");
            return;
        }
        if (name.Length < 2 || name.Length > 100)
        {
            await DisplayAlert("B³¹d", "Nazwa musi mieæ od 2 do 100 znaków.", "OK");
            return;
        }
        if (!string.IsNullOrEmpty(desc) && desc.Length > 500)
        {
            await DisplayAlert("B³¹d", "Opis mo¿e mieæ maksymalnie 500 znaków.", "OK");
            return;
        }

        if (_editingId == 0)
        {
            var et = new ExerciseType
            {
                Name = name,
                Description = string.IsNullOrEmpty(desc) ? null : desc
            };
            _data.AddExerciseType(et);
            await DisplayAlert("Sukces", "Dodano nowy typ æwiczenia.", "OK");
        }
        else
        {
            var existing = _data.GetExerciseTypeById(_editingId);
            if (existing == null)
            {
                await DisplayAlert("B³¹d", "Nie znaleziono edytowanego typu æwiczenia.", "OK");
                return;
            }

            existing.Name = name;
            existing.Description = string.IsNullOrEmpty(desc) ? null : desc;
            _data.UpdateExerciseType(existing);
            await DisplayAlert("Sukces", "Zapisano zmiany.", "OK");
        }

        LoadList();
        ClearForm();
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (!_auth.IsAdmin)
        {
            await DisplayAlert("Brak uprawnieñ", "Tylko administrator mo¿e edytowaæ typy æwiczeñ.", "OK");
            return;
        }

        if (sender is Button btn && btn.CommandParameter is int id)
        {
            var et = _data.GetExerciseTypeById(id);
            if (et == null)
                return;

            _editingId = et.Id;
            NameEntry.Text = et.Name;
            DescriptionEditor.Text = et.Description;
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (!_auth.IsAdmin)
        {
            await DisplayAlert("Brak uprawnieñ", "Tylko administrator mo¿e usuwaæ typy æwiczeñ.", "OK");
            return;
        }

        if (sender is Button btn && btn.CommandParameter is int id)
        {
            var et = _data.GetExerciseTypeById(id);
            if (et == null)
                return;

            var confirm = await DisplayAlert("Potwierdzenie",
                $"Czy na pewno chcesz usun¹æ \"{et.Name}\" (oraz powi¹zane æwiczenia)?",
                "Tak", "Nie");
            if (!confirm) return;

            _data.DeleteExerciseType(id);
            LoadList();
            if (_editingId == id)
                ClearForm();
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using BeFit.Data;
using BeFit.Models;
using BeFit.Services;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Pages;

public partial class ExerciseTypesPage : ContentPage
{
    private readonly BeFitDbContext _context;
    private readonly AuthService _auth;

    public ExerciseTypesPage()
    {
        InitializeComponent();
        _context = ServiceHelper.GetService<BeFitDbContext>();
        _auth = ServiceHelper.GetService<AuthService>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadExerciseTypes();
        UpdateButtonsVisibility();
    }

    private async Task LoadExerciseTypes()
    {
        var list = await _context.ExerciseTypes
            .OrderBy(e => e.Name)
            .ToListAsync();

        ExerciseTypesList.ItemsSource = list;
    }

    private void UpdateButtonsVisibility()
    {
        AddButton.IsEnabled = _auth.IsAdmin;
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (!_auth.IsAdmin)
        {
            await DisplayAlert("Brak uprawnieñ", "Tylko administrator mo¿e dodawaæ typy æwiczeñ.", "OK");
            return;
        }

        await Navigation.PushAsync(new ExerciseTypeEditPage());
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
            await Navigation.PushAsync(new ExerciseTypeEditPage(id));
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (!_auth.IsAdmin)
        {
            await DisplayAlert("Brak uprawnieñ", "Tylko administrator mo¿e usuwaæ typy æwiczeñ.", "OK");
            return;
        }

        if (sender is not Button btn || btn.CommandParameter is not int id)
            return;

        var toDelete = await _context.ExerciseTypes.FindAsync(id);
        if (toDelete == null)
            return;

        var confirm = await DisplayAlert("Potwierdzenie usuniêcia",
            $"Czy na pewno chcesz usun¹æ æwiczenie \"{toDelete.Name}\"?",
            "Tak", "Nie");

        if (!confirm) return;

        _context.ExerciseTypes.Remove(toDelete);
        await _context.SaveChangesAsync();

        await LoadExerciseTypes();
    }
}
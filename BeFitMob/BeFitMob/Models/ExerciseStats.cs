using System.ComponentModel.DataAnnotations;

namespace BeFitMob.Models;

public class ExerciseStats
{
    [Display(Name = "Nazwa ćwiczenia")]
    public string ExerciseName { get; set; } = string.Empty;

    [Display(Name = "Liczba treningów z tym ćwiczeniem")]
    public int TimesPerformed { get; set; }

    [Display(Name = "Łączna liczba powtórzeń")]
    public int TotalReps { get; set; }

    [Display(Name = "Średnie obciążenie (kg)")]
    public double AverageWeight { get; set; }

    [Display(Name = "Maksymalne obciążenie (kg)")]
    public double MaxWeight { get; set; }
}
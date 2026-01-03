using System.ComponentModel.DataAnnotations;

namespace BeFit.Models;

public class ExerciseStats
{
    [Display(Name = "Nazwa æwiczenia")]
    public string ExerciseName { get; set; } = string.Empty;

    [Display(Name = "Liczba treningów z tym æwiczeniem")]
    public int TimesPerformed { get; set; }

    [Display(Name = "£¹czna liczba powtórzeñ")]
    public int TotalReps { get; set; }

    [Display(Name = "Œrednie obci¹¿enie (kg)")]
    public double AverageWeight { get; set; }

    [Display(Name = "Maksymalne obci¹¿enie (kg)")]
    public double MaxWeight { get; set; }
}
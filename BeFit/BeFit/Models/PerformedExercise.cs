using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Models;

public class PerformedExercise
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Sesja treningowa")]
    public int TrainingSessionId { get; set; }

    [ForeignKey(nameof(TrainingSessionId))]
    public TrainingSession? TrainingSession { get; set; }

    [Required]
    [Display(Name = "Typ ćwiczenia")]
    public int ExerciseTypeId { get; set; }

    [ForeignKey(nameof(ExerciseTypeId))]
    public ExerciseType? ExerciseType { get; set; }

    [Required]
    [Display(Name = "Obciążenie (kg)")]
    [Range(0.0, 2000.0, ErrorMessage = "Obciążenie musi być w zakresie od 0 do 2000 kg.")]
    public double Weight { get; set; }

    [Required]
    [Display(Name = "Liczba serii")]
    [Range(1, 50, ErrorMessage = "Liczba serii musi być w zakresie od 1 do 50.")]
    public int Sets { get; set; }

    [Required]
    [Display(Name = "Liczba powtórzeń w serii")]
    [Range(1, 200, ErrorMessage = "Liczba powtórzeń musi być w zakresie od 1 do 200.")]
    public int Reps { get; set; }

    [Display(Name = "Notatki")]
    [StringLength(500, ErrorMessage = "Notatki mogą mieć maksymalnie 500 znaków.")]
    public string? Notes { get; set; }

    [NotMapped]
    [Display(Name = "Łączna liczba powtórzeń")]
    public int TotalReps => Sets * Reps;
}
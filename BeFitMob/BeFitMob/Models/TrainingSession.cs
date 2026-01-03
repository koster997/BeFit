using System.ComponentModel.DataAnnotations;

namespace BeFitMob.Models;

public class TrainingSession
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Data i godzina rozpoczęcia")]
    public DateTime StartTime { get; set; }

    [Required]
    [Display(Name = "Data i godzina zakończenia")]
    public DateTime EndTime { get; set; }

    [Required]
    [Display(Name = "Id użytkownika (wewnętrzne)")]
    [StringLength(50)]
    public string UserId { get; set; } = string.Empty;

    public ICollection<PerformedExercise> PerformedExercises { get; set; } = new List<PerformedExercise>();

    public override string ToString() => $"{StartTime:dd.MM.yyyy HH:mm} - {EndTime:HH:mm}";
}
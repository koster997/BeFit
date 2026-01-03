using System.ComponentModel.DataAnnotations;

namespace BeFit.Models;

public class ExerciseType
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Nazwa ćwiczenia")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nazwa ćwiczenia musi mieć od 2 do 100 znaków.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Opis ćwiczenia")]
    [StringLength(500, ErrorMessage = "Opis może mieć maksymalnie 500 znaków.")]
    public string? Description { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
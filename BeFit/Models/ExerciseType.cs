using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class ExerciseType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)] // maksymalna długość
        public string Name { get; set; } = string.Empty;
    }
}
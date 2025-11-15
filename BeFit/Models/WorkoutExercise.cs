using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Models
{
    public class WorkoutExercise
    {
        public int Id { get; set; }

        [Display(Name = "Rodzaj ćwiczenia")]
        public int ExerciseTypeId { get; set; }
        public ExerciseType ExerciseType { get; set; }

        [Display(Name = "Sesja treningowa")]
        public int WorkoutSessionId { get; set; }
        public WorkoutSession WorkoutSession { get; set; }

        [Display(Name = "Obciążenie (kg)")]
        [Range(0, 1000)]
        public double Weight { get; set; }

        [Display(Name = "Serie")]
        [Range(1, 20)]
        public int Sets { get; set; }

        [Display(Name = "Powtórzenia")]
        [Range(1, 50)]
        public int Repetitions { get; set; }

        // Użytkownik wykonujący ćwiczenie
        public string? ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class WorkoutExercise
    {
        public int Id { get; set; }

        [Display(Name = "Rodzaj ćwiczenia")]
        public int ExerciseTypeId { get; set; }

        [Display(Name = "Sesja treningowa")]
        public int WorkoutSessionId { get; set; }

        [Display(Name = "Obciążenie (kg)")]
        public double Weight { get; set; }

        [Display(Name = "Serie")]
        public int Sets { get; set; }

        [Display(Name = "Powtórzenia w serii")]
        public int Repetitions { get; set; }

        public ExerciseType ExerciseType { get; set; }
        public WorkoutSession WorkoutSession { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Models
{
    public class WorkoutExercise
    {
        public int Id { get; set; }

        // 🔗 Relacja do typu ćwiczenia
        public int ExerciseTypeId { get; set; }
        public ExerciseType ExerciseType { get; set; }

        // 🔗 Relacja do sesji
        public int WorkoutSessionId { get; set; }
        public WorkoutSession WorkoutSession { get; set; }

        // 🏋️ Dane treningowe
        [Range(0, 1000)]
        public double Weight { get; set; } // kg

        [Range(1, 20)]
        public int Sets { get; set; }

        [Range(1, 50)]
        public int Repetitions { get; set; }
    }
}
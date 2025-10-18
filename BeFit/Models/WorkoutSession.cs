using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class WorkoutSession
    {
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        // ewentualnie walidacja (opcjonalna):
        // public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        // {
        //     if (EndTime < StartTime)
        //         yield return new ValidationResult("Data zakończenia nie może być wcześniejsza niż rozpoczęcia.");
        // }
    }
}
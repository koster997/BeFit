using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Models
{
    public class WorkoutSession
    {
        public int Id { get; set; }

        [Display(Name = "Data rozpoczęcia")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Display(Name = "Data zakończenia")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        // Użytkownik, który wykonał trening
        public string? ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
namespace BeFit.Models
{
    public class ExerciseStats
    {
        public string ExerciseName { get; set; }
        public int Count { get; set; }
        public int TotalRepetitions { get; set; }
        public double AvgWeight { get; set; }
        public double MaxWeight { get; set; }
    }
}
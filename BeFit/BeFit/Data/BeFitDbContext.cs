using Microsoft.EntityFrameworkCore;
using BeFit.Models;

namespace BeFit.Data;

public class BeFitDbContext : DbContext
{
    public BeFitDbContext(DbContextOptions<BeFitDbContext> options) : base(options)
    {
    }

    public DbSet<ExerciseType> ExerciseTypes => Set<ExerciseType>();
    public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
    public DbSet<PerformedExercise> PerformedExercises => Set<PerformedExercise>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TrainingSession>()
            .HasMany(s => s.PerformedExercises)
            .WithOne(e => e.TrainingSession!)
            .HasForeignKey(e => e.TrainingSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExerciseType>()
            .HasMany<PerformedExercise>()
            .WithOne(e => e.ExerciseType!)
            .HasForeignKey(e => e.ExerciseTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
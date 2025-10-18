using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BeFit.Models; // potrzebne, by odwołać się do Twoich modeli

namespace BeFit.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 🔗 Rejestracja modeli w kontekście bazy danych
        public DbSet<ExerciseType> ExerciseTypes { get; set; }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }

        // Opcjonalnie możesz tu nadpisywać model tworzenia, np. relacje, ograniczenia itp.
        // protected override void OnModelCreating(ModelBuilder builder)
        // {
        //     base.OnModelCreating(builder);
        //     // dodatkowa konfiguracja modelu, jeśli potrzebna
        // }
    }
}
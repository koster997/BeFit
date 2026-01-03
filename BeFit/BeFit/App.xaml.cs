using System.Linq;
using BeFit.Data;

namespace BeFit;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        InitializeDatabase();

        MainPage = new AppShell();
    }

    private void InitializeDatabase()
    {
        var context = ServiceHelper.GetService<BeFitDbContext>();

        context.Database.EnsureCreated();

        if (!context.ExerciseTypes.Any())
        {
            context.ExerciseTypes.AddRange(
                new Models.ExerciseType { Name = "Przysiad ze sztangą", Description = "Ćwiczenie na nogi i pośladki." },
                new Models.ExerciseType { Name = "Wyciskanie na ławce", Description = "Ćwiczenie na klatkę piersiową." },
                new Models.ExerciseType { Name = "Martwy ciąg", Description = "Ćwiczenie ogólnorozwojowe." }
            );
            context.SaveChanges();
        }
    }
}
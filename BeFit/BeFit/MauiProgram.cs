using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BeFit.Data;
using BeFit.Services;

namespace BeFit;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Ścieżka do lokalnej bazy SQLite
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "befit.db3");

        // Rejestracja DbContext
        builder.Services.AddDbContext<BeFitDbContext>(options =>
        {
            options.UseSqlite($"Filename={dbPath}");
        });

        // Prosta usługa autoryzacji
        builder.Services.AddSingleton<AuthService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Inicjalizacja helpera do dostępu do DI
        ServiceHelper.Initialize(app.Services);

        return app;
    }
}
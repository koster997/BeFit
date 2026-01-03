using Microsoft.Extensions.Logging;
using BeFitMob.Services;

namespace BeFitMob;

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

        // Rejestracja usług (serwisów)
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<DataService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Inicjalizacja helpera do dostępu do serwisów
        ServiceHelper.Initialize(app.Services);

        return app;
    }
}
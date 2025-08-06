using ElasticLogSender.Data;
using ElasticLogSender.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Scoped servisler (her istekte yeni örnek)
        builder.Services.AddScoped<IElasticService, ElasticService>();
        builder.Services.AddScoped<ILogRepository, LogRepository>();

        // Hosted service (singleton)
        builder.Services.AddHostedService<LogWorker>();

        var host = builder.Build();
        host.Run();
    }
}
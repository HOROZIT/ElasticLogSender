using ElasticLogSender.Data;
using ElasticLogSender.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddWindowsService(options =>
        {
            options.ServiceName = "HrzElasticLogWorker";
        });

        builder.Services.AddScoped<IElasticService, ElasticService>();
        builder.Services.AddScoped<ILogRepository, LogRepository>();

        builder.Services.AddHostedService<LogWorker>();

        var host = builder.Build();
        host.Run();
    }
}
using ElasticLogSender.Data;
using ElasticLogSender.Enums;
using ElasticLogSender.Services;

public class LogWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LogWorker> _logger;

    public LogWorker(IServiceScopeFactory scopeFactory, ILogger<LogWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            var elasticService = scope.ServiceProvider.GetRequiredService<IElasticService>();
            var logRepository = scope.ServiceProvider.GetRequiredService<ILogRepository>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<LogWorker>>();

            try
            {
                var fromTime = DateTime.UtcNow.AddMinutes(-5);

                // Test DB'den logları çek
                var testLogs = (await logRepository.GetRecentLogsAsync(fromTime, DbType.Test)).ToList();
                if (testLogs.Any())
                {
                    await elasticService.SendLogsBulkAsync(testLogs, DbType.Test);
                    var testLogIds = testLogs.Select(x => x.Id).ToList();
                    await logRepository.MarkLogsAsIndexedAsync(testLogIds, DbType.Test);
                }

                // Canlı DB'den logları çek
                var canliLogs = (await logRepository.GetRecentLogsAsync(fromTime, DbType.Canli)).ToList();
                if (canliLogs.Any())
                {
                    await elasticService.SendLogsBulkAsync(canliLogs, DbType.Canli);
                    var canliLogIds = canliLogs.Select(x => x.Id).ToList();
                    await logRepository.MarkLogsAsIndexedAsync(canliLogIds, DbType.Canli);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Log gönderilirken hata oluştu.");
            }
            await Task.Delay(TimeSpan.FromMinutes(4), stoppingToken);
        }
    }
}

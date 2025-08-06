using ElasticLogSender.Enums;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElasticLogSender.Services
{
    // Elasticsearch veri gönderme ve yönetim 6. İşlem
    public class ElasticService : IElasticService
    {
        private readonly HttpClient _httpClient;
        private readonly string _elasticUri;
        private readonly ILogger<ElasticService> _logger;

        public ElasticService(IConfiguration configuration, ILogger<ElasticService> logger)
        {
            _elasticUri = configuration["ElasticSearch:Uri"];
            var username = configuration["ElasticSearch:Username"];
            var password = configuration["ElasticSearch:Password"];

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri(_elasticUri);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var byteArray = System.Text.Encoding.ASCII.GetBytes($"{username}:{password}");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            _logger = logger;
        }


        //Elasticsearch'a log gönderme işlemi
        public async Task<bool> SendLogsBulkAsync(IEnumerable<object> logs, DbType dbType)
        {
            try
            {
                var sb = new StringBuilder();
                var indexName = dbType switch
                {
                    DbType.Test => "apilogtest",
                    DbType.Canli => "apilogcanli",
                    _ => throw new ArgumentException("Geçersiz DbType")
                };


                foreach (var log in logs)
                {
                    sb.AppendLine("{ \"index\" : { \"_index\" : \"" + indexName + "\" } }");

                    var logJson = JsonSerializer.Serialize(log, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    });

                    sb.AppendLine(logJson);
                }

                var bulkPayload = sb.ToString();

                var content = new StringContent(bulkPayload, Encoding.UTF8, "application/x-ndjson");

                //var response = await _httpClient.PostAsync("https://localhost:9200/_bulk", content);
                var response = await _httpClient.PostAsync("_bulk", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    // Hata logla, istersen detaylı işleyebilirsin
                    _logger.LogError("Elasticsearch Bulk API hatası: {Error}", error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Elasticsearch Bulk API exception");
                return false;
            }
        }
    }
}
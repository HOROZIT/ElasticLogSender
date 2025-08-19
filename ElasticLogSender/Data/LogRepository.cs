using Dapper;
using ElasticLogSender.Enums;
using ElasticLogSender.Models;
using Microsoft.Data.SqlClient;

namespace ElasticLogSender.Data
{
    // Dapper ile DB işlemleri 3. İşlem
    public class LogRepository : ILogRepository
    {
        private readonly IConfiguration _configuration;

        public LogRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString(DbType dbType)
        {
            return dbType switch
            {
              DbType.Test => _configuration.GetConnectionString("EcomDbTest"),
              DbType.Canli => _configuration.GetConnectionString("EcomDbCanli"),
                _ => throw new ArgumentException("Geçersiz DbType")
            };
        }

        public async Task<IEnumerable<ApiLog>> GetRecentLogsAsync(DbType dbType)
        {
            using var connection = new SqlConnection(GetConnectionString(dbType));
            await connection.OpenAsync();

            var sql = @"
            SELECT * FROM TBL_API_LOGS
            WHERE IsIndexed = 0";

            var logs = await connection.QueryAsync<ApiLog>(sql);
            return logs;
        }

        //Gönderilen dataların IsIndexed değerini 1 olarak günceller
        public async Task MarkLogsAsIndexedAsync(IEnumerable<int> ids, DbType dbType)
        {
            if (!ids.Any())
                return;

            using var connection = new SqlConnection(GetConnectionString(dbType));
            await connection.OpenAsync();

            var sql = "UPDATE TBL_API_LOGS SET IsIndexed = 1 WHERE Id IN @Ids";
            await connection.ExecuteAsync(sql, new { Ids = ids });
        }
    }
}

using ElasticLogSender.Enums;
using ElasticLogSender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticLogSender.Services
{
    // Elasticsearch ile ilgili operasyonların arayüzü 5. İşlem
    public interface IElasticService
    {
        Task<bool> SendLogsBulkAsync(IEnumerable<object> logs, DbType dbType);
    }
}

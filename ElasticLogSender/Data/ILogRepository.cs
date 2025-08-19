using ElasticLogSender.Enums;
using ElasticLogSender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticLogSender.Data
{
    // Log veri erişim arayüzü (interface)   2. İşlem
    public interface ILogRepository
    {
        Task<IEnumerable<ApiLog>> GetRecentLogsAsync(DbType dbType);
        Task MarkLogsAsIndexedAsync(IEnumerable<int> ids, DbType dbType);
    }
}

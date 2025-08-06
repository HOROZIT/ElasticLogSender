using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticLogSender.Models
{
    // Veritabanı log kaydı modelin  1. İşlem
    public class ApiLog
    {
        public int Id { get; set; }
        public string? Endpoint { get; set; }
        public string? CustomerName { get; set; }
        public string? HttpMethod { get; set; }
        public string? MethodName { get; set; }
        public int StatusCode { get; set; }
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public DateTime Datetime { get; set; } = DateTime.UtcNow;

    }
}

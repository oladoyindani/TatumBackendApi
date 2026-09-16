using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TatumBackendApi.DTOs
{
    public class AdminSummaryDto
    {
        public string Period { get; set;} = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers {get; set; }
        public int TotalAccounts { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal TotalTransactions { get; set; }
    }
}
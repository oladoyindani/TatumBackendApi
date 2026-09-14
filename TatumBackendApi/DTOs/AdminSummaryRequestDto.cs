using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TatumBackendApi.Common.Constants;

namespace TatumBackendApi.DTOs
{
    public class AdminSummaryRequestDto
    {
        public Transaction.TransactionPeriod? Period { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
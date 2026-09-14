using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TatumBackendApi.Common.Constants
{
    public class Transaction
    {
        public enum TransactionPeriod
        {
            [Display(Name = "Yesterday")]
            Yesterday,

            [Display(Name = "Last 7 Days")]
            Last7Days,

            [Display(Name = "Last 30 Days")]
            Last30Days,

            [Display(Name = "Last 90 Days")]
            Last90Days
        }

        public enum TransactionStatus
        {
            Pending = 0,
            Successful = 1,
            Failed = 2,
            Cancelled = 3
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TatumBackendApi.Entities;
using static TatumBackendApi.Common.Constants.Transaction;

namespace TatumBackendApi.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string? UserName { get; set; }

        public Guid AccountId { get; set; }

        public string? AccountNumber { get; set; }

        public Guid? BillerId { get; set; }

        public string? BillerName { get; set; }

        public string? BillerCode { get; set; }

        public Guid? ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? ProductCategory { get; set; }

        public Guid? ProductItemId { get; set; }

        public string? ProductItemName { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Reference { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }

    public class TransactionFilterDto
    {
        public Guid? TransactionId { get; set; }
        public Guid? AccountId { get; set; }

        public string? AccountNumber { get; set; }

        public Guid? UserId { get; set; }

        public Guid? BillerId { get; set; }

        public Guid? ProductId { get; set; }

        public TransactionStatus? Status { get; set; }

        public ProductCategory? Category { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }

    public class TransactionSummaryFilterDto
    {
        public TransactionPeriod? Period { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
using MaidLinker.Data.Entites;
using static MaidLinker.Enums.SharedEnum;

namespace MaidLinker.Models
{
    public class FinancialReportViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int CompletedCount { get; set; }
        public int PreparedCount { get; set; }

        public List<Request> FilteredRequests { get; set; } = new List<Request>();

        public List<FinancialEntry> FinancialEntries { get; set; } = new List<FinancialEntry>();

        public decimal TotalIncome => FinancialEntries.Where(e => e.Type == FinancialEntryType.Income).Sum(e => e.Amount);
        public decimal TotalExpenses => FinancialEntries.Where(e => e.Type == FinancialEntryType.Expense).Sum(e => e.Amount);
        public decimal Profit => TotalIncome - TotalExpenses;
    }

  

    public class RequestItemViewModel
    {
        public int RequestId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime ContractSignedDate { get; set; }
        public string ContractSignedDateFormatted => ContractSignedDate.ToString("yyyy-MM-dd HH:mm:ss");
        public string AttachmentUrl { get; set; }
        public string Status { get; set; } // Completed or Prepared
    }

    public class FinancialTransactionViewModel
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // income or expense
    }
}

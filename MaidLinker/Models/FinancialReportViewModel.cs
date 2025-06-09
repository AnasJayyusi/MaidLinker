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
}

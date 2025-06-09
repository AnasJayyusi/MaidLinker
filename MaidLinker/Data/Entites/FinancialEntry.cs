using MaidLinker.Models;
using System.ComponentModel.DataAnnotations;
using static MaidLinker.Enums.SharedEnum;

namespace MaidLinker.Data.Entites
{
    public class FinancialEntry
    {
        public int Id { get; set; }

        [MaxLength(256)]
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public FinancialEntryType Type { get; set; } // Income or Expense

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

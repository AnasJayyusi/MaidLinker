using System.ComponentModel.DataAnnotations.Schema;

namespace MaidLinker.Models
{
    public class InvoiceViewModel
    {
        public string CustomerName { get; set; }
        public string InvoiceDate { get; set; }
        public List<InvoiceItem> Items { get; set; }
        public decimal AmountLeft { get; set; }     // المبلغ المتبقي من العميل
    }
    public class InvoiceItem
    {
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }    // إجمالي مبلغ الخدمة
        public decimal AmountPaid { get; set; }     // المبلغ المدفوع من العميل

    }
}

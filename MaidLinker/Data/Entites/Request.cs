using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using static MaidLinker.Enums.SharedEnum;

namespace MaidLinker.Data.Entites
{
    public class Request
    {
        public int Id { get; set; }
        public int MaidId { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        [MaxLength(10)]
        public string Phone { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string? ServedByUserId { get; set; } // FK to AspNetUsers.Id
        public string? FollowByUsername { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.New;
        public string? Notes { get; set; }

        // Navigation properties
        public Maid Maid { get; set; }
        public IdentityUser? ServedByUser { get; set; }
        public bool ContractSigned { get; set; }
        public DateTime? SignedDate { get; set; }  // Nullable because it may not be signed yet
        [NotMapped]
        public string SignedDateFormatted => SignedDate.HasValue
            ? SignedDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            : string.Empty;

        [NotMapped]
        public string RequestDateFormatted => RequestDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        
        public string? ContractFilePath { get; set; }
        public WorkFlowStatus? WorkFlowStatus { get; set; } = Enums.SharedEnum.WorkFlowStatus.NotStarted;

        // الحقول الجديدة
        public decimal TotalAmount { get; set; }    // إجمالي مبلغ الخدمة
        public decimal AmountPaid { get; set; }     // المبلغ المدفوع من العميل

        // حساب المبلغ المتبقي بشكل غير مخزن في قاعدة البيانات (غير مخزن)
        [NotMapped]
        public decimal AmountLeft => TotalAmount - AmountPaid;

    }
}


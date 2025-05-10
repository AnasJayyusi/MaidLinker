using MaidLinker.Data.Entites;
using MaidLinker.Enums;
using static MaidLinker.Data.SharedEnum;

namespace MaidLinker.Models
{
    public class ReferralOrderDetailModal
    {
        public int OrderId { get; set; }
        public string ReferralRequestNumber { get; set; }
        public int ReferralRequestId { get; set; }
        public ReferralStatusEnum Status { get; set; }
        public string CreatedBy { get; set; }
        public string AssignedTo { get; set; }
        public string Date { get; set; }
        public string PatientName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string CountryTextAr { get; set; }
        public string CountryTextEn { get; set; }
        public string StateTextAr { get; set; }
        public string StateTextEn { get; set; }
        public string CityTextAr { get; set; }
        public string CityTextEn { get; set; }
        public Gender Gender { get; set; }
        public Age Age { get; set; }
        public string ChronicDisease { get; set; }
        public string RejectionReason { get; set; }

    }
}

using MaidLinker.Enums;

namespace MaidLinker.Models
{
    public class ReferralOrderStatus
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public ReferralStatusEnum Status { get; set; }
    }
}

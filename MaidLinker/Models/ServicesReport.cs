using MaidLinker.Enums;

namespace MaidLinker.Models
{
    public class ServicesReport
    {
        public int Id { get; set; }
        public string CreatedByUserName { get; set; }
        public string AssignedToUserName { get; set; }
        public string ServiceNameAr { get; set; }
        public string ServiceNameEn { get; set; }
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public ServicesStatusEnum Status { get; set; }
        public double Fee { get; set; }
        public double Price { get; set; }
    }
}

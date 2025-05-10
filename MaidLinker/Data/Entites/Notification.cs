using System.ComponentModel.DataAnnotations.Schema;

namespace MaidLinker.Data.Entites
{
    public class Notification
    {
        public int Id { get; set; }
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; } 
        public string MessageAr { get; set; }
        public string MessageEn { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreationDate { get; set; }
        [ForeignKey(nameof(CreatedByUser))]
        public int? CreatedByUserId { get; set; }
        [ForeignKey(nameof(AssignedToUser))]
        public int AssignedToUserId { get; set; }
        public UserProfile CreatedByUser { get; set; } 
        public UserProfile AssignedToUser { get; set; }
        [NotMapped]
        public string CreationDateFormatted { get; set; }
    }
}

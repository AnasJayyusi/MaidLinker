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
       
        public string? CreatedByUserId { get; set; }
        public string AssignedToUserId { get; set; }
       
        [NotMapped]
        public string CreationDateFormatted { get; set; }
    }
}

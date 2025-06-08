using static MaidLinker.Enums.SharedEnum;

namespace MaidLinker.Data.Entites
{
    public class Attachment
    {
        public int Id { get; set; }

        public int MaidId { get; set; }         
        public Maid Maid { get; set; }             

        public string FileName { get; set; }
        public string FilePath { get; set; }       
        public AttachmentType AttachmentType { get; set; } // e.g. "Medical", "Residency", "Passport" ,"Other"

        public DateTime UploadedAt { get; set; }
    }
}

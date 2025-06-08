using static MaidLinker.Enums.SharedEnum;

namespace MaidLinker.Dto
{
    public class AttachmentDto
    {
        public int MaidId { get; set; }

        public IFormFile? MedicalFile { get; set; }
        public IFormFile? ResidencyFile { get; set; }
        public IFormFile? PassportFile { get; set; }
        public IFormFile? OtherFile { get; set; }
    }
}

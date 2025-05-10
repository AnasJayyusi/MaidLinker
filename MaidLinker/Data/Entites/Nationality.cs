using System.ComponentModel.DataAnnotations;

namespace MaidLinker.Data.Entites
{
    public class Nationality
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string TitleEn { get; set; }

        [Required]
        [MaxLength(100)]
        public string TitleAr { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<Maid> Users { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace MaidLinker.Data.Entites
{
    public class Langauge
    {
        public int Id { get; set; }
        [MaxLength(250)]
        public string Name { get; set; } // e.g., "English", "Arabic"
        [MaxLength(4)]
        public string Code { get; set; } // e.g., "en", "ar"
        public List<Maid> Maids{ get; set; }
    }
}

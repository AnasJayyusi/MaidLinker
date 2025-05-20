using System.ComponentModel.DataAnnotations;

namespace MaidLinker.Data.Entites
{
    public class Language
    {
        public int Id { get; set; }
        [MaxLength(250)]
        public string NameAr { get; set; } 
        public string NameEn { get; set; } 
        [MaxLength(4)]
        public List<Maid> Maids{ get; set; }
    }
}

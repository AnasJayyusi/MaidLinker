using System.ComponentModel.DataAnnotations.Schema;

namespace MaidLinker.Data.Entites
{
    public class City
    {
        public int Id { get; set; }
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }

        [ForeignKey(nameof(Country))]
        public int? CountryId { get; set; }
        public Country Country { get; set; }
    }
}

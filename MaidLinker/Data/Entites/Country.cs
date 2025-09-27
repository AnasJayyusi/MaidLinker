using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace MaidLinker.Data.Entites
{
    public class Country
    {
        public int Id { get; set; }
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public List<Maid> Maids { get; set; }
        [NotMapped]
        public string Title
        {
            get
            {
                var culture = CultureInfo.CurrentCulture.Name;

                // Check if the culture starts with "ar" for Arabic
                if (culture.StartsWith("ar", StringComparison.OrdinalIgnoreCase))
                {
                    return TitleAr;
                }
                else
                {
                    return TitleEn;
                }
            }
        }
    }
}

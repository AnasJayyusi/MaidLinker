using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

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

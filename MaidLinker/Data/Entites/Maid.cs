using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Net.Mail;
using static MaidLinker.Data.SharedEnum;

namespace MaidLinker.Data.Entites
{
    public class Maid
    {
        public int Id { get; set; }
        public string? ImagePath { get; set; }
        public string? VideoURL { get; set; }

        #region English Name
        [MaxLength(250)]
        public string FirstNameEn { get; set; }
        [MaxLength(250)]
        public string SecondNameEn { get; set; }
        [MaxLength(250)]
        public string ThirdNameEn { get; set; }
        [MaxLength(250)]
        public string LastNameEn { get; set; }
        #endregion

        #region Arabic Name
        [MaxLength(250)]
        public string FirstNameAr { get; set; }
        [MaxLength(250)]
        public string SecondNameAr { get; set; }
        [MaxLength(250)]
        public string ThirdNameAr { get; set; }
        [MaxLength(250)]
        public string LastNameAr { get; set; }
        #endregion
        public double TotalExperience { get; set; }

        public List<Country> ServedCountries { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public int Childs { get; set; }
        public List<Language> Langauges { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Note { get; set; }



        #region Navagations
        public int NationalityId { get; set; }

        [ForeignKey("NationalityId")]
        public Nationality Nationality { get; set; }
        public List<Attachment> Attachments { get; set; }
        #endregion

        [NotMapped]
        public string FullName
        {
            get
            {
                var isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar";

                if (isArabic)
                {
                    return string.Join(" ", new[] { FirstNameAr, SecondNameAr, ThirdNameAr, LastNameAr }
                        .Where(s => !string.IsNullOrWhiteSpace(s)));
                }
                else
                {
                    return string.Join(" ", new[] { FirstNameEn, SecondNameEn, ThirdNameEn, LastNameEn }
                        .Where(s => !string.IsNullOrWhiteSpace(s)));
                }
            }
        }

    }
}



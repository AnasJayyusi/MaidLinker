using System.ComponentModel.DataAnnotations;
using static MaidLinker.Enums.SharedEnum;

namespace MaidLinker.Models
{
    public class MaidDto
    {
        public int? Id { get; set; }
        public string? VideoURL { get; set; }

        public string FirstNameEn { get; set; }
        public string SecondNameEn { get; set; }
        public string ThirdNameEn { get; set; }
        public string LastNameEn { get; set; }

        public string FirstNameAr { get; set; }
        public string SecondNameAr { get; set; }
        public string ThirdNameAr { get; set; }
        public string LastNameAr { get; set; }

        public double TotalExperience { get; set; }
        public List<int> ServedCountryIds { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public int Childs { get; set; }
        public List<int> LanguageIds { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:mm/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        public string Note { get; set; }

        public int NationalityId { get; set; }

        public IFormFile? ImagePath { get; set; }
    }
}

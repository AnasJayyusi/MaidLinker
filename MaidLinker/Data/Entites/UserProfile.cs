using System.ComponentModel.DataAnnotations;
using static MaidLinker.Data.SharedEnum;

namespace MaidLinker.Data.Entites
{
    public class UserProfile
    {

        public int Id { get; set; }
        #region  Synced With AspNetUser
        public string? UserId { get; set; }
        public string? FullName { get; set; }
        public int? AccountTypeId { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        #endregion

      
        #region Editable Fields
        public string? ProfilePicturePath { get; set; }

        [MaxLength(4096)]
        public string? Bio { get; set; }
        #endregion

        #region Navigations 
        public PractitionerType? PractitionerType { get; set; }
        public Country? Country { get; set; }
        #endregion

        public ProfileStatus ProfileStatus { get; set; }
    }
}


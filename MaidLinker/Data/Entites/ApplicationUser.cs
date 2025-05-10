using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaidLinker.Data.Entites
{
    public class ApplicationUser : IdentityUser
    {

        public string? FullName { get; set; }

        [ForeignKey(nameof(AccountType))]
        public int? AccountTypeId { get; set; }
        public AccountType? AccountType { get; set; }

    }
}

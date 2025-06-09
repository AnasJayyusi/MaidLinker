using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaidLinker.Data.Entites
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}

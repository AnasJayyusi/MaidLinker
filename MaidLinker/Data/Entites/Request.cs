using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static MaidLinker.Enums.SharedEnum;

namespace MaidLinker.Data.Entites
{
    public class Request
    {
        public int Id { get; set; }
        public int MaidId { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        [MaxLength(10)]
        public string Phone { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string? ServedByUserId { get; set; } // FK to AspNetUsers.Id
        public RequestStatus Status { get; set; } =  RequestStatus.New;
        public string? Notes { get; set; }

        // Navigation properties
        public Maid Maid { get; set; }
        public IdentityUser? ServedByUser { get; set; }
        public bool ContractSigned { get; set; }
        public DateTime? SignedDate { get; set; }  // Nullable because it may not be signed yet
        public string? ContractFilePath { get; set; }
    }
}

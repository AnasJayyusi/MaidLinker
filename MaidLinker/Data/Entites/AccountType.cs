namespace MaidLinker.Data.Entites
{
    public class AccountType
    {
        public int Id { get; set; }
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public bool IsActive { get; set; }
        public List<ApplicationUser> ApplicationUsers { get; set; }
    }
}

namespace MaidLinker.Data.Entites
{
    public class Country
    {
        public int Id { get; set; }
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public List<Maid> Maids { get; set; }
    }
}

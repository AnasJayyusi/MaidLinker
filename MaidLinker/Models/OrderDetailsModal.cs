namespace MaidLinker.Models
{
    public class OrderDetailsModal
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string ChronicDisease { get; set; }
        public string SelectedServicesIds { get; set; }
        public string DoctorId { get; set; }
        public string Quantities { get; set; }
    }
}

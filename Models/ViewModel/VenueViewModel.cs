namespace CldvPOEnew.Models.ViewModel
{
    public class VenueViewModel
    {
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}

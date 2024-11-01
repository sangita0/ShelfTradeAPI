namespace ServicesForShelfSwap.Models
{
    public class AddBookRequest
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Condition { get; set; }
        public string AvailabilityStatus { get; set; }
        public int UserId { get; set; } // Accepts UserId as string from the request
    }

}

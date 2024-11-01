public class Book
{
    public int BookId { get; set; } // Assuming this is an auto-incremented primary key
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public string Condition { get; set; }
    public string AvailabilityStatus { get; set; }
    public int UserId { get; set; } // Ensure this is also an int
    public string? Location { get; set; } // Nullable Location
    public DateTime? CreatedAt { get; set; } // Nullable DateTime
    public DateTime? UpdatedAt { get; set; } // Nullable DateTime
}
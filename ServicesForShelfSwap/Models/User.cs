namespace ServicesForShelfSwap.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FavouriteGenre { get; set; }
        public string ReadingPreferences { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // New properties for password reset
        public string? ResetToken { get; set; } // Nullable string for ResetToken
        public DateTime? ResetTokenExpiry { get; set; } // Nullable DateTime for ResetTokenExpiry
    }
}

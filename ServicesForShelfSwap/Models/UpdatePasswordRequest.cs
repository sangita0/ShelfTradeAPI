namespace ServicesForShelfSwap.Models
{
    public class UpdatePasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

}

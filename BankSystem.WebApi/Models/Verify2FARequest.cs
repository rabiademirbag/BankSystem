namespace BankSystem.WebApi.Models
{
    public class Verify2FARequest
    {
        public int UserId { get; set; }
        public string Code { get; set; }
    }
}

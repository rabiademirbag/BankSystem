using BankSystem.Data.Enums;

namespace BankSystem.WebApi.Models
{
    public class AddAccountRequest
    {
        public AccountType AccountType { get; set; }
    }
}

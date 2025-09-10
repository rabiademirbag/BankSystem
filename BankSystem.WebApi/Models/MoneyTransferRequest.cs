using BankSystem.Data.Enums;

namespace BankSystem.WebApi.Models
{
    public class MoneyTransferRequest
    {
        public decimal Amount { get; set; }
        public string ReceiverAccountNo { get; set; }
        public string ReceiverFullName { get; set; }
        public string Description { get; set; }
    }
}

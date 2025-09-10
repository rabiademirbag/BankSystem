using BankSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.Operations.Account.Dtos
{
    public class AccountsInfoDto
    {
        public string AccountNo { get; set; }
        public int UserId { get; set; }
        public string AccountOwnerName { get; set; }
        public decimal Balance { get; set; }
        public AccountType  AccountType { get; set; }
        public List<TransactionDto> Transactions { get; set; }
    }
    public class TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ReceiverFullName { get; set; }
    }
}

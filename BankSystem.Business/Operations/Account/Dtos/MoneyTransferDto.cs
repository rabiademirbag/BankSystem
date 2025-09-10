using BankSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.Operations.Account.Dtos
{
    public class MoneyTransferDto
    {
        public decimal Amount { get; set; }
        public string ReceiverAccountNo { get; set; }
        public string ReceiverFullName { get; set; }
        public string Description { get; set; }
    }
}

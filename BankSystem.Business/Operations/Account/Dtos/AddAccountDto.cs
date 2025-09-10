using BankSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.Operations.Account.Dtos
{
    public class AddAccountDto
    {
        public AccountType AccountType { get; set; }
    }
}

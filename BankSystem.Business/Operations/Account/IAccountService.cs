using BankSystem.Business.Operations.Account.Dtos;
using BankSystem.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.Operations.Account
{
    public interface IAccountService
    {
        Task<List<AccountsInfoDto>> GetAccounts();
        Task<ServiceMessage> AddAccount(AddAccountDto account, int userId);
        Task<ServiceMessage<decimal>> GetBalance(int id);
        Task<List<TransactionDto>> GetTransactions(int id);
        Task<ServiceMessage> MoneyTransfer(MoneyTransferDto moneyTransferDto, int id);
        Task<ServiceMessage> SetDefaultAccount(int id, int userId);
    }
}

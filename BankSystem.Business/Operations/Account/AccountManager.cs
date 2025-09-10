using BankSystem.Business.Operations.Account.Dtos;
using BankSystem.Business.Operations.User;
using BankSystem.Business.Types;
using BankSystem.Data.Entities;
using BankSystem.Data.Enums;
using BankSystem.Data.Repositories;
using BankSystem.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.Operations.Account
{
    public class AccountManager : IAccountService
    {
        private readonly IRepository<AccountEntity> _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TransactionEntity> _transactionRepository;
        public AccountManager(IRepository<AccountEntity> accountRepository , IUnitOfWork unitOfWork, IRepository<TransactionEntity> transactionRepository)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _transactionRepository = transactionRepository;
        }

        public async Task<ServiceMessage> AddAccount(AddAccountDto account, int userId)
        {
            var userAccounts = await _accountRepository.GetAll(x => x.UserId == userId).ToListAsync();


            var accountEntity = new AccountEntity
            {
                AccountType = account.AccountType,
                AccountNo = await GenerateAccountNumberAsync(),
                Balance = 0,
                UserId = userId,
                IsDefault = !userAccounts.Any()
            };

            await _accountRepository.AddAsync(accountEntity);
            try
            {
               await _unitOfWork.SaveChangesAsync();

            } catch (Exception) 
            { 
                throw new Exception("Hesap oluşturulurken bir hata meydana geldi");
            }
            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Hesap başarıyla oluşturuldu"
            };
        }
        private async Task<string> GenerateAccountNumberAsync()
        {
            string accountNo;
            do
            {
                accountNo = Guid.NewGuid().ToString("N").Substring(0, 10);
            } while ((_accountRepository.GetAll(a => a.AccountNo == accountNo)).Any());

            return accountNo;
        }

        public Task<List<AccountsInfoDto>> GetAccounts()
        {
            var accounts = _accountRepository.GetAll().Include(x => x.Transactions).Include(x => x.User).Select(x => new AccountsInfoDto
            {
                AccountNo = x.AccountNo,
                AccountOwnerName = $"{x.User.FirstName}  {x.User.LastName}",
                Balance = x.Balance,
                AccountType = x.AccountType,
                UserId = x.UserId,
                Transactions = x.Transactions.Select(t => new TransactionDto
                {
                    TransactionDate = t.TransactionDate,
                    TransactionType = t.TransactionType,
                    Description = t.Description,
                    Amount = t.Amount,
                    ReceiverFullName = t.ReceiverFullName,
                    Id = t.Id
                }).ToList()
            }).ToListAsync();
            if(accounts is null)
            {
                return null;
            }
            return accounts;
        }

        public async Task<ServiceMessage<decimal>> GetBalance(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if(account is null)
            {
                return new ServiceMessage<decimal>
                {
                    IsSucceed = false,
                    Message = "Hesap bulunamadı"

                };
            }
            var balance =account.Balance;
            return new ServiceMessage<decimal>
            {
                IsSucceed = true,
                Data = balance
            };

        }

        public async Task<List<TransactionDto>> GetTransactions(int id)
        {
            var account =await _accountRepository.GetAll(x=>x.Id==id).Include(x=>x.Transactions).FirstOrDefaultAsync(); 
            
            if(account is null)
            {
                return null;
            }
            var transactions =account.Transactions.Select(x=> new TransactionDto
            {
                TransactionDate=x.TransactionDate,
                TransactionType = x.TransactionType,
                Description = x.Description,
                Amount = x.Amount,
                ReceiverFullName = x.ReceiverFullName,
                Id = x.Id
            }).ToList();

            return transactions;
        }

        public async Task<ServiceMessage> MoneyTransfer(MoneyTransferDto moneyTransferDto, int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account is null)
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Hesap bulunamadı"
                };
            if(account.Balance < moneyTransferDto.Amount)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Hesabınızda yeterli bakiye yok"
                };
            }
            var receiver = await _accountRepository.GetAsync(x=>x.AccountNo==moneyTransferDto.ReceiverAccountNo);
            if(receiver is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Alıcı hesap bulunamadı"
                };
            }

            var transactionEntity = new TransactionEntity
            {
                Amount = moneyTransferDto.Amount,
                Description = moneyTransferDto.Description,
                AccountId = id,
                ReceiverAccountNo = moneyTransferDto.ReceiverAccountNo,
                ReceiverFullName = moneyTransferDto.ReceiverFullName,
                TransactionDate = DateTime.Now,
                TransactionStatus = TransactionStatus.Success,
                TransactionType=TransactionType.TransferOutGoing
            };
            var transactionEntity2 = new TransactionEntity
            {
                Amount = moneyTransferDto.Amount,
                Description = moneyTransferDto.Description,
                AccountId = receiver.Id,
                ReceiverAccountNo = receiver.AccountNo,
                ReceiverFullName = moneyTransferDto.ReceiverFullName,
                TransactionDate = DateTime.Now,
                TransactionStatus = TransactionStatus.Success,
                TransactionType = TransactionType.TransferInGoing
            };
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                account.Balance -= moneyTransferDto.Amount;
                receiver.Balance += moneyTransferDto.Amount;
                await _accountRepository.UpdateAsync(account);
                await _accountRepository.UpdateAsync(receiver);
                await _transactionRepository.AddAsync(transactionEntity);
                await _transactionRepository.AddAsync(transactionEntity2);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }

            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw new Exception("İşlem sırasında bir hata oluştu");
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "İşlem başarıyla gerçekleştirildi"
            };
        }

        public async Task<ServiceMessage> SetDefaultAccount(int id, int userId)
        {
            var account = await _accountRepository.GetByIdAsync(id);

            if(account is null || account.UserId != userId)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Hesap bulunamadı"
                };
            }
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var userAccounts = await _accountRepository.GetAll(x => x.UserId == userId).ToListAsync();
                foreach(var acc in userAccounts)
                {
                    acc.IsDefault = false;
                    await _accountRepository.UpdateAsync(acc);
                }

                account.IsDefault = true;
                await _accountRepository.UpdateAsync(account);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();


            }catch(Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
            return new ServiceMessage
            {
                IsSucceed = true,
                Message = $"'{account.AccountNo}' numaralı hesap varsayılan olarak ayarlandı"
            };
        }
    }
}

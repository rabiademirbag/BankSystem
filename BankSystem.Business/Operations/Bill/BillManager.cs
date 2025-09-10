using BankSystem.Business.Operations.Bill.Dtos;
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

namespace BankSystem.Business.Operations.Transaction
{
    public class BillManager : IBillService
    {
        private readonly IRepository<BillEntity> _billRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AccountEntity> _accountRepository;
        private readonly IRepository<TransactionEntity> _transactionRepository;
        public BillManager(IRepository<BillEntity> billRepository, IUnitOfWork unitOfWork, IRepository<AccountEntity> accountRepository, IRepository<TransactionEntity> transactionRepository)
        {
            _billRepository = billRepository;
            _unitOfWork = unitOfWork;  
            _accountRepository= accountRepository;
            _transactionRepository= transactionRepository;
        }

        public async Task<ServiceMessage> Add(AddBillDto billDto)
        {
            var bill = await _billRepository.GetAll(x => x.BillNo == billDto.BillNo).ToListAsync();

            if (bill.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu fatura zaten ekli"
                };
            }
            var billEntity = new BillEntity
            {
                BillNo = billDto.BillNo,
                BillType = billDto.BillType,
                Amount = billDto.Amount,
                DueDate = billDto.DueDate,
                IsPaid = billDto.IsPaid,
                UserId = billDto.UserId,

            };

            await _billRepository.AddAsync(billEntity);
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Fatura kaydı sırasında bir hata oluştu");
            };

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Fatura başarıyla kaydedildi"
            };
        }

        public async Task<ServiceMessage> PayBill(int id, int userId)
        {
            var bill = await _billRepository.GetAll(x=>x.Id==id).Include(x=>x.User).Include(x=>x.User.Accounts).FirstOrDefaultAsync();
            if(bill is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Fatura bulunamadı"
                };
            }
            if (bill.IsPaid)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Fatura zaten ödenmiş"
                };
            }
            var defaultAccount = await _accountRepository.GetAsync(x => x.UserId == userId && x.IsDefault);

        if(defaultAccount is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Varsayılan hesap bulunamadı"
                };
            }
            if (defaultAccount.Balance < bill.Amount)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Hesabınızda yeterli bakiye yok"
                };
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                defaultAccount.Balance -= bill.Amount;
                await _accountRepository.UpdateAsync(defaultAccount);

                bill.IsPaid = true;
                bill.PaidDate = DateTime.Now;
                await _billRepository.UpdateAsync(bill);

                var transactionEntity = new TransactionEntity
                {
                    Amount = bill.Amount,
                    Description = $"{bill.BillType} faturası ödendi",
                    AccountId = defaultAccount.Id,
                    ReceiverAccountNo = "BILL-PAY",
                    ReceiverFullName = "Fatura ödeme sistemi",
                    TransactionDate = DateTime.Now,
                    TransactionStatus = TransactionStatus.Success,
                    TransactionType = TransactionType.BillPayment
                };
                await _transactionRepository.AddAsync(transactionEntity);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                
                
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Fatura başarıyla ödendi"
            };
        }
    }
}

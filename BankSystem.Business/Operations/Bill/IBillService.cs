using BankSystem.Business.Operations.Bill.Dtos;
using BankSystem.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.Operations.Transaction
{
    public interface IBillService
    {
        Task<ServiceMessage> PayBill(int id, int userId);
        Task<ServiceMessage> Add(AddBillDto billDto);
    }
}

using BankSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.Operations.Bill.Dtos
{
    public class AddBillDto
    {
        public BillType BillType { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string BillNo { get; set; }
        public bool IsPaid { get; set; }
        public int UserId { get; set; }
    }
}

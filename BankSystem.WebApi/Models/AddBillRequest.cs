using BankSystem.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankSystem.WebApi.Models
{
    public class AddBillRequest
    {
        [Required]
        public BillType BillType { get; set; }
        [Required]
        [Range(0.01,double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalı")]
        public decimal Amount { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public string BillNo { get; set; }
        [Required]
        public bool IsPaid { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}

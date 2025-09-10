using BankSystem.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities
{
    public class TransactionEntity : BaseEntity
    {
        public TransactionType TransactionType { get; set; }
        public TransactionStatus TransactionStatus { get; set; } = TransactionStatus.Pending;
        public decimal Amount { get; set; }
        public string ReceiverAccountNo { get; set; }
        public string ReceiverFullName { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public int AccountId { get; set; }
        public AccountEntity Account { get; set; }
    }
    public class TransactionConfiguration : BaseConfiguration<TransactionEntity>
    {
        public override void Configure(EntityTypeBuilder<TransactionEntity> builder)
        {
            builder.Property(x => x.ReceiverAccountNo).IsRequired().HasMaxLength(20);
            builder.Property(x => x.ReceiverFullName).IsRequired().HasMaxLength(200);
            builder.Property(x=>x.Description).IsRequired().HasMaxLength(200);
            builder.Property(x => x.TransactionType).IsRequired();
            builder.Property(x => x.TransactionStatus).IsRequired();
            builder.Property(x => x.AccountId).IsRequired();
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();

            base.Configure(builder);
        }
    }
}

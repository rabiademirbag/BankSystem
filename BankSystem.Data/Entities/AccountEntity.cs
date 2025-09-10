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
    public class AccountEntity : BaseEntity
    {
        public string AccountNo { get; set; }
        public decimal Balance { get; set; }
        public AccountType AccountType { get; set; }
        public bool IsDefault { get; set; } = false;

        public int UserId { get; set; }
        public UserEntity User { get; set; }
        public ICollection<TransactionEntity> Transactions { get; set; }
    }
    public class AccountConfiguration : BaseConfiguration<AccountEntity>
    {
        public override void Configure(EntityTypeBuilder<AccountEntity> builder)
        {
            builder.HasIndex(x => x.AccountNo).IsUnique();
            builder.Property(x => x.AccountNo).IsRequired().HasMaxLength(20);
            builder.Property(x => x.AccountType).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Balance).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.IsDefault).IsRequired();
            base.Configure(builder);
        }
    }
}

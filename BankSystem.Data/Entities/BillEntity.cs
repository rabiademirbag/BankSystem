using BankSystem.Data.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities
{
    public class BillEntity : BaseEntity
    {
        public BillType BillType { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public string BillNo { get; set; }
        public bool IsPaid { get; set; }
        public int UserId { get; set; }
        public UserEntity User { get; set; }

        public class BillConfiguration : BaseConfiguration<BillEntity>
        {
            public override void Configure(EntityTypeBuilder<BillEntity> builder)
            {
                builder.Property(x => x.BillNo).IsRequired().HasMaxLength(20);
                builder.Property(x=>x.Amount).IsRequired();
                builder.Property(x => x.BillType).IsRequired();
                builder.Property(x => x.DueDate).IsRequired();
                builder.Property(x => x.UserId).IsRequired();
                builder.Property(x => x.IsPaid).IsRequired();
                builder.Property(x => x.PaidDate).IsRequired(false);
                builder.HasIndex(x => x.BillNo).IsUnique();
                base.Configure(builder);
            }
        }
    }
}

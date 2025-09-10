using BankSystem.Data.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities
{
    public class UserEntity : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
        public string PhoneNumber { get; set; }
        public string? TwoFactorCode { get; set; }
        public DateTime? TwoFactorExpireTime { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public ICollection<SecurityEntity> Securities { get; set; }
        public ICollection<AccountEntity> Accounts { get; set; }
        public ICollection<BillEntity> Bills { get; set; }
    }
    public class UserConfiguration : BaseConfiguration<UserEntity>
    {
        public override void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            builder.Property(x=>x.NationalId).IsRequired().HasMaxLength(11);
            builder.Property(x => x.Email).IsRequired();
            builder.HasIndex(x=>x.Email).IsUnique();
            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.PhoneNumber).IsRequired();
            builder.HasIndex(x => x.NationalId).IsUnique();
            builder.Property(x=>x.TwoFactorCode).IsRequired(false);
            builder.Property(x => x.TwoFactorExpireTime).IsRequired(false);

            base.Configure(builder);
        }
    }
}

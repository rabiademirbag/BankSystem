using BankSystem.Data.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities
{
    public class SecurityEntity : BaseEntity
    {
        public int UserId { get; set; }
        public UserEntity User { get; set; }
        public SecurityActionType ActionType { get; set; }
        public string IpAddress { get; set; }
        public DateTime ActionDate { get; set; }

    }
    public class SecurityConfiguration : BaseConfiguration<SecurityEntity>
    {
        public override void Configure(EntityTypeBuilder<SecurityEntity> builder)
        {
            builder.Property(x => x.ActionType).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.IpAddress).IsRequired();

            base.Configure(builder);
        }
    }
}

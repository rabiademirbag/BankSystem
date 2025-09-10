using BankSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BankSystem.Data.Entities.BillEntity;

namespace BankSystem.Data.Context
{
    public class BankSystemDbContext : DbContext
    {
        public BankSystemDbContext(DbContextOptions<BankSystemDbContext> options):base(options) 
        { 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new BillConfiguration());
            modelBuilder.ApplyConfiguration(new SecurityConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<AccountEntity> Accounts => Set<AccountEntity>();
        public DbSet<BillEntity> Bills => Set<BillEntity>();
        public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<SecurityEntity> Securities => Set<SecurityEntity>(); 

    }
}

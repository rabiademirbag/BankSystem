using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable 
    {
        Task BeginTransactionAsync();
        Task RollBackAsync();
        Task CommitAsync();

        Task<int> SaveChangesAsync();

    }
}

using FAP.Common.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
namespace FAP.Common.Application.Interfaces

    {
        public interface IUnitOfWork
        {
            ITermRepository Terms { get; }
            Task BeginTransactionAsync();
            Task CommitTransactionAsync();
            Task RollbackTransactionAsync();
            void BeginScope(TransactionScopeOption option = TransactionScopeOption.Required);
            void CompleteScope();
            Task<int> SaveChangesAsync(CancellationToken cancellation);
            
        }
    }



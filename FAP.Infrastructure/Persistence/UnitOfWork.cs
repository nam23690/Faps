using FAP.Common.Application.Interfaces;
using FAP.Common.Infrastructure.Persistence;
using FAP.Common.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;



namespace FAP.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UniversityDbContext _context;
        private IDbContextTransaction? _transaction;
        private TransactionScope? _scope;
        //Thêm Repository ở đây   
        private ITermRepository? _terms;

        public UnitOfWork(UniversityDbContext context)
        {
            _context = context;
        }

        //Khởi tạo các Repository ở đây
        public ITermRepository Terms =>
            _terms ??= new TermRepository(_context);

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null) return; // Tránh mở 2 transaction

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction to commit");

            try
            {
                await _context.SaveChangesAsync(default);
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null) return;

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public void BeginScope(
            TransactionScopeOption option = TransactionScopeOption.Required)
        {
            _scope = new TransactionScope(
                option,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted
                },
                TransactionScopeAsyncFlowOption.Enabled
            );
        }

        public void CompleteScope()
        {
            if (_scope == null)
                throw new InvalidOperationException("No TransactionScope started");

            _scope.Complete();
            _scope.Dispose();
            _scope = null;
        }
        //Lưu thay đổi
        public Task<int> SaveChangesAsync(CancellationToken cancellation)
            => _context.SaveChangesAsync(cancellation);

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }


}

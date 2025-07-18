using Microsoft.EntityFrameworkCore.Storage;
using Pawfectmatch_V._1.Data;

namespace Pawfectmatch_V._1.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private IPetRepository? _petRepository;
        private IAdoptionApplicationRepository? _adoptionApplicationRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IPetRepository Pets => _petRepository ??= new PetRepository(_context);

        public IAdoptionApplicationRepository AdoptionApplications => 
            _adoptionApplicationRepository ??= new AdoptionApplicationRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
} 
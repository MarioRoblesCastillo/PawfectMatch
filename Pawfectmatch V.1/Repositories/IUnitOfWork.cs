namespace Pawfectmatch_V._1.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IPetRepository Pets { get; }
        IAdoptionApplicationRepository AdoptionApplications { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
} 
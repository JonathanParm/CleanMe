namespace CleanMe.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CommitAsync();
    }
}
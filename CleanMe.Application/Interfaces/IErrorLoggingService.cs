namespace CleanMe.Application.Interfaces
{
    public interface IErrorLoggingService
    {
        Task LogErrorAsync(Exception ex, string requestPath, string? applicationUserId = null);
    }
}
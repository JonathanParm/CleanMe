using Microsoft.Extensions.Logging;
using CleanMe.Infrastructure.Data;
using CleanMe.Domain.Entities;
using CleanMe.Application.Interfaces;

namespace CleanMe.Infrastructure.Logging
{
    public class ErrorLoggingService : IErrorLoggingService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ErrorLoggingService> _logger;

        public ErrorLoggingService(ApplicationDbContext dbContext, ILogger<ErrorLoggingService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task LogErrorAsync(Exception ex, string requestPath, string? applicationUserId = null)
        {
            try
            {
                var errorLog = new ErrorExceptionsLog
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available",
                    Source = ex.Source ?? "Unknown",
                    RequestPath = requestPath,
                    ApplicationUserId = applicationUserId,
                    OccurredAt = DateTime.UtcNow
                };

                await _dbContext.ErrorExceptionsLogs.AddAsync(errorLog);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception logEx)
            {
                _logger.LogError(logEx, "Failed to log error to ErrorExceptionsLog.");
            }
        }
    }
}
using CleanMe.Domain.Entities;
using CleanMe.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace CleanMe.Web.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");

                await LogExceptionToDatabase(dbContext, context, ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task LogExceptionToDatabase(ApplicationDbContext dbContext, HttpContext context, Exception ex)
        {
            var errorLog = new ErrorExceptionsLog
            {
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Source = ex.Source,
                RequestPath = context.Request.Path
            };

            dbContext.ErrorExceptionsLogs.Add(errorLog);
            await dbContext.SaveChangesAsync();
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = _env.IsDevelopment()
                ? new { Message = exception.Message, StackTrace = exception.StackTrace }
                : new { Message = "An unexpected error occurred. Please try again later.", StackTrace = "" };

            var jsonResponse = JsonSerializer.Serialize(errorResponse);

            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await response.WriteAsync(jsonResponse);
        }
    }
}
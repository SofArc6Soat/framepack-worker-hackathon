using Core.WebApi.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Core.WebApi.GlobalErrorMiddleware
{
    [ExcludeFromCodeCoverage]
    public class GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context);

                logger.LogError("Error: {Message}", ex.Message);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(new BaseApiResponse()
            {
                Success = false,
                Errors = [$"HTTP StatusCode {context.Response.StatusCode} - Internal Server Error."]
            }.ToString());
        }
    }
}
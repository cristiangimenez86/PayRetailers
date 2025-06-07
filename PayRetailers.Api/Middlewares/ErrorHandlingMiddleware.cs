using System.Net;
using System.Text.Json;

namespace PayRetailers.Api.Middlewares;

public class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> logger,
    IWebHostEnvironment env)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            context.Response.ContentType = "application/json";

            int statusCode;
            string message;

            switch (ex)
            {
                case KeyNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = ex.Message;
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = new Dictionary<string, object>
            {
                ["message"] = message
            };

            if (env.IsDevelopment())
            {
                response["details"] = ex.ToString();
            }

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
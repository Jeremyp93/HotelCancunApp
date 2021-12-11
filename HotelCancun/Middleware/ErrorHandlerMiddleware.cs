using System.Net;
using System.Text.Json;

namespace HotelCancun.Middleware;

/// <summary>
/// Error handler middleware to return correct status codes based on exception
/// </summary>
public class ErrorHandlerMiddleware
{
    // RequestDelegate denotes a HTTP Request completion
    private readonly RequestDelegate _next;
    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    // Try-catch block over the request delegate. It means that whenever there is an exception of any type in the pipeline for the current request, control goes to the catch block.
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (System.Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, System.Exception exception)
    {
        context.Response.ContentType = "application/json";
        switch (exception)
        {
            case ReservationNotFoundException e:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
        }
        var result = JsonSerializer.Serialize(new
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message
        });
        await context.Response.WriteAsync(result);
    }
}


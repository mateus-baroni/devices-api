using Devices.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Devices.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        
        if (exception is DeviceUpdateConflictException or
            DeviceDeletionConflictException)
        {
            _logger.LogWarning(
                "Device operation rejected: {Message}",
                exception.Message);
        }
        else
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing {HttpMethod} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }

        var statusCode = exception switch
        {
            DeviceUpdateConflictException => StatusCodes.Status409Conflict,
            DeviceDeletionConflictException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode == StatusCodes.Status409Conflict
                ? "Operation could not be completed."
                : "An unexpected error occurred."
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}
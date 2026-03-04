using System.Net;
using System.Text.Json;
using B2BPlatform.Shared.Models.Commons;
using Microsoft.AspNetCore.Diagnostics;

namespace B2BPlatform.API.Controllers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var code = HttpStatusCode.InternalServerError; 
        var errorCode = "500";
        var message = exception.Message;
        
        switch (exception)
        {
            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                errorCode = "401";
                break;
            case KeyNotFoundException:
                code = HttpStatusCode.NotFound;
                errorCode = "404";
                break;
            case ApplicationException:
            case NotImplementedException:
                code = HttpStatusCode.NotImplemented;
                errorCode = "501";
                break;
        }

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)code;
        
        var response = new ServiceBaseResponse
        {
            Status = new ServiceStatus
            {
                Code = string.Empty,
                Message = string.Empty,
                Error = new ServiceError
                {
                    ErrorCode =  errorCode,
                    ErrorMessage = message
                }
            }
        };
        
        var json = JsonSerializer.Serialize(response, SerializerOptions);
        
        await httpContext.Response.WriteAsync(json, cancellationToken);
        
        return true;
    }
}
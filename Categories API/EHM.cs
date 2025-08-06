// Error Handling Middleware
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

public class GlobalExceptionHandler : ExceptionHandler
{
    public override void Handle(ExceptionHandlerContext context)
    {
        var exception = context.Exception;
        
        var response = new
        {
            Success = false,
            Message = "An unexpected error occurred",
            ErrorCode = "INTERNAL_SERVER_ERROR",
            Timestamp = DateTime.UtcNow
        };

        context.Result = new ResponseMessageResult(
            context.Request.CreateResponse(HttpStatusCode.InternalServerError, response));
    }
}

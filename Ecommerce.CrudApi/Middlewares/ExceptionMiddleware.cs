using Ecommerce.Shared.Exceptions;
using System.Net;

namespace Ecommerce.CrudApi.Middlewares;

public sealed class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                type = "validation_error",
                code = ex.Code,
                message = ex.Message,
                errors = ex.Errors
            });
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            await context.Response.WriteAsJsonAsync(new
            {
                type = "domain_error",
                code = ex.Code,
                message = ex.Message
            });
        }
    }
}

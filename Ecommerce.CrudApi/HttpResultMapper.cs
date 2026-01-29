using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.CrudApi
{
    public static class HttpResultMapper
    {
        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess) return new NoContentResult();

            return result.ErrorCode switch
            {
                ErrorCodes.NOT_FOUND => new NotFoundObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage }),
                ErrorCodes.VALIDATION => new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage }),
                ErrorCodes.DOMAIN => new ConflictObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage }),
                _ => new ObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage }) { StatusCode = 400 }
            };
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess) return new OkObjectResult(result.Value);
            return Result.Failure(result.ErrorCode ?? ErrorCodes.VALIDATION, result.ErrorMessage ?? "Error").ToActionResult();
        }
    }
}

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dummy.api.Infrastructure.ActionFilters
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var errorResponse = new
            {
                Description = context.Exception.Message,
            };

            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = MapToStatusCode(context.Exception),
            };
        }

        private static int MapToStatusCode(Exception e)
        {
            switch (e)
            {
                default: return 500;
            }
        }
    }
}
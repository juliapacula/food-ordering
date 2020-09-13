using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Server.Services
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (!(context.Exception is HttpResponseException exception))
            {
                return;
            }

            context.Result = new ObjectResult(exception.Value)
            {
                StatusCode = exception.Status,
            };

            context.ExceptionHandled = true;
        }
    }

    public class HttpResponseException : Exception
    {
        public int Status { get; set; } = 500;

        public object Value { get; set; }
    }
}
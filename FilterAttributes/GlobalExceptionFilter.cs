using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastWXCallBackend.FilterAttributes
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            Logger.Error(context.Exception.Message);
            Models.Result result = new Models.Result();
            result.Code = 1;
            result.Data = context.Exception.Message;
            context.Result = new JsonResult(result);
            context.ExceptionHandled = true;
        }
    }
}

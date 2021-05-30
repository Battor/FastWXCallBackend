using FastWXCallBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastWXCallBackend.Controllers
{
    public class TemplateController : ControllerBase
    {
        public Result result = new Result()
        {
            Code = 0,
            Data = null,
            Total = 0,
            Rows = null
        };
    }
}

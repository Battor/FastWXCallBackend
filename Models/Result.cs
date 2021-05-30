using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastWXCallBackend.Models
{
    public class Result
    {
        public byte Code { get; set; }
        public object Data { get; set; }
        public int Total { get; set; }
        public object[] Rows { get; set; }
    }
}

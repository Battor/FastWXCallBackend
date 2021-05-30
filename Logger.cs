using log4net;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace FastWXCallBackend
{
    public class Logger
    { 
        private static ILog log = LogManager.GetLogger("logger", typeof(Logger));

        public static void Info(object message)
        {
            log.Info(message);
        }

        public static void Debug(object message)
        {
            log.Debug(message);
        }

        public static void Error(object message)
        {
            log.Error(message);
        }

        public static void Fatal(object message)
        {
            log.Fatal(message);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfLib.Helpers.Implement
{
    public class LoggerBase : ILogger
    {
        public void Log(object message)
        {
            Console.WriteLine($"输出:[ {message} ]");
        }

        public void Debug(object message)
        {
            Console.WriteLine($"调试:[ {message} ]");
        }

        public void Info(object message)
        {
            Console.WriteLine($"提示:[ {message} ]");
        }

        public void Warn(object message)
        {
            Console.WriteLine($"警告:[ {message} ]");
        }

        public void Error(object message)
        {
            Console.WriteLine($"错误:[ {message} ]");
        }

        public void Fatal(object message)
        {
            Console.WriteLine($"严重:[ {message} ]");
        }
    }
}

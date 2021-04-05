using System;
using LazyConsole;

namespace AppSentinel.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            LazyConsole.LazyConsole.StartConsole(typeof(AppSentinelConsole));
        }
    }
}

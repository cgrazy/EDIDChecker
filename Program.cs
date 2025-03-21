
using System;

namespace EDIDChecker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var edidChecker = new EDIDChecker(args);
            edidChecker.Run();
        }
    }
}

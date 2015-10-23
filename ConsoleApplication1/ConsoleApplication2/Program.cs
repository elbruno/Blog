using System;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
            Console.WriteLine(GetName());
            Console.WriteLine(GetName2());
        }

        static string GetName2() => @"El " + @"Bruno - " + DateTime.Now.Year;





        static string GetName()
        {
            return @"El " + @"Bruno - " + DateTime.Now.Year;
        }
    }
}

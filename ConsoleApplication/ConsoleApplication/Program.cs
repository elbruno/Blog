
using System;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
            {
                i++;
                Console.WriteLine("i:" + i);
            }
            Console.ReadLine();
        }
    }
}

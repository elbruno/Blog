using System;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new People();
            var address = p.Address;
            Console.WriteLine("Variables");
            Console.WriteLine(nameof(p));         // p
            Console.WriteLine(nameof(p.Address)); // Address
            Console.WriteLine(nameof(p.Email));   // EMail
            Console.WriteLine(nameof(address));   // address
            Console.WriteLine("");
            Console.WriteLine("Members");
            Console.WriteLine(nameof(p.GetCompleteName)); // GetCompleteName
            Console.WriteLine(nameof(string.GetType));    // GetType
            Console.ReadLine();
        }
    }

    class People
    {
        public string Email { get; set; }
        public string Address { get; set; }

        public string GetCompleteName()
        {
            return string.Empty;
        }
    }
}

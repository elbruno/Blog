using System;
using System.Net;

namespace ConsoleApplicationStringFormatParams
{
    class Program
    {
        static void Main(string[] args)
        {
            var constString = @"api/Area?id={0}&name={1}&description={2}&background={3}";
            string[] pars = {"1", "newName", "new description", "bg"};
            var ret = FormatHttpUri(constString, pars);
            Console.WriteLine(ret);

            string[] pars2 = { "1", "newName", "new ^description", "#bg#" };
            ret = FormatHttpUri(constString, pars2);
            Console.WriteLine(ret);

            Console.ReadLine();
        }

        private static string FormatHttpUri(string baseUri, params string[] parameters)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                parameters[i] = WebUtility.UrlEncode(parameter);
            }
            var uri = string.Format(baseUri, parameters);
            return uri;
        }
    }
}

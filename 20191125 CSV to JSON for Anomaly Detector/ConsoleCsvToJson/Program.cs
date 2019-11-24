using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ConsoleCsvToJson
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = CsvToJson(@"data\product-sales.csv");
            Console.WriteLine(json);
            Console.ReadLine();
        }

        static string CsvToJson(string csvPath, string granularity = "daily", bool hasHeaders = true)
        {
            char[] fieldSeparator = { ',' };
            var lines = System.IO.File.ReadAllLines(csvPath);

            // remove header
            if (hasHeaders)
                lines = lines.Skip(1).ToArray();

            // build series
            var arraySeries = new JArray();
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                var fields = line.Split(fieldSeparator);

                var jsonSerie = new JObject
                {
                    ["timestamp"] = fields[0],
                    ["value"] = fields[1]
                };

                arraySeries.Add(jsonSerie);
            }

            var jobjectMain = new JObject
            {
                ["granularity"] = granularity,
                ["series"] = arraySeries,
            };

            var jsonComplete = new JArray {jobjectMain};
            return jsonComplete.ToString();
        }

        static string CsvToJsonRaw(string csvPath, string granularity = "daily", bool hasHeaders = true)
        {
            char[] fieldSeparator = { ',' };
            var jsonCompleteRef = @"{""granularity"": ""granv"", ""series"": [jsonSeries]}";
            var jsonSerieRef = @"{""timestamp"": ""dt"",""value"": fv},";

            var lines = System.IO.File.ReadAllLines(csvPath);

            // remove header if necessary
            if (hasHeaders)
                lines = lines.Skip(1).ToArray();

            // build series
            var jsonSeries = string.Empty;
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                var fields = line.Split(fieldSeparator);
                var jsonLine = jsonSerieRef.Replace("dt", fields[0]).Replace("fv", fields[1]);
                jsonSeries += jsonLine;
            }
            if (!string.IsNullOrEmpty(jsonSeries))
                jsonSeries = jsonSeries.Remove(jsonSeries.Length - 1);

            // build full json
            var json = jsonCompleteRef.Replace("granv", granularity).Replace("jsonSeries", jsonSeries);

            Console.WriteLine(json);
            return json;
        }
    }
}

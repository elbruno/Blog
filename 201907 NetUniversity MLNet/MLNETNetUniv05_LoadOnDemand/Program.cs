using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;

namespace MLNETNetUniv06_LoadOnDemand
{
    class Program
    {
        private static ITransformer _trainedModel;
        private static PredictionEngine<SentimentIssue, SentimentPrediction> _predEngine;
        private static MLContext _mlContext;
        private static string _modelFileName = @"Models\sentimentAnalysis.zip";

        static void Main(string[] args)
        {
            string dirToWatch = GetAbsolutePath("Models");
            var fsw = new FileSystemWatcher(dirToWatch);
            fsw.Changed += Fsw_Changed;
            fsw.EnableRaisingEvents = true;

            _mlContext = new MLContext(1);
            LoadModel();

            while (true)
            {
                Console.WriteLine("Enter input for analysis. Type [exit] to exit app.");
                string line = Console.ReadLine();
                if (line == "exit") break;
                Predict(line);
                Console.WriteLine(" character(s)");
            }
        }
        
        private static void Fsw_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            Console.WriteLine("New model detected!");
            _trainedModel = null;
            LoadModel();
        }

        static void LoadModel()
        {
            _mlContext = new MLContext(1);
            _trainedModel = _mlContext.Model.Load(_modelFileName, out var modelInputSchema);
            _predEngine = _mlContext.Model.CreatePredictionEngine<SentimentIssue, SentimentPrediction>(_trainedModel);
        }

        public static void Predict(string text)
        {
            var testSentiment = new SentimentIssue { Text = text };

            var resultprediction = _predEngine.Predict(testSentiment);
            Console.WriteLine($"Text: {testSentiment.Text}");
            Console.WriteLine($"  -- Prediction: {(Convert.ToBoolean(resultprediction.Prediction) ? "Toxic" : "Good")}");
            Console.WriteLine($"  -- Probability of being toxic: {resultprediction.Probability} ");
            Console.WriteLine();
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;
            string fullPath = Path.Combine(assemblyFolderPath, relativePath);
            return fullPath;
        }
    }
    public class SentimentIssue
    {
        [LoadColumn(0)]
        public bool Label { get; set; }
        [LoadColumn(2)]
        public string Text { get; set; }
    }
    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}

using System;
using System.Linq;
using Common;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;

namespace MLNETModelBuilderConsole01
{
    class Program
    {
        private const uint ExperimentTime = 180;

        static void Main(string[] args)
        {
            var mlContext = new MLContext();
            Train(mlContext);
            Console.WriteLine("Process complete! Press any key to close the app.");
            Console.ReadKey();
        }

        public static void Train(MLContext mlContext)
        {
            try
            {
                // STEP 1: Load the data
                var trainData = mlContext.Data.LoadFromTextFile(path: "AgeRangeData03_AgeGenderLabelEncodedMoreData.csv",
                        columns: new[]
                        {
                            new TextLoader.Column("Age", DataKind.Single, 0),
                            new TextLoader.Column("Gender", DataKind.Single, 1)
                            ,
                            new TextLoader.Column("Label", DataKind.Single, 2)
                        },
                        hasHeader: true,
                        separatorChar: ','
                        );

                var progressHandler = new MulticlassExperimentProgressHandler();

                ConsoleHelper.ConsoleWriteHeader("=============== Running AutoML experiment ===============");
                Console.WriteLine($"Running AutoML multiclass classification experiment for {ExperimentTime} seconds...");
                ExperimentResult<MulticlassClassificationMetrics> experimentResult = mlContext.Auto()
                    .CreateMulticlassClassificationExperiment(ExperimentTime)
                    .Execute(trainData, "Label", progressHandler: progressHandler);

                // Print top models found by AutoML
                Console.WriteLine();
                PrintTopModels(experimentResult);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void PrintTopModels(ExperimentResult<MulticlassClassificationMetrics> experimentResult)
        {
            // Get top few runs ranked by accuracy
            var topRuns = experimentResult.RunDetails
                .Where(r => r.ValidationMetrics != null && !double.IsNaN(r.ValidationMetrics.MicroAccuracy))
                .OrderByDescending(r => r.ValidationMetrics.MicroAccuracy).Take(3);

            Console.WriteLine("Top models ranked by accuracy --");
            ConsoleHelper.PrintMulticlassClassificationMetricsHeader();
            for (var i = 0; i < topRuns.Count(); i++)
            {
                var run = topRuns.ElementAt(i);
                ConsoleHelper.PrintIterationMetrics(i + 1, run.TrainerName, run.ValidationMetrics, run.RuntimeInSeconds);
            }
        }

    }
}

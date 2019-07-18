using Common;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;

namespace MLNETUniv07_ModelEval
{
    class Program
    {
        private static PredictionEngine<SentimentIssue, SentimentPrediction> _predEngine;

        static void Main(string[] args)
        {
            var mlContext = new MLContext(1);

            var data = mlContext.Data.LoadFromTextFile<SentimentIssue>("wikiDetoxAnnotated40kRows.tsv", hasHeader: true);

            // Split data 80 / 20
            var trainTestSplit = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
            var trainingData = trainTestSplit.TrainSet;
            var testData = trainTestSplit.TestSet;

            var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText("Features", "Text");
            var trainer = mlContext.BinaryClassification.Trainers.SdcaLogisticRegression("Label", "Features");
            var trainingPipeline = dataProcessPipeline.Append(trainer);

            // train using training data
            var trainedModel = trainingPipeline.Fit(trainingData);

            // eval
            var predictions = trainedModel.Transform(testData);
            var metrics = mlContext.BinaryClassification.Evaluate(data: predictions, labelColumnName: "Label", scoreColumnName: "Score");

            ConsoleHelper.PrintBinaryClassificationMetrics(trainer.ToString(), metrics);

            _predEngine = mlContext.Model.CreatePredictionEngine<SentimentIssue, SentimentPrediction>(trainedModel);
            Predict("This is not a good movie");
            Predict("I hate this movie");

            Console.ReadLine();
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

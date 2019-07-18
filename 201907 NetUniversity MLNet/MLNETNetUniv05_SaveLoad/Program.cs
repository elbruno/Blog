using Microsoft.ML;
using Microsoft.ML.Data;
using System;

namespace MLNETNetUniv05_SaveLoad
{
    class Program
    {
        static void Main(string[] args)
        {
            var mlContext = new MLContext(1);

            var data = mlContext.Data.LoadFromTextFile<SentimentIssue>("wikiDetoxAnnotated40kRows.tsv", hasHeader: true);
            var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText("Features", "Text");
            var trainer = mlContext.BinaryClassification.Trainers.SdcaLogisticRegression("Label", "Features");
            var trainingPipeline = dataProcessPipeline.Append(trainer);
            var trainedModel = trainingPipeline.Fit(data);

             //mlContext.Model.Save(trainedModel, data.Schema, "sentimentAnalysis.zip");
            //var trainedModel = mlContext.Model.Load("sentimentAnalysis.zip", out var modelInputSchema);

            var predEngine = mlContext.Model.CreatePredictionEngine<SentimentIssue, SentimentPrediction>(trainedModel);
            Predict("This is not a good movie", predEngine);
            Predict("I hate this movie", predEngine);
            Predict("This vacuum is amazing", predEngine);
            Predict("This vacuum sucks a lot", predEngine);

            Console.ReadLine();
        }

        public static void Predict(string text, PredictionEngine<SentimentIssue, SentimentPrediction> predEngine)
        {
            var testSentiment = new SentimentIssue { Text = text };

            var resultprediction = predEngine.Predict(testSentiment);
            Console.WriteLine($"Text: {testSentiment.Text} | Prediction: {(Convert.ToBoolean(resultprediction.Prediction) ? "Toxic" : "Good")} | Probability of being toxic: {resultprediction.Probability} ");
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

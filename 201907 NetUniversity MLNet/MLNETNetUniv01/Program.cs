using System;
using System.IO;
using Microsoft.ML;
using MLNetDemo011.Shared;

namespace MLNETNetUniv01
{
    class Program
    {
        static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "AgeRangeData03.csv");

        static void Main(string[] args)
        {

            var ml = new MLContext(1);
            // Read the data into a data view.
            var data = ml.Data.LoadFromTextFile<AgeRange>(TrainDataPath, hasHeader: true, separatorChar: ',');

            // Train
            var pipeline = ml.Transforms.Conversion.MapValueToKey("Label")
                .Append(ml.Transforms.Text.FeaturizeText("GenderFeat", "Gender"))
                .Append(ml.Transforms.Concatenate("Features", "Age", "GenderFeat"))
                .AppendCacheCheckpoint(ml)
                .Append(ml.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = pipeline.Fit(data);
            Console.WriteLine("Model trained");

            // Predict
            var engine = ml.Model.CreatePredictionEngine<AgeRange, AgeRangePrediction>(model);
            PredictSimple("Jeff", 2, "M", engine);
            PredictSimple("Shelley", 9, "F", engine);
            PredictSimple("Jackie", 3, "F", engine);
            PredictSimple("Jim", 5, "M", engine);
            PredictSimple("T1", 13, "F", engine);
            PredictSimple("T2", 15, "M", engine);
            PredictSimple("T3", 33, "F", engine);
            PredictSimple("T4", 25, "M", engine);

            Console.ReadLine();
        }

        private static void PredictSimple(string name, float age, string gender, PredictionEngine<AgeRange, AgeRangePrediction> predictionFunction)
        {
            var example = new AgeRange()
            {
                Age = age,
                Name = name,
                Gender = gender
            };
            var prediction = predictionFunction.Predict(example);
            Console.WriteLine($"Name: {example.Name}\t Age: {example.Age:00}\t Gender: {example.Gender}\t >> Predicted Label: {prediction.Label}");
        }
    }
}

using System;
using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;

namespace MlNetConsole01
{
    class Program
    {
        static void Main(string[] args)
        {
            var agesRangesCsv = "AgeRangeData.csv";
            var pipeline = new LearningPipeline
            {
                new TextLoader<AgeRangeData>(agesRangesCsv, separator: ","),
                new Dictionarizer("Label"),
                new ColumnConcatenator("Features", "AgeStart", "AgeEnd"),
                new StochasticDualCoordinateAscentClassifier(),
                new PredictedLabelColumnOriginalValueConverter {PredictedLabelColumn = "PredictedLabel"}
            };
            var model = pipeline.Train<AgeRangeData, AgeRangePrediction>();

            var prediction = model.Predict(new AgeRangeData()
            {
                AgeStart = 1,
                AgeEnd = 2
            });
            Console.WriteLine($"Predicted age range is: {prediction.PredictedLabels}");

            prediction = model.Predict(new AgeRangeData()
            {
                AgeStart = 7,
                AgeEnd = 7
            });
            Console.WriteLine($"Predicted age range is: {prediction.PredictedLabels}");

            Console.ReadLine();
        }
    }

    public class AgeRangeData
    {
        [Column(ordinal: "0")]
        public float AgeStart;

        [Column(ordinal: "1")]
        public float AgeEnd;

        [Column(ordinal: "2", name: "Label")]
        public string Label;
    }

    public class AgeRangePrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabels;
    }
}

using System;
using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Trainers;

namespace MLNetConsole12
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataPath = "AgeRangeData.csv";
            var env = new LocalEnvironment();
            var reader = TextLoader.CreateReader(env, ctx => (
                    Age: ctx.LoadFloat(1),
                    Label: ctx.LoadText(3)),
                separator: ',', hasHeader: true);
            var trainData = reader.Read(new MultiFileSource(dataPath));

            var classification = new MulticlassClassificationContext(env);
            var learningPipeline = reader.MakeNewEstimator()
                .Append(r => (
                    r.Label,
                    Predictions: classification.Trainers.Sdca
                        (label: r.Label.ToKey(),
                        features: r.Age.AsVector())))
                .Append(r => r.Predictions.predictedLabel.ToValue());

            var model = learningPipeline.Fit(trainData);

            var predictionFunc = model.AsDynamic.MakePredictionFunction<AgeRangeNewApi, AgeRangePredictionNewApi>(env);

            var example = new AgeRangeNewApi()
            {
                Age = 6,
                Name = "John",
                Gender = "M"
            };
            var prediction = predictionFunc.Predict(example);

            Console.WriteLine("prediction: " + prediction.PredictedLabel);
            Console.ReadLine();
        }
    }

    public class AgeRangeNewApi
    {
        [Column(ordinal: "0")]
        public string Name;

        [Column(ordinal: "1")]
        public float Age;

        [Column(ordinal: "2")]
        public string Gender;

        [Column(ordinal: "3")]
        public string Label;
    }

    public class AgeRangePredictionNewApi
    {
        [ColumnName("Data")]
        public string PredictedLabel;
    }
}


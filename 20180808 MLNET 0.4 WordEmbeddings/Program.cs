using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BinaryClassification_SentimentAnalysis;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Models;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;

namespace MLNetConsole09
{
    internal static class Program
    {
        private static PredictionModel<SentimentData, SentimentPrediction> _model;
        private static PredictionModel<SentimentData, SentimentPrediction> _modelWordEmbeddings;
        private static PredictionModel<SentimentData, SentimentPrediction> _modelWordEmbeddingsFastTextWikipedia300D;
        private static PredictionModel<SentimentData, SentimentPrediction> _modelWordEmbeddingsGloVe300D;
        private static PredictionModel<SentimentData, SentimentPrediction> _modelWordEmbeddingsGloVeTwitter200D;
        private static PredictionModel<SentimentData, SentimentPrediction> _modelWordEmbeddingsGloVeSswe;
        private static PredictionModel<SentimentData, SentimentPrediction> _modelWordEmbeddingsGloVe50D;
        private static PredictionModel<SentimentData, SentimentPrediction> _modelWordEmbeddingsGloVeTwitter50D;
        private static string AppPath => Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        private static string TrainDataPath => Path.Combine(AppPath, "datasets", "sentiment-imdb-train.txt");
        private static string TestDataPath => Path.Combine(AppPath, "datasets", "sentiment-yelp-test.txt");
        private static string ModelPath => Path.Combine(AppPath, "SentimentModel.zip");

        private static void Main(string[] args)
        {
            TrainModel();
            TrainModelWordEmbeddings();

            _modelWordEmbeddingsFastTextWikipedia300D = TrainModelWordEmbeddings(WordEmbeddingsTransformPretrainedModelKind.FastTextWikipedia300D);
            _modelWordEmbeddingsGloVe50D = TrainModelWordEmbeddings(WordEmbeddingsTransformPretrainedModelKind.GloVe50D);
            _modelWordEmbeddingsGloVe300D = TrainModelWordEmbeddings(WordEmbeddingsTransformPretrainedModelKind.GloVe300D);
            _modelWordEmbeddingsGloVeTwitter50D = TrainModelWordEmbeddings(WordEmbeddingsTransformPretrainedModelKind.GloVeTwitter50D);
            _modelWordEmbeddingsGloVeTwitter200D = TrainModelWordEmbeddings(WordEmbeddingsTransformPretrainedModelKind.GloVeTwitter200D);
            _modelWordEmbeddingsGloVeSswe = TrainModelWordEmbeddings(WordEmbeddingsTransformPretrainedModelKind.Sswe);

            Evaluate(_model, "normal");
            Evaluate(_modelWordEmbeddings, "using WordEmbeddings");
            Evaluate(_modelWordEmbeddingsFastTextWikipedia300D, "using WordEmbeddings FastTextWikipedia300D");
            Evaluate(_modelWordEmbeddingsGloVe50D, "using WordEmbeddings GloVe50D");
            Evaluate(_modelWordEmbeddingsGloVe300D, "using WordEmbeddings GloVe300D");
            Evaluate(_modelWordEmbeddingsGloVeTwitter50D, "using WordEmbeddings GloVeTwitter50D");
            Evaluate(_modelWordEmbeddingsGloVeTwitter200D, "using WordEmbeddings GloVeTwitter200D");
            Evaluate(_modelWordEmbeddingsGloVeSswe, "using WordEmbeddings Sswe");

            Console.ReadLine();
        }

        public static void TrainModel()
        {
            var pipeline = new LearningPipeline();
            pipeline.Add(new TextLoader(TrainDataPath).CreateFrom<SentimentData>());
            pipeline.Add(new TextFeaturizer("Features", "SentimentText"));
            pipeline.Add(new FastTreeBinaryClassifier() { NumLeaves = 5, NumTrees = 5, MinDocumentsInLeafs = 2 });
            Console.WriteLine("=============== Training model ===============");
            _model = pipeline.Train<SentimentData, SentimentPrediction>();
            Console.WriteLine("=============== End training ===============");
        }

        public static void TrainModelWordEmbeddings()
        {
            var pipeline = new LearningPipeline();
            pipeline.Add(new TextLoader(TrainDataPath).CreateFrom<SentimentData>());
            pipeline.Add(new TextFeaturizer("FeaturesA", "SentimentText") { OutputTokens = true });
            pipeline.Add(new WordEmbeddings(("FeaturesA_TransformedText", "FeaturesB")));
            pipeline.Add(new ColumnConcatenator("Features", "FeaturesA", "FeaturesB"));
            pipeline.Add(new FastTreeBinaryClassifier() { NumLeaves = 5, NumTrees = 5, MinDocumentsInLeafs = 2 });
            Console.WriteLine("=============== Training model with Word Embeddings ===============");
            _modelWordEmbeddings = pipeline.Train<SentimentData, SentimentPrediction>();
            Console.WriteLine("=============== End training ===============");
        }

        public static PredictionModel<SentimentData, SentimentPrediction> TrainModelWordEmbeddings(WordEmbeddingsTransformPretrainedModelKind? modelKind)
        {
            var pipeline = new LearningPipeline
            {
                new TextLoader(TrainDataPath).CreateFrom<SentimentData>(),
                new TextFeaturizer("FeaturesA", "SentimentText") {OutputTokens = true}
            };
            var we = new WordEmbeddings(("FeaturesA_TransformedText", "FeaturesB"))
            {
                ModelKind = modelKind
            };
            pipeline.Add(we);
            pipeline.Add(new ColumnConcatenator("Features", "FeaturesA", "FeaturesB"));
            pipeline.Add(new FastTreeBinaryClassifier() { NumLeaves = 5, NumTrees = 5, MinDocumentsInLeafs = 2 });
            Console.WriteLine("=============== Training model with Word Embeddings ===============");
            var model = pipeline.Train<SentimentData, SentimentPrediction>();
            Console.WriteLine("=============== End training ===============");
            return model;
        }

        private static void Evaluate(PredictionModel<SentimentData, SentimentPrediction> model, string name)
        {
            var testData = new TextLoader(TestDataPath).CreateFrom<SentimentData>();
            var evaluator = new BinaryClassificationEvaluator();
            Console.WriteLine("=============== Evaluating model {0} ===============", name);
            var metrics = evaluator.Evaluate(model, testData);
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.Auc:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("=============== End evaluating ===============");
            Console.WriteLine();
        }
    }
}
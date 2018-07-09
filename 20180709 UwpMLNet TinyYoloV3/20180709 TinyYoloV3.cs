using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// MPSNNGraph

namespace UwpAppYolo01
{
    public sealed class MpsnnGraphModelInput
    {
        public VideoFrame Image { get; set; }
    }

    public sealed class MpsnnGraphModelOutput
    {
        public List<float> Grid { get; set; }
        public MpsnnGraphModelOutput()
        {
            Grid = new List<float>();
            Grid.AddRange(new float[21125]);  // Total size of TinyYOLO output
        }
    }

    public sealed class MpsnnGraphModel
    {
        private LearningModelPreview _learningModel;
        public static async Task<MpsnnGraphModel> CreateMpsnnGraphModel(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            MpsnnGraphModel model = new MpsnnGraphModel();
            model._learningModel = learningModel;
            return model;
        }
        public async Task<MpsnnGraphModelOutput> EvaluateAsync(MpsnnGraphModelInput input) {
            MpsnnGraphModelOutput output = new MpsnnGraphModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(_learningModel);
            binding.Bind("image", input.Image);
            binding.Bind("grid", output.Grid);
            LearningModelEvaluationResultPreview evalResult = await _learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}

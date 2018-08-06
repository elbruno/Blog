using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

namespace UwpAppYolo01
{
    public sealed class TinyYoloV212ModelInput
    {
        public VideoFrame Image { get; set; }
    }

    public sealed class TinyYoloV212ModelOutput
    {
        public List<float> Grid { get; set; }
        public TinyYoloV212ModelOutput()
        {
            Grid = new List<float>();
            Grid.AddRange(new float[21125]);  // Total size of TinyYOLO output
        }
    }

    public sealed class TinyYoloV212Model
    {
        private LearningModelPreview _learningModel;
        public static async Task<TinyYoloV212Model> CreateTinyYoloV212Model(StorageFile file)
        {
            var learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            var model = new TinyYoloV212Model {_learningModel = learningModel};
            return model;
        }
        public async Task<TinyYoloV212ModelOutput> EvaluateAsync(TinyYoloV212ModelInput input) {
            var output = new TinyYoloV212ModelOutput();
            var binding = new LearningModelBindingPreview(_learningModel);
            binding.Bind("image", input.Image);
            binding.Bind("grid", output.Grid);
            var evalResult = await _learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}

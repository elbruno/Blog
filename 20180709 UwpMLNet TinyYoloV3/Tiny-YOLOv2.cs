using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// TinyYoloV2

namespace UwpAppYolo01
{
    public sealed class TinyYoloV2ModelInput
    {
        public VideoFrame Image { get; set; }
    }

    public sealed class TinyYoloV2ModelOutput
    {
        public List<float> Grid { get; set; }
        public TinyYoloV2ModelOutput()
        {
            Grid = new List<float>();
            Grid.AddRange(new float[21125]);  // Total size of TinyYOLO output
        }
    }

    public sealed class TinyYoloV2Model
    {
        private LearningModelPreview _learningModel;
        public static async Task<TinyYoloV2Model> CreateTinyYoloV2Model(StorageFile file)
        {
            var learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            var model = new TinyYoloV2Model {_learningModel = learningModel};
            return model;
        }
        public async Task<TinyYoloV2ModelOutput> EvaluateAsync(TinyYoloV2ModelInput input) {
            var output = new TinyYoloV2ModelOutput();
            var binding = new LearningModelBindingPreview(_learningModel);
            binding.Bind("image", input.Image);
            binding.Bind("grid", output.Grid);
            var evalResult = await _learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}

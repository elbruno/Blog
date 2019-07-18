using Microsoft.ML.Data;

namespace MLNetDemo011.Shared
{
    public class AgeRangePrediction
    {
        [ColumnName("PredictedLabel")]
        public string Label;

        public float[] Score;
    }
}
using System;

namespace ElBruno.PolarH7.Model
{
    public class HrModel
    {
        public HrModel()
        {
            Time = DateTime.Now;
            PreviousMeasureTime = Time;
        }
        public int TimeInt { get; set; }
        public DateTime Time { get; set; }
        public int Stress { get; set; }
        public int HeartRate { get; set; }

        public DateTime PreviousMeasureTime { get; set; }
        public int PreviousMeasureHeartRate { get; set; }

        public int TimeDiffBetweenCurrentAndPreviousMeasure
        {
            get
            {
                var span = Time - PreviousMeasureTime;
                return (int)span.TotalMilliseconds;
            }
        }

        public override string ToString()
        {
            return $"{Time.ToString("hh:mm:ss.ffff")} - {TimeInt} - {HeartRate} - {Stress}";
        }
    }
}

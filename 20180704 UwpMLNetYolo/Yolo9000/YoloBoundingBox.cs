using System.Drawing;

namespace UwpAppYolo01.Yolo9000
{
    class YoloBoundingBox
    {
        public string Label { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public float Height { get; set; }
        public float Width { get; set; }

        public float Confidence { get; set; }

        public RectangleF Rect
        {
            get { return new RectangleF(X, Y, Width, Height); }
        }

        public override string ToString()
        {
            return $"Label: {Label}, X: {X}, Y: {Y}, Height: {Height}, Width: {Width}, Confidence: {Confidence}";
        }
    }
}

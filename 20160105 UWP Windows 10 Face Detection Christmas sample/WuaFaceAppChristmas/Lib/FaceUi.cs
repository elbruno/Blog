using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace WuaFaceAppChristmas.Lib
{
    public class FaceUi
    {
        public static Rectangle ConvertPreviewToUiRectangle(BitmapBounds faceBoxInPreviewCoordinates, CaptureElement cameraViewer, IMediaEncodingProperties previewProperties)
        {
            var result = new Rectangle {StrokeThickness = 4};
            var previewStream = previewProperties as VideoEncodingProperties;
            if (previewStream == null) return result;
            if (previewStream.Width == 0 || previewStream.Height == 0) return result;

            double streamWidth = previewStream.Width;
            double streamHeight = previewStream.Height;

            var previewInUi = GetPreviewStreamRectInControl(previewStream, cameraViewer);

            result.Width = (faceBoxInPreviewCoordinates.Width / streamWidth) * previewInUi.Width;
            result.Height = (faceBoxInPreviewCoordinates.Height / streamHeight) * previewInUi.Height;

            var x = (faceBoxInPreviewCoordinates.X / streamWidth) * previewInUi.Width;
            var y = (faceBoxInPreviewCoordinates.Y / streamHeight) * previewInUi.Height;
            Canvas.SetLeft(result, x);
            Canvas.SetTop(result, y + 20);

            return result;
        }

        public static Image ConvertPreviewToUiHatImage(BitmapBounds faceBoxInPreviewCoordinates, CaptureElement cameraViewer, IMediaEncodingProperties previewProperties, Image imageHat)
        {
            var previewStream = previewProperties as VideoEncodingProperties;
            if (previewStream == null) return imageHat;
            if (previewStream.Width == 0 || previewStream.Height == 0) return imageHat;

            double streamWidth = previewStream.Width;
            double streamHeight = previewStream.Height;

            var previewInUi = GetPreviewStreamRectInControl(previewStream, cameraViewer);

            imageHat.Width = (faceBoxInPreviewCoordinates.Width / streamWidth) * previewInUi.Width;
            imageHat.Height = (faceBoxInPreviewCoordinates.Height / streamHeight) * previewInUi.Height;

            var x = (faceBoxInPreviewCoordinates.X / streamWidth) * previewInUi.Width;
            var y = (faceBoxInPreviewCoordinates.Y / streamHeight) * previewInUi.Height;
            Canvas.SetLeft(imageHat, x);
            Canvas.SetTop(imageHat, y - 20);

            imageHat.Visibility = Visibility.Visible;

            return imageHat;
        }

        public static Rect GetPreviewStreamRectInControl(VideoEncodingProperties previewResolution, CaptureElement previewControl)
        {
            var result = new Rect();

            // In case this function is called before everything is initialized correctly, return an empty result
            if (previewControl == null || previewControl.ActualHeight < 1 || previewControl.ActualWidth < 1 ||
                previewResolution == null || previewResolution.Height == 0 || previewResolution.Width == 0)
            {
                return result;
            }

            var streamWidth = previewResolution.Width;
            var streamHeight = previewResolution.Height;

            // Start by assuming the preview display area in the control spans the entire width and height both (this is corrected in the next if for the necessary dimension)
            result.Width = previewControl.ActualWidth;
            result.Height = previewControl.ActualHeight;

            // If UI is "wider" than preview, letterboxing will be on the sides
            if ((previewControl.ActualWidth / previewControl.ActualHeight > streamWidth / (double)streamHeight))
            {
                var scale = previewControl.ActualHeight / streamHeight;
                var scaledWidth = streamWidth * scale;

                result.X = (previewControl.ActualWidth - scaledWidth) / 2.0;
                result.Width = scaledWidth;
            }
            else // Preview stream is "wider" than UI, so letterboxing will be on the top+bottom
            {
                var scale = previewControl.ActualWidth / streamWidth;
                var scaledHeight = streamHeight * scale;

                result.Y = (previewControl.ActualHeight - scaledHeight) / 2.0;
                result.Height = scaledHeight;
            }

            return result;
        }

    }
}

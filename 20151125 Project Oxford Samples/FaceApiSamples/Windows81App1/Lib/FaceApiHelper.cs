using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows81App1.UserControls;
using Microsoft.ProjectOxford.Face;

namespace Windows81App1.Lib
{
    internal class FaceApiHelper
    {
        public int MaxImageSize => 300;

        public async Task<ObservableCollection<Face>> StartFaceDetection(string selectedFile, StorageFile file, Tuple<int, int> imageInfo, string subscriptionKey)
        {
            var detectedFaces = new ObservableCollection<Face>();

            Debug.WriteLine("Request: Detecting {0}", selectedFile);
            var sampleFile = await StorageFile.GetFileFromPathAsync(selectedFile);
            var fs = await FileIO.ReadBufferAsync(sampleFile);

            using (var stream = fs.AsStream())
            {
                try
                {
                    var client = new FaceServiceClient(subscriptionKey);
                    var faces = await client.DetectAsync(stream, true, true, true);
                    Debug.WriteLine("Response: Success. Detected {0} face(s) in {1}", faces.Length, selectedFile);
                    Debug.WriteLine("{0} face(s) has been detected", faces.Length);

                    foreach (var face in faces)
                    {

                        var fileFaceImage = await FileHelper.SaveFaceImageFile(file, face);

                        var newFace = new Face
                        {
                            ImagePath = selectedFile,
                            Left = face.FaceRectangle.Left,
                            Top = face.FaceRectangle.Top,
                            Width = face.FaceRectangle.Width,
                            Height = face.FaceRectangle.Height,
                            FaceId = face.FaceId.ToString(),
                            Gender = face.Attributes.Gender,
                            Age = face.Attributes.Age,
                            AgeComplete = string.Format("{0:#} years old", face.Attributes.Age),
                            ImageFacePath = fileFaceImage.Path
                        };

                        // calculate rect image
                        newFace = CalculateFaceRectangleForRendering(newFace, MaxImageSize, imageInfo);

                        detectedFaces.Add(newFace);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            return detectedFaces;
        }

        private static Face CalculateFaceRectangleForRendering(Face face, int maxSize, Tuple<int, int> imageInfo)
        {
            var imageWidth = imageInfo.Item1;
            var imageHeight = imageInfo.Item2;
            var newWidth = maxSize;
            var newHeight = maxSize * imageHeight / imageWidth;
            face.RectLeft = newWidth * face.Left / imageWidth; ;
            face.RectTop = newHeight * face.Top / imageHeight; ;
            face.RectHeight = newHeight * face.Height / imageHeight;
            face.RectWidth = newWidth*face.Width/ imageWidth;
            return face;
        }
    }
}
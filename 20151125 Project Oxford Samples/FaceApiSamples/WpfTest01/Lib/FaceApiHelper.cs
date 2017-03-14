using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.ProjectOxford.Face;
using WpfTest01.UserControls;

namespace WpfTest01.Lib
{
    internal class FaceApiHelper
    {
        #region FaceApi

        public int MaxImageSize
        {
            get
            {
                return 300;
            }
        }

        public async Task<Tuple<ObservableCollection<Face>, ObservableCollection<Face>>> StartFaceDetection(string selectedFile, string subscriptionKey)
        {
            var detectedFaces = new ObservableCollection<Face>();
            var facesRect = new ObservableCollection<Face>();

            Debug.WriteLine("Request: Detecting {0}", selectedFile);

            using (var fileStream = File.OpenRead(selectedFile))
            {
                try
                {
                    var client = new FaceServiceClient(subscriptionKey);
                    var faces = await client.DetectAsync(fileStream, false, true, true);
                    Debug.WriteLine("Response: Success. Detected {0} face(s) in {1}", faces.Length, selectedFile);
                    var imageInfo = GetImageInfoForRendering(selectedFile);
                    Debug.WriteLine("{0} face(s) has been detected", faces.Length);

                    foreach (var face in faces)
                    {
                        detectedFaces.Add(item: new Face()
                        {
                            ImagePath = selectedFile,
                            Left = face.FaceRectangle.Left,
                            Top = face.FaceRectangle.Top,
                            Width = face.FaceRectangle.Width,
                            Height = face.FaceRectangle.Height,
                            FaceId = face.FaceId.ToString(),
                            Gender = face.Attributes.Gender,
                            Age = string.Format("{0:#} years old", face.Attributes.Age),
                        });
                    }

                    // Convert detection result into UI binding object for rendering
                    foreach (var face in CalculateFaceRectangleForRendering(faces, MaxImageSize, imageInfo))
                    {
                        facesRect.Add(face);
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                var returnData = new Tuple<ObservableCollection<Face>, ObservableCollection<Face>>(detectedFaces, facesRect);
                return returnData;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Calculate the rendering face rectangle
        /// </summary>
        /// <param name="faces">Detected face from service</param>
        /// <param name="maxSize">Image rendering size</param>
        /// <param name="imageInfo">Image width and height</param>
        /// <returns>Face structure for rendering</returns>
        private static IEnumerable<Face> CalculateFaceRectangleForRendering(IEnumerable<Microsoft.ProjectOxford.Face.Contract.Face> faces, int maxSize, Tuple<int, int> imageInfo)
        {
            var imageWidth = imageInfo.Item1;
            var imageHeight = imageInfo.Item2;
            float ratio = (float)imageWidth / imageHeight;
            int uiWidth;
            int uiHeight;
            if (ratio > 1.0)
            {
                uiWidth = maxSize;
                uiHeight = (int)(maxSize / ratio);
            }
            else
            {
                uiHeight = maxSize;
                uiWidth = (int)(ratio * uiHeight);
            }

            int uiXOffset = (maxSize - uiWidth) / 2;
            int uiYOffset = (maxSize - uiHeight) / 2;
            float scale = (float)uiWidth / imageWidth;

            return faces.Select(face => new Face()
            {
                FaceId = face.FaceId.ToString(),
                Left = (int)((face.FaceRectangle.Left * scale) + uiXOffset),
                Top = (int)((face.FaceRectangle.Top * scale) + uiYOffset),
                Height = (int)(face.FaceRectangle.Height * scale),
                Width = (int)(face.FaceRectangle.Width * scale),
            });
        }

        /// <summary>
        /// Get image basic information for further rendering usage
        /// </summary>
        /// <param name="imageFilePath">Path to the image file</param>
        /// <returns>Image width and height</returns>
        public Tuple<int, int> GetImageInfoForRendering(string imageFilePath)
        {
            try
            {
                using (var s = File.OpenRead(imageFilePath))
                {
                    JpegBitmapDecoder decoder = new JpegBitmapDecoder(s, BitmapCreateOptions.None, BitmapCacheOption.None);
                    var frame = decoder.Frames.First();

                    // Store image width and height for following rendering
                    return new Tuple<int, int>(frame.PixelWidth, frame.PixelHeight);
                }
            }
            catch
            {
                return new Tuple<int, int>(0, 0);
            }
        }

        /// <summary>
        /// Append detected face to UI binding collection
        /// </summary>
        /// <param name="collections">UI binding collection</param>
        /// <param name="path">Original image path, used for rendering face region</param>
        /// <param name="face">Face structure returned from service</param>
        public static void UpdateFace(ObservableCollection<Face> collections, string path, Microsoft.ProjectOxford.Face.Contract.Face face)
        {
            collections.Add(new Face()
            {
                ImagePath = path,
                Left = face.FaceRectangle.Left,
                Top = face.FaceRectangle.Top,
                Width = face.FaceRectangle.Width,
                Height = face.FaceRectangle.Height,
                FaceId = face.FaceId.ToString(),
            });
        }

        #endregion Methods
    }
}
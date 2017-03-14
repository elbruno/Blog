using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using ClientException = Microsoft.ProjectOxford.Face.ClientException;
using Face = ElBruno.ProjectOxford.FaceApiEmotionVisionSample.UserControls.Face;
using Rectangle = Microsoft.ProjectOxford.Common.Rectangle;

namespace ElBruno.ProjectOxford.FaceApiEmotionVisionSample.Lib
{
    internal class ProjectOxfordHelper
    {
        private readonly string _subscriptionKeyFace;
        private readonly string _subscriptionKeyEmotions;
        private readonly string _subscriptionKeyVision;
        public int MaxImageSize => 450;

        public ProjectOxfordHelper(string subscriptionKeyFace, string subscriptionKeyEmotions, string subscriptionKeyVision)
        {
            _subscriptionKeyFace = subscriptionKeyFace;
            _subscriptionKeyEmotions = subscriptionKeyEmotions;
            _subscriptionKeyVision = subscriptionKeyVision;
        }

        public async Task<Tuple<ObservableCollection<Face>, ObservableCollection<Face>>> StartFaceDetection(string selectedFile, bool analyzeEmotion)
        {
            var detectedFaces = new ObservableCollection<Face>();
            var facesRect = new ObservableCollection<Face>();

            Debug.WriteLine("Request: Detecting {0}", selectedFile);

            using (var fileStreamImage = File.OpenRead(selectedFile))
            {
                try
                {
                    var client = new FaceServiceClient(_subscriptionKeyFace);
                    var faces = await client.DetectAsync(fileStreamImage, false, true, true);
                    Debug.WriteLine("Response: Success. Detected {0} face(s) in {1}", faces.Length, selectedFile);
                    var imageInfo = GetImageInfoForRendering(selectedFile);
                    Debug.WriteLine("{0} face(s) has been detected", faces.Length);

                    foreach (var face in faces)
                    {
                        var detectedFace = new Face()
                        {
                            ImagePath = selectedFile,
                            Left = face.FaceRectangle.Left,
                            Top = face.FaceRectangle.Top,
                            Width = face.FaceRectangle.Width,
                            Height = face.FaceRectangle.Height,
                            FaceId = face.FaceId,
                            Gender = face.Attributes.Gender,
                            Age = $"{face.Attributes.Age:#} years old",
                        };
                        detectedFaces.Add(detectedFace);

                    }

                    // Convert detection result into UI binding object for rendering
                    foreach (var face in CalculateFaceRectangleForRendering(faces, MaxImageSize, imageInfo))
                    {
                        facesRect.Add(face);
                    }

                    // update emotions
                    if (analyzeEmotion)
                    {
                        detectedFaces = await UpdateEmotions(selectedFile, detectedFaces);
                        foreach (var faceRect in facesRect)
                        {
                            foreach (var detectedFace in detectedFaces.Where(detectedFace => faceRect.FaceId == detectedFace.FaceId))
                            {
                                faceRect.Scores = detectedFace.Scores;
                            }
                        }
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

        private async Task<ObservableCollection<Face>> UpdateEmotions(string selectedFile, ObservableCollection<Face> faces)
        {
            using (var fileStreamEmotions = File.OpenRead(selectedFile))
            {
                var emotionServiceClient = new EmotionServiceClient(_subscriptionKeyEmotions);
                var emotions = await emotionServiceClient.RecognizeAsync(fileStreamEmotions, faces.Select(
                    face => new Rectangle
                    {
                        Height = face.Height,
                        Left = face.Left,
                        Top = face.Top,
                        Width = face.Width
                    }).ToArray());
                foreach (var emotion in emotions)
                {
                    foreach (var face in faces.Where(face => face.Height == emotion.FaceRectangle.Height &&
                                                             face.Left == emotion.FaceRectangle.Left &&
                                                             face.Top == emotion.FaceRectangle.Top &&
                                                             face.Width == emotion.FaceRectangle.Width))
                    {
                        face.Scores = emotion.Scores;
                        face.CalculateEmotion();
                    }
                }

                return faces;
            }
        }

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
                FaceId = face.FaceId,
                Left = (int)((face.FaceRectangle.Left * scale) + uiXOffset),
                Top = (int)((face.FaceRectangle.Top * scale) + uiYOffset),
                Height = (int)(face.FaceRectangle.Height * scale),
                Width = (int)(face.FaceRectangle.Width * scale),
            });
        }

        private Face CalculateTextRectangleForRendering(Face face, int maxSize, Tuple<int, int> imageInfo, int left, int top, int height, int width)
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

            face.Left = (int) ((left*scale) + uiXOffset);
            face.Top = (int) ((top*scale) + uiYOffset);
            face.Height = (int) (height*scale);
            face.Width = (int) (width*scale);

            return face;
        }

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

        public async Task<AnalysisResult> AnalyzeImage(string selectedFile)
        {
            IVisionServiceClient visionClient = new VisionServiceClient(_subscriptionKeyVision);
            AnalysisResult analysisResult = null;

            ErrorMesssage = string.Empty;
            try
            {
                if (File.Exists(selectedFile))
                {
                    //using (FileStream stream = File.Open(imagePathOrUrl, FileMode.Open))
                    using (var fileStreamVision = File.OpenRead(selectedFile))
                    {
                        analysisResult = await visionClient.AnalyzeImageAsync(fileStreamVision);
                    }
                }
                else if (Uri.IsWellFormedUriString(selectedFile, UriKind.RelativeOrAbsolute))
                {
                    analysisResult = await visionClient.AnalyzeImageAsync(selectedFile);
                }
                else
                {
                    ErrorMesssage = "Invalid image path or Url";
                }
            }
            catch (ClientException e)
            {
                ErrorMesssage = e.Error != null ? e.Error.Message : e.Message;
            }
            catch (Exception exception)
            {
                ErrorMesssage = exception.ToString();
            }

            return analysisResult;
        }

        public async Task<string> AnalyzeImageAsString(string imagePathOrUrl)
        {
            var result = await AnalyzeImage(imagePathOrUrl);
            var resultToString = string.Empty;
            if (result == null)
            {
                resultToString = "null";
            }
            else if (result.Metadata != null)
            {
                resultToString +=
                    $"Image Format : {result.Metadata.Format} - Image Dimensions : {result.Metadata.Width} x {result.Metadata.Height}{Environment.NewLine}";
            }

            if (result != null && result.ImageType != null)
            {
                string clipArtType;
                switch (result.ImageType.ClipArtType)
                {
                    case 0:
                        clipArtType = "0 Non-clipart";
                        break;
                    case 1:
                        clipArtType = "1 ambiguous";
                        break;
                    case 2:
                        clipArtType = "2 normal-clipart";
                        break;
                    case 3:
                        clipArtType = "3 good-clipart";
                        break;
                    default:
                        clipArtType = "Unknown";
                        break;
                }

                string lineDrawingType;
                switch (result.ImageType.LineDrawingType)
                {
                    case 0:
                        lineDrawingType = "0 Non-LineDrawing";
                        break;
                    case 1:
                        lineDrawingType = "1 LineDrawing";
                        break;
                    default:
                        lineDrawingType = "Unknown";
                        break;
                }
                resultToString += $"Clip Art Type : {clipArtType}{Environment.NewLine}";
                resultToString += $"Line Drawing Type : {lineDrawingType}{Environment.NewLine}";
            }


            if (result.Adult != null)
            {
                resultToString += $"Is Adult Content : {result.Adult.IsAdultContent} - Adult Score : {result.Adult.AdultScore}{Environment.NewLine}";
                resultToString += $"Is Racy Content : {result.Adult.IsRacyContent} - Racy Score : {result.Adult.RacyScore}{Environment.NewLine}";
            }

            if (result.Categories != null && result.Categories.Length > 0)
            {
                resultToString += $"Categories : {Environment.NewLine}";
                resultToString = result.Categories.Aggregate(resultToString, (current, category) => current + $"   Name : {category.Name}; Score : {category.Score}{Environment.NewLine}");
            }

            if (result.Faces != null && result.Faces.Length > 0)
            {
                resultToString += "Faces : " + Environment.NewLine;
                resultToString = result.Faces.Aggregate(resultToString, (current, face) => current + $"  Age : {face.Age}; Gender : {face.Gender}{Environment.NewLine}");
            }

            if (result.Color != null)
            {
                resultToString += "AccentColor : " + result.Color.AccentColor + Environment.NewLine;
                resultToString += "Dominant Color Background : " + result.Color.DominantColorBackground + Environment.NewLine;
                resultToString += "Dominant Color Foreground : " + result.Color.DominantColorForeground + Environment.NewLine;

                if (result.Color.DominantColors != null && result.Color.DominantColors.Length > 0)
                {
                    resultToString += "Dominant Colors : " + Environment.NewLine;
                    resultToString = result.Color.DominantColors.Aggregate(resultToString, (current, color) => $"{current}, {color}{Environment.NewLine}");
                }
            }
            return resultToString;
        }

        public async Task<OcrResults> OcrRecognizeText(string selectedFile, bool detectOrientation = true, string languageCode = LanguageCodes.AutoDetect)
        {
            IVisionServiceClient visionClient = new VisionServiceClient(_subscriptionKeyVision);
            OcrResults ocrResult = null;
            try
            {
                if (File.Exists(selectedFile))
                {
                    using (var fileStreamVision = File.OpenRead(selectedFile))
                    {
                        ocrResult = await visionClient.RecognizeTextAsync(fileStreamVision, languageCode, detectOrientation);
                    }
                }
                else
                {
                    ErrorMesssage = "Invalid image path or Url";
                }
            }
            catch (ClientException e)
            {
                ErrorMesssage = e.Error != null ? e.Error.Message : e.Message;
            }
            catch (Exception exception)
            {
                ErrorMesssage = exception.ToString();
            }
            return ocrResult;

        }

        public async Task<string> OcrRecognizeTextAsString(string selectedFile, bool detectOrientation = true, string languageCode = LanguageCodes.AutoDetect)
        {
            OcrResults ocrResult = await OcrRecognizeText(selectedFile, detectOrientation, languageCode);
            return OcrRecognizeTextAsString(ocrResult);
        }

        public string OcrRecognizeTextAsString(OcrResults ocrResult)
        {
            var result = $@"Language: {ocrResult.Language}
Orientation: {ocrResult.Orientation}
Text Angle: {ocrResult.TextAngle}
";

            foreach (var region in ocrResult.Regions)
            {
                result += $@"Region

BoundingBox: {region.BoundingBox}
Rectangle: 
    Left {region.Rectangle.Left}, Top {region.Rectangle.Top}, 
    Height {region.Rectangle.Height}, Width {region.Rectangle.Width} 

Lines
";
                foreach (var line in region.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        result += $@"{word.Text} ";
                    }
                    result += $@"
";
                }
            }

            return result;
        }

        public string ErrorMesssage { get; set; }

        public ObservableCollection<Face> OcrGetFramesRectanglesForRecognizedText(OcrResults ocrResults, string selectedFile)
        {
            var frames = new ObservableCollection<Face>();
            var imageInfo = GetImageInfoForRendering(selectedFile);

            foreach (var region in ocrResults.Regions)
            {
                var face = new Face();
                face = CalculateTextRectangleForRendering(face, MaxImageSize, imageInfo, region.Rectangle.Left,
                    region.Rectangle.Top, region.Rectangle.Height, region.Rectangle.Width);
                face.ScoredEmotion = $"lines {region.Lines.Length}";
                frames.Add(face);
            }

            return frames;
        }
    }
}
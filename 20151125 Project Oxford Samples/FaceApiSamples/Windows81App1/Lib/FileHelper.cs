using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace Windows81App1.Lib
{
    public static class FileHelper
    {
        public static async void ClearTempFolder()
        {
            var files = await ApplicationData.Current.TemporaryFolder.GetFilesAsync();
            for (var i = 1; i < files.Count; i++)
            {
                await files[i].DeleteAsync(StorageDeleteOption.Default);
            }
        }

        public static async Task<StorageFile> CreateCopyOfSelectedImage(StorageFile file)
        {
            var newSourceFileName = string.Format(@"{0}.jpg", Guid.NewGuid());
            var newSourceFile =
                await
                    ApplicationData.Current.TemporaryFolder.CreateFileAsync(newSourceFileName,
                        CreationCollisionOption.ReplaceExisting);
            await file.CopyAndReplaceAsync(newSourceFile);

            return newSourceFile;
        }

        public static async Task<StorageFile> SaveFaceImageFile(StorageFile file, Microsoft.ProjectOxford.Face.Contract.Face face)
        {
            // get face file
            var faceStartPoint = new Point(face.FaceRectangle.Left, face.FaceRectangle.Top);
            var faceSize = new Size(face.FaceRectangle.Width, face.FaceRectangle.Height);

            // save face file
            var fileName = string.Format(@"{0}.jpg", face.FaceId);
            var fileFaceImage =
                await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await CropBitmap.SaveCroppedBitmapAsync(file, fileFaceImage, faceStartPoint, faceSize);
            return fileFaceImage;
        }

        public static async Task<Tuple<int, int>> GetImageInfoForRendering(string imageFilePath)
        {
            try
            {
                var sampleFile = await StorageFile.GetFileFromPathAsync(imageFilePath);
                var file = await sampleFile.OpenAsync(FileAccessMode.ReadWrite);
                var decoder = await BitmapDecoder.CreateAsync(file);
                var pixelWidth = int.Parse(decoder.PixelWidth.ToString());
                var pixelHeight = int.Parse(decoder.PixelHeight.ToString());
                return new Tuple<int, int>(pixelWidth, pixelHeight);
            }
            catch
            {
                return new Tuple<int, int>(0, 0);
            }
        }
    }
}

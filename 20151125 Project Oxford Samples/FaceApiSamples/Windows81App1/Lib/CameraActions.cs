using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace Windows81App1.Lib
{
    public static class CameraActions
    {
        public static async Task<StorageFile> TakeWebCamPictureAndReturnFile(bool takeSilentPicture)
        {
            StorageFile file;
            if (takeSilentPicture)
            {
                var takePhotoManager = new MediaCapture();
                await takePhotoManager.InitializeAsync();
                var imgFormat = ImageEncodingProperties.CreateJpeg();
                file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("CameraPhoto.jpg", CreationCollisionOption.ReplaceExisting);
                await takePhotoManager.CapturePhotoToStorageFileAsync(imgFormat, file);
            }
            else
            {
                var dialog = new CameraCaptureUI();
                dialog.PhotoSettings.AllowCropping = false;
                dialog.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.HighestAvailable;
                file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
            }
            return file;
        }

    }
}

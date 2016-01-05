using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace WuaFaceAppChristmas.Lib
{
    public class CameraActions
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

        public static async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Panel desiredPanel)
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the desired camera by panel
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);

            // If there is no device mounted on the desired panel, return the first device found
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }
    }
}

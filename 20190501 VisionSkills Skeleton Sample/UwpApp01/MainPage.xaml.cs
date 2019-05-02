using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Microsoft.AI.Skills.SkillInterfacePreview;
using Microsoft.AI.Skills.Vision.SkeletalDetectorPreview;
using Microsoft.Toolkit.Uwp.Helpers;

namespace UwpApp01
{
    public sealed partial class MainPage
    {
        // Skill-related variables
        private SkeletalDetectorSkill m_skeletalDetectorSkill;
        private SkeletalDetectorBinding m_skeletalDetectorBinding;
        private SkeletalDetectorDescriptor m_skeletalDetectorDescriptor;

        // UI Related
        private BodyRenderer m_bodyRenderer;
        private IReadOnlyList<ISkillExecutionDevice> m_availableExecutionDevices;
        private uint m_cameraFrameWidth, m_cameraFrameHeight;
        private bool m_isCameraFrameDimensionInitialized = false;
        private enum FrameSourceToggledType { None, ImageFile, Camera, Capture };
        private FrameSourceToggledType m_currentFrameSourceToggled = FrameSourceToggledType.None;

        //Debug
        private Stopwatch m_evalPerfStopwatch = new Stopwatch();
        private long m_skeletalDetectionRunTime = 0;

        // Synchronization
        private SemaphoreSlim m_lock = new SemaphoreSlim(1);

        private VideoFrame m_cachedFrame = null;


        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void UIImageViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (UIImageViewer.Visibility == Visibility.Visible) // we are using an image file that we stretch, match UI control dimension
            {
                UICanvasOverlay.Width = UIImageViewer.ActualWidth;
                UICanvasOverlay.Height = UIImageViewer.ActualHeight;
            }
            else // we are using a camera preview, make sure the aspect ratio is honored when rendering the face rectangle
            {
                float aspectRatio = (float)m_cameraFrameWidth / m_cameraFrameHeight;
                UICanvasOverlay.Width = aspectRatio >= 1.0f ? UICameraPreview.ActualWidth : UICameraPreview.ActualWidth * aspectRatio;
                UICanvasOverlay.Height = aspectRatio >= 1.0f ? UICameraPreview.ActualHeight / aspectRatio : UICameraPreview.ActualHeight;
            }
        }

        private async Task RunSkillAsync(VideoFrame frame, bool isStream)
        {
            m_evalPerfStopwatch.Restart();

            // Update input image and run the skill against it
            await m_skeletalDetectorBinding.SetInputImageAsync(frame);
            await m_skeletalDetectorSkill.EvaluateAsync(m_skeletalDetectorBinding);

            m_evalPerfStopwatch.Stop();
            m_skeletalDetectionRunTime = m_evalPerfStopwatch.ElapsedMilliseconds;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                m_bodyRenderer.Update(m_skeletalDetectorBinding.Bodies, !isStream);
                m_bodyRenderer.IsVisible = true;
                UISkillOutputDetails.Text = $"Found {m_skeletalDetectorBinding.Bodies.Count} bodies (took {m_skeletalDetectionRunTime} ms)";
            });
        }

        private async void UIButtonStart_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                UICameraPreview.Stop();
                if (UICameraPreview.CameraHelper != null)
                {
                    await UICameraPreview.CameraHelper.CleanUpAsync();
                }
                m_isCameraFrameDimensionInitialized = false;

                // Initialize skill with the selected supported device
                m_skeletalDetectorSkill = await m_skeletalDetectorDescriptor.CreateSkillAsync(m_availableExecutionDevices[0]) as SkeletalDetectorSkill;

                // Initialize the CameraPreview control, register frame arrived event callback
                UIImageViewer.Visibility = Visibility.Collapsed;
                UICameraPreview.Visibility = Visibility.Visible;
                await UICameraPreview.StartAsync();

                UICameraPreview.CameraHelper.FrameArrived += CameraHelper_FrameArrived;
                m_currentFrameSourceToggled = FrameSourceToggledType.Camera;
            }
            catch (Exception ex)
            {
                await (new MessageDialog(ex.Message)).ShowAsync();
                m_currentFrameSourceToggled = FrameSourceToggledType.None;
            }
            finally
            {
                m_lock.Release();
            }
        }

        private async void CameraHelper_FrameArrived(object sender, FrameEventArgs e)
        {
            await m_lock.WaitAsync();

            try
            {
                // Use a lock to process frames one at a time and bypass processing if busy
                if (m_lock.Wait(0))
                {
                    uint cameraFrameWidth = UICameraPreview.CameraHelper.PreviewFrameSource.CurrentFormat.VideoFormat.Width;
                    uint cameraFrameHeight = UICameraPreview.CameraHelper.PreviewFrameSource.CurrentFormat.VideoFormat.Height;

                    // Allign overlay canvas and camera preview so that face detection rectangle looks right
                    if (!m_isCameraFrameDimensionInitialized || cameraFrameWidth != m_cameraFrameWidth || cameraFrameHeight != m_cameraFrameHeight)
                    {
                        // Can't bind frames of different sizes to same binding.
                        // As a workaround, recreate the binding for each eval if framesize changed.
                        m_skeletalDetectorBinding = await m_skeletalDetectorSkill.CreateSkillBindingAsync() as SkeletalDetectorBinding;

                        m_cameraFrameWidth = UICameraPreview.CameraHelper.PreviewFrameSource.CurrentFormat.VideoFormat.Width;
                        m_cameraFrameHeight = UICameraPreview.CameraHelper.PreviewFrameSource.CurrentFormat.VideoFormat.Height;

                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            UIImageViewer_SizeChanged(null, null);
                        });

                        m_isCameraFrameDimensionInitialized = true;
                    }

                    // Run the skill against the frame
                    SoftwareBitmap copyBitmap = SoftwareBitmap.Convert(e.VideoFrame.SoftwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    m_cachedFrame = VideoFrame.CreateWithSoftwareBitmap(copyBitmap);
                    await RunSkillAsync(m_cachedFrame, true);
                    m_lock.Release();
                }
                e.VideoFrame.Dispose();
            }
            catch (Exception ex)
            {
                // Show the error
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => UISkillOutputDetails.Text = ex.Message);
                m_lock.Release();
            }
            finally
            {
                m_lock.Release();
            }
        }


        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize helper class used to render the skill results on screen
            m_bodyRenderer = new BodyRenderer(UICanvasOverlay);

            await Task.Run(async () =>
            {
                try
                {
                    m_skeletalDetectorDescriptor = new SkeletalDetectorDescriptor();
                    m_availableExecutionDevices = await m_skeletalDetectorDescriptor.GetSupportedExecutionDevicesAsync();
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message).ShowAsync();
                }
            });

        }
    }
}


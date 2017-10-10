using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace SurfaceDialApp01.Views
{
    public sealed partial class MediaPlayerPagePage : Page, INotifyPropertyChanged
    {
        // TODO WTS: Set your video default and image here
        // For more on the MediaPlayer and adjusting controls and behavior see https://docs.microsoft.com/en-us/windows/uwp/controls-and-patterns/media-playback
        private const string DefaultSource = "https://sec.ch9.ms/ch9/db15/43c9fbed-535e-4013-8a4a-a74cc00adb15/C9L12WinTemplateStudio_high.mp4";

        // The poster image is displayed until the video is started
        private const string DefaultPoster = "https://sec.ch9.ms/ch9/db15/43c9fbed-535e-4013-8a4a-a74cc00adb15/C9L12WinTemplateStudio_960.jpg";

        // The DisplayRequest is used to stop the screen dimming while watching for extended periods
        private DisplayRequest _displayRequest = new DisplayRequest();
        private bool _isRequestActive = false;

        readonly RadialController _controller;

        public MediaPlayerPagePage()
        {
            InitializeComponent();

            mpe.PosterSource = new BitmapImage(new Uri(DefaultPoster));
            mpe.Source = MediaSource.CreateFromUri(new Uri(DefaultSource));

            _controller = RadialController.CreateForCurrentView();
            _controller.RotationResolutionInDegrees = 5;
            _controller.UseAutomaticHapticFeedback = false;

            var myItem = RadialControllerMenuItem.CreateFromFontGlyph("El Bruno - Playback", "\xE714", "Segoe MDL2 Assets");
            _controller.Menu.Items.Add(myItem);
            _controller.ButtonClicked += ControllerButtonClicked;
            _controller.RotationChanged += ControllerRotationChanged;
        }


        private void ControllerRotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            mpe.MediaPlayer.Position = mpe.MediaPlayer.Position + TimeSpan.FromSeconds(args.RotationDeltaInDegrees);
        }

        private void ControllerButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        {
            if (mpe.MediaPlayer.CurrentState == MediaPlayerState.Playing)
            {
                mpe.MediaPlayer.Pause();
            }
            else
            {
                mpe.MediaPlayer.Play();
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            mpe.MediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            mpe.MediaPlayer.Pause();
            mpe.MediaPlayer.PlaybackSession.PlaybackStateChanged -= PlaybackSession_PlaybackStateChanged;
        }

        private async void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            if (sender is MediaPlaybackSession playbackSession && playbackSession.NaturalVideoHeight != 0)
            {
                if (playbackSession.PlaybackState == MediaPlaybackState.Playing)
                {
                    if (!_isRequestActive)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            _displayRequest.RequestActive();
                            _isRequestActive = true;
                        });
                    }
                }
                else
                {
                    if (_isRequestActive)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            _displayRequest.RequestRelease();
                            _isRequestActive = false;
                        });
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

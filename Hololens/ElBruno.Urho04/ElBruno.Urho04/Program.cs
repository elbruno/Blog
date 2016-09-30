using System;
using Windows.ApplicationModel.Core;
using Urho.HoloLens;

namespace ElBruno.Urho04
{
    internal class Program
    {
        [MTAThread]
        private static void Main()
        {
            var exclusiveViewApplicationSource = new AppViewSource();
            CoreApplication.Run(exclusiveViewApplicationSource);
        }
    }

    internal class AppViewSource : IFrameworkViewSource
    {
        public IFrameworkView CreateView()
        {
            return UrhoAppView.Create<ShowHideGazeSample>(null);
        }
    }
}
using System;

using Android.Content;
using Android.Util;

using EstimoteSdk;

using JavaObject = Java.Lang.Object;

namespace Estimotes.Droid
{
    /// <summary>
    ///   Base class for managing the Estimote BeaconManager.
    /// </summary>
    abstract class BeaconFinder : JavaObject, BeaconManager.IServiceReadyCallback
    {
        static readonly string TAG = typeof(BeaconFinder).Name;
        protected BeaconFinder(Context context)
        {
#if DEBUG
            //L.EnableDebugLogging(true);
#endif

            Context = context;
            BeaconManager = new BeaconManager(context);
            BeaconManager.Error += BeaconManager_Error;

            if (!BeaconManager.HasBluetooth)
            {
                throw new Exception("The device does not have have Bluetooth!");
            }
        }

        void BeaconManager_Error(object sender, BeaconManager.ErrorEventArgs e)
        {
            Log.Error(TAG, "Something terrible happened with the BeaconManager, Error code {0}.", e.ErrorId);
        }

        protected Context Context { get; private set; }
        protected BeaconManager BeaconManager { get; private set; }
        public bool IsBluetoothEnabled { get { return BeaconManager.IsBluetoothEnabled; } }

        public abstract void OnServiceReady();

        public virtual void Stop()
        {
            BeaconManager.Disconnect();
        }
    }
}

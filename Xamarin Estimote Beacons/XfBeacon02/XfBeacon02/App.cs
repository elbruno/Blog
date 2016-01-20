using System;
using Estimotes;
using Xamarin.Forms;

namespace XfBeacon02
{
    public class App : Application
    {
        private readonly Label _labelTitle;
        private readonly Label _labelContent;
        private readonly Label _labelStatus;

        // iOS Virtual Beacon UUid
        private const string EstimoteUuid = @"8492E75F-4FD6-469D-B132-043FE94921D8";
        // Estimote UUid
        // private const string EstimoteUuid = @"B9407F30-F5F8-466E-AFF9-25556B57FE6D"; 

        public App()
        {
            _labelTitle = new Label
            {
                XAlign = TextAlignment.Center,
                Text = "El Bruno - Estimote Labs 2"
            };
            _labelContent = new Label
            {
                XAlign = TextAlignment.Center,
                Text = "content"
            };
            _labelStatus = new Label
            {
                XAlign = TextAlignment.Center,
                Text = "status"
            };

            // The root page of your application
            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = { _labelTitle, _labelContent, _labelStatus }
                }
            };
        }

        protected override async void OnStart()
        {
            try
            {
                var status = await EstimoteManager.Instance.Initialize();
                if (status != BeaconInitStatus.Success)
                {
                    LogStatus(@"could not initialice beacons");
                }
                else
                {
                    LogStatus(@"beacons initialized");
                    EstimoteManager.Instance.Ranged += Instance_Ranged;
                    EstimoteManager.Instance.RegionStatusChanged += Instance_RegionStatusChanged;
                    EstimoteManager.Instance.StartRanging(new BeaconRegion("iOS Beacon", EstimoteUuid));
                }
            }
            catch (Exception exception)
            {
                LogStatus(exception.ToString());
            }
        }

        private void Instance_RegionStatusChanged(object sender, BeaconRegionStatusChangedEventArgs e)
        {
            try
            {

                _labelContent.Text = $@"entering {e.IsEntering}
region id: {e.Region}";
                LogStatus(@"Region Status Changed");
            }
            catch (Exception exception)
            {
                LogStatus(exception.ToString());
            }
        }

        private void Instance_Ranged(object sender, System.Collections.Generic.IEnumerable<IBeacon> e)
        {
            try
            {
                var data = string.Empty;
                foreach (var beacon in e)
                {
                    data = $@"Time: {DateTime.Now.TimeOfDay}
Uuid: {beacon.Uuid}
Major: {beacon.Major}
Minor: {beacon.Minor}
Proximity: {beacon.Proximity}";
                }
                _labelContent.Text = data;
                LogStatus(@"Ranged");
            }
            catch (Exception exception)
            {
                LogStatus(exception.ToString());
            }
        }

        private void LogStatus(string message = "")
        {
            _labelStatus.Text = $@"{message}";
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            EstimoteManager.Instance.StopMonitoring(new BeaconRegion("Beacon Identifier", EstimoteUuid));
        }

        protected override void OnResume()
        {

        }
    }
}

using Xamarin.Forms;

namespace XfBeacon02
{
    public class App : Application
    {
        public App()
        {
            MainPage = new MainBeaconPage();
        }


        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {

        }
    }
}

namespace XfBeacon02
{
    public partial class MainBeaconPage
    {
        public MainBeaconPage()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.BeaconExplorerViewModel;
        }

        protected override void OnAppearing()
        {
            ViewModelLocator.BeaconExplorerViewModel.OnAppearing();
            base.OnAppearing();
        }
    }
}

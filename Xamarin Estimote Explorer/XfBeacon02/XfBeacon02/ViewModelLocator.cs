namespace XfBeacon02
{
    public class ViewModelLocator
    {
        private static BeaconExplorerViewModel _beaconExplorerViewModel;
        public static BeaconExplorerViewModel BeaconExplorerViewModel => _beaconExplorerViewModel ?? (_beaconExplorerViewModel = new BeaconExplorerViewModel());
    }
}

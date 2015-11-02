using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Microsoft.Band;

namespace App1
{
    public class BandHub
    {
        private static BandHub _instance;
        public static readonly object Padlock = new object();
        private IBandClientManager _bandManager;
        private IBandInfo[] _pairedBands;

        public static BandHub Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (_instance != null) return _instance;
                    _instance = new BandHub();
                    return _instance;
                }
            }
        }

        public IBandInfo BandInfo { get; set; }

        public IBandClient BandClient { get; set; }

        public string BandInformation { get; set; }

        public async Task<bool> LoadBands()
        {
            var ret = false;
            _bandManager = BandClientManager.Instance;
            _pairedBands = await _bandManager.GetBandsAsync();
            if (_pairedBands.Length == 0)
            {
                var md = new MessageDialog("no band connected");
                await md.ShowAsync();
            }
            else
            {
                await ConnectToBand();
                await GetBandInformation();
                ret = true;
            }
            return ret;
        }

        private async Task ConnectToBand()
        {
            BandInfo = _pairedBands[0];
            BandClient = await BandClientManager.Instance.ConnectAsync(BandInfo);
        }
        private async Task GetBandInformation()
        {
            var bi = new BandInformation();
            BandInformation = await bi.RetrieveInfo(BandInfo, BandClient);
            var tileCapacity = await BandClient.TileManager.GetRemainingTileCapacityAsync();
            var tilesInfo = $"Remaining Tiles Capacity: {tileCapacity}";
            BandInformation += string.Format("\n{0}", tilesInfo);
        }

        public async void RemoveAllCustomTiles()
        {
            if (BandClient == null) return;
            var tiles = await BandClient.TileManager.GetTilesAsync();
            foreach (var tile in tiles)
            {
                await BandClient.TileManager.RemoveTileAsync(tile.TileId);
            }
        }
    }
}
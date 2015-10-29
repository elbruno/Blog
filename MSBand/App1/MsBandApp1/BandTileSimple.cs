
using System;
using System.Threading.Tasks;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;

namespace App1
{
    public class BandTileSimple
    {
        public Guid TileId = new Guid("E27830EA-AF6A-404F-A256-47EC79DE0768");

        public async Task AddTile()
        {
            // Tile Definition
            var myTile = new BandTile(TileId)
            {
                Name = "El Bruno Tile Simple",
                TileIcon = await ImageHelper.LoadIcon("ms-appx:///Assets/RugbyBall46.png"),
                SmallIcon = await ImageHelper.LoadIcon("ms-appx:///Assets/RugbyBall24.png")
            };

            // Page UI 
            var button = new TextButton
            {
                ElementId = 1,
                Rect = new PageRect(10, 10, 200, 90)
            };
            var panel = new FilledPanel(button)
            {
                Rect = new PageRect(0, 0, 220, 150)
            };
            myTile.PageLayouts.Add(new PageLayout(panel));

            await BandHub.Instance.BandClient.TileManager.RemoveTileAsync(TileId);
            await BandHub.Instance.BandClient.TileManager.AddTileAsync(myTile);

            // Page Data
            var pageId = new Guid("5F5FD06E-BD37-4B71-B36C-3ED9D721F200");
            var textButtonA = new TextButtonData(1, "El Button");
            var page = new PageData(pageId, 0, textButtonA);
            await BandHub.Instance.BandClient.TileManager.SetPagesAsync(TileId, page);
        }

    }
}

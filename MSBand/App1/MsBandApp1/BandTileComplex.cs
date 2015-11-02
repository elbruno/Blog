
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;

namespace App1
{
    public class BandTileComplex
    {
        private readonly IBandClient _bandClient;
        public Guid TileId = new Guid("A4FA709D-8EE0-4F7C-84F0-4CCCA0ACABA5");
        public Guid PageId = new Guid("36B7DCEE-5462-4D4E-9EBC-AE2E8E4C974D");

        public const short TitleId = 1;
        public const short BarcodeId = 2;
        public const short PanelButtonsId = 3;
        public const short ButtonAId = 4;
        public const short ButtonBId = 5;

        public BandTileComplex(IBandClient bandClient)
        {
            _bandClient = bandClient;
        }

        public async Task AddTile()
        {
            await _bandClient.TileManager.RemoveTileAsync(TileId);
            await _bandClient.TileManager.RemovePagesAsync(TileId);

            // Tile Definicion
            var myTile = new BandTile(TileId)
            {
                Name = "El Bruno Tile Complex",
                TileIcon = await ImageHelper.LoadIcon("ms-appx:///Assets/Barcode46.png"),
                SmallIcon = await ImageHelper.LoadIcon("ms-appx:///Assets/Barcode24.png")
            };

            // Page Layout UI definition
            foreach (var pageUi in BuildTileUi())
            {
                myTile.PageLayouts.Add(pageUi);
            }
            await _bandClient.TileManager.AddTileAsync(myTile);
            await _bandClient.TileManager.AddTileAsync(myTile);
        }

        public async Task<bool> SetTileData(string sampleCard, string btnA, string btnB, string barcode)
        {
            PageElementData[] pageElementData =
            {
                new TextBlockData(TitleId, sampleCard),
                new BarcodeData(BarcodeType.Code39, BarcodeId, barcode),
                new TextButtonData(ButtonAId, btnA),
                new TextButtonData(ButtonBId, btnB)
            };
            var page = new PageData(PageId, 0, pageElementData);
            await _bandClient.TileManager.SetPagesAsync(TileId, page);
            return true;
        }

        private IEnumerable<PageLayout> BuildTileUi()
        {
            var textBlockTitle = new TextBlock()
            {
                Color = Colors.Red.ToBandColor(),
                ElementId = TitleId,
                Rect = new PageRect(0, 0, 200, 25)
            };
            var barcode = new Barcode(BarcodeType.Code39)
            {
                ElementId = BarcodeId,
                Rect = new PageRect(0, 0, 250, 40)
            };
            var buttonA = new TextButton
            {
                ElementId = ButtonAId,
                Rect = new PageRect(0, 0, 100, 45)
            };
            var buttonB = new TextButton
            {
                ElementId = ButtonBId,
                Rect = new PageRect(0, 0, 100, 45)
            };
            var panelButtons = new ScrollFlowPanel(buttonA, buttonB)
            {
                ElementId = PanelButtonsId,
                Orientation = FlowPanelOrientation.Horizontal,
                Rect = new PageRect(0, 0, 250, 250)
            };
            var panelMain = new ScrollFlowPanel(textBlockTitle, barcode, panelButtons)
            {
                Orientation = FlowPanelOrientation.Vertical,
                Rect = new PageRect(0, 0, 250, 250)
            };
            
            var bandUi = new List<PageLayout> { new PageLayout(panelMain) };
            return bandUi;
        }
    }
}

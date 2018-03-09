using System;
using System.Diagnostics;
using System.Globalization;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace CoffeeUtilities
{
    public sealed class TileHelper
    {
        private TileHelper() { }

        public static void UpdatePrimaryTile(TileUpdateType sender)
        {
			XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text01);

			XmlNodeList textNodes = tileXml.GetElementsByTagName("text");
			textNodes[0].InnerText = (sender == TileUpdateType.Foreground) ? "FG" : "BG";
			textNodes[1].InnerText = DateTime.Now.ToString("HH:mm:ss");
			textNodes[2].InnerText = DateTime.Now.Millisecond.ToString(CultureInfo.CurrentCulture);

			TileNotification tileNotification = new TileNotification(tileXml);
			TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }
    }

    public enum TileUpdateType { Foreground, Background };
}

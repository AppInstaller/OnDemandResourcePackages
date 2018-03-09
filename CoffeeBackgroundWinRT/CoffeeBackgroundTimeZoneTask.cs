using System.Diagnostics;
using CoffeeUtilities;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System;

namespace CoffeeBackgroundWinRT
{
	public sealed class CoffeeBackgroundTimeZoneTask : IBackgroundTask
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			TileHelper.UpdatePrimaryTile(TileUpdateType.Background);
			SendToast();
		}

		private void SendToast()
		{
            try
            {
                ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

                XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
                ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/SecondaryTiles/espresso.jpg");
                ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", SharedStrings.TOAST_TEXT_TITLE);

                IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
                ((XmlElement)toastNode).SetAttribute("launch", SharedStrings.TOAST_PARAMETER_PREFIX + SharedStrings.TOAST_PARAMETER_STRING);

                XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                toastTextElements[0].AppendChild(toastXml.CreateTextNode(SharedStrings.TOAST_TEXT_TITLE));
                toastTextElements[1].AppendChild(toastXml.CreateTextNode(SharedStrings.TOAST_TEXT_BODY));

                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

	}
}

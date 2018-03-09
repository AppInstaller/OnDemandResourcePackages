using CoffeeUniversal.ViewModels;
using CoffeeUniversal.Helpers;
using CoffeeUtilities;
using System;
using System.Diagnostics;
using System.Globalization;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
	public sealed partial class ToastsPage : Page
	{

        #region Init

        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public ToastsPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        private const string webImageUrl = "http://static.wixstatic.com/media/66e425_c0daa1882f654b4199b296400e78ff57.jpg_srz_p_315_254_75_22_0.50_1.20_0.00_jpg_srz";

        private string launchArguments;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is string)
            {
                launchArguments = (string)e.Parameter;
                if (!string.IsNullOrEmpty(launchArguments))
                {
                    if (launchArguments.StartsWith(SharedStrings.TOAST_PARAMETER_PREFIX, StringComparison.CurrentCultureIgnoreCase))
                    {
                        string payload = launchArguments.Substring(launchArguments.IndexOf("|", StringComparison.CurrentCultureIgnoreCase) + 1);
                        try
                        {
                            status.Log(string.Format(CultureInfo.CurrentCulture,
                                LocalizableStrings.TOAST_LAUNCHED_FROM_TOAST, payload));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("ToastsPage.OnNavigatedTo: " +ex.ToString());
                        }
                    }
                }
            }

            navigationHelper.OnNavigatedTo(e);
        }

        #endregion


        #region Old School Toasts

        private void SendOldSchoolToast(XmlDocument toastXml, string message)
        {
            try
            {
                XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                toastTextElements[0].AppendChild(toastXml.CreateTextNode(SharedStrings.TOAST_TEXT_TITLE));
                toastTextElements[1].AppendChild(toastXml.CreateTextNode(SharedStrings.TOAST_TEXT_BODY));
                Debug.WriteLine(toastXml.GetXml());

                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(toast);

                status.Log(string.Format(CultureInfo.CurrentCulture, LocalizableStrings.TOAST_SENT, message));
            }
            catch (Exception ex)
            {
                status.Log(string.Format(CultureInfo.CurrentCulture, LocalizableStrings.TOAST_FAILED, ex.Message));
            }
        }

        /*
        <toast>
          <visual>
            <binding template="ToastText02">
              <text id="1">Espresso</text>
              <text id="2"> Lorem ipsum dolor sit amet, consectetur elit.</text>
            </binding>
          </visual>
        </toast>
        */
        private void SimpleTextOldSchool()
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            SendOldSchoolToast(toastXml, LocalizableStrings.TOAST_SIMPLE_TEXT);
        }

        /*
        <toast>
          <visual>
            <binding template="ToastImageAndText02">
              <image id="1" src="ms-appx:///Assets/SecondaryTiles/espresso.jpg" alt="Espresso"/>
              <text id="1">Espresso</ text>
              <text id="2">Lorem ipsum dolor sit amet, consectetur elit.</text>
            </binding>
          </visual>
        </toast>
        */
        private void LocalImageOldSchool()
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
            ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/SecondaryTiles/espresso.jpg");
            ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", SharedStrings.TOAST_TEXT_TITLE);
            SendOldSchoolToast(toastXml, LocalizableStrings.TOAST_LOCAL_IMAGE);
        }

        /*
        <toast>
          <visual>
            <binding template="ToastImageAndText02">
              <image id="1" src="http://static.wixstatic.com/media/66e425_c0daa1882f654b4199b296400e78ff57.jpg_srz_p_315_254_75_22_0.50_1.20_0.00_jpg_srz" alt="Espresso"/>
              <text id="1">Espresso</text>
              <text id="2">Lorem ipsum dolor sit amet, consectetur elit.</text>
            </binding>
          </visual>
        </toast>
        */
        private void WebImageOldSchool()
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
            ((XmlElement)toastImageAttributes[0]).SetAttribute("src", webImageUrl);
            ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", SharedStrings.TOAST_TEXT_TITLE);
            SendOldSchoolToast(toastXml, LocalizableStrings.TOAST_WEB_IMAGE);
        }

        /*
        <toast>
          <visual>
            <binding template="ToastImageAndText02">
              <image id="1" src=""/>
              <text id="1">Espresso</text>
              <text id="2">Lorem ipsum dolor sit amet, consectetur elit.</text>
            </binding>
          </visual>
          <audio src="ms-winsoundevent:Notification.IM"/>
         </toast>
        */
        private void SoundToastOldSchool()
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.IM");
            toastNode.AppendChild(audio);
            SendOldSchoolToast(toastXml, LocalizableStrings.TOAST_WITH_SOUND);
        }

        /*
        <toast duration="long">
          <visual>
            <binding template="ToastImageAndText02">
              <image id="1" src=""/>
              <text id="1">Espresso</text>
              <text id="2">Lorem ipsum dolor sit amet, consectetur elit.</text>
            </binding>
          </visual>
          <audio src="ms-winsoundevent:Notification.Looping.Alarm" loop="true"/>
         </toast>
        */
        private void LongToastOldSchool()
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "long");
            XmlElement audio = toastXml.CreateElement("audio");
            toastNode.AppendChild(audio);
            audio.SetAttribute("src", "ms-winsoundevent:Notification.Looping.Alarm");
            audio.SetAttribute("loop", "true");

            SendOldSchoolToast(toastXml, LocalizableStrings.TOAST_LONG_DURATION);
        }

        #endregion


        #region Local Toast UI handlers

        private void simpleTextToast_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)oldSchool.IsChecked)
            {
                SimpleTextOldSchool();
            }
            else
            {
                SimpleTextToastGeneric();
            }
        }

        private void localImageToast_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)oldSchool.IsChecked)
            {
                LocalImageOldSchool();
            }
            else
            {
                LocalImageToastGeneric();
            }
        }

        private void webImageToast_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)oldSchool.IsChecked)
            {
                WebImageOldSchool();
            }
            else
            {
                WebImageToastGeneric();
            }
        }

        private void soundToast_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)oldSchool.IsChecked)
            {
                SoundToastOldSchool();
            }
            else
            {
                SoundToastGeneric();
            }
        }

        private void longToast_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)oldSchool.IsChecked)
            {
                LongToastOldSchool();
            }
            else
            {
                LongToastGeneric();
            }
        }

        #endregion


        #region Local Toasts ToastGeneric

        /*
        <toast>
          <visual>
            <binding template="ToastGeneric">
              <text>Espresso</text>
              <text>Lorem ipsum dolor sit amet, consectetur elit.</text>
            </binding>
          </visual>
        </toast>
        */
        private void SimpleTextToastGeneric()
        {
            string xmlString =
                "<toast>"
                    + "<visual>"
                        + "<binding template=\"ToastGeneric\">"
                        + "<text>Espresso</text>"
                        + "<text>Lorem ipsum dolor sit amet, consectetur elit.</text>"
                        + "</binding>"
                    + "</visual>"
                + "</toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(xmlString);
            SendNewToast(toastXml, LocalizableStrings.TOAST_SIMPLE_TEXT);
        }

        /*
        <toast>
          <visual>
            <binding template="ToastImageAndText02">
              <image id="1" src="ms-appx:///Assets/SecondaryTiles/espresso.jpg" alt="Espresso"/>
              <text id="1">Espresso</ text>
              <text id="2">Lorem ipsum dolor sit amet, consectetur elit.</text>
            </binding>
          </visual>
        </toast>
        */
        private void LocalImageToastGeneric()
        {
            string xmlString =
                "<toast>"
                    + "<visual>"
                        + "<binding template=\"ToastGeneric\">"
                        + "<image src=\"ms-appx:///Assets/SecondaryTiles/espresso.jpg\" alt=\"Espresso\"/>"
                        + "<text>Espresso</text>"
                        + "<text>Lorem ipsum dolor sit amet, consectetur elit.</text>"
                        + "</binding>"
                    + "</visual>"
                + "</toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(xmlString);
            SendNewToast(toastXml, LocalizableStrings.TOAST_LOCAL_IMAGE);
        }

        /*
        <toast>
          <visual>
            <binding template="ToastGeneric">
              <image src="http://static.wixstatic.com/media/66e425_c0daa1882f654b4199b296400e78ff57.jpg_srz_p_315_254_75_22_0.50_1.20_0.00_jpg_srz" alt="Espresso"/>
              <text>Espresso</text>
              <text>Lorem ipsum dolor sit amet, consectetur elit.</text>
            </binding>
          </visual>
        </toast>
        */
        private void WebImageToastGeneric()
        {
            string xmlString =
                "<toast>"
                    + "<visual>"
                        + "<binding template=\"ToastGeneric\">"
                        + "<image src=\"" +webImageUrl +"\" alt=\"Espresso\" />"
                        + "<text>Espresso</text>"
                        + "</binding>"
                    + "</visual>"
                + "</toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(xmlString);
            SendNewToast(toastXml, LocalizableStrings.TOAST_WEB_IMAGE);
        }

        /*
        <toast>
          <visual>
            <binding template="ToastGeneric">
              <image src="ms-appx:///Assets/SecondaryTiles/espresso.jpg" alt="Espresso"/>
              <text>Espresso</text>
              <text>Lorem ipsum dolor sit amet, consectetur elit.</text>
            </binding>
          </visual>
          <audio src="ms-winsoundevent:Notification.IM"/>
        </toast>
        */
        private void SoundToastGeneric()
        {
            string xmlString =
                "<toast>"
                    + "<visual>"
                        + "<binding template=\"ToastGeneric\">"
                        + "<image src=\"ms-appx:///Assets/SecondaryTiles/espresso.jpg\" alt=\"Espresso\" />"
                        + "<text>Espresso</text>"
                        + "</binding>"
                    + "</visual>"
                    + "<audio src=\"ms-winsoundevent:Notification.IM\"/>"
                    + "</toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(xmlString);
            SendNewToast(toastXml, LocalizableStrings.TOAST_WITH_SOUND);
        }

        /*
        <toast>
          <visual>
            <binding template="ToastGeneric">
              <image src="ms-appx:///Assets/SecondaryTiles/espresso.jpg" alt="Espresso"/>
              <text>Espresso</text>
              <text>Lorem ipsum dolor sit amet, consectetur elit.</text>
            </binding>
          </visual>
          <audio src="ms-winsoundevent:Notification.Looping.Alarm" loop="true"/>
        </toast>
        */
        private void LongToastGeneric()
        {
            string xmlString =
                "<toast>"
                    + "<visual>"
                        + "<binding template=\"ToastGeneric\">"
                        + "<image src=\"ms-appx:///Assets/SecondaryTiles/espresso.jpg\" alt=\"Espresso\" />"
                        + "<text>Espresso</text>"
                        + "</binding>"
                    + "</visual>"
                    + "<audio src=\"ms-winsoundevent:Notification.Looping.Alarm\" loop=\"true\"/>"
                    + "</toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(xmlString);
            SendNewToast(toastXml, LocalizableStrings.TOAST_LONG_DURATION);
        }

        #endregion


        #region Actionable Toasts

        private void SendNewToast(XmlDocument toastXml, string message)
        {
            try
            {
                Debug.WriteLine(toastXml.GetXml());

                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(toast);

                status.Log(string.Format(CultureInfo.CurrentCulture, LocalizableStrings.TOAST_SENT, message));
            }
            catch (Exception ex)
            {
                status.Log(string.Format(CultureInfo.CurrentCulture, LocalizableStrings.TOAST_FAILED, ex.Message));
            }
        }

        //<toast activationType="protocol" launch="http://www.bing.com/images/search?q=coffee">
        //    <visual>
        //      <binding template="ToastGeneric">
        //        <image src="ms-appx:///Assets/SecondaryTiles/espresso.jpg" alt="Espresso"/>
        //        <text>Tap me!</text>
        //        <text>...to find coffee images.</text>
        //      </binding>
        //    </visual>
        //</toast>
        private void protocolToast_Click(object sender, RoutedEventArgs e)
        {
            string xmlString =
                "<toast activationType=\"protocol\" launch=\"http://www.bing.com/images/search?q=coffee\">"
                    + "<visual>"
                        + "<binding template=\"ToastGeneric\">"
                        + "<image src=\"ms-appx:///Assets/SecondaryTiles/espresso.jpg\" alt=\"Espresso\"/>"
                        + "<text>Tap me!</text>"
                        + "<text>...to find coffee images.</text>"
                        + "</binding>"
                    + "</visual>"
                + "</toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(xmlString);
            SendNewToast(toastXml, LocalizableStrings.TOAST_PROTOCOL);
        }


        /*
        <toast>
            <visual>
              <binding template="ToastGeneric">
                <image src="ms-appx:///Assets/SecondaryTiles/espresso.jpg" alt="Espresso"/>
                <text>Espresso</text>
                <text>Select snooze time.</text>
              </binding>
            </visual>
            <actions>
                <input id="snoozeTime" type="selection" defaultSelection="10">
                  <selection id="1" content="1 minute" />
                  <selection id="5" content="5 minutes" />
                  <selection id="10" content="10 minutes" />
                  <selection id="30" content="30 minutes" />
                  <selection id="60" content="1 hour" />
                </input>
                <action activationType="system" arguments="snooze" hint-inputId="snoozeTime" content=""/>
                <action activationType="system" arguments="dismiss" content=""/>
            </actions>
        </toast>
        */
        private void snoozeToast_Click(object sender, RoutedEventArgs e)
        {
            string xmlString =
                "<toast>"
                    + "<visual>"
                        + "<binding template=\"ToastGeneric\">"
                        + "<image src=\"ms-appx:///Assets/SecondaryTiles/espresso.jpg\" alt=\"Espresso\"/>"
                        + "<text>\"Espresso\"</text>"
                        + "<text>\"Select snooze time.\"</text>"
                        + "</binding>"
                    + "</visual>"
                    + "<actions>"
                        + "<input id=\"snoozeTime\" type=\"selection\" defaultSelection=\"10\">"
                            + "<selection id=\"1\" content=\"1 minute\" />"
                            + "<selection id=\"5\" content=\"5 minutes\" />"
                            + "<selection id=\"10\" content=\"10 minutes\" />"
                            + "<selection id=\"30\" content=\"30 minutes\" />"
                            + "<selection id=\"60\" content=\"1 hour\" />"
                        + "</input>"
                        + "<action activationType=\"system\" arguments=\"snooze\" hint-inputId=\"snoozeTime\" content=\"\"/>"
                        + "<action activationType=\"system\" arguments=\"dismiss\" content=\"\"/>"
                    + "</actions>"
                + "</toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(xmlString);
            SendNewToast(toastXml, LocalizableStrings.TOAST_SNOOZE);
        }

        /*
        <toast>
          <visual>
            <binding template="ToastGeneric">
              <image placement="appLogoOverride" src="ms-appx:///Assets/SecondaryTiles/espresso.jpg"/>
              <text hint-style="header">Espresso</text>
              <text hint-style="subheader" hint-maxLnes="2">Lorem ipsum dolor sit amet, consectetur elit.</text>
            </binding>
          </visual>
          <actions>
            <input type="text" id="1" placeHolderContent="Type a message"/>
            <action content="send" hint-inputId="1" imageUri="ms-appx:///Assets/Toasts/icon-white-espresso.png" activationType="foreground" arguments="action?=send"/>
          </actions>
        </toast>
        */
        private void inputToast_Click(object sender, RoutedEventArgs e)
        {
            string xmlString =
                "<toast>"
                    + "<visual>"
                    + "<binding template=\"ToastGeneric\">"
                        + "<image placement=\"appLogoOverride\" src=\"ms-appx:///Assets/SecondaryTiles/espresso.jpg\"/>"
                        + "<text hint-style=\"header\">Espresso</text>"
                        + "<text hint-style=\"subheader\" hint-maxLines=\"2\">Lorem ipsum dolor sit amet, consectetur elit.</text>"
                    + "</binding>"
                    + "</visual>"
                    + "<actions>"
                    + "<input type=\"text\" id=\"1\" placeHolderContent=\"Type a message\"/>"
                    + "<action content=\"send\" hint-inputId=\"1\" imageUri=\"ms-appx:///Assets/Toasts/icon-white-espresso.png\" activationType=\"foreground\" arguments=\"action?=send\"/>"
                    + "</actions>"
                + "</toast>";
            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(xmlString);
            SendNewToast(toastXml, LocalizableStrings.TOAST_INPUT);
        }

        /*
        <toast>
          <visual>
            <binding template="ToastGeneric">
              <image src="ms-appx:///Assets/SecondaryTiles/espresso.jpg" alt="Espresso" placement="appLogoOverride"/>
              <text>Espresso</text>
            </binding>
          </visual>
        </toast>
        */
        private void logoOverride_Click(object sender, RoutedEventArgs e)
        {
            string xmlString =
                "<toast>"
                    + "<visual>"
                        + "<binding template=\"ToastGeneric\">"
                        + "<image src=\"ms-appx:///Assets/SecondaryTiles/espresso.jpg\" alt=\"Espresso\" placement=\"appLogoOverride\"/>"
                        + "<text>Espresso</text>"
                        + "</binding>"
                    + "</visual>"
                + "</toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(xmlString);
            SendNewToast(toastXml, LocalizableStrings.TOAST_ICON_OVERRIDE);
        }

        #endregion


        #region Background Task

        // NOTE: Both the ToastsPage and the BackgroundTaskPage can register/unregister the same background task.

        private async void registerTask_Click(object sender, RoutedEventArgs e)
		{
            bool result = await BackgroundTaskHelper.RegisterTimeZoneChangeBackgroundTask(
                LocalizableStrings.BACKGROUND_TASK_TIMEZONE_NAME, LocalizableStrings.BACKGROUND_TASK_TIMEZONE_ENTRYPOINT);
            if (result)
			{
				status.Log(LocalizableStrings.BACKGROUND_TASK_REGISTRATION_SUCCESS);
			}
		}

		private void unregisterTask_Click(object sender, RoutedEventArgs e)
		{
			BackgroundTaskHelper.UnregisterBackgroundTasks();
			status.Log(LocalizableStrings.BACKGROUND_TASK_UNREGISTERED);
		}

		#endregion

	}
}

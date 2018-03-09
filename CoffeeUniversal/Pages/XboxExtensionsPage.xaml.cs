using CoffeeUniversal.Helpers;
using CoffeeUniversal.ViewModels;
using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Xbox.Audio;
using Windows.Xbox.System;

namespace CoffeeUniversal.Pages
{
    public sealed partial class XboxExtensionsPage : Page
    {

        #region init

        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public XboxExtensionsPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);

            if (ApiInformation.IsTypePresent("Windows.Xbox.Audio.SoundClip"))
            {
                status.Log(LocalizableStrings.XBOX_EXTENSION_SOUNDCLIP_FOUND);
                playSoundClip.IsEnabled = true;
            }
            else
            {
                status.Log(LocalizableStrings.XBOX_EXTENSION_SOUNDCLIP_NOT_FOUND);
            }
            if (ApiInformation.IsTypePresent("Windows.Xbox.System.User"))
            {
                status.Log(LocalizableStrings.XBOX_EXTENSION_USER_FOUND);
                getUserInfo.IsEnabled = true;
            }
            else
            {
                status.Log(LocalizableStrings.XBOX_EXTENSION_USER_NOT_FOUND);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        #endregion


        private const string SOUNDCLIP_URL = "ms-appx:///Assets/Media/Ring04.wav";

        private void playSoundClip_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SoundClip soundClip = new SoundClip(new Uri(SOUNDCLIP_URL, UriKind.RelativeOrAbsolute));
                soundClip.Play();
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
            }
        }

        private void getUserInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User user = User.Users[0];
                UserDisplayInfo info = user.DisplayInfo;
                userName.Text = info.ApplicationDisplayName;
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
            }
        }
    }
}

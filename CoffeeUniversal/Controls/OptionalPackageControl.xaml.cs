using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CoffeeUniversal.Controls
{
    public sealed partial class OptionalPackageControl : UserControl
    {
        private Pages.OptionalPackagesPage optionalPackagesPage;
        private Windows.ApplicationModel.Package optionalPackage;

        public Windows.ApplicationModel.Package Package { get { return optionalPackage; } }

        public UInt32 CodeInputValue
        {
            get
            {
                UInt32 parsed = 0;
                UInt32.TryParse(InputCodeValue.Text, out parsed);
                return parsed;
            }
        }

        public UInt32 CodeOutputValue
        {
            set
            {
                OutputCodeValue.Text = value.ToString();
            }
        }

        public UInt32 WinRTInputValue
        {
            get
            {
                UInt32 parsed = 0;
                UInt32.TryParse(InputWinRTValue.Text, out parsed);
                return parsed;
            }
        }

        public UInt32 WinRTOutputValue
        {
            set
            {
                OutputWinRTValue.Text = value.ToString();
            }
        }

        public string TextOutputValue
        {
            set
            {
                OutputTextValue.Text = value;
            }
        }

        public string TextOutputValueLoc
        {
            set
            {
                OutputTextValueLoc.Text = value;
            }
        }

        public Image ScaleImage
        {
            set
            {
                scaleImage = value;
            }
            get
            {
                return scaleImage;
            }
        }


        public OptionalPackageControl(Pages.OptionalPackagesPage OptionalPackagesPage, Windows.ApplicationModel.Package OptionalPackage)
        {
            this.InitializeComponent();

            optionalPackagesPage = OptionalPackagesPage;
            optionalPackage = OptionalPackage;

            SetupControl();
        }

        private async void SetupControl()
        {
            IReadOnlyList<AppListEntry> appListEntry = await optionalPackage.GetAppListEntriesAsync();

            AppListEntry firstEntry = appListEntry.First();
            var filestream = await(firstEntry.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1))).OpenReadAsync();
            BitmapImage logo = new BitmapImage();
            await logo.SetSourceAsync(filestream);

            PackageLogo.Source = logo;

            PackageName.Text = firstEntry.DisplayInfo.DisplayName;
        }

        private void Button_Click_Code(object sender, RoutedEventArgs e)
        {
            optionalPackagesPage.LoadCode(optionalPackage, this);
        }

        private void Button_Click_Code_WinRT(object sender, RoutedEventArgs e)
        {
            optionalPackagesPage.LoadCodeWinRT(optionalPackage, this);
        }

        private void Button_Click_Text(object sender, RoutedEventArgs e)
        {
            optionalPackagesPage.LoadText(optionalPackage, this);
        }

        private void Button_Click_Text_Loc(object sender, RoutedEventArgs e)
        {
            optionalPackagesPage.LoadTextLoc(optionalPackage, this);
        }
        private void Button_Click_Scale_Image(object sender, RoutedEventArgs e)
        {
            optionalPackagesPage.LoadImage(optionalPackage, this);
        }

        private void Button_Click_Show_Details(object sender, RoutedEventArgs e)
        {
            ShowDetailsButton.Visibility = Visibility.Collapsed;
            HidDetailsButton.Visibility = Visibility.Visible;
            OptionalPackageDetails.Visibility = Visibility.Visible;
        }
        private void Button_Click_Hide_Details(object sender, RoutedEventArgs e)
        {
            ShowDetailsButton.Visibility = Visibility.Visible;
            HidDetailsButton.Visibility = Visibility.Collapsed;
            OptionalPackageDetails.Visibility = Visibility.Collapsed;
        }
    }
}

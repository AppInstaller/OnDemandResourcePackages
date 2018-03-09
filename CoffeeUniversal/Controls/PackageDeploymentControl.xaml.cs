using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class PackageDeploymentControl : UserControl
    {
        private Windows.ApplicationModel.Package deployedPackage;

        public PackageDeploymentControl(Windows.ApplicationModel.Package Package)
        {
            this.InitializeComponent();

            this.deployedPackage = Package;

            SetupControl();
        }

        private async void SetupControl()
        {
            IReadOnlyList<AppListEntry> appListEntry = await deployedPackage.GetAppListEntriesAsync();

            if (appListEntry.Count > 0)
            {
                AppListEntry firstEntry = appListEntry.First();
                var filestream = await (firstEntry.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1))).OpenReadAsync();
                BitmapImage logo = new BitmapImage();
                await logo.SetSourceAsync(filestream);

                PackageLogo.Source = logo;
                PackageName.Text = firstEntry.DisplayInfo.DisplayName;
            }
            else
            {
                PackageName.Text = deployedPackage.Id.FullName;
            }

            ProgressBar.IsIndeterminate = true;
        }

        public ProgressBar ProgressBar
        {
            get { return downloadProgressBar; }
            set
            {
                downloadProgressBar.IsIndeterminate = false;
                downloadProgressBar = value;
            }
        }


        public string PackageFamilyName
        {
            get { return deployedPackage.Id.FamilyName; }
        }

        public Windows.ApplicationModel.Package Package
        {
            get { return deployedPackage; }
        }

        public void SetAsDone()
        {
            Brush b = new SolidColorBrush(Windows.UI.Colors.Green);
            ProgressBar.Foreground = b;
            ProgressBar.Value = 100;
        }

    }
}

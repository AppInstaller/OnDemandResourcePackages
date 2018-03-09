using CoffeeUniversal.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using CoffeeUniversal.Controls;
using Windows.UI.Popups;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CoffeeUniversal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MandatoryUpdatesPage : Page
    {
        private static StoreContext updateManager;
        private CoreDispatcher dispatcher;
        private Dictionary<string, PackageDeploymentControl> packageUpdates;
        private ObservableCollection<PackageDeploymentControl> packageUpdatesUI;
        IReadOnlyList<StorePackageUpdate> updates;

        #region Init

        private NavigationHelper navigationHelper;

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public MandatoryUpdatesPage()
        {
            this.InitializeComponent();

            navigationHelper = new NavigationHelper(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            updateManager = StoreContext.GetDefault();
            dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            packageUpdates = new Dictionary<string, PackageDeploymentControl>();
            packageUpdatesUI = new ObservableCollection<PackageDeploymentControl>();

            PackagesUpdating.ItemsSource = packageUpdatesUI;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {

            }
            navigationHelper.OnNavigatedFrom(e);
        }
        #endregion

        #region Mandatory Updates

        private async void Button_Click_Update_Blocking(object sender, RoutedEventArgs e)
        {
            try
            {
                // Block trying to get all updates
                updates = await updateManager.GetAppAndOptionalStorePackageUpdatesAsync();

                if (updates.Count > 0)
                {
                    TotalProgressBar.Visibility = Visibility.Visible;

                    // Trigger download and monitor progress
                    IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> downloadOperation = updateManager.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);
                    downloadOperation.Progress = async (asyncInfo, progress) =>
                    {
                        await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            TotalProgressBar.Value = progress.TotalDownloadProgress * 100;
                        });
                    };

                    // Wait for download and install to complete
                    StorePackageUpdateResult result = await downloadOperation.AsTask();
                }
            }
            catch (Exception ex)
            {
                await new MessageDialog("Unable to perform simple blocking update. {" + err(ex) + "}").ShowAsync();
            }
        }

        private async void Button_Click_Download_Update(object sender, RoutedEventArgs e)
        {
            try
            {
                updates = await updateManager.GetAppAndOptionalStorePackageUpdatesAsync();

                // This isn't necessary, but is an optimization to reduce cycles
                //if (updates.Count > 0)
                {
                    // Local function to download and track progress
                    //      returns: true if success, false otherwise
                    bool downloaded = await DownloadPackageUpdatesAsync(updates);

                    if (downloaded)
                    {
                        // Everything worked, now install the update. This will kill the app on success.
                        InstallButton.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                await new MessageDialog("Unable to perform async update. {" + err(ex) + "}").ShowAsync();
            }
        }
        private async void Button_Click_Install_Update(object sender, RoutedEventArgs e)
        {
            await InstallPackageUpdatesAsync(updates);
        }

        private async Task<bool> DownloadPackageUpdatesAsync(IEnumerable<StorePackageUpdate> updates)
        {
            bool downloadedSuccessfully = false;

            // If automatic updates are on, the download will begin, otherwise it may prompt
            IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> downloadOperation =
                updateManager.RequestDownloadStorePackageUpdatesAsync(updates);

            downloadOperation.Progress = async (asyncInfo, progress) =>
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // Progress gets called once for each progress step of each package.
                    // If you are downloading 10 packages, and each gets 10 notifications...
                    // You will receive 100 progress updates.
                    ShowProgress(
                    updates.SingleOrDefault(update => update.Package.Id.FamilyName == progress.PackageFamilyName),
                    progress);
                });
            };

            // Wait for the download operation to complete
            StorePackageUpdateResult result = await downloadOperation.AsTask();

            switch (result.OverallState)
            {
                case StorePackageUpdateState.Completed:
                    // Everything worked, now ready to install the updates.
                    downloadedSuccessfully = true;
                    break;
                default:
                    // The overall progress didn't complete
                    // Find failed updates
                    var failedUpdates = result.StorePackageUpdateStatuses.Where(status => status.PackageUpdateState != StorePackageUpdateState.Completed);

                    // See if any failed updates were mandatory
                    if (updates.Any(u => u.Mandatory && failedUpdates.Any(failed => failed.PackageFamilyName == u.Package.Id.FamilyName)))
                    {
                        // At least one of the updates is mandatory, so tell the user.
                        ShowMandatoryMessage();
                    }
                    break;
            }

            return downloadedSuccessfully;
        }

        private async Task InstallPackageUpdatesAsync(IEnumerable<StorePackageUpdate> updates)
        {
            // This will prompt the user with a dialog depending on the state:
            //      Download and Install these updates? (size, etc)
            //      Install these updates?
            IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> installOperation =
                updateManager.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);

            // Note: This has "progress" as it can download if necessary
            // Since we separated our download and install into two steps, we won't have anything to download
            // and the download part will complete immediately.  If we hadn't done that, then we would get progress
            // notifications as the package was downloaded.
            StorePackageUpdateResult result = await installOperation.AsTask();

            await new MessageDialog("Installing...").ShowAsync();

            switch (result.OverallState)
            {

                case StorePackageUpdateState.Completed:
                    // Should never hit for this sample. The install kills the app.
                    break;
                default:

                    // The install failed for some reason
                    // Find failed updates
                    var failedUpdates = result.StorePackageUpdateStatuses.Where(status => status.PackageUpdateState != StorePackageUpdateState.Completed);

                    // See if any failed updates were mandatory
                    if (updates.Any(u => u.Mandatory && failedUpdates.Any(failed => failed.PackageFamilyName == u.Package.Id.FamilyName)))
                    {
                        // At least one of the updates is mandatory, so tell the user.
                        ShowMandatoryMessage();
                    }
                    break;

                    //await new MessageDialog("Here 3").ShowAsync();
            }
        }

        private static void ShowMandatoryMessage()
        {
            // TODO: App notify the user that an update is mandatory
            // and the user will be limited in some fashion, or exits.
        }

        private async void ShowProgress(StorePackageUpdate update, StorePackageUpdateStatus progressInfo)
        {
            try
            {
                string displayName = update.Package.DisplayName;
                string packageFamilyName = progressInfo.PackageFamilyName;
                ulong bytesDownloaded = progressInfo.PackageBytesDownloaded;
                ulong totalBytes = progressInfo.PackageDownloadSizeInBytes;
                double downloadProgress = progressInfo.PackageDownloadProgress;
                double overallDownloadProgress = progressInfo.TotalDownloadProgress;

                PackageDeploymentControl currentPackage;
                lock (packageUpdates)
                {
                    // Get the UI Context and create one if one doesn't exist
                    if (!packageUpdates.ContainsKey(packageFamilyName))
                    {
                        PackageDeploymentControl control = new PackageDeploymentControl(update.Package);
                        packageUpdates.Add(packageFamilyName, control);
                        packageUpdatesUI.Add(control);
                    }

                    currentPackage = packageUpdates[packageFamilyName];
                }

                lock (currentPackage)
                {
                    // Possible states for each package
                    switch (progressInfo.PackageUpdateState)
                    {
                        case StorePackageUpdateState.Pending:
                            currentPackage.ProgressBar.IsIndeterminate = true;
                            break;
                        case StorePackageUpdateState.Downloading:
                            currentPackage.ProgressBar.IsIndeterminate = false;
                            currentPackage.ProgressBar.Value = overallDownloadProgress * 100;
                            break;
                        case StorePackageUpdateState.Completed:
                            //packageUpdates.Remove(currentPackage.PackageFamilyName);
                            //packageUpdatesUI.Remove(currentPackage);
                            currentPackage.SetAsDone();
                            break;
                        case StorePackageUpdateState.Canceled:
                            packageUpdates.Remove(currentPackage.PackageFamilyName);
                            packageUpdatesUI.Remove(currentPackage);

                            break;
                        case StorePackageUpdateState.OtherError:
                            // Download and/or Install errored
                            break;
                        case StorePackageUpdateState.ErrorLowBattery:
                            // Download and/or Install stopped due to low battery
                            break;
                        case StorePackageUpdateState.ErrorWiFiRecommended:
                            // Download and/or Install stopped due to Wi-Fi recommendated
                            break;
                        case StorePackageUpdateState.ErrorWiFiRequired:
                            // Download and/or Install stopped due to Wi-Fi required
                            break;
                        default:
                            // Future proof the switch block
                            throw new InvalidOperationException();
                    }
                }
            }
            catch (Exception ex)
            {
                await new MessageDialog("Error while showing progress during async update. {" + err(ex) + "}").ShowAsync();
            }
        }

        #endregion

        #region Helper
        private string err(Exception ex)
        {
            return ex.Message + " HResult: 0x" + ex.HResult.ToString("X");
        }
        #endregion
    }
}

using CoffeeUniversal.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CoffeeUniversal.ViewModels;
using Windows.ApplicationModel.DataTransfer;

namespace CoffeeUniversal.Pages
{
    public sealed partial class LaunchersPage : Page
    {

        #region Init

        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public LaunchersPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("LaunchersPage_OnNavigatedFrom");
            navigationHelper.OnNavigatedFrom(e);
        }


        #endregion


        #region Uri

        private async void OnLaunchUri(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetUri = "http://www.bing.com/images/search?q=coffee";
                Uri uri = new Uri(targetUri, UriKind.Absolute);
                bool success = await Launcher.LaunchUriAsync(uri);
                if (success)
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_SUCCESS, targetUri));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_FAIL, targetUri));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchUri: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchBingMaps(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetUri = "bingmaps:?q=coffee&where=Seattle";
                Uri uri = new Uri(targetUri, UriKind.Absolute);
                bool success = await Launcher.LaunchUriAsync(uri);
                if (success)
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_SUCCESS, targetUri));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_FAIL, targetUri));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchBingMaps: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchMailTo(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                    "mailto:andreww@microsoft.com?subject=Coffee&body={0}",
                    LocalizableStrings.LAUNCHERS_LOREM_IPSUM_BODY);
                Uri mailto = new Uri(message, UriKind.Absolute);

                bool success = await Launcher.LaunchUriAsync(mailto);
                if (success)
                {
                    status.Log(LocalizableStrings.LAUNCHERS_EMAIL_SUCCESS);
                }
                else
                {
                    status.Log(LocalizableStrings.LAUNCHERS_EMAIL_FAIL);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchMailTo: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchStore(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetUri = "ms-windows-store:Search?query=coffee";
                Uri uri = new Uri(targetUri, UriKind.Absolute);
                bool success = await Launcher.LaunchUriAsync(uri);
                if (success)
                {
                    status.Log(LocalizableStrings.LAUNCHERS_STORE_SUCCESS);
                }
                else
                {
                    status.Log(LocalizableStrings.LAUNCHERS_STORE_FAIL);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchStore: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchTargetPfn(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetUri = @"ms-cortana:/StartMode=Proactive";
                string cortanaPfn = @"Microsoft.Windows.Cortana_cw5n1h2txyewy";
                Uri uri = new Uri(targetUri, UriKind.Absolute);
                LauncherOptions options = new LauncherOptions();
                options.TargetApplicationPackageFamilyName = cortanaPfn;
                bool success = await Launcher.LaunchUriAsync(uri, options);
                if (success)
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_SUCCESS, targetUri));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_FAIL, targetUri));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchTargetPfn: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchFallbackUri(object sender, RoutedEventArgs e)
        {
            try
            {
                string unsupportedTargetUri = "wechat:";
                Uri invalidUri = new Uri(unsupportedTargetUri, UriKind.Absolute);

                LauncherOptions options = new LauncherOptions();
                options.FallbackUri = new Uri(@"https://en.wikipedia.org/wiki/Fallback", UriKind.Absolute);

                bool success = await Launcher.LaunchUriAsync(invalidUri, options);
                if (success) // Must launch the fallback Uri
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_FALLBACK_URI_SUCCESS, options.FallbackUri));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_FALLBACK_URI_FAIL, options.FallbackUri));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchFallbackUri: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchTargetPfnUnsupportedUri(object sender, RoutedEventArgs e)
        {
            try
            {
                string unsupportedTargetUri = @"ms-cortanaspoof:/StartMode=Proactive";
                string cortanaPfn = @"Microsoft.Windows.Cortana_cw5n1h2txyewy";
                Uri invalidUri = new Uri(unsupportedTargetUri, UriKind.Absolute);
                LauncherOptions options = new LauncherOptions();
                options.TargetApplicationPackageFamilyName = cortanaPfn;
                bool success = await Launcher.LaunchUriAsync(invalidUri, options);
                if (!success)
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_TARGET_PFN_BAD_URI_SUCCESS, unsupportedTargetUri, cortanaPfn));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_TARGET_PFN_BAD_URI_FAIL, unsupportedTargetUri, cortanaPfn));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchTargetPfnUnsupportedUri: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchUnsupportedUri(object sender, RoutedEventArgs e)
        {
            try
            {
                string unsupportedTargetUri = "wechat:";
                Uri invalidUri = new Uri(unsupportedTargetUri, UriKind.Absolute);
                bool success = await Launcher.LaunchUriAsync(invalidUri);
                if (success) // Store Launch expected
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_UNSUPPORTED_URI_SUCCESS, unsupportedTargetUri));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_UNSUPPORTED_URI_FAIL, unsupportedTargetUri));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchUnsupportedUri: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchUriForResultsInvalidPfn(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetUri = @"for-results:";
                string launcherTargetPfn = @"Microsoft.Windows.Illusion_8wekyb3d8bbwe";
                Uri uri = new Uri(targetUri, UriKind.Absolute);
                LauncherOptions options = new LauncherOptions();
                options.TargetApplicationPackageFamilyName = launcherTargetPfn;
                LaunchUriResult result = await Launcher.LaunchUriForResultsAsync(uri, options);
                if (result.Status == LaunchUriStatus.AppUnavailable)
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_URI_BAD_PFN_SUCCESS, result.Status));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_URI_BAD_PFN_FAIL, result.Status));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchUriForResultsInvalidPFN: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchSL80App(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetUri = "target-sl80:";
                Uri uri = new Uri(targetUri, UriKind.Absolute);
                bool success = await Launcher.LaunchUriAsync(uri);
                if (success)
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_SUCCESS, targetUri));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_FAIL, targetUri));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchSL80App: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchSL81App(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetUri = "target-sl81:";
                Uri uri = new Uri(targetUri, UriKind.Absolute);
                bool success = await Launcher.LaunchUriAsync(uri);
                if (success)
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_SUCCESS, targetUri));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_FAIL, targetUri));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchSL81App: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        #endregion


        #region LaunchUriTargetMultiApp_8wekyb3d8bbwe

        private async void OnLaunchUriForResults(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetUri = @"for-results:";
                string launcherTargetPfn = @"LaunchUriTargetMultiApp_8wekyb3d8bbwe";
                Uri uri = new Uri(targetUri, UriKind.Absolute);
                LauncherOptions options = new LauncherOptions();
                options.TargetApplicationPackageFamilyName = launcherTargetPfn;
                LaunchUriResult result = await Launcher.LaunchUriForResultsAsync(uri, options);
                if (result.Status == LaunchUriStatus.Success)
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_SUCCESS, targetUri));

                    // Change to validation later
                    string dataReceived = result.Result["Result"].ToString() + " : " +
                                            result.Result["UriScheme"].ToString() + " : " +
                                            result.Result["CallerPfn"].ToString();
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_FOR_RESULTS_SUCCESS, dataReceived));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_FOR_RESULTS_FAIL, result.Status));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchUriForResults: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchUriWithData(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetUri = @"for-results2:";
                string targetUriScheme = @"for-results";
                string launcherTargetPfn = @"LaunchUriTargetMultiApp_8wekyb3d8bbwe";
                Uri uri = new Uri(targetUri, UriKind.Absolute);
                LauncherOptions options = new LauncherOptions();
                options.TargetApplicationPackageFamilyName = launcherTargetPfn;
                ValueSet data = new ValueSet();
                // Get UTC filetime and reduce resolution from 100ns to 10ms, expecting target to receive data within 10ms
                long timeSuffix = DateTime.Now.ToFileTimeUtc();
                timeSuffix = (timeSuffix / 10000000000L);
                data.Add("Data", "Hello Target App_" + timeSuffix);
                data.Add("UriScheme", targetUriScheme);
                bool success = await Launcher.LaunchUriAsync(uri, options, data);
                if (success)
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_SUCCESS, targetUri));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_FAIL, targetUri));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchUriWithData: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchUriForResultsWithData(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetUri = @"for-results:";
                string targetUriScheme = @"for-results";
                string launcherTargetPfn = @"LaunchUriTargetMultiApp_8wekyb3d8bbwe";
                Uri uri = new Uri(targetUri, UriKind.Absolute);
                LauncherOptions options = new LauncherOptions();
                options.TargetApplicationPackageFamilyName = launcherTargetPfn;
                ValueSet data = new ValueSet();
                long timeSuffix = DateTime.Now.ToFileTimeUtc();
                timeSuffix = (timeSuffix / 10000000000L);
                data.Add("Data", "Hello Target App_" + timeSuffix);
                data.Add("UriScheme", targetUriScheme);
                LaunchUriResult result = await Launcher.LaunchUriForResultsAsync(uri, options, data);
                if (result.Status == LaunchUriStatus.Success)
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_SUCCESS, targetUri));
                    string dataReceived = result.Result["Result"].ToString() + " : " +
                                           result.Result["UriScheme"].ToString() + " : " +
                                            result.Result["CallerPfn"].ToString() + " : " +
                                           result.Result["DataStatus"].ToString();
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_FOR_RESULTS_WITH_DATA_SUCCESS, dataReceived));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_FAIL, result.Status));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchUriForResultsWithData: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchUriForResultsUnsupportedProtocol(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetUri = @"for-noresults:";
                string launcherTargetPfn = @"LaunchUriTargetMultiApp_8wekyb3d8bbwe";
                Uri uri = new Uri(targetUri, UriKind.Absolute);
                LauncherOptions options = new LauncherOptions();
                options.TargetApplicationPackageFamilyName = launcherTargetPfn;
                LaunchUriResult result = await Launcher.LaunchUriForResultsAsync(uri, options);
                if (result.Status == LaunchUriStatus.ProtocolUnavailable)
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_URI_BAD_PROTOCOL_SUCCESS, result.Status));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_URI_BAD_PROTOCOL_SUCCESS, result.Status));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchUriForResultsUnsupportedProtocol: " + ex.ToString());
                status.Log(ex.Message);
            }
        }
        
        #endregion


        #region File

        private async void OnLaunchFile(object sender, RoutedEventArgs e)
        {
            try
            {
                string imageFile = @"\Assets\Share\espresso_450x450.jpg";
                string installPath = Package.Current.InstalledLocation.Path;
                StorageFile file = await StorageFile.GetFileFromPathAsync(installPath + imageFile);
                if (file != null)
                {
                    bool success = await Launcher.LaunchFileAsync(file);
                    if (success)
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_APP_LAUNCH_SUCCESS, imageFile));
                    }
                    else
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_APP_LAUNCH_FAIL, imageFile));
                    }
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_INVALID_FILE, imageFile));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchFile: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchFileTargetPfn(object sender, RoutedEventArgs e)
        {
            try
            {
                string photosAppPfn = @"Microsoft.Windows.Photos_8wekyb3d8bbwe";
                string imageFile = @"\Assets\SecondaryTiles\latte.jpg";
                string installPath = Package.Current.InstalledLocation.Path;
                StorageFile file = await StorageFile.GetFileFromPathAsync(installPath + imageFile);
                if (file != null)
                {
                    LauncherOptions options = new LauncherOptions();
                    options.TargetApplicationPackageFamilyName = photosAppPfn;
                    bool success = await Launcher.LaunchFileAsync(file, options);
                    if (success)
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_APP_LAUNCH_SUCCESS, imageFile));
                    }
                    else
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_APP_LAUNCH_FAIL, imageFile));
                    }
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_INVALID_FILE, imageFile));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchFileTargetPfn: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchFileUnsupported(object sender, RoutedEventArgs e)
        {
            string fileExtNotSupported = @"\Assets\Share\AlienFile.wechat";
            string installPath = Package.Current.InstalledLocation.Path;
            StorageFile file = await StorageFile.GetFileFromPathAsync(installPath + fileExtNotSupported);
            try
            {
                if (file != null)
                {
                    bool success = await Launcher.LaunchFileAsync(file);
                    if (success)  // Store Launch expected
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_FILE_BAD_FILE_SUCCESS));
                    }
                    else
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_FILE_BAD_FILE_FAIL));
                    }
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_INVALID_FILE, fileExtNotSupported));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchFileUnsupported: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchTargetPfnFileUnsupported(object sender, RoutedEventArgs e)
        {
            string photosAppPfn = @"Microsoft.Windows.Photos_8wekyb3d8bbwe";
            string fileExtNotSupported = @"\Assets\Share\AlienFile.wechat";
            string installPath = Package.Current.InstalledLocation.Path;
            StorageFile file = await StorageFile.GetFileFromPathAsync(installPath + fileExtNotSupported);
            try
            {
                if (file != null)
                {
                    LauncherOptions options = new LauncherOptions();
                    options.TargetApplicationPackageFamilyName = photosAppPfn;
                    bool success = await Launcher.LaunchFileAsync(file, options);
                    if (!success) // Photos app do not support files with .unknwn extension.
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_PFN_BAD_FILE_SUCCESS, photosAppPfn));
                    }
                    else
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_PFN_BAD_FILE_FAIL, photosAppPfn));
                    }
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_INVALID_FILE, fileExtNotSupported));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchTargetPfnFileUnsupported: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchFileFallbackUri(object sender, RoutedEventArgs e)
        {
            string fileExtNotSupported = @"\Assets\Share\AlienFile.wechat";
            string installPath = Package.Current.InstalledLocation.Path;
            string fileName = installPath + fileExtNotSupported;
            StorageFile file = await StorageFile.GetFileFromPathAsync(fileName);
            try
            {
                LauncherOptions options = new LauncherOptions();
                options.FallbackUri = new Uri(@"https://en.wikipedia.org/wiki/Fallback", UriKind.Absolute);
                if (file != null)
                {
                    bool success = await Launcher.LaunchFileAsync(file, options);
                    if (success)
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_FALLBACK_URI_SUCCESS, options.FallbackUri));
                    }
                    else
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_FALLBACK_URI_FAIL, options.FallbackUri));
                    }
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_CREATE_FILE_FAIL, fileName));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchFileFallbackUri: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchFileNFQ(object sender, RoutedEventArgs e)
        {
            try
            {
                string imageFile = @"\Assets\Launchers\technicaldebts.jpg";
                List<string> fileTypeFilter = new List<string>();
                fileTypeFilter.Add(".jpg");
                fileTypeFilter.Add(".png");
                fileTypeFilter.Add(".tif");
                string installPath = Package.Current.InstalledLocation.Path;
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(installPath + @"\Assets\Launchers");
                QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);
                StorageFileQueryResult nfq = folder.CreateFileQueryWithOptions(queryOptions);
                StorageFile file = await StorageFile.GetFileFromPathAsync(installPath + imageFile);

                if (file != null)
                {
                    LauncherOptions options = new LauncherOptions();
                    options.NeighboringFilesQuery = nfq;
                    bool success = await Launcher.LaunchFileAsync(file, options);
                    if (success)
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_APP_LAUNCH_SUCCESS, imageFile));
                    }
                    else
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_APP_LAUNCH_FAIL, imageFile));
                    }
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_INVALID_FILE, imageFile));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchFileNFQ: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchFileNFQPicker(object sender, RoutedEventArgs e)
        {
            try
            {
                string imageFile = @"\Assets\Launchers\technicaldebts.jpg";
                List<string> fileTypeFilter = new List<string>();
                fileTypeFilter.Add(".jpg");
                fileTypeFilter.Add(".png");
                fileTypeFilter.Add(".tif");
                string installPath = Package.Current.InstalledLocation.Path;
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(installPath + @"\Assets\Launchers");
                QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);
                StorageFileQueryResult nfq = folder.CreateFileQueryWithOptions(queryOptions);
                StorageFile file = await StorageFile.GetFileFromPathAsync(installPath + imageFile);

                if (file != null)
                {
                    LauncherOptions options = new LauncherOptions();
                    options.NeighboringFilesQuery = nfq;
                    options.DisplayApplicationPicker = true;
                    bool success = await Launcher.LaunchFileAsync(file, options);
                    if (success)
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_APP_LAUNCH_SUCCESS, imageFile));
                    }
                    else
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_APP_LAUNCH_FAIL, imageFile));
                    }
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_INVALID_FILE, imageFile));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchFileNFQPicker: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        #endregion


        #region Folder
        private async void OnLaunchFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                string installPath = Package.Current.InstalledLocation.Path;
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(installPath);
                if (folder != null)
                {
                    bool success = await Launcher.LaunchFolderAsync(folder);
                    if (success)
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_LAUNCH_FOLDER_SUCCESS, installPath));
                    }
                    else
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_LAUNCH_FOLDER_FAIL, installPath));
                    }
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_OPEN_FOLDER_FAIL, installPath));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchFolder: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchFolderSelectedItems(object sender, RoutedEventArgs e)
        {
            try
            {
                string imageFile1 = @"\Assets\Share\espresso_450x450.jpg";
                string imageFile2 = @"\Assets\Share\AlienFile.wechat";
                string installPath = Package.Current.InstalledLocation.Path;
                StorageFile file1 = await StorageFile.GetFileFromPathAsync(installPath + imageFile1);
                StorageFile file2 = await StorageFile.GetFileFromPathAsync(installPath + imageFile2);
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(installPath + @"\Assets\Share");

                if (folder != null)
                {
                    FolderLauncherOptions options = new FolderLauncherOptions();
                    options.ItemsToSelect.Add(file1);
                    options.ItemsToSelect.Add(file2);
                    bool success = await Launcher.LaunchFolderAsync(folder, options);
                    if (success)
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_FOLDER_SELECTED_SUCCESS, installPath));
                    }
                    else
                    {
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.LAUNCHERS_FOLDER_SELECTED_SUCCESS, installPath));
                    }
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_OPEN_FOLDER_FAIL, installPath));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchFolderSelectedItems: " + ex.ToString());
                status.Log(ex.Message);
            }
        }

        private async void OnLaunchUriSharedStorageToken(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetUri = @"for-results2:";
                string launcherTargetPfn = @"LaunchUriTargetMultiApp_8wekyb3d8bbwe";
                Uri uri = new Uri(targetUri, UriKind.Absolute);
                LauncherOptions options = new LauncherOptions();
                options.TargetApplicationPackageFamilyName = launcherTargetPfn;
                ValueSet data = new ValueSet();
                // Get UTC filetime and reduce resolution from 100ns to 10ms, expecting target to receive data within 10ms
                long timeSuffix = DateTime.Now.ToFileTimeUtc();
                timeSuffix = (timeSuffix / 10000000000L);
                data.Add("Data", "Hello Target App_" + timeSuffix);

                string imageFile = @"\Assets\Launchers\technicaldebts.jpg";
                string installPath = Package.Current.InstalledLocation.Path;
                StorageFile file = await StorageFile.GetFileFromPathAsync(installPath + imageFile);
                string filetoken = SharedStorageAccessManager.AddFile(file);
                data.Add("SSAM Token", filetoken);

                bool success = await Launcher.LaunchUriAsync(uri, options, data);
                if (success)
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_SUCCESS, file.Name));
                }
                else
                {
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.LAUNCHERS_APP_LAUNCH_FAIL, file.Name));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LaunchersPage.OnLaunchUriSharedStorageToken: " + ex.ToString());
                status.Log(ex.Message);
            }
        }
        #endregion

    }
}

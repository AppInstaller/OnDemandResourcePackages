using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Storage;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;
using System.Reflection;
using Windows.UI.Core;
using CoffeeUniversal.Helpers;
using Windows.Services.Store;
using CoffeeUniversal.Controls;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml.Media.Imaging;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CoffeeUniversal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame. //Jasosal
    /// </summary>
    public sealed partial class OptionalPackagesPage : Page
    {
        #region Globals
        private PackageCatalog packageCatalog;
        private ObservableCollection<OptionalPackageControl> optionalPackagesList;
        private Dictionary<string, PackageDeploymentControl> deployingOptionalPackagesList;
        private ObservableCollection<PackageDeploymentControl> deployingOptionalPackagesListUI;
        private ObservableCollection<StoreProductControl> storeOptionalPackagesList;
        private CoreDispatcher dispatcher;
        private StoreContext context;

        private const string OPT_PKG_TEXT_FILE = @"OptionalPackageText.txt";
        private const string OPT_PKG_LIB_FILE = @"CoffeeOptionalPackageDLL.dll";
        private const string OPT_PKG_LIB_FACTORIAL_EXPORT = @"?Factorial@@YAII@Z";
        #endregion

        #region Init

        private NavigationHelper navigationHelper;

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public OptionalPackagesPage()
        {
            this.InitializeComponent();

            navigationHelper = new NavigationHelper(this);

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            try
            {
                packageCatalog = PackageCatalog.OpenForCurrentPackage();
                packageCatalog.PackageInstalling += Catalog_PackageInstalling;
                packageCatalog.PackageStaging += Catalog_PackageStaging;
                packageCatalog.PackageStatusChanged += Catalog_PackageStatusChanged;
            }
            catch (Exception ex)
            {
                await new MessageDialog("Unable to setup deployment event handlers. {" + ex.InnerException + "}").ShowAsync();
            }

            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            optionalPackagesList = new ObservableCollection<Controls.OptionalPackageControl>();
            OptionalPackagesListView.ItemsSource = optionalPackagesList;

            storeOptionalPackagesList = new ObservableCollection<StoreProductControl>();
            StoreOptionalPackagesListView.ItemsSource = storeOptionalPackagesList;

            deployingOptionalPackagesList = new Dictionary<string, PackageDeploymentControl>();
            deployingOptionalPackagesListUI = new ObservableCollection<PackageDeploymentControl>();
            DeployingOptionalPackagesListView.ItemsSource = deployingOptionalPackagesListUI;

            context = StoreContext.GetDefault();

            LoadInstalledOptionalPackages();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {

            }
            navigationHelper.OnNavigatedFrom(e);
        }
        #endregion

        #region Loading Pre-installed Packages

        private void LoadInstalledOptionalPackages()
        {
            //get the main package (even if the calling application was installed in an optional package.)
            Package currentPackage = Package.Current;

            //get the list of dependencies for the main package, includes all resource packages, framework packages, and optional packages.
            IReadOnlyList<Package> dependencies = currentPackage.Dependencies;

            IEnumerable<Package> optionalPackages = dependencies.Where(p => p.IsOptional);

            foreach(Package package in optionalPackages)
            {
                LoadPackageToUI(package);
            }
        }

        private async void LoadPackageToUI(Package package)
        {
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Controls.OptionalPackageControl opc = new Controls.OptionalPackageControl(this, package);
                optionalPackagesList.Add(opc);
            });
        }

        private async void LoadDeployingPackageToUI(Package package, double progress)
        {
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (!deployingOptionalPackagesList.ContainsKey(package.Id.FullName))
                {
                    Controls.PackageDeploymentControl pdc = new Controls.PackageDeploymentControl(package);
                    deployingOptionalPackagesList.Add(package.Id.FullName, pdc);
                    deployingOptionalPackagesListUI.Add(pdc);
                }
                deployingOptionalPackagesList[package.Id.FullName].ProgressBar.Value = progress;
            });
        }

        private async void RemoveDeployingPackageFromUI(Package package)
        {
            try
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var control = deployingOptionalPackagesList[package.Id.FullName];

                    deployingOptionalPackagesList.Remove(package.Id.FullName);
                    deployingOptionalPackagesListUI.Remove(control);
                });
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region Deployment Event Handlers
        private void Catalog_PackageInstalling(PackageCatalog sender, PackageInstallingEventArgs args)
        {
            if(args.Progress < 100)
                LoadDeployingPackageToUI(args.Package, args.Progress);
            else if (args.Progress == 100 && args.IsComplete ) 
            {
                RemoveDeployingPackageFromUI(args.Package);

                bool isPresent = false;
                foreach (var control in optionalPackagesList)
                    if (control.Package.Id.FullName.CompareTo(args.Package.Id.FullName) == 0)
                        isPresent = true;
                if(!isPresent)
                    LoadPackageToUI(args.Package);
            }
        }

        private void Catalog_PackageStatusChanged(PackageCatalog sender, PackageStatusChangedEventArgs args)
        {
            // Doesn't fire - 7535335
        }

        private void Catalog_PackageStaging(PackageCatalog sender, PackageStagingEventArgs args)
        {
            // Doesn't fire - 7535335
            //LoadDeployingPackageToUI(args.Package, args.Progress);
        }

        #endregion

        #region Loading Text / Code / Image
        public async void LoadText(Package package, Controls.OptionalPackageControl control)
        {
            try
            {
                StorageFolder installedLocation = package.InstalledLocation;
                StorageFile textFile = await installedLocation.GetFileAsync(OPT_PKG_TEXT_FILE);
                string textContent = await FileIO.ReadTextAsync(textFile);

                control.TextOutputValue = textContent;
            }
            catch(Exception ex)
            {
                await new MessageDialog("Unable to load text from Optional Package. {" + err(ex) + "}").ShowAsync();
            }
        }

        public async void LoadTextLoc(Package package, Controls.OptionalPackageControl control)
        {
            try
            {
                var resourceManager = ResourceManager.Current;
                var resourceMap = resourceManager.AllResourceMaps[package.Id.Name];
                var fileMap = resourceMap.GetSubtree("Files");
                var textFile = await fileMap.GetValue("Assets/Languages/textfile1.txt").GetValueAsFileAsync();
                string textContent = await FileIO.ReadTextAsync(textFile);
                control.TextOutputValueLoc = textContent;
            }
            catch (Exception ex)
            {
                await new MessageDialog("Unable to load localized text from Optional Package. {" + err(ex) + "}").ShowAsync();
            }
        }

        delegate UInt32 CodeDelegate(UInt32 smallNumber);
        public async void LoadCode(Package package, Controls.OptionalPackageControl control)
        {
            try
            {
                IntPtr handle = LoadPackagedLibrary(OPT_PKG_LIB_FILE);

                if (handle == IntPtr.Zero)
                    await new MessageDialog("Unable to load code file from Optional Package - Restart Main Package").ShowAsync();
                else
                {
                    IntPtr factorialFuncPTR = GetProcAddress(handle, OPT_PKG_LIB_FACTORIAL_EXPORT);
                    if (factorialFuncPTR != IntPtr.Zero)
                    {
                        CodeDelegate function = Marshal.GetDelegateForFunctionPointer<CodeDelegate>(factorialFuncPTR);
                        UInt32 inputValue = control.CodeInputValue;
                        UInt32 functionReturn = function(inputValue);
                        control.CodeOutputValue = functionReturn;
                    }
                }
            }
            catch (Exception ex)
            {
                await new MessageDialog("Unable to load code from Optional Package. {" + err(ex) + "}").ShowAsync();
            }
        }

        public async void LoadImage(Package package, Controls.OptionalPackageControl control)
        {
            try
            {
                var resourceManager = ResourceManager.Current;
                var resourceMap = resourceManager.AllResourceMaps[package.Id.Name];
                var fileMap = resourceMap.GetSubtree("Files");
                var imageFile = await fileMap.GetValue("Assets/Scales/scale-image.jpg").GetValueAsFileAsync();
                var filestream = await imageFile.OpenReadAsync();
                var logo = new BitmapImage();
                await logo.SetSourceAsync(filestream);
                control.ScaleImage.Source = logo;
            }
            catch (Exception ex)
            {
                await new MessageDialog("Unable to load scale image from Optional Package. {" + err(ex) + "}").ShowAsync();
            }
        }

        public async void LoadCodeWinRT(Package package, Controls.OptionalPackageControl control)
        {
            try
            {
                //var optionalPackageWinRTClass = new OptWinRT.Class1();
                //UInt32 inputValue = control.WinRTInputValue;
                //UInt32 squareReturn = optionalPackageWinRTClass.Square(inputValue);
                //control.WinRTOutputValue = squareReturn;
            }
            catch (Exception ex)
            {
                await new MessageDialog("Unable to execute WinRT Function. {" + err(ex) + "}").ShowAsync();
            }
        }

        #endregion

        #region Helper
        private string err(Exception ex)
        {
            return ex.Message + " HResult: 0x" + ex.HResult.ToString("X");
        }
        #endregion

        #region Interop
        [DllImport("kernel32", EntryPoint = "LoadPackagedLibrary", SetLastError = true)]
        static extern IntPtr LoadPackagedLibrary([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, int reserved = 0);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        #endregion

        #region Store Services + Optional Packages
        private async void Enumerate_Optional_Packages_From_Store(object sender, RoutedEventArgs e)
        {
            try
            {
                String[] filterList = new string[] { "Consumable", "Durable", "UnmanagedConsumable" };

                StoreProductQueryResult addOns = await context.GetAssociatedStoreProductsAsync(filterList);

                foreach (var addOn in addOns.Products)
                {
                    StoreProductControl spc = new StoreProductControl();
                    spc.PackageName = addOn.Value.Title;
                    spc.DataContext = addOn.Value;

                    storeOptionalPackagesList.Add(spc);
                }
                
            }
            catch(Exception ex)
            {
                await new MessageDialog("Unable to enumerate store optional packages. {" + ex.Message + "}").ShowAsync();
            }
        }
        #endregion
    }
}

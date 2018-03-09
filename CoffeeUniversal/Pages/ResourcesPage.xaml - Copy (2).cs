using CoffeeUniversal.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.UI.Popups;

namespace CoffeeUniversal.Pages
{
    public sealed partial class ResourcesPage : Page
    {

        #region Standard init

        private List<ProgressBar> ProgressBarList;
        private List<Button> DownloadButtonList;
        private List<Button> CancelButtonList;
        private List<Button> RemoveButtonList;


        private enum AvailableLanguages
        {
            ar_SA = 0,
            cs_CZ,
            de_DE,
            en_US,
            es_ES,
            fr_FR,
            he_IL,
            ru_RU
        }


        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public ResourcesPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);            

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                DownloadButtonList = new List<Button>();
                CancelButtonList = new List<Button>();
                RemoveButtonList = new List<Button>();
                ProgressBarList = new List<ProgressBar>();


                // load localized text from file
                Uri fileUri = new Uri("ms-appx:///Assets/Languages/textfile1.txt");
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(fileUri);
                string fileText = await FileIO.ReadTextAsync(file);

                // set localized text
                localizedText.Text = fileText;

                // local scale image
                scaleImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Scales/scale-image.jpg"));
            }
            catch (Exception ex)
            {
                localizedText.Text = ex.Message;
            }

            navigationHelper.OnNavigatedTo(e);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetHandlesOfUXElements();
        }

        private void GetHandlesOfUXElements()
        {
            ResourceContext resContext = ResourceContext.GetForCurrentView();
            
                ProgressBarList = AllChildren(this.ItemPacksList).OfType<ProgressBar>().ToList();                   

                var _ButtonList = AllChildren(this.ItemPacksList).OfType<Button>();
                foreach(var button in _ButtonList)
                {
                    switch(button.Tag.ToString())                        
                    {
                        case "DownloadItem":
                            DownloadButtonList.Add(button);
                            break;
                        case "CancelItem":
                            CancelButtonList.Add(button);
                            break;
                        case "RemoveItem":
                            RemoveButtonList.Add(button);
                            break;
                    }
                }

            foreach (AvailableLanguages lang in Enum.GetValues(typeof(AvailableLanguages)))
            {

                string bcpLang = Enum.GetName(typeof(AvailableLanguages), lang);
                bcpLang = bcpLang.Replace('_', '-');
                if (IsResourcePackageAvailable(bcpLang))
                {
                    ManipulateUX(lang, false, false, false, true);
                }
                else
                {
                    ManipulateUX(lang, false, true, false, false);
                }                
            }                 
               
        }


        private bool IsResourcePackageAvailable(string lang)
        {
            //ResourceContext resContext = ResourceContext.GetForCurrentView();
            //return resContext.Languages.Contains(lang);

            bool ret = false;
            ResourceContext resContext = ResourceContext.GetForCurrentView();
            string resourceId = @"files/Assets/Languages/textfile1.txt";

            var namedResource = ResourceManager.Current.MainResourceMap[resourceId];
            var resourceCandidates = namedResource.ResolveAll(resContext);
            foreach (var candidate in resourceCandidates)
            {
                if (candidate.Qualifiers[0].QualifierValue.Contains(lang.ToUpper()))
                {
                    return ret = true;
                }
            }

            return ret;
        }

        private void ManipulateUX(AvailableLanguages Language, bool ShowProgressBar, bool ShowDownloadButton, bool ShowCancelButton, bool ShowRemoveButton)
        {
            if(ShowProgressBar)
            {                
                ProgressBarList[(int)Language].Visibility = Visibility.Visible;
            }
            else
            {
                ProgressBarList[(int)Language].Visibility = Visibility.Collapsed;
            }

            if (ShowDownloadButton)
            {
                DownloadButtonList[(int)Language].Visibility = Visibility.Visible;
            }
            else
            {
                DownloadButtonList[(int)Language].Visibility = Visibility.Collapsed;
            }

            if (ShowCancelButton)
            {
                CancelButtonList[(int)Language].Visibility = Visibility.Visible;
            }
            else
            {
                CancelButtonList[(int)Language].Visibility = Visibility.Collapsed;
            }

            if (ShowRemoveButton)
            {
                RemoveButtonList[(int)Language].Visibility = Visibility.Visible;
            }
            else
            {
                RemoveButtonList[(int)Language].Visibility = Visibility.Collapsed;
            }

        }

        private async void LevelItemClick(object sender, ItemClickEventArgs e)
        {
            //string lang = ((sender as ListBox).SelectedItem.ToString());

            String lang = (String)e.ClickedItem;

            //var qualifierValues = resContext.QualifierValues;
            //qualifierValues["language"] = lang;            

            //// load localized text from file
            //Uri fileUri = new Uri("ms-appx:///Assets/Languages/textfile1.txt");
            //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(fileUri);
            //string fileText = await FileIO.ReadTextAsync(file);

            //// set localized text
            //localizedText.Text = fileText;

            //ResourceContext resContext = ResourceContext.GetForCurrentView();

            //string resourceId = @"files/Assets/Languages/textfile1.txt";

            //var qualifierValues = resContext.QualifierValues;
            //var checkString = ";" + lang;
            //string originalLanguageList = qualifierValues["language"];
            //if(originalLanguageList.Contains(checkString))
            //{
            //    string requestedLanguage = originalLanguageList.Replace(checkString , "");
            //    requestedLanguage = lang + ";" + requestedLanguage;
            //    qualifierValues["language"] = requestedLanguage;

            //    var namedResource = ResourceManager.Current.MainResourceMap[resourceId];
            //    var resourceCandidates = namedResource.Resolve(resContext);

            //    StorageFile file = await resourceCandidates.GetValueAsFileAsync();
            //    string fileText = await FileIO.ReadTextAsync(file);

            //    // set localized text
            //    localizedText.Text = fileText;

            //}
            //else
            //{

            //    await new MessageDialog("MRT could not find language. Current MRT ResourceContext: " + originalLanguageList).ShowAsync();
            //    var namedResource = ResourceManager.Current.MainResourceMap[resourceId];
            //    var resourceCandidates = namedResource.ResolveAll(resContext);
            //    foreach (var candidate in resourceCandidates)
            //    {
            //        await new MessageDialog(candidate.Qualifiers[0].QualifierName + "***" + candidate.Qualifiers[0].QualifierValue).ShowAsync();

            //        StorageFile file = await candidate.GetValueAsFileAsync();
            //        string fileText = await FileIO.ReadTextAsync(file);
            //        await new MessageDialog(fileText).ShowAsync();
            //    }

            if (IsResourcePackageAvailable(lang))
            {
                ResourceContext resContext = ResourceContext.GetForCurrentView();
                string resourceId = @"files/Assets/Languages/textfile1.txt";

                var namedResource = ResourceManager.Current.MainResourceMap[resourceId];
                var resourceCandidates = namedResource.ResolveAll(resContext);
                foreach (var candidate in resourceCandidates)
                {
                    //Qualifier[0] == Language
                    if (candidate.Qualifiers[0].QualifierValue.Contains(lang.ToUpper()))
                    {
                        StorageFile file = await candidate.GetValueAsFileAsync();
                        string fileText = await FileIO.ReadTextAsync(file);
                        await new MessageDialog(fileText).ShowAsync();

                        // set localized text
                        localizedText.Text = fileText;
                    }
                }
            }
            else
            {
                await new MessageDialog("Language not found").ShowAsync();

            }            

            //string resourceId = @"files/Assets/Languages/textfile1.txt";

            //var namedResource = ResourceManager.Current.MainResourceMap[resourceId];
            //var resourceCandidates = namedResource.Resolve(resContext);

            //StorageFile file = await resourceCandidates.GetValueAsFileAsync();
            //string fileText = await FileIO.ReadTextAsync(file);

            //// set localized text
            //localizedText.Text = fileText;

           

        }

        private async void DownloadItemPackClick(object sender, RoutedEventArgs e)
        {
            String lang = ((Button)sender).DataContext as String;
            string resourceId = "split.language-" + lang.Substring(0, 2).ToLower();

            AvailableLanguages languageSel = (AvailableLanguages)Enum.Parse(typeof(AvailableLanguages), lang.Replace('-','_'));

            var packageCatalog = PackageCatalog.OpenForCurrentPackage();
            var result = await packageCatalog.AddResourcePackageAsync("29270depappf.CaffeMacchiato_gah1vdar1nn7a", resourceId, AddResourcePackageOptions.ApplyUpdateIfAvailable | AddResourcePackageOptions.ForceTargetApplicationShutdown).AsTask<PackageCatalogAddResourcePackageResult, PackageInstallProgress>(new Progress<PackageInstallProgress>
        (progress =>
        {

            DownloadButtonList[(int)languageSel].Visibility = Visibility.Collapsed;
            ProgressBarList[(int)languageSel].Visibility = Visibility.Visible;
            CancelButtonList[(int)languageSel].Visibility = Visibility.Visible;

            ProgressBarList[(int)languageSel].Value = progress.PercentComplete;

        }));
            if (result.ExtendedError != null)
            {
                await new MessageDialog(result.ExtendedError.ToString()).ShowAsync();

                DownloadButtonList[(int)languageSel].Visibility = Visibility.Visible;
                ProgressBarList[(int)languageSel].Visibility = Visibility.Collapsed;
                CancelButtonList[(int)languageSel].Visibility = Visibility.Collapsed;
            }
            else if (result.IsComplete)
            {
                ProgressBarList[(int)languageSel].Visibility = Visibility.Collapsed;
                RemoveButtonList[(int)languageSel].Visibility = Visibility.Visible;
            }

        }

        private void CancelItemPackClick(object sender, RoutedEventArgs e)
        {
        }

        private async void RemoveItemPackClick(object sender, RoutedEventArgs e)
        {
            String lang = ((Button)sender).DataContext as String;

            var packageCatalog = PackageCatalog.OpenForCurrentPackage();

            Package package = Package.Current;
            List<Windows.ApplicationModel.Package> resourcePackagesToRemove = new List<Windows.ApplicationModel.Package>();
            foreach (Package p in package.Dependencies)
            {
                if (p.IsResourcePackage)
                {
                    //the resource package name only contains the language code for example ar-sa is 29270depappf.UWPCoffeeApplication_1.2.0.0_neutral_split.language-ar_gah1vdar1nn7a
                    if (p.Id.ResourceId.ToLower().Contains(lang.ToLower().Substring(0,2)))
                    {
                        resourcePackagesToRemove.Add(p);
                        break;

                    }
                }
            }

            if (resourcePackagesToRemove.Count < 1)
            {
                await new MessageDialog("Did not find a resource package that matches " + lang).ShowAsync();
            }
            else
            {
                await new MessageDialog("Going to remove " + resourcePackagesToRemove[0].Id.FullName + " resource package. App will need to restart").ShowAsync();

                var removePackageResult = await packageCatalog.RemoveResourcePackagesAsync(resourcePackagesToRemove);
                if (removePackageResult != null)
                {
                    await new MessageDialog("RemoveResourcePackagesAsync failed with error " + removePackageResult.ExtendedError).ShowAsync();
                }
            }

        }

        private void ViewLevelClick(object sender, RoutedEventArgs e)
        {
        }

        private void ProgressValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
        }

        public List<Control> AllChildren(DependencyObject parent)
        {
            var _List = new List<Control>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var _Child = VisualTreeHelper.GetChild(parent, i);
                if (_Child is Control)
                    _List.Add(_Child as Control);
                _List.AddRange(AllChildren(_Child));
            }
            return _List;
        }
    }
}

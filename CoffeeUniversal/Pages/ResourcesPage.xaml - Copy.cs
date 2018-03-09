using CoffeeUniversal.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class ResourcesPage : Page
    {

		#region Standard init

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

                redrawListbox();

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

        private async void DefaultLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string lang = ((sender as ListBox).SelectedItem.ToString());

            var sel = (sender as ListBox).SelectedItem;
            

            ResourceContext resContext = ResourceContext.GetForCurrentView();
            //var qualifierValues = resContext.QualifierValues;
            //qualifierValues["language"] = lang;            

            //// load localized text from file
            //Uri fileUri = new Uri("ms-appx:///Assets/Languages/textfile1.txt");
            //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(fileUri);
            //string fileText = await FileIO.ReadTextAsync(file);

            //// set localized text
            //localizedText.Text = fileText;

            var qualifierValues = resContext.QualifierValues;
            var checkString = ";" + lang;
            string originalLanguageList = qualifierValues["language"];
            if(originalLanguageList.Contains(checkString))
            {
                string requestedLanguage = originalLanguageList.Replace(checkString , "");
                requestedLanguage = lang + ";" + requestedLanguage;
                qualifierValues["language"] = requestedLanguage;
                
            }
            else
            {
                //the selected language does not exist or is already at the front
            }   
            
            string resourceId = @"files/Assets/Languages/textfile1.txt";

            var namedResource = ResourceManager.Current.MainResourceMap[resourceId];
            var resourceCandidates = namedResource.Resolve(resContext);

            StorageFile file = await resourceCandidates.GetValueAsFileAsync();
            string fileText = await FileIO.ReadTextAsync(file);

            // set localized text
            localizedText.Text = fileText;

        }

        private void redrawListbox()
        {
            ResourceContext resContext = ResourceContext.GetForCurrentView();            
            foreach (string lang in resContext.Languages)
            {
                DefaultLanguage.Items.Add(lang);
            }
        }
    }
}

using CoffeeUniversal.ViewModels;
using CoffeeUniversal.Helpers;
using System;
using System.Linq;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class SpeechPage : Page
    {
        
        #region Standard init

        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public SpeechPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            speechRecognizer = new SpeechRecognizer();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        private SpeechSynthesizer speechSynthesizer;
        private VoiceInformation voiceInformation;
        private bool isInitialized;
        private SpeechRecognizer speechRecognizer;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            try
            {
                installedVoicesComboBox.ItemsSource = from voiceInformation in SpeechSynthesizer.AllVoices select voiceInformation;
                installedVoicesComboBox.SelectedIndex = 0;
                isInitialized = true;

                speechSynthesizer = new SpeechSynthesizer();
                if (e.Parameter != null)
                {
                    string message = e.Parameter as string;
                    resultTextBox.Text = message;
                    SpeechSynthesisStream stream = await speechSynthesizer.SynthesizeTextToStreamAsync(resultTextBox.Text);
                    feedbackMediaElement.SetSource(stream, stream.ContentType);
                    feedbackMediaElement.Play();
                }
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
            }
        }

        private async void startRecognition_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startRecognition.IsEnabled = false;

                // Compile the dictation grammar that is loaded by default, and start recognition.
                await speechRecognizer.CompileConstraintsAsync();
                SpeechRecognitionResult speechRecognitionResult = await speechRecognizer.RecognizeAsync();
                if (speechRecognitionResult.Status == SpeechRecognitionResultStatus.Success)
                {
                    resultTextBox.Text = speechRecognitionResult.Text;
                }
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog(LocalizableStrings.SPEECH_RECOGNITION_DISABLED);
                await dialog.ShowAsync();
                status.Log(ex.Message);
            }
            finally
            {
                startRecognition.IsEnabled = true;
            }
        }

        private void installedVoicesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                if (e.AddedItems.Count > 0)
                {
                    voiceInformation = e.AddedItems[0] as VoiceInformation;
                    speechSynthesizer.Voice = voiceInformation;
                }
            }
        }

        private async void speakTextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SpeechSynthesisStream stream = await speechSynthesizer.SynthesizeTextToStreamAsync(resultTextBox.Text);
                feedbackMediaElement.SetSource(stream, stream.ContentType);
                feedbackMediaElement.Play();
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
            }
        }
    }
}
using CoffeeUniversal.Helpers;
using CoffeeUniversal.Pages;
using CoffeeUniversal.ViewModels;
using CoffeeUtilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.System;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal
{
	sealed partial class App : Application
    {

        #region Fields & Properties

        private static MainViewModel viewModel = null;
		public static MainViewModel ViewModel
		{
			get
			{
				if (viewModel == null)
				{
					viewModel = new MainViewModel();
				}
				return viewModel;
			}
		}

		private static TilesViewModel tilesViewModel = null;
		public static TilesViewModel TilesViewModel
		{
			get
			{
				if (tilesViewModel == null)
				{
					tilesViewModel = new TilesViewModel();
				}

				return tilesViewModel;
			}
		}

		private static bool isHardwareBackButtonAvailable = false;
		public static bool IsHardwareBackButtonAvailable
		{
			get
			{
				return isHardwareBackButtonAvailable;
			}
		}

        public static bool IsActivationDone { get; set; }

        public ObservableCollection<ViewLifetimeController> SecondaryViews = new ObservableCollection<ViewLifetimeController>();

        private CoreDispatcher mainDispatcher;
        public CoreDispatcher MainDispatcher
        {
            get
            {
                return mainDispatcher;
            }
        }

        private int mainViewId;
        public int MainViewId
        {
            get
            {
                return mainViewId;
            }
        }

        private static string versionString = string.Empty;
        public static string VersionString
        {
            get
            {
                if (string.IsNullOrEmpty(versionString))
                {
                    PackageVersion packageVersion = Package.Current.Id.Version;
                    versionString = string.Format("{0}.{1}.{2}.{3}",
                        packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
                }
                return versionString;
            }
        }

        #endregion


        #region Init

        public App()
        {
			UnhandledException += App_UnhandledException;
            InitializeComponent();
            Suspending += OnSuspending;
			Resuming += OnResuming;

			if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
			{
				isHardwareBackButtonAvailable = true;
			}
		}

        private Frame CreateRootFrame()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                mainDispatcher = Window.Current.Dispatcher;
                mainViewId = ApplicationView.GetForCurrentView().Id;

                rootFrame = new Frame();
                rootFrame.Language = ApplicationLanguages.Languages[0];
                rootFrame.NavigationFailed += OnNavigationFailed;

                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                //If we had any app state to load, this is where we'd do it (from previously suspended app).
                //if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                //{
                //}
                Window.Current.Content = rootFrame;
            }

            return rootFrame;
        }

        private async void RestoreStatus(ApplicationExecutionState previousExecutionState)
        {
            if (previousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore the saved session state only when appropriate
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException)
                {
                    //Something went wrong restoring state.
                    //Assume there is no state and continue
                }
            }
        }


        #endregion


        #region Lifecycle events

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
			Debug.WriteLine("OnLaunched");
			ViewModel.LoadData();

			Frame rootFrame = CreateRootFrame();
			RestoreStatus(args.PreviousExecutionState);

			// Note: when launched from a secondary tile or toast, the app creates the MainPage as normal, and then immediately
			// navigates to the selected SecondaryTilePage. Therefore, if the user goes back from this SecondaryTilePage,
			// this will take him back to the MainPage (and not to the intervening TilesPage). This is by design.
			if (!string.IsNullOrEmpty(args.TileId) && args.TileId != "PrimaryBaseTile" && args.TileId != "CoffeeUniversal")
			{
			    Debug.WriteLine("OnLaunched:SecondaryTile");
				if (rootFrame.Content == null)
				{
					if (!rootFrame.Navigate(typeof(MainPage)))
					{
						throw new Exception(LocalizableStrings.APP_NO_INITIAL_PAGE);
					}
				}

                // If we recognize the specified tile name, we navigate to that page;
                // if not, this must be a HoloLens primary activation, in which case we just stay on the MainPage.
                if (!string.IsNullOrEmpty(args.Arguments))
                {
                    try
                    {
                        TileViewModel item = TilesViewModel.Items.First(x => x.Title == args.Arguments);
                        if (item != null)
                        {
                            rootFrame.Navigate(typeof(SecondaryTilePage), args.Arguments);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("OnLaunched:SecondaryTile: " + ex.ToString());
                    }
                }
			}
			else if (!string.IsNullOrEmpty(args.Arguments) 
                && args.Arguments.StartsWith(SharedStrings.TOAST_PARAMETER_PREFIX, StringComparison.CurrentCultureIgnoreCase))
			{
			    Debug.WriteLine("OnLaunched:Toast");
				if (rootFrame.Content == null)
				{
					if (!rootFrame.Navigate(typeof(MainPage)))
					{
						throw new Exception(LocalizableStrings.APP_NO_INITIAL_PAGE);
					}
				}

				rootFrame.Navigate(typeof(ToastsPage), args.Arguments);
			}
			else
			{
				rootFrame.Navigate(typeof(MainPage), args.Arguments);
			}

			Window.Current.Activate();

            // This is run on another thread because it takes ~2000 msec to complete.
            Task.Run(InstallVoiceCommandSets);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
			Debug.WriteLine("OnActivated");
			ViewModel.LoadData();

			if (args.Kind == ActivationKind.Protocol)
            {
			    Debug.WriteLine("OnActivated:Protocol");
				Frame rootFrame = CreateRootFrame();
				RestoreStatus(args.PreviousExecutionState);

				if (rootFrame.Content == null)
				{
					if (!rootFrame.Navigate(typeof(MainPage)))
					{
						throw new Exception(LocalizableStrings.APP_NO_INITIAL_PAGE);
					}
				}

				MainPage mainPage = rootFrame.Content as MainPage;
                ProtocolActivatedEventArgs protocolArgs = args as ProtocolActivatedEventArgs;
				if (protocolArgs.Uri != null && !string.IsNullOrEmpty(protocolArgs.Uri.PathAndQuery))
				{
					string itemTitle = protocolArgs.Uri.PathAndQuery;
					Debug.WriteLine("protocol activation: itemTitle = " + itemTitle);

					try
					{
						TileViewModel pinnedItem = TilesViewModel.Items.First(x => x.Title == itemTitle);
						if (pinnedItem != null)
						{
							rootFrame.Navigate(typeof(SecondaryTilePage), protocolArgs.Uri.PathAndQuery);
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine("OnActivated: " +ex.Message);
                    }
				}
			}
            else if (args.Kind == ActivationKind.ToastNotification)
            {
                ToastNotificationActivatedEventArgs toastArgs = args as ToastNotificationActivatedEventArgs;
                ValueSet values  = toastArgs.UserInput;
                foreach (var v in values)
                {
                    Debug.WriteLine("ToastNotificationActivatedEventArgs: {0}={1}", v.Key, v.Value);
                }
            }
            else if (args.Kind == ActivationKind.VoiceCommand)
            {
                OnVoiceActivation(args);
            }
            Window.Current.Activate();
        }

		#endregion


		#region Exceptions

		private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception(string.Format(CultureInfo.CurrentCulture, 
				LocalizableStrings.APP_FAILED_TO_LOAD_PAGE, e.SourcePageType.FullName));
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.Exception is UnhandleableException)
            {
                throw e.Exception;
            }
            else
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                else
                {
                    if (e.Exception != null)
                    {
                        string exceptionText = e.Exception.ToString();
                        if (!string.IsNullOrEmpty(exceptionText))
                        {
                            ShowMessage(string.Format(CultureInfo.CurrentCulture,
                                LocalizableStrings.APP_UNHANDLED_EXCEPTION + ": {0}.", exceptionText));
                        }
                    }
                    else
                    {
                        ShowMessage(LocalizableStrings.APP_UNHANDLED_EXCEPTION);
                    }
                    e.Handled = true;
                }
            }
        }

		internal async static void ShowMessage(string message, bool forceShow = false)
		{
			if (Debugger.IsAttached && !forceShow)
			{
				Debug.WriteLine(message);
			}
			else
			{
				if (IsActivationDone)
				{
					if (CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess)
					{
						MessageDialog dialog = new MessageDialog(message);
						await dialog.ShowAsync();
					}
					else
					{
						await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
							CoreDispatcherPriority.Normal, async () =>
							{
								MessageDialog dialog = new MessageDialog(message);
								await dialog.ShowAsync();
							});
					}
				}
			}
		}

        #endregion


        #region Display Requests

        private static DisplayRequest appDisplayRequest;
        private static bool isDisplayRequestActive = false;
        private static DateTime displayRequestActivationTime;

        public static DisplayRequestInfo ActivateDisplayRequest()
        {
            DisplayRequestInfo requestInfo = new DisplayRequestInfo();
            if (!isDisplayRequestActive)
            {
                if (appDisplayRequest == null)
                {
                    appDisplayRequest = new DisplayRequest();
                }
                appDisplayRequest.RequestActive();
                displayRequestActivationTime = DateTime.Now;
                isDisplayRequestActive = true;
                requestInfo.ActivationResult = ActivationResult.Activated;
            }
            else
            {
                requestInfo.ActivationResult = ActivationResult.PreviouslyActivated;
            }
            requestInfo.ActivationTime = displayRequestActivationTime;
            return requestInfo;
        }

        public static DisplayRequestInfo ReleaseDisplayRequest()
        {
            DisplayRequestInfo requestInfo = new DisplayRequestInfo();
            if (isDisplayRequestActive)
            {
                appDisplayRequest.RequestRelease();
                isDisplayRequestActive = false;
                requestInfo.ActivationResult = ActivationResult.Released;
            }
            else
            {
                requestInfo.ActivationResult = ActivationResult.NotActivated;
            }
            return requestInfo;
        }

        #endregion


        #region Voice Commands

        private async Task<bool> InstallVoiceCommandSets()
        {
            bool result = false;
            try
            {
                StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///CoffeeCommands.xml"));
                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(storageFile);
                VoiceCommandDefinition commandSetEnUs;
                if (VoiceCommandDefinitionManager.InstalledCommandDefinitions == null 
                    || VoiceCommandDefinitionManager.InstalledCommandDefinitions.Count == 0)
                {
                    Debug.WriteLine("No InstalledCommandDefinitions");
                }
                if (VoiceCommandDefinitionManager.InstalledCommandDefinitions.TryGetValue("CoffeeCommandSet_en-us", out commandSetEnUs))
                {
                    // Build a phrase list for pages from the pages in the main viewmodel.
                    List<MenuItem> menuItems = ViewModel.MenuItems;
                    string[] pageNames = new string[menuItems.Count];
                    for (int i = 0; i < menuItems.Count; i++)
                    {
                        pageNames[i] = menuItems[i].Content;
                    }

                    await commandSetEnUs.SetPhraseListAsync("page", pageNames);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("InstallVoiceCommandSets: " + ex.ToString());
            }
            return result;
        }

        private string GetSemanticInterpretation(string key, SpeechRecognitionResult result)
        {
            if (result.SemanticInterpretation.Properties.ContainsKey(key))
            {
                return result.SemanticInterpretation.Properties[key][0];
            }
            else
            {
                return "unknown";
            }
        }

        private void OnVoiceActivation(IActivatedEventArgs args)
        {
            VoiceCommandActivatedEventArgs commandArgs = args as VoiceCommandActivatedEventArgs;
            SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;

            // The commandMode is either "voice" or "text", and it indicates how the voice command was entered by the user.
            // We should respect "text" mode by providing feedback in a silent form.
            string commandMode = GetSemanticInterpretation("commandMode", speechRecognitionResult);

            // If so, get the name of the voice command, the actual text spoken, and the value of Command/Navigate@Target.
            string voiceCommandName = speechRecognitionResult.RulePath[0];
            Type navigateToPageType = typeof(MainPage);
            string navigationParameter = null;

            switch (voiceCommandName)
            {
                case "goToAPage":
                    string pageName = GetSemanticInterpretation("page", speechRecognitionResult);
                    foreach (MenuItem item in ViewModel.MenuItems)
                    {
                        if (pageName == item.Content)
                        {
                            navigateToPageType = item.PageType;
                            break;
                        }
                    }
                    break;
                case "saySomething":
                    navigateToPageType = typeof(SpeechPage);
                    navigationParameter = GetSemanticInterpretation("message", speechRecognitionResult);
                    break;
                case "playVideo":
                    navigateToPageType = typeof(MediaPage);
                    navigationParameter = GetSemanticInterpretation("video", speechRecognitionResult);
                    break;
                default:
                    break;
            }
            Frame rootFrame = CreateRootFrame();
            RestoreStatus(args.PreviousExecutionState);
            rootFrame.Navigate(navigateToPageType, navigationParameter);
        }

        #endregion


        #region Extended Execution

        private const int MB = 1024 * 1024;
        private static Timer extensionWorkTimer;
        private static ExtendedExecutionSession extendedExecutionSession;
        public static bool IsExtensionEnabled { get; set; }

        public static bool IsExtensionWorkRunning { get; set; }

        public static void StartWork()
        {
            extensionWorkTimer = new Timer(OnTimer, null, 0, 10000);
            IsExtensionWorkRunning = true;
        }

        public static void StopWork()
        {
            if (extensionWorkTimer != null)
            {
                extensionWorkTimer.Dispose();
            }
            IsExtensionWorkRunning = false;
        }

        private static void OnTimer(object state)
        {
            SendToast(GetMemoryUsage());
        }

        private static void SendToast(string message)
        {
            try
            {
                ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

                XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                toastTextElements[0].AppendChild(toastXml.CreateTextNode(message));
                toastTextElements[1].AppendChild(toastXml.CreateTextNode(DateTime.Now.ToString()));

                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private static string GetMemoryUsage()
        {
            string memoryUsage = string.Empty;
            try
            {
                ulong usageLimit = MemoryManager.AppMemoryUsageLimit;
                ulong usage = MemoryManager.AppMemoryUsage;
                memoryUsage = string.Format(CultureInfo.CurrentCulture, "EE: limit={0} MB, usage={1} MB",
                    Math.Ceiling((double)usageLimit / MB), Math.Ceiling((double)usage / MB));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return memoryUsage;
        }

        public static async void RequestExtension()
        {
            if (extendedExecutionSession == null)
            {
                extendedExecutionSession = new ExtendedExecutionSession();
                extendedExecutionSession.Description = LocalizableStrings.EXTENDED_EXECUTION_DESCRIPTION;
                extendedExecutionSession.Reason = ExtendedExecutionReason.Unspecified;
                extendedExecutionSession.Revoked += ExtendedExecutionSession_Revoked;
            }
            ExtendedExecutionResult result = await extendedExecutionSession.RequestExtensionAsync();
            IsExtensionEnabled = (result == ExtendedExecutionResult.Allowed);

            if (IsExtensionEnabled)
            {
                MemoryManager.AppMemoryUsageLimitChanging += MemoryManager_AppMemoryUsageLimitChanging;
            }
        }

        private static void ExtendedExecutionSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            SendToast(LocalizableStrings.EXTENDED_EXECUTION_REVOKED);
            CancelExtension();
        }

        public static void CancelExtension()
        {
            if (extendedExecutionSession != null)
            {
                extendedExecutionSession.Dispose();
            }
            IsExtensionEnabled = false;
            MemoryManager.AppMemoryUsageLimitChanging -= MemoryManager_AppMemoryUsageLimitChanging;
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            Debug.WriteLine("OnSuspending");
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();

            if (extensionWorkTimer != null)
            {
                extensionWorkTimer.Dispose();
                extensionWorkTimer = null;
            }

            deferral.Complete();
        }

        private void OnResuming(object sender, object e)
        {
            Debug.WriteLine("OnResuming");
        }

        private static void MemoryManager_AppMemoryUsageLimitChanging(object sender, AppMemoryUsageLimitChangingEventArgs e)
        {
            SendToast(string.Format(CultureInfo.CurrentCulture, "old={0}MB, new={1}MB, cur={2}MB",
                Math.Ceiling((double)e.OldLimit / MB), Math.Ceiling((double)e.NewLimit / MB), Math.Ceiling((double)MemoryManager.AppMemoryUsageLimit / MB)));
        }

        #endregion
    }

    public enum ActivationResult {  Activated, PreviouslyActivated, Released, NotActivated }
    public class DisplayRequestInfo
    {
        public DateTime ActivationTime { get; set; }
        public ActivationResult ActivationResult { get; set; }
    }
}

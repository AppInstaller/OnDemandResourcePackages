using CoffeeUniversal.Helpers;
using CoffeeUtilities;
using System;
using System.Diagnostics;
using System.Globalization;
using Windows.Devices.Power;
using Windows.Foundation;
using Windows.System.Display;
using Windows.System.Power;
using Windows.System.Power.Diagnostics;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CoffeeUniversal.ViewModels;

namespace CoffeeUniversal.Pages
{
    public sealed partial class EnergyPage : Page
    {

        #region Standard stuff

        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public EnergyPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);

            BatteryReport batteryReport = Battery.AggregateBattery.GetReport();
            isBatteryPresent = !(batteryReport.Status == BatteryStatus.NotPresent);
        }

        #endregion


        #region Init

        private bool isStopped = true;
        private const int PI_DIGITS = 10000;
        private bool isEnergyManagerConnected = false;
        private DisplayRequest displayRequest;
        private DispatcherTimer timer;
        private bool isBatteryPresent = false;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
            if (!isBatteryPresent)
            {
                status.Log(LocalizableStrings.ENERGY_NO_BATTERY_PRESENT);
            }
            else
            {
                status.Log(LocalizableStrings.ENERGY_BATTERY_PRESENT);
            }

            // Keep the screen active indefinitely, so we can exercise energy consumption uninterrupted.
            if (displayRequest == null)
            {
                displayRequest = new DisplayRequest();
            }
            displayRequest.RequestActive();

            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(5000);
                timer.Tick += Timer_Tick;
            }

            try
            {
                ForegroundEnergyManager.RecentEnergyUsageIncreased += ForegroundEnergyManager_RecentEnergyUsageIncreased;
                ForegroundEnergyManager.RecentEnergyUsageReturnedToLow += ForegroundEnergyManager_RecentEnergyUsageReturnedToLow;
                isEnergyManagerConnected = true;
                resetFg.IsEnabled = true;
                status.Log(LocalizableStrings.FOREGROUND_ENERGY_MANAGER_CONNECTED);
                ShowConversionFactor();
                ShowEnergyUsage();
            }
            catch (Exception)
            {
                resetFg.IsEnabled = false;
                status.Log(LocalizableStrings.FOREGROUND_ENERGY_MANAGER_FAIL);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (timer != null)
            {
                timer.Tick -= Timer_Tick;
                timer.Stop();
                timer = null;
            }

            ForegroundEnergyManager.RecentEnergyUsageIncreased -= ForegroundEnergyManager_RecentEnergyUsageIncreased;
            ForegroundEnergyManager.RecentEnergyUsageReturnedToLow -= ForegroundEnergyManager_RecentEnergyUsageReturnedToLow;
            isEnergyManagerConnected = false;

            if (displayRequest != null)
            {
                displayRequest.RequestRelease();
            }
            navigationHelper.OnNavigatedFrom(e);
        }

        private void Timer_Tick(object sender, object e)
        {
            ShowEnergyUsage();
        }

        #endregion


        #region Foreground Work

        async private void startFgWork_Click(object sender, RoutedEventArgs e)
        {
            isStopped = false;
            startFgWork.IsEnabled = false;
            stopFgWork.IsEnabled = true;

            status.Log(string.Format(CultureInfo.CurrentCulture, 
                LocalizableStrings.ENERGY_WORK_STARTED, DateTime.Now.ToString("HH:mm:ss")));
            MediaPlayback(true);
            timer.Start();

            while (!isStopped)
            {
                await ThreadPool.RunAsync(CalculatePi, WorkItemPriority.High, WorkItemOptions.TimeSliced);
            }
        }

        private void stopFgWork_Click(object sender, RoutedEventArgs e)
        {
            StopWork();
        }

        async private void StopWork()
        {
            isStopped = true;
            if (timer != null)
            {
                timer.Stop();
            }
            MediaPlayback(false);

            status.Log(string.Format(CultureInfo.CurrentCulture,
                LocalizableStrings.ENERGY_WORK_STOPPED, DateTime.Now.ToString("HH:mm:ss")));

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                startFgWork.IsEnabled = true;
                stopFgWork.IsEnabled = false;
                ShowEnergyUsage();
            });
        }

        private void MediaPlayback(bool play)
        {
            if (play)
            {
                localVideo.Play();
            }
            else
            {
                localVideo.Pause();
            }
        }

        private void CalculatePi(IAsyncAction operation)
        {
            if (isStopped)
            {
                return;
            }

            string result = PiCalculator.Calculate(PI_DIGITS);
            Debug.WriteLine(result);
        }

        private void ForegroundEnergyManager_RecentEnergyUsageReturnedToLow(object sender, object e)
        {
            ShowEnergyUsage();
        }

        private void ForegroundEnergyManager_RecentEnergyUsageIncreased(object sender, object e)
        {
            ShowEnergyUsage();
        }

        private void resetFg_Click(object sender, RoutedEventArgs e)
        {
            if (isEnergyManagerConnected)
            {
                ForegroundEnergyDiagnostics.ResetTotalEnergyUsage();
                ShowEnergyUsage();
            }
        }

        #endregion


        #region ShowEnergyUsage

        async private void ShowEnergyUsage()
        {
            if (isEnergyManagerConnected)
            {
                try
                {
                    uint usageFG = ForegroundEnergyManager.RecentEnergyUsage;
                    uint levelFG = ForegroundEnergyManager.RecentEnergyUsageLevel;
                    ulong totalFG = ForegroundEnergyDiagnostics.ComputeTotalEnergyUsage();
                    Debug.WriteLine("usage={0}, level={1}, total={2}", usageFG, levelFG, totalFG);

                    string level = "Undefined";
                    if (isBatteryPresent)
                    {
                        if (levelFG == ForegroundEnergyManager.ExcessiveUsageLevel)
                        {
                            level = "Excessive";
                        }
                        else if (levelFG == ForegroundEnergyManager.MaxAcceptableUsageLevel)
                        {
                            level = "MaxAcceptable";
                        }
                        else if (levelFG == ForegroundEnergyManager.NearMaxAcceptableUsageLevel)
                        {
                            level = "NearMaxAcceptable";
                        }
                        else if (levelFG == ForegroundEnergyManager.LowUsageLevel)
                        {
                            level = "Low";
                        }
                    }

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        fgUsage.Text = string.Format(CultureInfo.CurrentCulture, "{0}%", usageFG);
                        fgLevel.Text = level;
                        fgTotal.Text = string.Format(CultureInfo.CurrentCulture, "{0}%", totalFG);
                    });
                }
                catch (Exception ex)
                {
                    status.Log("ShowEnergyUsage: " + ex.Message);
                }
            }
        }

        async private void ShowConversionFactor()
        {
            if (isEnergyManagerConnected)
            {
                try
                {
                    double conversionFactor = ForegroundEnergyDiagnostics.DeviceSpecificConversionFactor;
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        conversion.Text = string.Format(CultureInfo.CurrentCulture, "{0:N}", conversionFactor);
                    });
                }
                catch (Exception ex)
                {
                    status.Log("ShowConversionFactor: " + ex.Message);
                }
            }
        }


        #endregion

    }
}

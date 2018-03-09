using CoffeeUniversal.ViewModels;
using CoffeeUniversal.Helpers;
using System;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
	public sealed partial class SensorsPage : Page
    {

		#region Init

        private Accelerometer accelerometer;
		private Gyrometer gyrometer;
        private Compass compass;
        private uint reportInterval;
        private const uint MAX_REPORT_INTERVAL = 100;
        private const uint TWO_DEGREES = 33;
        private const double COMPASS_ADJUSTMENT = 90.0;
        private NavigationHelper navigationHelper;

        public SensorsPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            InitializeAccelerometer();
			InitializeGyrometer();
            InitializeCompass();
		}

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

		private void InitializeAccelerometer()
        {
            if (accelerometer == null)
            {
                accelerometer = Accelerometer.GetDefault();
                if (accelerometer == null)
                {
                    status.Log(LocalizableStrings.SENSORS_NO_ACCELEROMETER);
                    return;
                }

				uint minInterval = accelerometer.MinimumReportInterval;
				accelerometer.ReportInterval = Math.Max(minInterval, MAX_REPORT_INTERVAL);
				startAccelerometer.IsEnabled = true;
                status.Log(LocalizableStrings.SENSORS_ACCELEROMETER_INITIALIZED);
            }
        }

		private void InitializeGyrometer()
		{
			if (gyrometer == null)
			{
				gyrometer = Gyrometer.GetDefault();
				if (gyrometer == null)
				{
					status.Log(LocalizableStrings.SENSORS_NO_GYROMETER);
					return;
				}

				uint minInterval = gyrometer.MinimumReportInterval;
				gyrometer.ReportInterval = Math.Max(minInterval, MAX_REPORT_INTERVAL);
				startGyrometer.IsEnabled = true;
                status.Log(LocalizableStrings.SENSORS_GYROMETER_INITIALIZED);
            }
        }

        private void InitializeCompass()
        {
            if (compass == null)
            {
                compass = Compass.GetDefault();
                if (compass == null)
                {
                    status.Log(LocalizableStrings.SENSORS_NO_COMPASS);
                    return;
                }

                uint minReportInterval = compass.MinimumReportInterval;
                reportInterval = minReportInterval > TWO_DEGREES ? minReportInterval : TWO_DEGREES;
                compass.ReportInterval = reportInterval;
                startCompass.IsEnabled = true;
                status.Log(LocalizableStrings.SENSORS_COMPASS_INITIALIZED);
            }
        }

        #endregion


        #region UI events

        private void startAccelerometer_Click(object sender, RoutedEventArgs e)
		{
			if (accelerometer != null)
			{
				accelerometer.ReadingChanged += accelerometer_ReadingChanged;
				startAccelerometer.IsEnabled = false;
				stopAccelerometer.IsEnabled = true;
				status.Log(stopAccelerometer, LocalizableStrings.SENSORS_ACCELEROMETER_STARTED);
			}
		}

		private void stopAccelerometer_Click(object sender, RoutedEventArgs e)
		{
			if (accelerometer != null)
			{
				accelerometer.ReadingChanged -= accelerometer_ReadingChanged;
				startAccelerometer.IsEnabled = true;
				stopAccelerometer.IsEnabled = false;
				status.Log(startAccelerometer, LocalizableStrings.SENSORS_ACCELEROMETER_STOPPED);
			}
		}

		private void startGyrometer_Click(object sender, RoutedEventArgs e)
		{
			if (gyrometer != null)
			{
				gyrometer.ReadingChanged += gyrometer_ReadingChanged;
				startGyrometer.IsEnabled = false;
				stopGyrometer.IsEnabled = true;
				status.Log(stopGyrometer, LocalizableStrings.SENSORS_GYROMETER_STARTED);
			}
		}

		private void stopGyrometer_Click(object sender, RoutedEventArgs e)
		{
			if (gyrometer != null)
			{
				gyrometer.ReadingChanged -= gyrometer_ReadingChanged;
				startGyrometer.IsEnabled = true;
				stopGyrometer.IsEnabled = false;
				status.Log(startGyrometer, LocalizableStrings.SENSORS_GYROMETER_STOPPED);
			}
		}

        private void startCompass_Click(object sender, RoutedEventArgs e)
        {
            if (compass != null)
            {
                compass.ReadingChanged += Compass_ReadingChanged;
                startCompass.IsEnabled = false;
                stopCompass.IsEnabled = true;
                status.Log(LocalizableStrings.SENSORS_COMPASS_STARTED);
            }
        }

        private void stopCompass_Click(object sender, RoutedEventArgs e)
        {
            if (compass != null)
            {
                compass.ReadingChanged -= Compass_ReadingChanged;
                startCompass.IsEnabled = true;
                stopCompass.IsEnabled = false;
                status.Log(LocalizableStrings.SENSORS_COMPASS_STOPPED);
            }
        }

        #endregion


        #region ReadingChanged events

        private async void accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
			AccelerometerReading reading = args.Reading;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
				accelerometerX.Text = string.Format("{0,5:0.00}", reading.AccelerationX);
				accelerometerY.Text = string.Format("{0,5:0.00}", reading.AccelerationY);
				accelerometerZ.Text = string.Format("{0,5:0.00}", reading.AccelerationZ);
			});
        }

		private async void gyrometer_ReadingChanged(Gyrometer sender, GyrometerReadingChangedEventArgs args)
		{
			GyrometerReading reading = args.Reading;
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				gyrometerX.Text = string.Format("{0,5:0.00}", reading.AngularVelocityX);
				gyrometerY.Text = string.Format("{0,5:0.00}", reading.AngularVelocityX);
				gyrometerZ.Text = string.Format("{0,5:0.00}", reading.AngularVelocityZ);
			});
		}

        private async void Compass_ReadingChanged(Compass sender, CompassReadingChangedEventArgs e)
        {
            CompassReading reading = e.Reading;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (reading.HeadingTrueNorth.HasValue)
                {
                    trueNorth.Text = string.Format("{0,5:0.00}", (double)reading.HeadingTrueNorth - COMPASS_ADJUSTMENT);
                }
                magNorth.Text = string.Format("{0,5:0.00}", reading.HeadingMagneticNorth - COMPASS_ADJUSTMENT);
            });
        }

        #endregion

    }
}

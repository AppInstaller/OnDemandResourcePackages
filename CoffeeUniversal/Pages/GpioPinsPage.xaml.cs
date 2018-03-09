using CoffeeUniversal.Helpers;
using CoffeeUniversal.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class GpioPinsPage : Page
    {
        private GpioController gpio;
        private ObservableCollection<GpioPinEntry> gpioPins;

        #region init

        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public GpioPinsPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            InitGPIO();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);

            foreach (GpioPinEntry pinEntry in gpioPins)
                if (pinEntry.pin != null)
                    pinEntry.pin.Dispose();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        private void InitGPIO()
        {
            gpioPins = new ObservableCollection<GpioPinEntry>();
            gridView.ItemsSource = gpioPins;

            try
            {
                gpio = GpioController.GetDefault();
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
            }

            if (gpio == null)
            {
                status.Log(LocalizableStrings.IOT_GPIO_NONE);
                gpioPins.Add(new GpioPinEntry() { num = 1 });
                gridView.IsEnabled = false;
            }
            else
            {
                AddPins();
            }
        }

        private void AddPins()
        {
            List<GpioPin> list = new List<GpioPin>();
            for (int i = 0; i < gpio.PinCount; i++)
            {
                GpioPin pin;
                GpioOpenStatus status;

                gpio.TryOpenPin(i, GpioSharingMode.Exclusive, out pin, out status);
                if (status == GpioOpenStatus.PinOpened)
                {
                    pin.Dispose();
                    gpioPins.Add(new GpioPinEntry()
                    {
                        num = i,
                    });
                }
            }
        }

        #endregion

        private void open_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var pinEntry = button.DataContext as GpioPinEntry;

            if (pinEntry.pin != null)
            {
                pinEntry.pin.Dispose();
                pinEntry.pin = null;
                pinEntry.value = GpioPinValue.High;
                button.Content = "Open";
            }
            else
            {
                GpioPin pin;
                GpioOpenStatus status;
                gpio.TryOpenPin(pinEntry.num, GpioSharingMode.Exclusive, out pin, out status);
                if (status == GpioOpenStatus.PinOpened)
                {
                    pinEntry.pin = pin;
                    button.Content = "Close";
                    var value = pinEntry.pin.Read();
                    pinEntry.value = value;
                }

            }
        }

        private void out_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var pinEntry = button.DataContext as GpioPinEntry;

            if (pinEntry.pin != null)
            {
                pinEntry.pin.SetDriveMode(GpioPinDriveMode.Output);

                if (pinEntry.value == GpioPinValue.High)
                    pinEntry.value = GpioPinValue.Low;
                else
                    pinEntry.value = GpioPinValue.High;

                pinEntry.pin.Write(pinEntry.value);
            }
            else
                status.Log(string.Format(LocalizableStrings.IOT_GPIO_PIN_NOTOPEN, pinEntry.num));
        }
        private void in_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var pinEntry = button.DataContext as GpioPinEntry;

            if (pinEntry.pin != null)
            {
                pinEntry.pin.SetDriveMode(GpioPinDriveMode.Input);
                GpioPinValue value = pinEntry.pin.Read();
                pinEntry.value = value;
            }
            else
                status.Log(string.Format(LocalizableStrings.IOT_GPIO_PIN_NOTOPEN, pinEntry.num));
        }


        class GpioPinEntry : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }

            public GpioPinEntry() { }

            public GpioPin pin { get; set; }
            public int num { get; set; }
            private GpioPinValue _value;
            public GpioPinValue value
            {
                get { return _value; }
                set { _value = value; OnPropertyChanged("status"); }
            }
            public string status
            {
                get
                {
                    if (pin != null)
                        return value.ToString();
                    else
                        return "";
                }
                set
                {
                    OnPropertyChanged("status");
                }
            }
        }
    }
}
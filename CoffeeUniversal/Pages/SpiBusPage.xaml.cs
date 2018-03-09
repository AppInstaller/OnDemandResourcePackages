using CoffeeUniversal.Helpers;
using CoffeeUniversal.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class SpiBusPage : Page
    {
        private ObservableCollection<SpiController> spiControllers;
        public enum SpiSendMode { Write, Transfer };

        private const string DEFAULT_FREQUENCY = "500000";
        private const string DEFAULT_CS = "0";
        private const string DEFAULT_SEND = "1, 2, 3";
        private const SpiMode DEFAULT_MODE = SpiMode.Mode3;
        private const string DEFAULT_NAME = "None";

        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public SpiBusPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);

            InitSpi();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
            spiControllers.Clear();
            foreach (SpiController spi in spiControllers)
            {
                if (spi != null)
                    if (spi.device != null)
                        spi.device.Dispose();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        private async void InitSpi()
        {
            cvsModeItems.Source = Enum.GetValues(typeof(SpiMode));
            spiControllers = new ObservableCollection<SpiController>();
            gridView.ItemsSource = spiControllers;
            try
            {
                string spiAqs = SpiDevice.GetDeviceSelector();
                var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);
                foreach (DeviceInformation info in devicesInfo)
                {
                    spiControllers.Add(new SpiController()
                    {
                        info = info,
                        device = null,
                        chipSelect = DEFAULT_CS,
                        frequency = DEFAULT_FREQUENCY,
                        mode = DEFAULT_MODE,
                        write = DEFAULT_SEND,
                        transfer = DEFAULT_SEND,
                        name = info.Id.Split('\\').Last()
                    });
                }
            }
            catch (Exception ex)
            {
                status.Log(string.Format(LocalizableStrings.IOT_SPI_NONE, ex.Message));
                gridView.IsEnabled = false;
                spiControllers.Add(new SpiController() { name = DEFAULT_NAME });
                return;
            }

            status.Log(string.Format(LocalizableStrings.IOT_SPI_FOUND, spiControllers.Count()));
        }

        private void spiSend(SpiController controller, SpiSendMode sendMode)
        {
            if (controller == null)
                return;

            if (controller.device != null)
            {
                switch (sendMode)
                {
                    case SpiSendMode.Write:
                        var WriteBuf = HexConverter.StringToByteArray(controller.write);
                        if (WriteBuf.Count() > 1)
                        {
                            controller.device.Write(WriteBuf);
                            status.Log(string.Format(LocalizableStrings.IOT_SPI_WROTE, controller.name, HexConverter.ByteToString(WriteBuf)));
                        }
                        break;
                    case SpiSendMode.Transfer:
                        var SendBuf = HexConverter.StringToByteArray(controller.transfer);
                        if (SendBuf.Count() > 1)
                        {
                            var ReadBuf = new byte[SendBuf.Count()];
                            controller.device.TransferFullDuplex(SendBuf, ReadBuf);
                            status.Log(string.Format(LocalizableStrings.IOT_SPI_OUT, controller.name, HexConverter.ByteToString(SendBuf)));
                            status.Log(string.Format(LocalizableStrings.IOT_SPI_IN, controller.name, HexConverter.ByteToString(ReadBuf)));
                        }
                        break;
                }
            }
            else
            {
                status.Log(string.Format(LocalizableStrings.IOT_SPI_NOT_INITIALIZED, controller.name));
            }
        }

        private async Task spiActivate(SpiController controller)
        {
            if (controller == null)
                return;

            if (controller.device != null)
            {
                controller.device.Dispose();
                controller.device = null;
            }

            int cs, f;
            try
            {
                cs = Int32.Parse(controller.chipSelect);
                f = Int32.Parse(controller.frequency);
            }
            catch (Exception ex)
            {
                status.Log(string.Format(LocalizableStrings.IOT_SPI_SETTINGS_INVALID, controller.name, ex.Message));
                return;
            }

            var settings = new SpiConnectionSettings(cs);
            settings.ClockFrequency = f;
            settings.Mode = controller.mode;

            try
            {
                controller.device = await SpiDevice.FromIdAsync(controller.info.Id, settings);
            }
            catch (Exception ex)
            {
                status.Log(string.Format(LocalizableStrings.IOT_SPI_ACTIVATE_ERROR, controller.name, ex.Message));
                return;
            }
            status.Log(string.Format(LocalizableStrings.IOT_SPI_ACTIVATED, controller.name));
        }

        private async void activate_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            await spiActivate(button.DataContext as SpiController);
        }

        private void deactivate_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            SpiController controller = button.DataContext as SpiController;

            if (controller.device != null)
            {
                controller.device.Dispose();
                controller.device = null;
            }
        }
        private void write_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            spiSend(button.DataContext as SpiController, SpiSendMode.Write);
        }

        private void transfer_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            spiSend(button.DataContext as SpiController, SpiSendMode.Transfer);
        }

        private void numeric_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;
            string onlyNumbers = new string(box.Text.Where(c => char.IsDigit(c)).ToArray());
            box.Text = onlyNumbers;
        }

        private async void accelerometer_Click(object sender, RoutedEventArgs e)
        {
            if (spiControllers.Count() < 1)
                return;
            var sc = spiControllers[0];
            SpiController tempController = new SpiController()
            {
                name = sc.name,
                device = sc.device,
                info = sc.info,
                write = "0x31, 0x01, 0x2D, 0x08",
                transfer = "0x32|0x80|0x40, 0, 0, 0, 0, 0",
                chipSelect = DEFAULT_CS,
                frequency = DEFAULT_FREQUENCY,
                mode = SpiMode.Mode3
            };
            bool activate = (sc.device == null);
            if (activate)
                await spiActivate(tempController);
            spiSend(tempController, SpiSendMode.Write);
            spiSend(tempController, SpiSendMode.Transfer);
            if (tempController.device != null && activate)
                tempController.device.Dispose();
        }
    }

    public class SpiController
    {
        public string name { get; set; }
        public SpiDevice device { get; set; }
        public DeviceInformation info { get; set; }

        public string write { get; set; }
        public string transfer { get; set; }
        public string chipSelect { get; set; }
        public string frequency { get; set; }
        public SpiMode mode { get; set; }

    }
}

using CoffeeUniversal.Helpers;
using CoffeeUniversal.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class I2cBusPage : Page
    {
        private ObservableCollection<I2cMaster> i2cMasters;
        public enum I2cSendMode { Write, WriteRead, Read };

        private const string DEFAULT_ADDRESS = "0x00";
        private const I2cBusSpeed DEFAULT_BUS_SPEED = I2cBusSpeed.StandardMode;
        private const string DEFAULT_SEND = "1, 2, 3";
        private const string DEFAULT_NAME = "None";
        private const int DEFAULT_READ = 1;

        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public I2cBusPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            InitI2c();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
            i2cMasters.Clear();
            foreach (I2cMaster i2c in i2cMasters)
            {
                if (i2c != null)
                    if (i2c.device != null)
                        i2c.device.Dispose();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        private async void InitI2c()
        {
            cvsBusSpeed.Source = Enum.GetValues(typeof(I2cBusSpeed));
            i2cMasters = new ObservableCollection<I2cMaster>();
            gridView.ItemsSource = i2cMasters;
            try
            {
                string i2cAqs = I2cDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(i2cAqs);
                foreach (DeviceInformation info in dis)
                {
                    i2cMasters.Add(new I2cMaster()
                    {
                        name = info.Id.Split('\\').Last(),
                        info = info,
                    });
                }
            }
            catch (Exception ex)
            {
                status.Log(string.Format(LocalizableStrings.IOT_I2C_NONE, ex.Message));
                gridView.IsEnabled = false;
                i2cMasters.Add(new I2cMaster());
                return;
            }
            status.Log(string.Format(LocalizableStrings.IOT_I2C_FOUND, i2cMasters.Count()));
        }

        private void i2cWrite(I2cMaster master, I2cSendMode sendMode)
        {
            if (master == null)
                return;
            if (master.device != null)
            {
                string addressUsed = HexConverter.ByteToString(master.device.ConnectionSettings.SlaveAddress);
                try
                {
                    string result = "";
                    switch (sendMode)
                    {
                        case I2cSendMode.Write:
                            byte[] SendBuf = HexConverter.StringToByteArray(master.write);
                            if (SendBuf.Count() > 0)
                            {
                                result = master.device.WritePartial(SendBuf).Status.ToString();
                                status.Log(string.Format(LocalizableStrings.IOT_I2C_WRITE, master.name, HexConverter.ByteToString(SendBuf), addressUsed));
                            }
                            break;
                        case I2cSendMode.WriteRead:
                            byte[] WriteBuf = HexConverter.StringToByteArray(master.writeRead);
                            if (WriteBuf.Count() > 0)
                            {
                                byte[] ReadBuf = new byte[master.readSize];
                                result = master.device.WriteReadPartial(WriteBuf, ReadBuf).Status.ToString();
                                status.Log(string.Format(LocalizableStrings.IOT_I2C_WRITE, master.name, HexConverter.ByteToString(WriteBuf), addressUsed));
                                status.Log(string.Format(LocalizableStrings.IOT_I2C_READ, master.name, HexConverter.ByteToString(ReadBuf), addressUsed));
                            }
                            break;
                        case I2cSendMode.Read:
                            byte[] RecBuf = new byte[master.readSize];
                            result = master.device.ReadPartial(RecBuf).Status.ToString();
                            status.Log(string.Format(LocalizableStrings.IOT_I2C_READ, master.name, HexConverter.ByteToString(RecBuf), addressUsed));
                            break;
                    }
                    status.Log(result);
                }
                catch
                {
                    status.Log(string.Format(LocalizableStrings.IOT_I2C_NORESPONSE, master.name, addressUsed));
                }
            }
            else
            {
                status.Log(string.Format(LocalizableStrings.IOT_I2C_NOT_INITIALIZED, master.name));
            }
        }

        private async Task i2cActivate(I2cMaster master)
        {
            if (master == null)
                return;

            if (master.device != null)
            {
                master.device.Dispose();
                master.device = null;
            }

            var address = HexConverter.StringToByteArray(master.slaveAddress);
            if (address.Count() < 1)
            {
                status.Log(string.Format(LocalizableStrings.IOT_I2C_ADDRESS_INVALID, master.name));
                return;
            }
            var settings = new I2cConnectionSettings(address.First());
            settings.BusSpeed = master.busSpeed;
            try
            {
                master.device = await I2cDevice.FromIdAsync(master.info.Id, settings);
                string addressUsed = HexConverter.ByteToString(master.device.ConnectionSettings.SlaveAddress);
                if (master.device == null)
                {
                    status.Log(string.Format(LocalizableStrings.IOT_I2C_IN_USE, addressUsed, master.name));
                }
                status.Log(string.Format(LocalizableStrings.IOT_I2C_INITIALIZED, addressUsed, master.name));
            }
            catch (Exception ex)
            {
                status.Log(string.Format(LocalizableStrings.IOT_I2C_INITIALIZE_FAILED, ex.Message));
            }
        }

        private async void activate_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            await i2cActivate(button.DataContext as I2cMaster);
        }

        private void deactivate_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            I2cMaster master = button.DataContext as I2cMaster;
            if (master != null)
            {
                if (master.device != null)
                {
                    master.device.Dispose();
                    master.device = null;
                }
            }
        }
        private void write_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            I2cMaster master = button.DataContext as I2cMaster;

            i2cWrite(master, I2cSendMode.Write);
        }

        private void WriteRead_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            I2cMaster master = button.DataContext as I2cMaster;

            i2cWrite(master, I2cSendMode.WriteRead);
        }

        private void Read_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            I2cMaster master = button.DataContext as I2cMaster;

            i2cWrite(master, I2cSendMode.Read);
        }

        private async void Accelerometer_Click(object sender, RoutedEventArgs e)
        {
            if (i2cMasters.Count() < 1)
                return;
            var sc = i2cMasters[0];
            I2cMaster tempController = new I2cMaster()
            {
                name = sc.name,
                device = sc.device,
                info = sc.info,
                write = "0x31, 0x01, 0x2D, 0x08",
                writeRead = "0x32",
                slaveAddress = "0x53",
                readSize = 6,
                busSpeed = I2cBusSpeed.FastMode
            };
            bool activate = (sc.device == null || sc.slaveAddress != tempController.slaveAddress);
            if (activate)
                await i2cActivate(tempController);
            i2cWrite(tempController, I2cSendMode.Write);
            i2cWrite(tempController, I2cSendMode.WriteRead);
            if (tempController.device != null && activate)
                tempController.device.Dispose();
            await i2cActivate(sc);
        }

        private async void Eeprom_Click(object sender, RoutedEventArgs e)
        {
            if (i2cMasters.Count() < 1)
                return;
            var sc = i2cMasters[0];
            I2cMaster tempController = new I2cMaster()
            {
                name = sc.name,
                device = sc.device,
                info = sc.info,
                write = "0 0 0 0xff 1",
                writeRead = "0 0 0 0xff",
                slaveAddress = "0x50",
                readSize = 1,
                busSpeed = I2cBusSpeed.FastMode
            };
            var array = HexConverter.StringToByteArray(sc.write);
            byte address, data;
            if (array.Count() >= 2)
            {
                address = array[0];
                data = array[1];
                string write = string.Format("0 {0} {0} 0xff ", address);
                tempController.write = write + data;
                tempController.writeRead = write;
            }
            bool activate = (sc.device == null || sc.slaveAddress != tempController.slaveAddress);
            if (activate)
                await i2cActivate(tempController);
            i2cWrite(tempController, I2cSendMode.Write);
            i2cWrite(tempController, I2cSendMode.WriteRead);
            if (tempController.device != null && activate)
                tempController.device.Dispose();
            await i2cActivate(sc);
        }

        public class I2cMaster
        {
            public I2cMaster()
            {
                name = DEFAULT_NAME;
                write = DEFAULT_SEND;
                writeRead = DEFAULT_SEND;
                slaveAddress = DEFAULT_ADDRESS;
                busSpeed = DEFAULT_BUS_SPEED;
                readSize = DEFAULT_READ;
            }
            public string name { get; set; }
            public I2cDevice device { get; set; }
            public DeviceInformation info { get; set; }

            public string write { get; set; }
            public string writeRead { get; set; }
            public string slaveAddress { get; set; }
            public I2cBusSpeed busSpeed { get; set; }
            public int readSize { get; set; }
        }
    }
}

using InTheHand.Net.Sockets;
using PolimasterIrDADevicesManagerGUI.Device;
using PolimasterIrDADevicesManagerGUI.Device.Protocols;
using PolimasterIrDADevicesManagerGUI.IrDA;
using PolimasterIrDADevicesManagerGUI.Utils;
using System.Windows;
using System.Windows.Controls;

namespace PolimasterIrDADevicesManagerGUI.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string ModuleName = nameof(MainWindow);

        private IrDADevice? CreatedDevice;

        private DeviceSettingsEditorWindow? _settingsEditorWindow;

        private HistoryViewerWindow? _historyViewerWindow;

        private CommandMapperWindow? _commandMapperWindow;

        private RawIrDACommunicationWindow? _rawIrDACommunicationWindow;

        public MainWindow()
        {
            InitializeComponent();
            IrDAPortManager.OnDeviceListChanged += IrDAPortManager_ListUpdated;
            IrDAPortManager.OnDeviceConnected += IrDAPortManager_OnDeviceConnected;
            IrDAPortManager.OnDeviceDisconnected += IrDAPortManager_OnDeviceDisconnected;
            Closing += MainWindow_Closing;
            foreach (string protocolName in ProtocolFactory.RegisteredProtocols.Values)
            {
                protocol_selection_combobox.Items.Add(protocolName);
            }
            foreach (IrDADeviceInfo deviceInfo in IrDAPortManager.AvailableDevices)
            {
                irda_device_listview.Items.Add(deviceInfo);
            }
        }

        private void MainWindow_Closing(object? sender, EventArgs e)
        {
            Logger.Log("Main window close requested", ModuleName);
            if (_settingsEditorWindow != null)
            {
                _settingsEditorWindow.Close();
            }
            if (_historyViewerWindow != null)
            {
                _historyViewerWindow.Close();
            }
            if (_commandMapperWindow != null)
            {
                _commandMapperWindow.Close();
            }
            if (_rawIrDACommunicationWindow != null)
            {
                _rawIrDACommunicationWindow.Close();
            }
        }

        private void IrDAPortManager_OnDeviceDisconnected(object? sender, Events.IrDA.IrDADeviceDisconnectedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                Title = string.Format("Disconnected from {0}", e.Device.DeviceName);
                connect_button.IsEnabled = true;
                open_eeprom_memory_manager_button.IsEnabled = false;
                open_history_reader_button.IsEnabled = false;
                open_settings_manager_button.IsEnabled = false;
                open_command_mapper_button.IsEnabled = false;
                open_irda_manager_button.IsEnabled = false;
                disconnect_button.IsEnabled = false;
                _settingsEditorWindow?.Close();
                _historyViewerWindow?.Close();
            });
        }

        private void IrDAPortManager_OnDeviceConnected(object? sender, Events.IrDA.IrDADeviceConnectedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                Title = string.Format("Connected to {0}", e.Device.DeviceName);
                connect_button.IsEnabled = false;
                CreatedDevice = ProtocolFactory.CreateNewInstance(ProtocolFactory.FindProtocolTypeByName(protocol_selection_combobox.SelectedItem.ToString()), IrDAPortManager.IrDAClient, IrDAPortManager.ConnectedDevice?.GetEndPoint());
                if (CreatedDevice is EEPROMAccessProtocolDevice)
                {
                    open_eeprom_memory_manager_button.IsEnabled = true;
                }
                if (CreatedDevice is ISettingsAccessDevice)
                {
                    open_settings_manager_button.IsEnabled = true;
                }
                if (CreatedDevice is IHistoryAccessDevice)
                {
                    open_history_reader_button.IsEnabled = true;
                }
                if (CreatedDevice is PMDeviceBaseProtocol)
                {
                    open_command_mapper_button.IsEnabled = true;
                }
                if (CreatedDevice is IrDADevice)
                {
                    open_irda_manager_button.IsEnabled = true;
                }
                disconnect_button.IsEnabled = true;
            });
        }

        private void IrDAPortManager_ListUpdated(object? sender, Events.IrDA.AvailableIrDADevicesListChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                int selectedItemIndex = irda_device_listview.SelectedIndex;
                irda_device_listview.Items.Clear();
                foreach (IrDADeviceInfo deviceInfo in e.List)
                {
                    irda_device_listview.Items.Add(deviceInfo);
                }
                irda_device_listview.SelectedIndex = selectedItemIndex;
            });
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (irda_device_listview.SelectedIndex < (IrDAPortManager.AvailableDevices.Length - 1)) return;
            IrDADeviceInfo info = IrDAPortManager.AvailableDevices[irda_device_listview.SelectedIndex];

            if (protocol_selection_combobox.SelectedItem == null)
            {
                for (int i = 0; i < protocol_selection_combobox.Items.Count; i++)
                {
                    if (info.DeviceName.Contains((string)(protocol_selection_combobox.Items[i])))
                    {
                        protocol_selection_combobox.SelectedIndex = i;
                        break;
                    }
                }
            }

            if (protocol_selection_combobox.SelectedItem == null)
            {
                MessageBox.Show(this, "Select protocol before connecting", "Protocol not selected");
                return;
            }

            try
            {
                protocol_selection_combobox.IsEnabled = false;
                Task.Run(() =>
                {
                    IrDAPortManager.Connect(info);
                }).ContinueWith((task) =>
                {
                    if (task.IsFaulted)
                    {
                        Logger.Error(string.Format("Unable to connect to {0} ({1})", info.DeviceName, task.Exception?.Message), ModuleName);
                        MessageBox.Show(this, string.Format("Unable to connect to {0} ({1})", info.DeviceName, task.Exception?.Message), "Connection failed");
                        connect_button.IsEnabled = true;
                    }
                });
            }
            finally
            {
                protocol_selection_combobox.IsEnabled = true;
            }
        }

        private void irda_device_listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            connect_button.IsEnabled = (irda_device_listview.SelectedItems.Count > 0) && !IrDAPortManager.Connected;
        }

        private void disconnect_button_Click(object sender, RoutedEventArgs e)
        {
            IrDAPortManager.Disconnect();
        }

        private void open_settings_manager_button_Click(object sender, RoutedEventArgs e)
        {
            if (CreatedDevice is not ISettingsAccessDevice)
            {
                return;
            }
            if (_settingsEditorWindow != null)
            {
                _settingsEditorWindow.Close();
            }
            _settingsEditorWindow = new DeviceSettingsEditorWindow((ISettingsAccessDevice)CreatedDevice);
            _settingsEditorWindow.Show();
        }

        private void open_history_reader_button_Click(object sender, RoutedEventArgs e)
        {
            if (CreatedDevice is not IHistoryAccessDevice)
            {
                return;
            }
            if (_historyViewerWindow != null)
            {
                _historyViewerWindow.Close();
            }
            double? calibrationValue = null;
            if (CreatedDevice is IDeviceWithCalibrationValue calibrationValueDevice)
            {
                // The only synchronious I/O call in the whole codebase so far :)
                calibrationValue = calibrationValueDevice.ReadCalibrationValue();
            }
            _historyViewerWindow = new HistoryViewerWindow((IHistoryAccessDevice)CreatedDevice, calibrationValue);
            _historyViewerWindow.Show();
        }

        private void open_command_mapper_button_Click(object sender, RoutedEventArgs e)
        {
            if (CreatedDevice is not PMDeviceBaseProtocol)
            {
                return;
            }
            if (_commandMapperWindow != null)
            {
                _commandMapperWindow.Close();
            }
            _commandMapperWindow = new CommandMapperWindow((PMDeviceBaseProtocol)CreatedDevice);
            _commandMapperWindow.Show();
        }

        private void open_irda_manager_button_Click(object sender, RoutedEventArgs e)
        {
            if (CreatedDevice is not IrDADevice)
            {
                return;
            }
            if (_rawIrDACommunicationWindow != null)
            {
                _rawIrDACommunicationWindow.Close();
            }
            _rawIrDACommunicationWindow = new RawIrDACommunicationWindow((IrDADevice)CreatedDevice);
            _rawIrDACommunicationWindow.Show();
        }
    }
}

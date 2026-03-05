using InTheHand.Net;
using InTheHand.Net.Sockets;
using PolimasterIrDADevicesManagerGUI.Events.IrDA;
using PolimasterIrDADevicesManagerGUI.Utils;
using System.Net.Sockets;

namespace PolimasterIrDADevicesManagerGUI.IrDA
{

    internal static class IrDAPortManager
    {

        private static CancellationTokenSource _cancellationTokenSource = new();

        /// <summary>
        /// Indicates whenever the scan for new irda devices is running
        /// </summary>
        public static bool ScanRunning => (!_cancellationTokenSource.Token.IsCancellationRequested);

        public static IrDAClient IrDAClient = new();

        /// <summary>
        /// List of available devices on the port
        /// </summary>
        public static IrDADeviceInfo[] AvailableDevices { get; private set; } = Array.Empty<IrDADeviceInfo>();

        /// <summary>
        /// Raises when a new device is discovered
        /// </summary>
        public static event EventHandler<NewIrDADeviceDiscoveredEventArgs>? OnNewDeviceDiscovered;

        /// <summary>
        /// Raises when the list of available devices changes
        /// </summary>
        public static event EventHandler<AvailableIrDADevicesListChangedEventArgs>? OnDeviceListChanged;

        public static event EventHandler<IrDADeviceConnectedEventArgs>? OnDeviceConnected;

        public static event EventHandler<IrDADeviceDisconnectedEventArgs>? OnDeviceDisconnected;

        private const string ModuleName = nameof(IrDAPortManager);

        private static IrDADeviceInfo? _lastConnected = null;

        public const int CheckLoopDelayMs = 25;

        /// <summary>
        /// Connected device, null if none connected
        /// </summary>
        public static IrDADeviceInfo? ConnectedDevice
        {
            get
            {
                if (IrDAClient.Connected)
                {
                    return _lastConnected;
                }
                return null;
            }
        }

        /// <summary>
        /// Is client connected to any device at the moment
        /// </summary>
        public static bool Connected => IrDAClient.Connected;

        static IrDAPortManager()
        {
            _cancellationTokenSource.Cancel();
            Task.Run(async () =>
            {
                try
                {
                    bool connectedOnPreviousCheck = false;
                    while (true)
                    {
                        if (IrDAClient.Connected && !connectedOnPreviousCheck && (_lastConnected != null))
                        {
                            connectedOnPreviousCheck = true;
                            OnDeviceConnected?.Invoke(null, new IrDADeviceConnectedEventArgs(_lastConnected));
                        }
                        if (!IrDAClient.Connected && connectedOnPreviousCheck && (_lastConnected != null))
                        {
                            connectedOnPreviousCheck = false;
                            OnDeviceDisconnected?.Invoke(null, new IrDADeviceDisconnectedEventArgs(_lastConnected));
                        }
                        await Task.Delay(CheckLoopDelayMs);
                    }
                }catch(Exception e)
                {
                    Logger.Warn(string.Format("IrDA event notifier loop task threw an exception ({0})", e.Message), ModuleName);
                }
            }).ContinueWith((task) =>
            {
                if (task.IsFaulted)
                {
                    Logger.Crash(string.Format("IrDA event notifier loop task crashed and exited! ({0})", task.Exception?.Message));
                }
            });
            Logger.Enable(string.Format("IrDA manager started with {0} ms check loop delay", CheckLoopDelayMs), ModuleName);
        }

        /**
        public static void Restart()
        {
            if (_lastConnected != null)
            {
                IrDAClient.Close();
                IrDAClient.Dispose();
                _lastConnected = null;
            }
        }
        **/

        /// <summary>
        /// Starts scanning for new devices on the IrDA port
        /// </summary>
        public static void StartScan()
        {
            if (ScanRunning)
            {
                return;
            }
            if (!_cancellationTokenSource.TryReset()) _cancellationTokenSource = new();
            Task.Run(async () =>
            {
                while (true)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (IrDAClient.Connected)
                    {
                        await Task.Delay(CheckLoopDelayMs);
                    }
                    try
                    {
                        IrDADeviceInfo[] discovered = IrDAClient.DiscoverDevices();
                        IrDADeviceInfo[] newDevices = discovered.Except(AvailableDevices).ToArray();
                        if (!Enumerable.SequenceEqual(AvailableDevices, discovered)) 
                        {
                            AvailableDevices = discovered;
                            OnDeviceListChanged?.Invoke(null, new AvailableIrDADevicesListChangedEventArgs(AvailableDevices));
                            foreach (IrDADeviceInfo newDevice in newDevices)
                            {
                                Logger.Log(string.Format("New device found ({0} at {1})", newDevice.DeviceName, newDevice.DeviceAddress.ToString()), ModuleName);
                                OnNewDeviceDiscovered?.Invoke(null, new NewIrDADeviceDiscoveredEventArgs(newDevice));
                            }
                        }
                        await Task.Delay(CheckLoopDelayMs);
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(string.Format("IrDA scanning loop task threw an exception: {0}", e.Message), ModuleName);
                    }
                }
            }).ContinueWith((_) =>
            {
                Logger.Important("Scanning task ended", ModuleName);
                if (!_cancellationTokenSource.Token.IsCancellationRequested) _cancellationTokenSource.Cancel();
            });
            Logger.Enable("Scanning task started", ModuleName);
        }

        public static void Connect(IrDADeviceInfo device)
        {
            if (!IrDAClient.Connected)
            {
                Logger.Log(string.Format("Attempting to connect to {0}...", device.DeviceName), ModuleName);
                // For whatever reason PM1208 refuses to connect with a default configuration
                // It need the service name to be set to IrDA:IrCOMM and for socket option 22 to be set to 255
                // Not a very OOP fix but it works (in theory), so whatever
                if (device.DeviceName.Contains("PM1208")) IrDAClient.Client.SetSocketOption((SocketOptionLevel)255, IrDASocketOptionName.NineWireMode, true); // Otherwise PM1208 connects but outputs gibberish
                IrDAClient.Connect(new IrDAEndPoint(device.DeviceAddress, (device.DeviceName.Contains("PM1208") ? "IrDA:IrCOMM" : device.DeviceName)));
                _lastConnected = device;
                Logger.Enable(string.Format("Connected successfully to {0}", device.DeviceName), ModuleName);
            }
        }

        public static void Disconnect()
        {
            if (IrDAClient.Connected)
            {
                Logger.Log(string.Format("Disconnecting from {0}...", _lastConnected.DeviceName), ModuleName);
                IrDAClient.Close();
                IrDAClient.Dispose(); // Unnecessary call (_irDAClient.Close() just calls _irDAClient.Dispose() inside)
                IrDAClient = new();
                Logger.Enable(string.Format("Disconnected from {0}", _lastConnected.DeviceName), ModuleName);
            }
        }

        public static void Stop()
        {
            if (!ScanRunning)
            {
                return;
            }
            _cancellationTokenSource.Cancel();
            Logger.Enable("Scanning task stopped", ModuleName);
        }
    }
}

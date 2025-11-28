using InTheHand.Net;
using InTheHand.Net.Sockets;
using PolimasterIrDADevicesManagerGUI.Events.IrDA;
using PolimasterIrDADevicesManagerGUI.Utils;

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
        /// Raises when a new device was discovered
        /// </summary>
        public static event EventHandler<NewIrDADeviceDiscoveredEventArgs>? OnNewDeviceDiscovered;

        /// <summary>
        /// Raises when the of available devices changes
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
                    Logger.Error(string.Format("IrDA event notifier loop task crashed and exited! ({0})", task.Exception?.Message));
                }
            });
            Logger.Enable(string.Format("IrDA manager started with {0} ms check loop delay", CheckLoopDelayMs), ModuleName);
        }

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
                        Logger.Warn(string.Format("IrDA scanning loop threw an exception: {0}", e.Message), ModuleName);
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
                IrDAClient.Connect(new IrDAEndPoint(device.DeviceAddress, device.DeviceName));
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

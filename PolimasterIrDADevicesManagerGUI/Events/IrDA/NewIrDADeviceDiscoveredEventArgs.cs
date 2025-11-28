using InTheHand.Net.Sockets;

namespace PolimasterIrDADevicesManagerGUI.Events.IrDA
{
    public sealed class NewIrDADeviceDiscoveredEventArgs : EventArgs
    {

        public readonly IrDADeviceInfo DeviceInfo;

        public NewIrDADeviceDiscoveredEventArgs(IrDADeviceInfo deviceInfo) : base()
        {
            DeviceInfo = deviceInfo;
        }
    }
}

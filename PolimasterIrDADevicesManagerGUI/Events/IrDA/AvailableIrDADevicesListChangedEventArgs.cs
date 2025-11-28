using InTheHand.Net.Sockets;

namespace PolimasterIrDADevicesManagerGUI.Events.IrDA
{
    public sealed class AvailableIrDADevicesListChangedEventArgs : EventArgs
    {

        public readonly IrDADeviceInfo[] List;

        public AvailableIrDADevicesListChangedEventArgs(IrDADeviceInfo[] list) : base()
        {
            List = list;
        }
    }
}

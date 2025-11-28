using InTheHand.Net.Sockets;

namespace PolimasterIrDADevicesManagerGUI.Events.IrDA
{
    public sealed class IrDADeviceConnectedEventArgs : EventArgs
    {

        public readonly IrDADeviceInfo Device;

        public IrDADeviceConnectedEventArgs(IrDADeviceInfo device) : base()
        {
            Device = device;
        }
    }
}

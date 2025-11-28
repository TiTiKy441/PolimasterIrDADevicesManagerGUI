using InTheHand.Net.Sockets;

namespace PolimasterIrDADevicesManagerGUI.Events.IrDA
{
    public sealed class IrDADeviceDisconnectedEventArgs : EventArgs
    {

        public readonly IrDADeviceInfo Device;

        public IrDADeviceDisconnectedEventArgs(IrDADeviceInfo device) : base()
        {
            Device = device;
        }
    }
}

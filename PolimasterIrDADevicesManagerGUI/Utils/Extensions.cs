using InTheHand.Net;
using InTheHand.Net.Sockets;

namespace PolimasterIrDADevicesManagerGUI.Utils
{
    public static class Extensions
    {

        public static IrDAEndPoint GetEndPoint(this IrDADeviceInfo info)
        {
            return new IrDAEndPoint(info.DeviceAddress, info.DeviceName);
        }

    }
}

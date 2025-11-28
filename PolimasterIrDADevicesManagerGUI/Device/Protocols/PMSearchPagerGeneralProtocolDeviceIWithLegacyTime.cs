using InTheHand.Net;
using InTheHand.Net.Sockets;

namespace PolimasterIrDADevicesManagerGUI.Device.Protocols
{
    public class PMSearchPagerGeneralProtocolDeviceIWithLegacyTime : PMSearchPagerGeneralProtocolDevice
    {
        public PMSearchPagerGeneralProtocolDeviceIWithLegacyTime(IrDAClient client, IrDAEndPoint deviceEndPoint) : base(client, deviceEndPoint, true)
        {
        }
    }
}

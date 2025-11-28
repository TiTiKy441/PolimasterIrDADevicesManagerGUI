using InTheHand.Net;
using InTheHand.Net.Sockets;
using PolimasterIrDADevicesManagerGUI.Device.Protocols.Implementations;
using PolimasterIrDADevicesManagerGUI.IrDA;
using PolimasterIrDADevicesManagerGUI.Utils;

namespace PolimasterIrDADevicesManagerGUI.Device.Protocols
{
    internal static class ProtocolFactory
    {

        public static Dictionary<Type, string> RegisteredProtocols = new Dictionary<Type, string>();

        public static Dictionary<Type, object[]> ProtocolArgs = new Dictionary<Type, object[]>();

        private const string ModuleName = nameof(ProtocolFactory);

        static ProtocolFactory()
        {
            RegisterProtocol(typeof(PM1401GNProtocolDevice),                            "PM1401GN");
            RegisterProtocol(typeof(PM1401MProtocolDevice),                             "PM1401M");
            RegisterProtocol(typeof(EEPROMAccessProtocolDevice),                        "EEPROM Access");
            RegisterProtocol(typeof(PMSearchPagerGeneralProtocolDevice),                "Search pager general protocol",                 new object[3] { new object(), new object(), false });
            RegisterProtocol(typeof(PMSearchPagerGeneralProtocolDeviceIWithLegacyTime), "Search pager general protocol with legacy time");
        }

        public static Type FindProtocolTypeByName(string name)
        {
            return RegisteredProtocols.Keys.ToArray()[RegisteredProtocols.Values.ToList().IndexOf(name)];
        }

        public static void RegisterProtocol(Type protocol, string deviceIndentifier, object[]? args = null)
        {
            if (!protocol.IsAssignableTo(typeof(IrDADevice)))
            {
                throw new InvalidOperationException(string.Format("{0} does not inherit IrDADevice class!", nameof(protocol)));
            }
            RegisteredProtocols.Add(protocol, deviceIndentifier);
            ProtocolArgs.Add(protocol, (args == null) ? new object[2] : args);
            Logger.Log(string.Format("Registered {0} protocol", protocol.Name), ModuleName);
        }

        public static IrDADevice CreateNewInstance(Type protocol, IrDAClient client, IrDAEndPoint deviceEndPoint)
        {
            if (!RegisteredProtocols.ContainsKey(protocol))
            {
                throw new ArgumentException(string.Format("Protocol {0} is not registered", nameof(protocol)));
            }
            object[] args = ProtocolArgs[protocol];
            args[0] = client;
            args[1] = deviceEndPoint;
            IrDADevice? result = (IrDADevice?)Activator.CreateInstance(protocol, args);
            if (result is null)
            {
                throw new InvalidOperationException();
            }
            Logger.Log(string.Format("Created new instance of {0}", protocol.Name), ModuleName);
            return result;
        }
    }
}

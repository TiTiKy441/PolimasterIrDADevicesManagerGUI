using InTheHand.Net;
using InTheHand.Net.Sockets;

namespace PolimasterIrDADevicesManagerGUI.Device.Protocols.Implementations
{
    public class PM1621ProtocolDevice : PMDeviceBaseProtocol
    {

        public static readonly PM1621DeviceParameterInfo[] SupportedParameters = new PM1621DeviceParameterInfo[]
        {
            //                            Parameter                Name                          Edit   Value type
            new(PM1621DeviceParameterType.SerialNumber,            "Serial number",              false, typeof(string)),
            new(PM1621DeviceParameterType.ManufactureDate,         "Manufacture date",           false, typeof(DateTime)),
            new(PM1621DeviceParameterType.DateTime,                "Date time",                  true,  typeof(DateTime)),
            new(PM1621DeviceParameterType.DoseRateThreshold1,      "Dose rate threshold 1",      true,  typeof(double)),
            new(PM1621DeviceParameterType.DoseRateThreshold2,      "Dose rate threshold 2",      true,  typeof(double)),
            new(PM1621DeviceParameterType.DoseThreshold1,          "Dose threshold 1",           true,  typeof(double)),
            new(PM1621DeviceParameterType.DoseThreshold2,          "Dose threshold 2",           true,  typeof(double)),
            new(PM1621DeviceParameterType.Voltage,                 "Voltage",                    false, typeof(ushort)),
            new(PM1621DeviceParameterType.Rate,                    "Rate",                       false, typeof(double)),
            new(PM1621DeviceParameterType.Dose,                    "Dose",                       false, typeof(double)),
            new(PM1621DeviceParameterType.Units,                   "Units flag",                 true,  typeof(bool)),
            new(PM1621DeviceParameterType.HistoryType,             "History type",               true,  typeof(bool)),
            new(PM1621DeviceParameterType.HistoryWriteDose,        "History write dose",         true,  typeof(bool)),
            new(PM1621DeviceParameterType.HistoryWriteRate,        "History write rate",         true,  typeof(bool)),
            new(PM1621DeviceParameterType.HistoryStep,             "History step",               true,  typeof(ushort)),
            new(PM1621DeviceParameterType.SoundRateAlarm,          "Sound rate alarm",           true,  typeof(bool)),
            new(PM1621DeviceParameterType.SoundDoseAlarm,          "Sound dose alarm",           true,  typeof(bool)),
            new(PM1621DeviceParameterType.AudioEnabled,            "Audio enabled",              true,  typeof(bool)),
            new(PM1621DeviceParameterType.VibroEnabled,            "Vibro enabled",              true,  typeof(bool)),
            new(PM1621DeviceParameterType.SearchEnabled,           "Search enabled",             true,  typeof(bool)),
            new(PM1621DeviceParameterType.EnabledResetDose,        "Enable reset dose",          true,  typeof(bool)),
            new(PM1621DeviceParameterType.DERIndication,           "DER Indication",             true,  typeof(bool)),
            new(PM1621DeviceParameterType.DEIndication,            "DE Indication",              true,  typeof(bool)),
            new(PM1621DeviceParameterType.SNIndication,            "SN Indication",              true,  typeof(bool)),
            new(PM1621DeviceParameterType.IndicationSearchMode,    "Indication search mode",     true,  typeof(bool)),
            new(PM1621DeviceParameterType.IndicationAudioMode,     "Indication audio mode",      true,  typeof(bool)),
            new(PM1621DeviceParameterType.IndicationVibroMode,     "Indication vibro mode",      true,  typeof(bool)),
            new(PM1621DeviceParameterType.DERIndicationValue,      "DER Indication value",       true,  typeof(bool)),
            new(PM1621DeviceParameterType.DERIndicationThreshold1, "DER Indication threshold 1", true,  typeof(bool)),
            new(PM1621DeviceParameterType.DERIndicationThreshold2, "DER Indication threshold 2", true,  typeof(bool)),
            new(PM1621DeviceParameterType.DERIndicationStat,       "DER Indication stat",        true,  typeof(bool)),
            new(PM1621DeviceParameterType.DERIndicationOff,        "DER Indication off",         true,  typeof(bool)),
            new(PM1621DeviceParameterType.DEIndicationValue,       "DE Indication value",        true,  typeof(bool)),
            new(PM1621DeviceParameterType.DEIndicationThreshold1,  "DE Indication threshold 1",  true,  typeof(bool)),
            new(PM1621DeviceParameterType.DEIndicationThreshold2,  "DE Indication threshold 2",  true,  typeof(bool)),
            new(PM1621DeviceParameterType.DEIndicationReset,       "DE Indication reset",        true,  typeof(bool)),
            new(PM1621DeviceParameterType.DEIndicationAccTime,     "DE Indication acc time",     true,  typeof(bool)),
            new(PM1621DeviceParameterType.DEIndicationOff,         "DE Indication off",          true,  typeof(bool))
        };

        public PM1621ProtocolDevice(IrDAClient irdaClient, IrDAEndPoint endpoint) : base(irdaClient, endpoint)
        {
        }

        public class PM1621DeviceParameterInfo : DeviceParameterInfo
        {
            public PM1621DeviceParameterInfo(PM1621DeviceParameterType id, string name, bool editable, Type type, string? unit = null) : base((int)id, name, editable, type, unit)
            {
            }
        }

        public enum PM1621DeviceParameterType : int
        {
            SerialNumber = 0,
            ManufactureDate = 1,
            DateTime = 2,
            DoseRateThreshold1 = 3,
            DoseRateThreshold2 = 4,
            DoseThreshold1 = 5,
            DoseThreshold2 = 6,
            Voltage = 7,
            Rate = 8,
            Dose = 9,
            Units = 10,
            HistoryType = 11,
            HistoryWriteDose = 12,
            HistoryWriteRate = 13,
            HistoryStep = 14,
            SoundRateAlarm = 15,
            SoundDoseAlarm = 16, 
            AudioEnabled = 17,
            VibroEnabled = 18,
            SearchEnabled = 19,
            EnabledResetDose = 20,
            DERIndication = 21,
            DEIndication = 22,
            SNIndication = 23,
            IndicationSearchMode = 24,
            IndicationAudioMode = 25,
            IndicationVibroMode = 26,
            DERIndicationValue = 27,
            DERIndicationThreshold1 = 28,
            DERIndicationThreshold2 = 29,
            DERIndicationStat = 30,
            DERIndicationOff = 31,
            DEIndicationValue = 32,
            DEIndicationThreshold1 = 33,
            DEIndicationThreshold2 = 34,
            DEIndicationReset = 35,
            DEIndicationAccTime = 36,
            DEIndicationOff = 37,
        }
    }
}

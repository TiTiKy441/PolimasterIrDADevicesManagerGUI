using InTheHand.Net;
using InTheHand.Net.Sockets;
using PolimasterIrDADevicesManagerGUI.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PolimasterIrDADevicesManagerGUI.Device.Protocols.Implementations
{
    
    public sealed class PM1603ProtocolDevice : PMDeviceBaseProtocol, ISettingsAccessDevice
    {

        public readonly PM1603DeviceParameterInfo[] SupportedParameters = new PM1603DeviceParameterInfo[]
        {
            //                            Parameter                Name                          Edit   Value type
            new(PM1603DeviceParameterType.SerialNumber,            "Serial number",              false, typeof(int)),
            new(PM1603DeviceParameterType.DoseRateThreshold1,      "Dose rate threshold 1",      true,  typeof(double)),
            new(PM1603DeviceParameterType.DoseRateThreshold2,      "Dose rate threshold 2",      true,  typeof(double)),
            new(PM1603DeviceParameterType.DoseThreshold1,          "Dose threshold 1",           true,  typeof(double)),
            new(PM1603DeviceParameterType.DoseThreshold2,          "Dose threshold 2",           true,  typeof(double)),
            new(PM1603DeviceParameterType.DateTime,                "Date time",                  true,  typeof(int)),
            new(PM1603DeviceParameterType.BatteryVoltage,          "Battery voltage",            true,  typeof(ushort)),
            new(PM1603DeviceParameterType.Rate,                    "Rate",                       false, typeof(double)),
            new(PM1603DeviceParameterType.Dose,                    "Dose",                       false, typeof(double)),
            new(PM1603DeviceParameterType.Units,                   "Units",                      true,  typeof(bool)),
            new(PM1603DeviceParameterType.HistoryType,             "History type",               true,  typeof(bool)),
            new(PM1603DeviceParameterType.HistoryStep,             "History step",               true,  typeof(ushort)),
            new(PM1603DeviceParameterType.HistoryWriteDose,        "History write dose",         true,  typeof(bool)),
            new(PM1603DeviceParameterType.HistoryWriteRate,        "History write rate",         true,  typeof(bool)),
            new(PM1603DeviceParameterType.SoundRateAlarm,          "Sound rate alarm",           true,  typeof(bool)),
            new(PM1603DeviceParameterType.SoundDoseAlarm,          "Sound dose alarm",           true,  typeof(bool)),
            new(PM1603DeviceParameterType.EnableResetDose,         "Enable reset dose",          true,  typeof(bool)),
            new(PM1603DeviceParameterType.DERIndication,           "DER indication",             true,  typeof(bool)),
            new(PM1603DeviceParameterType.DEIndication,            "DE indication",              true,  typeof(bool)),
            new(PM1603DeviceParameterType.SNIndication,            "SN indication",              true,  typeof(bool)),
            new(PM1603DeviceParameterType.AlarmClockIndication,    "Alarm clock indication",     true,  typeof(bool)),
            new(PM1603DeviceParameterType.TimerIndication,         "Time indication",            true,  typeof(bool)),
            new(PM1603DeviceParameterType.StopwatchIndication,     "Stopwatch indication",       true,  typeof(bool)),
            new(PM1603DeviceParameterType.CalendarIndication,      "Calendar indication",        true,  typeof(bool)),
            new(PM1603DeviceParameterType.DERIndicationValue,      "DER indication",             true,  typeof(bool)),
            new(PM1603DeviceParameterType.DERIndicationThreshold1, "DER indication threshold 1", true,  typeof(bool)),
            new(PM1603DeviceParameterType.DERIndicationThreshold2, "DER indication threshold 2", true,  typeof(bool)),
            new(PM1603DeviceParameterType.DERIndicationStatus,     "DER indication status",      true,  typeof(bool)),
            new(PM1603DeviceParameterType.DERIndicationTime,       "DER indication time",        true,  typeof(bool)),
            new(PM1603DeviceParameterType.DERIndicationOff,        "DER indication off",         true,  typeof(bool)),
            new(PM1603DeviceParameterType.DEIndicationValue,       "DE indication value",        true,  typeof(bool)),
            new(PM1603DeviceParameterType.DEIndicationThreshold1,  "DE indication threshold 1",  true,  typeof(bool)),
            new(PM1603DeviceParameterType.DEIndicationThreshold2,  "DE indication threshold 2",  true,  typeof(bool)),
            new(PM1603DeviceParameterType.DEIndicationReset,       "DE indication reset",        true,  typeof(bool)),
            new(PM1603DeviceParameterType.DEIndicationAccTime,     "DE indication acc time",     true,  typeof(bool)),
            new(PM1603DeviceParameterType.DEIndicationTime,        "DE indication time",         true,  typeof(bool)),
            new(PM1603DeviceParameterType.DEIndicationOff,         "DE indication off",          true,  typeof(bool))
        };

        public PM1603ProtocolDevice(IrDAClient irdaClient, IrDAEndPoint endpoint, bool legacyFormat = false) : base(irdaClient, endpoint, legacyFormat)
        {
        }


        public void WriteParameter(int parameter, object value)
        {
            throw new NotImplementedException();
        }

        public void WriteParameterAsString(int parameter, string value)
        {
            throw new NotImplementedException();
        }

        public Task WriteParameterAsync(int parameter, object value, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task WriteParameterAsStringAsync(int parameter, string value, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public object ReadParameter(int parameter)
        {
            throw new NotImplementedException();
        }

        public Task<object> ReadParameterAsync(int parameter, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public DeviceParameterInfo[] GetSupportedParameters()
        {
            throw new NotImplementedException();
        }

        public DeviceParameterInfo FindParameterByType(int type)
        {
            throw new NotImplementedException();
        }

        #region Dose rate threshold 1

        public double ReadDoseRateThreshold1()
        {
            return RawBytesConverter.ReadDouble(ReadFromMC(33)) * 100000.0d;
        }

        public async Task<double> ReadDoseRateThreshold1Async(CancellationToken cancellationToken)
        {
            return RawBytesConverter.ReadDouble(await ReadFromMCAsync(33, cancellationToken)) * 100000.0d;
        }

        public void WriteDoseRateThreshold1(double value)
        {
            WriteArrToMC(33, RawBytesConverter.GetDoubleAsBytes(value));
        }

        public async Task WriteDoseRateThreshold1(double value, CancellationToken cancellationToken)
        {
            await WriteArrToMCAsync(33, RawBytesConverter.GetDoubleAsBytes(value), cancellationToken);
        }

        #endregion

        #region Dose rate threshold 2

        public double ReadDoseRateThreshold2()
        {
            return RawBytesConverter.ReadDouble(ReadFromMC(34)) * 100000.0d;
        }

        public async Task<double> ReadDoseRateThreshold2Async(CancellationToken cancellationToken)
        {
            return RawBytesConverter.ReadDouble(await ReadFromMCAsync(34, cancellationToken)) * 100000.0d;
        }

        public void WriteDoseRateThreshold2(double value)
        {
            WriteArrToMC(34, RawBytesConverter.GetDoubleAsBytes(value));
        }

        public async Task WriteDoseRateThreshold2(double value, CancellationToken cancellationToken)
        {
            await WriteArrToMCAsync(34, RawBytesConverter.GetDoubleAsBytes(value), cancellationToken);
        }

        #endregion

        #region Dose threshold 1

        public double ReadDoseThreshold1()
        {
            return RawBytesConverter.ReadDouble(ReadFromMC(50));
        }

        public async Task<double> ReadDoseThreshold1Async(CancellationToken cancellationToken)
        {
            return RawBytesConverter.ReadDouble(await ReadFromMCAsync(50, cancellationToken));
        }

        public void WriteDoseThreshold1(double value)
        {
            WriteArrToMC(50, RawBytesConverter.GetDoubleAsBytes(value));
        }

        public async Task WriteDoseThreshold1Async(double value, CancellationToken cancellationToken)
        {
            await WriteArrToMCAsync(50, RawBytesConverter.GetDoubleAsBytes(value), cancellationToken);
        }

        #endregion

        #region Dose threshold 2

        public double ReadDoseThreshold2()
        {
            return RawBytesConverter.ReadDouble(ReadFromMC(51));
        }

        public async Task<double> ReadDoseThreshold2Async(CancellationToken cancellationToken)
        {
            return RawBytesConverter.ReadDouble(await ReadFromMCAsync(51, cancellationToken));
        }

        public void WriteDoseThreshold2(double value)
        {
            WriteArrToMC(51, RawBytesConverter.GetDoubleAsBytes(value));
        }

        public async Task WriteDoseThreshold2Async(double value, CancellationToken cancellationToken)
        {
            await WriteArrToMCAsync(51, RawBytesConverter.GetDoubleAsBytes(value), cancellationToken);
        }

        #endregion

        #region Date time

        public DateTime ReadDateTime()
        {
            return RawBytesConverter.Bytes4ToTime(ReadFromMC(17));
        }

        public async Task<DateTime> ReadDateTimeAsync(CancellationToken cancellationToken)
        {
            return RawBytesConverter.Bytes4ToTime(await ReadFromMCAsync(17, cancellationToken));
        }

        public void WriteDateTime(DateTime value)
        {
            WriteArrToMC(17, RawBytesConverter.TimeTo4Bytes(value));
        }

        public async Task WriteDateTimeAsync(DateTime value, CancellationToken cancellationToken)
        {
            await WriteArrToMCAsync(17, RawBytesConverter.TimeTo4Bytes(value), cancellationToken);
        }

        #endregion

        #region Voltage

        public ushort ReadBatteryVoltage()
        {
            return ReadUshortFromMC(2);
        }

        public async Task<ushort> ReadBatteryVoltageAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromMCAsync(2, cancellationToken);
        }

        #endregion

        #region Rate

        public double ReadRate()
        {
            return RawBytesConverter.ReadDouble(ReadFromMC(163)) * 100000.0d;
        }

        public async Task<double> ReadRateAsync(CancellationToken cancellationToken)
        {
            return RawBytesConverter.ReadDouble(await ReadFromMCAsync(163, cancellationToken)) * 100000.0d;
        }

        #endregion

        #region Dose

        public double ReadDose()
        {
            return RawBytesConverter.ReadDouble(ReadFromMC(55)) * 100000.0d;
        }

        public async Task<double> ReadDoseAsync(CancellationToken cancellationToken)
        {
            return RawBytesConverter.ReadDouble(await ReadFromMCAsync(55, cancellationToken)) * 100000.0d;
        }

        #endregion

        #region Units (huh?)



        #endregion

    }

    public class PM1603DeviceParameterInfo : DeviceParameterInfo
    {
        public PM1603DeviceParameterInfo(PM1603DeviceParameterType type, string name, bool changable, Type valueType, string? unit = null) : base((int)type, name, changable, valueType, unit)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Id, Name);
        }
    }

    public enum PM1603DeviceParameterType : int
    {
        SerialNumber = 0,
        DoseRateThreshold1 = 1,
        DoseRateThreshold2 = 2,
        DoseThreshold1 = 3,
        DoseThreshold2 = 4,
        DateTime = 5,
        BatteryVoltage = 6,
        Rate = 7,
        Dose = 8,
        Units = 9,
        HistoryType = 10,
        HistoryStep = 11,
        HistoryWriteDose = 12,
        HistoryWriteRate = 13,
        SoundRateAlarm = 14,
        SoundDoseAlarm = 15,
        EnableResetDose = 16,
        DERIndication = 17,
        DEIndication = 18,
        SNIndication = 19,
        AlarmClockIndication = 20,
        TimerIndication = 21,
        StopwatchIndication = 22,
        CalendarIndication = 23,
        DERIndicationValue = 24,
        DERIndicationThreshold1 = 25,
        DERIndicationThreshold2 = 26,
        DERIndicationStatus = 27,
        DERIndicationTime = 28,
        DERIndicationOff = 29,
        DEIndicationValue = 30,
        DEIndicationThreshold1 = 31,
        DEIndicationThreshold2 = 32,
        DEIndicationReset = 33,
        DEIndicationAccTime = 34,
        DEIndicationTime = 35,
        DEIndicationOff = 36,
    }
}

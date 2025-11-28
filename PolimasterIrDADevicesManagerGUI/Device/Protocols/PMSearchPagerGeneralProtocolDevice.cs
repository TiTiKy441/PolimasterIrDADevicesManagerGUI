using InTheHand.Net.Sockets;
using InTheHand.Net;
using PolimasterIrDADevicesManagerGUI.Utils;

namespace PolimasterIrDADevicesManagerGUI.Device.Protocols
{
    public class PMSearchPagerGeneralProtocolDevice : EEPROMAccessProtocolDevice, ISettingsAccessDevice
    {

        public readonly bool IsLegacyTime;

        public double BatteryReadoutCoefficient { get; protected set; } = 0.838d;

        public static readonly PMSearchPagerDeviceParameterInfo[] SupportedParameters = new PMSearchPagerDeviceParameterInfo[]
        {
            new(PMSearchPagerGeneralProtocolParameters.SerialNumber,             "Serial number",              false, typeof(string)),
            new(PMSearchPagerGeneralProtocolParameters.NGamma,                   "N Gamma",                    true,  typeof(double)),
            new(PMSearchPagerGeneralProtocolParameters.BatteryLevel,             "Battery level",              false, typeof(int),      "V"),
            new(PMSearchPagerGeneralProtocolParameters.BatteryCutoffLevel,       "Battery cutoff level",       false, typeof(int),      "V"),
            new(PMSearchPagerGeneralProtocolParameters.BatteryPercentage,        "Battery percentage",         false, typeof(double),   "%"),
            new(PMSearchPagerGeneralProtocolParameters.DateTime,                 "Device time",                true,  typeof(DateTime)),
            new(PMSearchPagerGeneralProtocolParameters.HistoryRecordingInterval, "History recording interval", true,  typeof(int),      "min"),
            new(PMSearchPagerGeneralProtocolParameters.StatDsp,                  "DSP Status word",            true,  typeof(ushort)),
            new(PMSearchPagerGeneralProtocolParameters.StatAlarm,                "Alarm status word",          true,  typeof(ushort)),
        };


        public PMSearchPagerGeneralProtocolDevice(IrDAClient client, IrDAEndPoint deviceEndPoint, bool legacyTime = false) : base(client, deviceEndPoint)
        {
            IsLegacyTime = legacyTime;
        }

        public DeviceParameterInfo FindParameterByType(int type) => SupportedParameters.First(x => (x.Id == type));


        public void WriteParameter(int parameter, object value) => WriteParameter((PMSearchPagerGeneralProtocolParameters)parameter, value);

        public async Task WriteParameterAsync(int parameter, object value, CancellationToken cancellationToken) => await WriteParameterAsync((PMSearchPagerGeneralProtocolParameters)parameter, value, cancellationToken);

        public void WriteParameterAsString(int parameter, string value) => WriteParameterAsString((PMSearchPagerGeneralProtocolParameters)parameter, value);

        public async Task WriteParameterAsStringAsync(int parameter, string value, CancellationToken cancellationToken) => await WriteParameterAsStringAsync((PMSearchPagerGeneralProtocolParameters)parameter, value, cancellationToken);

        public object ReadParameter(int parameter) => ReadParameter((PMSearchPagerGeneralProtocolParameters)parameter);

        public async Task<object> ReadParameterAsync(int parameter, CancellationToken cancellationToken) => await ReadParameterAsync((PMSearchPagerGeneralProtocolParameters)parameter, cancellationToken);

        public object ReadParameter(PMSearchPagerGeneralProtocolParameters parameter)
        {
            switch (parameter)
            {
                case PMSearchPagerGeneralProtocolParameters.SerialNumber:
                    return ReadSerialNumber();
                case PMSearchPagerGeneralProtocolParameters.NGamma:
                    return ReadGammaCoefficient();
                case PMSearchPagerGeneralProtocolParameters.BatteryLevel:
                    return ReadBatteryVoltage();
                case PMSearchPagerGeneralProtocolParameters.BatteryCutoffLevel:
                    return ReadBatteryThreshold();
                case PMSearchPagerGeneralProtocolParameters.DateTime:
                    return ReadDateTime();
                case PMSearchPagerGeneralProtocolParameters.HistoryRecordingInterval:
                    return ReadHistoryRecordInterval();
                case PMSearchPagerGeneralProtocolParameters.BatteryPercentage:
                    return ReadBatteryPercentage();
                case PMSearchPagerGeneralProtocolParameters.StatDsp:
                    return ReadStatDsp();
                case PMSearchPagerGeneralProtocolParameters.StatAlarm:
                    return ReadStatAlarm();
                default:
                    throw new ArgumentException("Type not supported");
            }
        }

        public async Task<object> ReadParameterAsync(PMSearchPagerGeneralProtocolParameters parameter, CancellationToken cancellationToken)
        {
            switch (parameter)
            {
                case PMSearchPagerGeneralProtocolParameters.SerialNumber:
                    return await ReadSerialNumberAsync(cancellationToken);
                case PMSearchPagerGeneralProtocolParameters.NGamma:
                    return await ReadGammaCoefficientAsync(cancellationToken);
                case PMSearchPagerGeneralProtocolParameters.BatteryLevel:
                    return await ReadBatteryVoltageAsync(cancellationToken);
                case PMSearchPagerGeneralProtocolParameters.BatteryCutoffLevel:
                    return await ReadBatteryThresholdAsync(cancellationToken);
                case PMSearchPagerGeneralProtocolParameters.DateTime:
                    return await ReadDateTimeAsync(cancellationToken);
                case PMSearchPagerGeneralProtocolParameters.HistoryRecordingInterval:
                    return await ReadHistoryRecordIntervalAsync(cancellationToken);
                case PMSearchPagerGeneralProtocolParameters.BatteryPercentage:
                    return await ReadBatteryPercentageAsync(cancellationToken);
                case PMSearchPagerGeneralProtocolParameters.StatDsp:
                    return await ReadStatDspAsync(cancellationToken);
                case PMSearchPagerGeneralProtocolParameters.StatAlarm:
                    return await ReadStatAlarmAsync(cancellationToken);
                default:
                    throw new ArgumentException("Type not supported");
            }
        }

        public void WriteParameterAsString(PMSearchPagerGeneralProtocolParameters parameter, string value)
        {
            switch (parameter)
            {
                case PMSearchPagerGeneralProtocolParameters.StatAlarm:
                case PMSearchPagerGeneralProtocolParameters.StatDsp:
                case PMSearchPagerGeneralProtocolParameters.HistoryRecordingInterval:
                case PMSearchPagerGeneralProtocolParameters.NGamma:
                    WriteParameter(parameter, double.Parse(value));
                    break;

                case PMSearchPagerGeneralProtocolParameters.DateTime:
                    WriteParameter(parameter, DateTime.Parse(value));
                    break;

                default:
                    throw new ArgumentException("Type not supported");
            }
        }


        public void WriteParameter(PMSearchPagerGeneralProtocolParameters parameter, object value)
        {
            switch (parameter)
            {
                case PMSearchPagerGeneralProtocolParameters.NGamma:
                    WriteGammaCoefficient((double)value);
                    break;

                case PMSearchPagerGeneralProtocolParameters.DateTime:
                    WriteDateTime((DateTime)value);
                    break;

                case PMSearchPagerGeneralProtocolParameters.HistoryRecordingInterval:
                    WriteHistoryRecordInterval((ushort)value);
                    break;

                case PMSearchPagerGeneralProtocolParameters.StatDsp:
                    WriteStatDsp((ushort)value);
                    break;

                case PMSearchPagerGeneralProtocolParameters.StatAlarm:
                    WriteStatAlarm((ushort)value);
                    break;

                default:
                    throw new ArgumentException("Type not supported");
            }
        }

        public async Task WriteParameterAsStringAsync(PMSearchPagerGeneralProtocolParameters parameter, string value, CancellationToken cancellationToken)
        {
            switch (parameter)
            {
                case PMSearchPagerGeneralProtocolParameters.StatDsp:
                case PMSearchPagerGeneralProtocolParameters.StatAlarm:
                case PMSearchPagerGeneralProtocolParameters.HistoryRecordingInterval:
                case PMSearchPagerGeneralProtocolParameters.NGamma:
                    await WriteParameterAsync(parameter, double.Parse(value), cancellationToken);
                    break;

                case PMSearchPagerGeneralProtocolParameters.DateTime:
                    await WriteParameterAsync(parameter, DateTime.Parse(value), cancellationToken);
                    break;


                default:
                    throw new ArgumentException("Type not supported");
            }
        }

        public async Task WriteParameterAsync(PMSearchPagerGeneralProtocolParameters parameter, object value, CancellationToken cancellationToken)
        {
            switch (parameter)
            {
                case PMSearchPagerGeneralProtocolParameters.NGamma:
                    await WriteGammaCoefficientAsync((double)value, cancellationToken);
                    break;

                case PMSearchPagerGeneralProtocolParameters.DateTime:
                    await WriteDateTimeAsync(cancellationToken, (DateTime)value);
                    break;

                case PMSearchPagerGeneralProtocolParameters.HistoryRecordingInterval:
                    await WriteHistoryRecordIntervalAsync((ushort)value, cancellationToken);
                    break;

                case PMSearchPagerGeneralProtocolParameters.StatDsp:
                    await WriteStatDspAsync((ushort)value, cancellationToken);
                    break;

                case PMSearchPagerGeneralProtocolParameters.StatAlarm:
                    await WriteStatAlarmAsync((ushort)value, cancellationToken);
                    break;

                default:
                    throw new ArgumentException("Type not supported");
            }
        }

        public DeviceParameterInfo[] GetSupportedParameters() => SupportedParameters;

        #region Legacy date time functions and methods

        public DateTime LegacyReadDateTime()
        {
            return RawBytesConverter.Bytes3ToTime(ReadFromMC(213));
        }

        public async Task<DateTime> LegacyReadDateTimeAsync(CancellationToken cancellationToken)
        {
            return RawBytesConverter.Bytes3ToTime(await ReadFromMCAsync(213, cancellationToken));
        }

        public async Task LegacyWriteDateTimeAsync(CancellationToken cancellationToken, DateTime? value = null)
        {
            await WriteArrToMCAsync(213, RawBytesConverter.TimeTo3Bytes(value == null ? DateTime.Now : (DateTime)value), cancellationToken);
        }

        public void LegacyWriteDateTime(DateTime? value = null)
        {
            WriteArrToMC(213, RawBytesConverter.TimeTo3Bytes(value == null ? DateTime.Now : (DateTime)value));
        }

        #endregion

        #region Date time functions and methods

        public void WriteDateTime(DateTime? dt = null)
        {
            if (IsLegacyTime)
            { 
                LegacyWriteDateTime(dt);
                return;
            }
            DateTime n = (dt == null) ? DateTime.Now : (DateTime)dt;
            long num = (long)(n - new DateTime(1999, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            WriteArrToMC(213, new byte[4] { (byte)num, (byte)(num >> 8), (byte)(num >> 16), (byte)(num >> 24) });
        }

        public async Task WriteDateTimeAsync(CancellationToken cancellationToken, DateTime? dt = null)
        {
            if (IsLegacyTime)
            {
                await LegacyWriteDateTimeAsync(cancellationToken, dt);
                return;
            }
            DateTime n = (dt == null) ? DateTime.Now : (DateTime)dt;
            long num = (long)(n - new DateTime(1999, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            await WriteArrToMCAsync(213, new byte[4] { (byte)num, (byte)(num >> 8), (byte)(num >> 16), (byte)(num >> 24) }, cancellationToken);
        }

        public DateTime ReadDateTime()
        {
            if (IsLegacyTime) return LegacyReadDateTime();
            return RawBytesConverter.Bytes4ToTime(ReadFromMC(213), 4);
        }

        public async Task<DateTime> ReadDateTimeAsync(CancellationToken cancellationToken)
        {
            if (IsLegacyTime) return await LegacyReadDateTimeAsync(cancellationToken);
            return RawBytesConverter.Bytes4ToTime(await ReadFromMCAsync(213, cancellationToken), 4);
        }

        #endregion

        public int ReadSerialNumber()
        {
            return int.Parse(RawBytesConverter.GetStringValue(ReadFromMC(1)));
        }

        public async Task<int> ReadSerialNumberAsync(CancellationToken cancellationToken)
        {
            return int.Parse(RawBytesConverter.GetStringValue(await ReadFromMCAsync(1, cancellationToken)));
        }

        public double ReadGammaCoefficient()
        {
            return ReadUshortFromMC(144) / 10.0d;
        }

        public async Task<double> ReadGammaCoefficientAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromMCAsync(144, cancellationToken) / 10.0d;
        }

        public void WriteGammaCoefficient(double value)
        {
            WriteUshortToMC(144, (ushort)(value * 10.0d));
        }

        public async Task WriteGammaCoefficientAsync(double value, CancellationToken cancellationToken)
        {
            await WriteUshortToMCAsync(144, (ushort)(value * 10.0d), cancellationToken);
        }

        public ushort ReadHistoryRecordInterval()
        {
            return ReadUshortFromMC(209);
        }

        public async Task<ushort> ReadHistoryRecordIntervalAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromMCAsync(209, cancellationToken);
        }

        public void WriteHistoryRecordInterval(ushort value)
        {
            WriteUshortToMC(209, value);
        }

        public async Task WriteHistoryRecordIntervalAsync(ushort value, CancellationToken cancellationToken)
        {
            await WriteUshortToMCAsync(209, value, cancellationToken);
        }

        public double ReadBatteryVoltage()
        {
            byte[] array = ReadFromMC(207);
            return (double)((int)array[0] + (256 * array[1])) * BatteryReadoutCoefficient / 4096.0;
        }

        public async Task<double> ReadBatteryVoltageAsync(CancellationToken cancellationToken)
        {
            byte[] array = await ReadFromMCAsync(207, cancellationToken);
            return (double)((int)array[0] + (256 * array[1])) * BatteryReadoutCoefficient / 4096.0;
        }

        public double ReadBatteryThreshold()
        {
            byte[] array = ReadFromMC(210);
            return (double)((int)array[0] + (256 * array[1])) * BatteryReadoutCoefficient / 4096.0;
        }

        public async Task<double> ReadBatteryThresholdAsync(CancellationToken cancellationToken)
        {
            byte[] array = await ReadFromMCAsync(210, cancellationToken);
            return (double)((int)array[0] + (256 * array[1])) * BatteryReadoutCoefficient / 4096.0;
        }

        public double ReadBatteryPercentage()
        {
            double battVoltage = ReadBatteryVoltage();
            double thresholdVoltage = ReadBatteryThreshold();
            return (battVoltage - thresholdVoltage) / (1.5 - thresholdVoltage) * 100.0;
        }

        public async Task<double> ReadBatteryPercentageAsync(CancellationToken cancellationToken)
        {
            double battVoltage = await ReadBatteryVoltageAsync(cancellationToken);
            double thresholdVoltage = await ReadBatteryThresholdAsync(cancellationToken);
            return (battVoltage - thresholdVoltage) / (1.5 - thresholdVoltage) * 100.0;
        }

        protected ushort ReadStatDsp()
        {
            return ReadUshortFromMC(211);
        }

        protected async Task<ushort> ReadStatDspAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromMCAsync(211, cancellationToken);
        }

        protected void WriteStatDsp(ushort newValue)
        {
            WriteUshortToMC(211, newValue);
        }

        protected async Task WriteStatDspAsync(ushort newValue, CancellationToken cancellationToken)
        {
            await WriteUshortToMCAsync(211, newValue, cancellationToken);
        }

        protected ushort ReadStatAlarm()
        {
            return ReadUshortFromMC(212);
        }

        protected async Task<ushort> ReadStatAlarmAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromMCAsync(212, cancellationToken);
        }

        protected void WriteStatAlarm(ushort newValue)
        {
            WriteUshortToMC(212, newValue);
        }

        protected async Task WriteStatAlarmAsync(ushort newValue, CancellationToken cancellationToken)
        {
            await WriteUshortToMCAsync(212, newValue, cancellationToken);
        }


        public class PMSearchPagerDeviceParameterInfo : DeviceParameterInfo
        {
            public PMSearchPagerDeviceParameterInfo(PMSearchPagerGeneralProtocolParameters type, string name, bool changable, Type valueType, string? unit = null) : base((int)type, name, changable, valueType, unit)
            {
            }
             
            public override string ToString()
            {
                return string.Format("{0} - {1}", Id, Name);
            }
        }

        public enum PMSearchPagerGeneralProtocolParameters : int
        {
            SerialNumber = 0,
            NGamma = 1,
            //NNeutron = 2,
            BatteryLevel = 3,
            BatteryCutoffLevel = 4,
            BatteryPercentage = 5,
            DateTime = 6,
            HistoryRecordingInterval = 7,
            //AutoCalibration = 8,
            //AlarmModeEditing = 9,
            //AlarmCoefficientEditing = 10,
            //AudioAlarm = 11,
            //VibroAlarm = 12,
            //CalibrationValue = 13,
            //Virtual parameters, they dont actually exist
            StatDsp = 14, 
            StatAlarm = 15,
            //SafetyAlarm = 16,
        }
    }
}

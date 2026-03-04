using InTheHand.Net;
using InTheHand.Net.Sockets;
using PolimasterIrDADevicesManagerGUI.Utils;

namespace PolimasterIrDADevicesManagerGUI.Device.Protocols.Implementations
{
    // Status word - 5
    public sealed class PM1603ProtocolDevice : PMDeviceBaseProtocol, ISettingsAccessDevice
    {

        public readonly PM1603DeviceParameterInfo[] SupportedParameters = new PM1603DeviceParameterInfo[]
        {
            //                            Parameter                Name                          Edit   Value type
            new(PM1603DeviceParameterType.ManufactureDate,         "Manufacture date",           false, typeof(DateTime)),
            new(PM1603DeviceParameterType.SerialNumber,            "Serial number",              false, typeof(int)),
            new(PM1603DeviceParameterType.DoseRateThreshold1,      "Dose rate threshold 1",      true,  typeof(double)),
            new(PM1603DeviceParameterType.DoseRateThreshold2,      "Dose rate threshold 2",      true,  typeof(double)),
            new(PM1603DeviceParameterType.DoseThreshold1,          "Dose threshold 1",           true,  typeof(double)),
            new(PM1603DeviceParameterType.DoseThreshold2,          "Dose threshold 2",           true,  typeof(double)),
            new(PM1603DeviceParameterType.DateTime,                "Date time",                  true,  typeof(DateTime)),
            new(PM1603DeviceParameterType.BatteryVoltage,          "Battery voltage",            false, typeof(ushort)),
            new(PM1603DeviceParameterType.Rate,                    "Current doserate",           false, typeof(double)),
            new(PM1603DeviceParameterType.Dose,                    "Current dose",               false, typeof(double)),
            new(PM1603DeviceParameterType.Units,                   "Alternative units",          true,  typeof(bool)),
            new(PM1603DeviceParameterType.HistoryType,             "Cyclic history recording",   true,  typeof(bool)),
            new(PM1603DeviceParameterType.HistoryStep,             "History recording step",     true,  typeof(ushort)),
            new(PM1603DeviceParameterType.HistoryWriteDose,        "History write dose",         true,  typeof(bool)),
            new(PM1603DeviceParameterType.HistoryWriteRate,        "History write rate",         true,  typeof(bool)),
            new(PM1603DeviceParameterType.SoundRateAlarm,          "Rate alarm (sound)",         true,  typeof(bool)),
            new(PM1603DeviceParameterType.SoundDoseAlarm,          "Dose alarm (sound)",         true,  typeof(bool)),
            new(PM1603DeviceParameterType.EnableResetDose,         "Dose reset enabled",         true,  typeof(bool)),
            new(PM1603DeviceParameterType.DERIndication,           "Show DER",                   true,  typeof(bool)),
            new(PM1603DeviceParameterType.DEIndication,            "Show DE",                    true,  typeof(bool)),
            new(PM1603DeviceParameterType.SNIndication,            "Show serial number",         true,  typeof(bool)),
            new(PM1603DeviceParameterType.AlarmClockIndication,    "Show Alarm clock",           true,  typeof(bool)),
            new(PM1603DeviceParameterType.TimerIndication,         "Show time",                  true,  typeof(bool)),
            new(PM1603DeviceParameterType.StopwatchIndication,     "Show stopwatch",             true,  typeof(bool)),
            new(PM1603DeviceParameterType.CalendarIndication,      "Show Calendar",              true,  typeof(bool)),
            new(PM1603DeviceParameterType.DERIndicationValue,      "DER indication value",       true,  typeof(bool)),
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

        public PM1603ProtocolDevice(IrDAClient irdaClient, IrDAEndPoint endpoint) : base(irdaClient, endpoint)
        {
        }

        public async Task WriteParameterAsync(int parameter, object value, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task WriteParameterAsStringAsync(int parameter, string value, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<object> ReadParameterAsync(int parameter, CancellationToken cancellationToken)
        {
            switch ((PM1603DeviceParameterType)parameter)
            {

                case PM1603DeviceParameterType.SerialNumber:
                    return await ReadSerialNumberAsync(cancellationToken);

                case PM1603DeviceParameterType.ManufactureDate:
                    return await ReadManufactureDateTimeAsync(cancellationToken);

                case PM1603DeviceParameterType.DoseRateThreshold1:
                    return await ReadDoseRateThreshold1Async(cancellationToken);

                case PM1603DeviceParameterType.DoseRateThreshold2:
                    return await ReadDoseRateThreshold2Async(cancellationToken);

                case PM1603DeviceParameterType.DoseThreshold1:
                    return await ReadDoseThreshold1Async(cancellationToken);

                case PM1603DeviceParameterType.DoseThreshold2:
                    return await ReadDoseThreshold2Async(cancellationToken);

                case PM1603DeviceParameterType.DateTime:
                    return await ReadDateTimeAsync(cancellationToken);

                case PM1603DeviceParameterType.BatteryVoltage:
                    return await ReadBatteryVoltageAsync(cancellationToken);

                case PM1603DeviceParameterType.Rate:
                    return await ReadRateAsync(cancellationToken);

                case PM1603DeviceParameterType.Dose:
                    return await ReadDoseAsync(cancellationToken);

                case PM1603DeviceParameterType.Units:
                    return await ReadAlternativeUnitsEnabledAsync(cancellationToken);

                case PM1603DeviceParameterType.HistoryType:
                    return await ReadHistoryTypeEnabledAsync(cancellationToken);

                case PM1603DeviceParameterType.HistoryStep:
                    return await ReadHistoryStepAsync(cancellationToken);

                case PM1603DeviceParameterType.HistoryWriteDose:
                    return await ReadHistoryWriteDoseAsync(cancellationToken);

                case PM1603DeviceParameterType.HistoryWriteRate:
                    return await ReadHistoryWriteRateAsync(cancellationToken);

                case PM1603DeviceParameterType.SoundRateAlarm:
                    return await ReadSoundRateAlarmEnabled(cancellationToken);

                case PM1603DeviceParameterType.SoundDoseAlarm:
                    return await ReadSoundDoseAlarmEnabled(cancellationToken);

                case PM1603DeviceParameterType.EnableResetDose:
                    return await ReadResetDoseEnabled(cancellationToken);

                case PM1603DeviceParameterType.DERIndication:
                    return await ReadDERIndicationEnabled(cancellationToken);

                case PM1603DeviceParameterType.DEIndication:
                    return await ReadDEIndicationEnabled(cancellationToken);

                case PM1603DeviceParameterType.SNIndication:
                    return await ReadSNIndicationEnabled(cancellationToken);

                case PM1603DeviceParameterType.AlarmClockIndication:
                    return await ReadAlarmClockIndicationEnabled(cancellationToken);

                case PM1603DeviceParameterType.TimerIndication:
                    return await ReadTimerIndicationEnabled(cancellationToken);

                case PM1603DeviceParameterType.StopwatchIndication:
                    return await ReadStopWatchIndicationEnabled(cancellationToken);

                case PM1603DeviceParameterType.CalendarIndication:
                    return await ReadCalendarIndicationEnabled(cancellationToken);

                case PM1603DeviceParameterType.DERIndicationValue:
                    return await ReadDERValueIndicationEnabled(cancellationToken);

                case PM1603DeviceParameterType.DERIndicationThreshold1:
                    return await ReadDERThreshold1IndicationEnabled(cancellationToken);

                case PM1603DeviceParameterType.DERIndicationThreshold2:
                    return await ReadDERThreshold2IndicationEnabled(cancellationToken);

                case PM1603DeviceParameterType.DERIndicationStatus:
                    return await ReadDERStatIndicationEnabled(cancellationToken);

                case PM1603DeviceParameterType.DERIndicationTime:
                    return await ReadDERTimeIndicationEnabled(cancellationToken);

                case PM1603DeviceParameterType.DERIndicationOff:
                    return await ReadDERIndicationOff(cancellationToken);

                case PM1603DeviceParameterType.DEIndicationValue:
                    return await ReadDEValueIndication(cancellationToken);

                case PM1603DeviceParameterType.DEIndicationThreshold1:
                    return await ReadDEThreshold1Indication(cancellationToken);

                case PM1603DeviceParameterType.DEIndicationThreshold2:
                    return await ReadDEThreshold2Indication(cancellationToken);

                case PM1603DeviceParameterType.DEIndicationReset:
                    return await ReadDEResetIndication(cancellationToken);

                case PM1603DeviceParameterType.DEIndicationAccTime:
                    return await ReadDEAccTimeIndication(cancellationToken);

                case PM1603DeviceParameterType.DEIndicationTime:
                    return await ReadDETimeIndication(cancellationToken);

                case PM1603DeviceParameterType.DEIndicationOff:
                    return await ReadDEIndicationOff(cancellationToken);
                
                default:
                    throw new Exception();
            }
        }

        public DeviceParameterInfo[] GetSupportedParameters()
        {
            return SupportedParameters;
        }

        public DeviceParameterInfo FindParameterByType(int type) => SupportedParameters.First(x => x.Id == type);

        #region Serial number

        public async Task<int> ReadSerialNumberAsync(CancellationToken cancellationToken)
        {
            byte[] array = await ReadFromMCAsync(1, cancellationToken);
            return RawBytesConverter.Bin2Dec(array[1]) + RawBytesConverter.Bin2Dec(array[0]) * 100 + RawBytesConverter.Bin2Dec(array[3]) * 10000;
        }

        #endregion

        #region Manufacture date

        public async Task<DateTime> ReadManufactureDateTimeAsync(CancellationToken cancellationToken)
        {
            /**
             * I believe that this is only supported for newer devices
             **/
            byte[] array = await ReadFromMCAsync(1, cancellationToken);
            int year = RawBytesConverter.Bin2Dec(array[2]) + 2000;
            int num = RawBytesConverter.Bin2Dec(array[4]);
            if (num < 1 || num > 12)
            {
                return DateTime.Now;
            }
            return new DateTime(year, num, 1);
        }

        #endregion

        #region Dose rate threshold 1

        public async Task<double> ReadDoseRateThreshold1Async(CancellationToken cancellationToken)
        {
            return RawBytesConverter.ReadDouble(await ReadFromMCAsync(33, cancellationToken)) * 100000.0d;
        }
        public async Task WriteDoseRateThreshold1(double value, CancellationToken cancellationToken)
        {
            await WriteArrToMCAsync(33, RawBytesConverter.GetDouble(value), cancellationToken);
        }

        #endregion

        #region Dose rate threshold 2

        public async Task<double> ReadDoseRateThreshold2Async(CancellationToken cancellationToken)
        {
            return RawBytesConverter.ReadDouble(await ReadFromMCAsync(34, cancellationToken)) * 100000.0d;
        }

        public async Task WriteDoseRateThreshold2(double value, CancellationToken cancellationToken)
        {
            await WriteArrToMCAsync(34, RawBytesConverter.GetDouble(value), cancellationToken);
        }

        #endregion

        #region Dose threshold 1

        public async Task<double> ReadDoseThreshold1Async(CancellationToken cancellationToken)
        {
            return RawBytesConverter.ReadDouble(await ReadFromMCAsync(50, cancellationToken)) * 100000.0d;
        }

        public async Task WriteDoseThreshold1Async(double value, CancellationToken cancellationToken)
        {
            await WriteArrToMCAsync(50, RawBytesConverter.GetDouble(value), cancellationToken);
        }

        #endregion

        #region Dose threshold 2

        public async Task<double> ReadDoseThreshold2Async(CancellationToken cancellationToken)
        {
            return RawBytesConverter.ReadDouble(await ReadFromMCAsync(51, cancellationToken)) * 100000.0d;
        }

        public async Task WriteDoseThreshold2Async(double value, CancellationToken cancellationToken)
        {
            await WriteArrToMCAsync(51, RawBytesConverter.GetDouble(value), cancellationToken);
        }

        #endregion

        #region Date time

        public async Task<DateTime> ReadDateTimeAsync(CancellationToken cancellationToken)
        {
            byte[] read = await ReadFromMCAsync(17, cancellationToken);
            long date = ((read[3] * 256 + read[2]) * 256 + read[1]) * 256 + read[0];
            return new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)date);//.ToLocalTime();
        }

        public async Task WriteDateTimeAsync(DateTime value, CancellationToken cancellationToken)
        {
            throw new Exception();
            await WriteArrToMCAsync(17, RawBytesConverter.TimeTo4Bytes(value), cancellationToken);
        }

        #endregion

        #region Voltage

        public async Task<ushort> ReadBatteryVoltageAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromMCAsync(2, cancellationToken);
        }

        #endregion

        #region Rate

        public async Task<double> ReadRateAsync(CancellationToken cancellationToken)
        {
            return RawBytesConverter.ReadDouble(await ReadFromMCAsync(163, cancellationToken)) * 100000.0d;
        }

        #endregion

        #region Dose

        public async Task<double> ReadDoseAsync(CancellationToken cancellationToken)
        {
            return RawBytesConverter.ReadDouble(await ReadFromMCAsync(55, cancellationToken)) * 100000.0d;
        }

        #endregion

        private async Task<bool> ReadFlagAsync(byte address, byte flag, CancellationToken cancellationToken)
        {
            return ByteHelper.GetBit(await ReadFromMCAsync(5, cancellationToken), flag);
        }

        private async Task WriteFlagAsync(byte address, byte flag, bool value, CancellationToken cancellationToken)
        {
            await Write2ToMCAsync(address, ByteHelper.SetBit(await ReadFromMCAsync(address, cancellationToken), flag, value), cancellationToken);
        }

        #region Units (huh?)

        public async Task<bool> ReadAlternativeUnitsEnabledAsync(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(5, 7, cancellationToken);
        }

        public async Task WriteAlternativeUnitsEnabledAsync(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(5, 7, value, cancellationToken);
        }

        #endregion

        #region History type (huh?)

        public async Task<bool> ReadHistoryTypeEnabledAsync(CancellationToken cancellationToken)
        {
            return !(await ReadFlagAsync(5, 10, cancellationToken));
        }

        public async Task WriteHistoryTypeEnabledAsync(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(5, 10, !value, cancellationToken);
        }

        #endregion

        #region History recording step

        public async Task<ushort> ReadHistoryStepAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromMCAsync(152, cancellationToken);
        }

        public async Task WriteHistoryStepAsync(ushort value, CancellationToken cancellationToken)
        {
            await WriteUshortToMCAsync(152, value, cancellationToken);
        }

        #endregion

        #region History write dose flag

        public async Task<bool> ReadHistoryWriteDoseAsync(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(5, 9, cancellationToken);
        }

        public async Task WriteHistoryWriteDoseAsync(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(5, 9, value, cancellationToken);
        }

        #endregion

        #region History write rate flag

        public async Task<bool> ReadHistoryWriteRateAsync(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(5, 8, cancellationToken);
        }

        public async Task WriteHistoryWriteRateAsync(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(5, 8, value, cancellationToken);
        }

        #endregion

        #region Sound rate alarm

        public async Task<bool> ReadSoundRateAlarmEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(5, 0, cancellationToken);
        }

        public async Task WriteSoundRateAlarmEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(5, 0, value, cancellationToken);
        }

        #endregion

        #region Sound dose alarm

        public async Task<bool> ReadSoundDoseAlarmEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(5, 1, cancellationToken);
        }

        public async Task WriteSoundDoseAlarmEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(5, 1, value, cancellationToken);
        }

        #endregion

        #region Dose reset

        public async Task<bool> ReadResetDoseEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(5, 3, cancellationToken);
        }

        public async Task WriteResetDoseEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(5, 3, value, cancellationToken);
        }

        #endregion

        #region DER Indication

        public async Task<bool> ReadDERIndicationEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(151, 0, cancellationToken);
        }

        public async Task WriteDERIndicationEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(151, 0, value, cancellationToken);
        }

        #endregion

        #region DE Indication

        public async Task<bool> ReadDEIndicationEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(151, 1, cancellationToken);
        }

        public async Task WriteDEIndicationEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(151, 1, value, cancellationToken);
        }

        #endregion

        #region SN Indication

        public async Task<bool> ReadSNIndicationEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(151, 2, cancellationToken);
        }

        public async Task WriteSNIndicationEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(151, 2, value, cancellationToken);
        }

        #endregion

        #region Alarm clock Indication

        public async Task<bool> ReadAlarmClockIndicationEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(151, 4, cancellationToken);
        }

        public async Task WriteAlarmClockIndicationEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(151, 4, value, cancellationToken);
        }

        #endregion

        #region Timer Indication

        public async Task<bool> ReadTimerIndicationEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(151, 5, cancellationToken);
        }

        public async Task WriteTimerIndicationEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(151, 5, value, cancellationToken);
        }

        #endregion


        #region Stop watch Indication

        public async Task<bool> ReadStopWatchIndicationEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(151, 6, cancellationToken);
        }

        public async Task WriteStopWatchIndicationEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(151, 6, value, cancellationToken);
        }

        #endregion


        #region Calendar Indication

        public async Task<bool> ReadCalendarIndicationEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(151, 7, cancellationToken);
        }

        public async Task WriteCalendarIndicationEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(151, 7, value, cancellationToken);
        }

        #endregion

        #region DER Value indication

        public async Task<bool> ReadDERValueIndicationEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(153, 0, cancellationToken);
        }

        public async Task WriteDERValueIndicaitonEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(153, 0, value, cancellationToken);
        }

        #endregion


        #region DER Threshold 1 indication

        public async Task<bool> ReadDERThreshold1IndicationEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(153, 1, cancellationToken);
        }

        public async Task WriteDERThreshold1IndicaitonEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(153, 1, value, cancellationToken);
        }

        #endregion

        #region DER Threshold 2 indication

        public async Task<bool> ReadDERThreshold2IndicationEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(153, 2, cancellationToken);
        }

        public async Task WriteDERThreshold2IndicaitonEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(153, 2, value, cancellationToken);
        }

        #endregion

        #region DER Stat indication

        public async Task<bool> ReadDERStatIndicationEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(168, 0, cancellationToken);
        }

        public async Task WriteDERStatIndicaitonEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(168, 0, value, cancellationToken);
        }

        #endregion

        #region DER Time indication

        public async Task<bool> ReadDERTimeIndicationEnabled(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(168, 2, cancellationToken);
        }

        public async Task WriteDERTimeIndicaitonEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(168, 2, value, cancellationToken);
        }

        #endregion


        #region DER indication off

        public async Task<bool> ReadDERIndicationOff(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(168, 1, cancellationToken);
        }

        public async Task WriteDERIndicaitonOff(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(168, 1, value, cancellationToken);
        }

        #endregion

        #region DE Value indication

        public async Task<bool> ReadDEValueIndication(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(167, 0, cancellationToken);
        }

        public async Task WriteDEValueIndication(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(167, 0, value, cancellationToken);
        }

        #endregion

        #region DE Threshold 1 indication

        public async Task<bool> ReadDEThreshold1Indication(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(167, 1, cancellationToken);
        }

        public async Task WriteDEThreshold1Indication(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(167, 1, value, cancellationToken);
        }

        #endregion


        #region DE Threshold 2 indication

        public async Task<bool> ReadDEThreshold2Indication(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(167, 2, cancellationToken);
        }

        public async Task WriteDEThreshold2Indication(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(167, 2, value, cancellationToken);
        }

        #endregion


        #region DE reset indication

        public async Task<bool> ReadDEResetIndication(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(167, 3, cancellationToken);
        }

        public async Task WriteDEResetIndication(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(167, 3, value, cancellationToken);
        }

        #endregion

        #region DE acc time indication

        public async Task<bool> ReadDEAccTimeIndication(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(169, 0, cancellationToken);
        }

        public async Task WriteDEAccTimeIndication(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(169, 0, value, cancellationToken);
        }

        #endregion

        #region DE time indication

        public async Task<bool> ReadDETimeIndication(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(169, 3, cancellationToken);
        }

        public async Task WriteDETimeIndication(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(169, 3, value, cancellationToken);
        }

        #endregion

        #region DE indication off

        public async Task<bool> ReadDEIndicationOff(CancellationToken cancellationToken)
        {
            return await ReadFlagAsync(169, 2, cancellationToken);
        }

        public async Task WriteDEIndicationOff(bool value, CancellationToken cancellationToken)
        {
            await WriteFlagAsync(169, 2, value, cancellationToken);
        }

        #endregion

    }

    public class PM1603DeviceParameterInfo : DeviceParameterInfo
    {
        public PM1603DeviceParameterInfo(PM1603DeviceParameterType type, string name, bool changable, Type valueType, string? unit = null) : base((int)type, name, changable, valueType, unit)
        {
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
        ManufactureDate = 37,
    }
}

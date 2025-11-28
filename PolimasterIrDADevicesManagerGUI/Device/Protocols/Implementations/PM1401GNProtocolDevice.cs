using InTheHand.Net;
using InTheHand.Net.Sockets;
using System.Text;
using PolimasterIrDADevicesManagerGUI.Utils;

namespace PolimasterIrDADevicesManagerGUI.Device.Protocols.Implementations
{
    /// <summary>
    /// Implementation of the PM1401GN (ИСП-РМ1401К-01) IrDA protocol 
    /// Read more at https://github.com/TiTiKy441/Polimaster-PM1401GN-Protocol-overview
    /// </summary>
    public sealed class PM1401GNProtocolDevice : EEPROMAccessProtocolDevice, ISettingsAccessDevice, IHistoryAccessDevice, IDeviceWithCalibrationValue
    {

        public bool Connected { get { return IrDAClient.Connected; } }

        public const double BatteryReadoutCoefficient = 0.838d;

        public static readonly PM1401GNDeviceParameterInfo[] SupportedParameters = new PM1401GNDeviceParameterInfo[]
        {
            //                              Parameter                 Name                          Edit   Value type         Unit
            new(PM1401GNDeviceParameterType.SerialNumber,             "Serial number",              false, typeof(string)),
            new(PM1401GNDeviceParameterType.NGamma,                   "N Gamma",                    true,  typeof(double)),
            new(PM1401GNDeviceParameterType.NNeutron,                 "N Neutron",                  true,  typeof(double)),
            new(PM1401GNDeviceParameterType.BatteryLevel,             "Battery level",              false, typeof(int),       "V"),
            new(PM1401GNDeviceParameterType.BatteryCutoffLevel,       "Battery cutoff level",       true,  typeof(int),       "V"),
            new(PM1401GNDeviceParameterType.BatteryPercentage,        "Battery percentage",         false, typeof(double),    "%"),
            new(PM1401GNDeviceParameterType.DateTime,                 "Device time",                true,  typeof(DateTime)),
            new(PM1401GNDeviceParameterType.HistoryRecordingInterval, "History recording interval", true,  typeof(int),       "min"),
            new(PM1401GNDeviceParameterType.AutoCalibration,          "Auto calibration",           true,  typeof(bool)),
            new(PM1401GNDeviceParameterType.AlarmModeEditing,         "Alarm mode editing",         true,  typeof(bool)),
            new(PM1401GNDeviceParameterType.AlarmCoefficientEditing,  "Alarm coefficient editing",  true,  typeof(bool)),
            new(PM1401GNDeviceParameterType.AudioAlarm,               "Audio alarm",                true,  typeof(bool)),
            new(PM1401GNDeviceParameterType.VibroAlarm,               "Vibro alarm",                true,  typeof(bool)),
            new(PM1401GNDeviceParameterType.CalibrationValue,         "Calibration value",          true,  typeof(double),    "cps/uSv/H"),
            new(PM1401GNDeviceParameterType.SafetyAlarm,              "Safety Alarm",               true,  typeof(int),       "mR/H"),
            new(PM1401GNDeviceParameterType.OverloadThreshold,        "Overload threshold",         true,  typeof(ushort),    "CPS"),
        };

        public PM1401GNProtocolDevice(IrDAClient client, IrDAEndPoint deviceEndPoint) : base(client, deviceEndPoint)
        {
            SetupCommunicationCommands();
        }

        public DeviceParameterInfo FindParameterByType(int type) => SupportedParameters.First(x => x.Id == type);


        public void WriteParameter(int parameter, object value) => WriteParameter((PM1401GNDeviceParameterType)parameter, value);

        public async Task WriteParameterAsync(int parameter, object value, CancellationToken cancellationToken) => await WriteParameterAsync((PM1401GNDeviceParameterType)parameter, value, cancellationToken);

        public void WriteParameterAsString(int parameter, string value) => WriteParameterAsString((PM1401GNDeviceParameterType)parameter, value);

        public async Task WriteParameterAsStringAsync(int parameter, string value, CancellationToken cancellationToken) => await WriteParameterAsStringAsync((PM1401GNDeviceParameterType)parameter, value, cancellationToken);

        public object ReadParameter(int parameter) => ReadParameter((PM1401GNDeviceParameterType)parameter);

        public async Task<object> ReadParameterAsync(int parameter, CancellationToken cancellationToken) => await ReadParameterAsync((PM1401GNDeviceParameterType)parameter, cancellationToken);

        public object ReadParameter(PM1401GNDeviceParameterType parameter)
        {
            switch (parameter)
            {
                case PM1401GNDeviceParameterType.SerialNumber:
                    return ReadSerialNumber();
                case PM1401GNDeviceParameterType.NGamma:
                    return ReadGammaCoefficient();
                case PM1401GNDeviceParameterType.NNeutron:
                    return ReadNeutronCoefficient();
                case PM1401GNDeviceParameterType.BatteryLevel:
                    return ReadBatteryVoltage();
                case PM1401GNDeviceParameterType.BatteryCutoffLevel:
                    return ReadBatteryThreshold();
                case PM1401GNDeviceParameterType.BatteryPercentage:
                    return ReadBatteryPercentage();
                case PM1401GNDeviceParameterType.DateTime:
                    return ReadDateTime();
                case PM1401GNDeviceParameterType.HistoryRecordingInterval:
                    return ReadHistoryRecordInterval();
                case PM1401GNDeviceParameterType.AutoCalibration:
                    return ReadAutoCalibrationEnabled();
                case PM1401GNDeviceParameterType.AlarmModeEditing:
                    return ReadAlarmChangeEnabled();
                case PM1401GNDeviceParameterType.AlarmCoefficientEditing:
                    return ReadCoefficientChangeEnabled();
                case PM1401GNDeviceParameterType.AudioAlarm:
                    return ReadAudioAlarmEnabled();
                case PM1401GNDeviceParameterType.VibroAlarm:
                    return ReadVibroAlarmEnabled();
                case PM1401GNDeviceParameterType.CalibrationValue:
                    return ReadCalibrationValue();
                case PM1401GNDeviceParameterType.SafetyAlarm:
                    return ReadSafetyAlarm();
                case PM1401GNDeviceParameterType.OverloadThreshold:
                    return ReadOverloadThreshold();
                default:
                    throw new ArgumentException("Type not supported");
            }
        }
        
        public async Task<object> ReadParameterAsync(PM1401GNDeviceParameterType parameter, CancellationToken cancellationToken)
        {
            switch (parameter)
            {
                case PM1401GNDeviceParameterType.SerialNumber:
                    return await ReadSerialNumberAsync(cancellationToken);
                case PM1401GNDeviceParameterType.NGamma:
                    return await ReadGammaCoefficientAsync(cancellationToken);
                case PM1401GNDeviceParameterType.NNeutron:
                    return await ReadNeutronCoefficientAsync(cancellationToken);
                case PM1401GNDeviceParameterType.BatteryLevel:
                    return await ReadBatteryVoltageAsync(cancellationToken);
                case PM1401GNDeviceParameterType.BatteryCutoffLevel:
                    return await ReadBatteryThresholdAsync(cancellationToken);
                case PM1401GNDeviceParameterType.BatteryPercentage:
                    return await ReadBatteryPercentageAsync(cancellationToken);
                case PM1401GNDeviceParameterType.DateTime:
                    return await ReadDateTimeAsync(cancellationToken);
                case PM1401GNDeviceParameterType.HistoryRecordingInterval:
                    return await ReadHistoryRecordIntervalAsync(cancellationToken);
                case PM1401GNDeviceParameterType.AutoCalibration:
                    return await ReadAutoCalibrationEnabledAsync(cancellationToken);
                case PM1401GNDeviceParameterType.AlarmModeEditing:
                    return await ReadAlarmChangeEnabledAsync(cancellationToken);
                case PM1401GNDeviceParameterType.AlarmCoefficientEditing:
                    return await ReadCoefficientChangeEnabledAsync(cancellationToken);
                case PM1401GNDeviceParameterType.AudioAlarm:
                    return await ReadAudioAlarmEnabledAsync(cancellationToken);
                case PM1401GNDeviceParameterType.VibroAlarm:
                    return await ReadVibroAlarmEnabledAsync(cancellationToken);
                case PM1401GNDeviceParameterType.CalibrationValue:
                    return await ReadCalibrationValueAsync(cancellationToken);
                case PM1401GNDeviceParameterType.SafetyAlarm:
                    return await ReadSafetyAlarmAsync(cancellationToken);
                case PM1401GNDeviceParameterType.OverloadThreshold:
                    return await ReadOverloadThresholdAsync(cancellationToken);
                default:
                    throw new ArgumentException("Type not supported");
            }
        }

        public void WriteParameterAsString(PM1401GNDeviceParameterType parameter, string value)
        {
            switch (parameter)
            {
                case PM1401GNDeviceParameterType.CalibrationValue:
                case PM1401GNDeviceParameterType.NGamma:
                case PM1401GNDeviceParameterType.NNeutron:
                case PM1401GNDeviceParameterType.BatteryCutoffLevel:
                    WriteParameter(parameter, double.Parse(value));
                    break;

                case PM1401GNDeviceParameterType.DateTime:
                    WriteParameter(parameter, DateTime.Parse(value));
                    break;

                case PM1401GNDeviceParameterType.AlarmModeEditing:
                case PM1401GNDeviceParameterType.AlarmCoefficientEditing:
                case PM1401GNDeviceParameterType.AudioAlarm:
                case PM1401GNDeviceParameterType.VibroAlarm:
                case PM1401GNDeviceParameterType.AutoCalibration:
                    WriteParameter(parameter, value.ToString().Trim() == "1");
                    break;

                case PM1401GNDeviceParameterType.OverloadThreshold:
                case PM1401GNDeviceParameterType.HistoryRecordingInterval:
                    WriteParameter(parameter, ushort.Parse(value));
                    break;

                case PM1401GNDeviceParameterType.SafetyAlarm:
                    WriteParameter(parameter, byte.Parse(value));
                    break;

                default:
                    throw new ArgumentException("Type not supported");
            }
        }

        
        public void WriteParameter(PM1401GNDeviceParameterType parameter, object value)
        {
            switch (parameter)
            {
                case PM1401GNDeviceParameterType.NGamma:
                    WriteGammaCoefficient((double)value);
                    break;
                case PM1401GNDeviceParameterType.NNeutron:
                    WriteNeutronCoefficient((double)value);
                    break;
                case PM1401GNDeviceParameterType.BatteryCutoffLevel:
                    WriteBatteryThreshold((double)value);
                    break;
                case PM1401GNDeviceParameterType.DateTime:
                    WriteDateTime((DateTime)value);
                    break;
                case PM1401GNDeviceParameterType.HistoryRecordingInterval:
                    WriteHistoryRecordInterval((ushort)value);
                    break;
                case PM1401GNDeviceParameterType.AutoCalibration:
                    WriteAutoCalibrationEnabled((bool)value);
                    break;
                case PM1401GNDeviceParameterType.AlarmModeEditing:
                    WriteAlarmChangeEnabled((bool)value);
                    break;
                case PM1401GNDeviceParameterType.AlarmCoefficientEditing:
                    WriteCoefficientChangeEnabled((bool)value);
                    break;
                case PM1401GNDeviceParameterType.AudioAlarm:
                    WriteAudioAlarmEnabled((bool)value);
                    break;
                case PM1401GNDeviceParameterType.VibroAlarm:
                    WriteVibroAlarmEnabled((bool)value);
                    break;
                case PM1401GNDeviceParameterType.CalibrationValue:
                    WriteCalibrationValue((double)value);
                    break;
                case PM1401GNDeviceParameterType.SafetyAlarm:
                    WriteSafetyAlarm((byte)value);
                    break;
                case PM1401GNDeviceParameterType.OverloadThreshold:
                    WriteOverloadThreshold((ushort)value);
                    break;
                default:
                    throw new ArgumentException("Type not supported");
            }
        }

        public async Task WriteParameterAsStringAsync(PM1401GNDeviceParameterType parameter, string value, CancellationToken cancellationToken)
        {
            switch (parameter)
            {
                case PM1401GNDeviceParameterType.CalibrationValue:
                case PM1401GNDeviceParameterType.NGamma:
                case PM1401GNDeviceParameterType.NNeutron:
                case PM1401GNDeviceParameterType.BatteryCutoffLevel:
                    await WriteParameterAsync(parameter, double.Parse(value), cancellationToken);
                    break;

                case PM1401GNDeviceParameterType.DateTime:
                    await WriteParameterAsync(parameter, DateTime.Parse(value), cancellationToken);
                    break;

                case PM1401GNDeviceParameterType.AlarmModeEditing:
                case PM1401GNDeviceParameterType.AlarmCoefficientEditing:
                case PM1401GNDeviceParameterType.AudioAlarm:
                case PM1401GNDeviceParameterType.VibroAlarm:
                case PM1401GNDeviceParameterType.AutoCalibration:
                    await WriteParameterAsync(parameter, value.ToString().Trim() == "1", cancellationToken);
                    break;

                case PM1401GNDeviceParameterType.OverloadThreshold:
                case PM1401GNDeviceParameterType.HistoryRecordingInterval:
                    await WriteParameterAsync(parameter, ushort.Parse(value), cancellationToken);
                    break;

                case PM1401GNDeviceParameterType.SafetyAlarm:
                    await WriteParameterAsync(parameter, byte.Parse(value), cancellationToken);
                    break;

                default:
                    throw new ArgumentException("Type not supported");
            }
        }

        public async Task WriteParameterAsync(PM1401GNDeviceParameterType parameter, object value, CancellationToken cancellationToken)
        {
            switch (parameter)
            {
                case PM1401GNDeviceParameterType.NGamma:
                    await WriteGammaCoefficientAsync((double)value, cancellationToken);
                    break;
                case PM1401GNDeviceParameterType.NNeutron:
                    await WriteNeutronCoefficientAsync((double)value, cancellationToken);
                    break;
                case PM1401GNDeviceParameterType.BatteryCutoffLevel:
                    await WriteBatteryThresholdAsync((double)value, cancellationToken);
                    break;
                case PM1401GNDeviceParameterType.DateTime:
                    await WriteDateTimeAsync(cancellationToken, (DateTime)value);
                    break;
                case PM1401GNDeviceParameterType.HistoryRecordingInterval:
                    await WriteHistoryRecordIntervalAsync((ushort)value, cancellationToken);
                    break;
                case PM1401GNDeviceParameterType.AutoCalibration:
                    await WriteAutoCalibrationEnabledAsync((bool)value, cancellationToken);
                    break;
                case PM1401GNDeviceParameterType.AlarmModeEditing:
                    await WriteAlarmChangeEnabledAsync((bool)value, cancellationToken);
                    break;
                case PM1401GNDeviceParameterType.AlarmCoefficientEditing:
                    await WriteCoefficientChangeEnabledAsync((bool)value, cancellationToken);
                    break;
                case PM1401GNDeviceParameterType.AudioAlarm:
                    await WriteAudioAlarmEnabledAsync((bool)value, cancellationToken);
                    break;
                case PM1401GNDeviceParameterType.VibroAlarm:
                    await WriteVibroAlarmEnabledAsync((bool)value, cancellationToken);
                    break;
                case PM1401GNDeviceParameterType.CalibrationValue:
                    await WriteCalibrationValueAsync((double)value, cancellationToken);
                    break;
                case PM1401GNDeviceParameterType.SafetyAlarm:
                    await WriteSafetyAlarmAsync((byte)value, cancellationToken);
                    break;
                case PM1401GNDeviceParameterType.OverloadThreshold:
                    await WriteOverloadThresholdAsync((ushort)value, cancellationToken);
                    break;
                default:
                    throw new ArgumentException("Type not supported");
            }
        }

        /// <summary>
        /// Returns parameters supported by the device
        /// </summary>
        /// <returns>Array of parameters supported by the device</returns>
        public DeviceParameterInfo[] GetSupportedParameters() => SupportedParameters;

        private void SetupCommunicationCommands()
        {
            CommunicationCommands["GetHistory"] =     new byte[] { 131, 0, 5, 177, 64 }; //ReadFromMC(64);
            CommunicationCommands["GetHistoryNext"] = new byte[] { 131, 0, 3 };
            CommunicationCommands["DelHistory"] =     new byte[] { 130, 0, 5, 177, 66 };
            CommunicationCommands["OkHistory"] =      new byte[] { 144, 0, 54, 114, 0 };

            CommunicationCommands["Ok"] =             new byte[] { 160, 0, 5, 178 };
            CommunicationCommands["Ok1"] =            new byte[] { 160, 0, 12, 114, 0, 9 };
            CommunicationCommands["Ok2"] =            new byte[] { 160, 0, 8, 242 };
            CommunicationCommands["Ok3"] =            new byte[] { 160, 0, 3 };
            CommunicationCommands["Ok4"] =            new byte[] { 160, 0, 8, 114, 0, 5 };
            CommunicationCommands["Ok5"] =            new byte[] { 160, 0, 8, 114, 0 };
        }

        private static string GetStringValue(byte[] b)
        {
            return Encoding.ASCII.GetString(b, 0, 6);
        }

        #region Overload threshold

        public ushort ReadOverloadThreshold()
        {
            return ReadUshortFromEEPROM(22);
        }

        public async Task<ushort> ReadOverloadThresholdAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromEEPROMAsync(22, cancellationToken);
        }

        public void WriteOverloadThreshold(ushort newValue)
        {
            WriteUshortToEEPROM(22, newValue);
        }

        public async Task WriteOverloadThresholdAsync(ushort newValue, CancellationToken cancellationToken)
        {
            await WriteUshortToEEPROMAsync(22, newValue, cancellationToken);
        }

        #endregion

        #region Serial number

        public int ReadSerialNumber()
        {
            return int.Parse(GetStringValue(ReadFromMC(1)));
        }

        public async Task<int> ReadSerialNumberAsync(CancellationToken cancellationToken)
        {
            return int.Parse(GetStringValue(await ReadFromMCAsync(1, cancellationToken)));
        }

        #endregion

        #region Date and time

        public DateTime ReadDateTime()
        {
            return RawBytesConverter.Bytes3ToTime(ReadFromMC(213));
        }

        public async Task<DateTime> ReadDateTimeAsync(CancellationToken cancellationToken)
        {
            return RawBytesConverter.Bytes3ToTime(await ReadFromMCAsync(213, cancellationToken));
        }

        public async Task WriteDateTimeAsync(CancellationToken cancellationToken, DateTime? value = null)
        {
            await WriteArrToMCAsync(213, RawBytesConverter.TimeTo3Bytes(value == null ? DateTime.Now : (DateTime)value), cancellationToken);
        }

        public void WriteDateTime(DateTime? value = null)
        {
            WriteArrToMC(213, RawBytesConverter.TimeTo3Bytes(value == null ? DateTime.Now : (DateTime)value));
        }

        #endregion

        #region Gamma N

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

        #endregion

        #region History record interval

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

        #endregion

        #region Battery voltage

        public double ReadBatteryVoltage()
        {
            byte[] array = ReadFromMC(207);
            return (array[0] + 256 * array[1]) * BatteryReadoutCoefficient / 4096.0;
        }

        public async Task<double> ReadBatteryVoltageAsync(CancellationToken cancellationToken)
        {
            byte[] array = await ReadFromMCAsync(207, cancellationToken);
            return (array[0] + 256 * array[1]) * BatteryReadoutCoefficient / 4096.0;
        }

        #endregion

        #region Battery threshold

        public double ReadBatteryThreshold()
        {
            byte[] array = ReadFromMC(210);
            return (array[0] + 256 * array[1]) * BatteryReadoutCoefficient / 4096.0;
        }

        public async Task<double> ReadBatteryThresholdAsync(CancellationToken cancellationToken)
        {
            byte[] array = await ReadFromMCAsync(210, cancellationToken);
            return (array[0] + 256 * array[1]) * BatteryReadoutCoefficient / 4096.0;
        }

        public void WriteBatteryThreshold(double value)
        {
            WriteByteToEEPROM(7, (byte)(value * 4096.0 / BatteryReadoutCoefficient / 256));
        }

        public async Task WriteBatteryThresholdAsync(double value, CancellationToken cancellationToken)
        {
            await WriteByteToEEPROMAsync(7, (byte)(value * 4096.0 / BatteryReadoutCoefficient / 256), cancellationToken);
        }

        #endregion

        #region Battery percentage

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

        #endregion

        #region Clear history

        public void ClearHistory()
        {
            TransmitCommandAndCheck("DelHistory", "Ok3");
            /**
            try
            {
                _dataSendSemaphore.Wait();
                byte[] received = UncheckedTransmit(CommunicationCommands["DelHistory"]);
                CheckResultAndThrow(received, CommunicationCommands["Ok3"]);
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
            **/
        }

        public async Task ClearHistoryAsync(CancellationToken cancellationToken)
        {
            await TransmitCommandAndCheckAsync("DelHistory", "Ok3", cancellationToken);
            /**
            try
            {
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                byte[] received = await UncheckedTransmitAsync(CommunicationCommands["DelHistory"], cancellationToken);
                CheckResultAndThrow(received, CommunicationCommands["Ok3"]);
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
            **/
        }

        #endregion

        private static double GetNeutronCoeffFromByte(byte b)
        {
            // format: i1.i2, eg 9.9; 8.8
            double i1 = (b & 240) >> 4; // First integer
            double i2 = (b & 15) / 16.0; // Second integer
            return Math.Round(i1 + i2, 1); // Can fluctuate a bit, +- 0.025 from the actual result, so we round it to make it just two digits
        }

        private static byte GetByteFromNeutronCoeff(double coeff)
        {
            // -1 error at low coefficient
            int i1 = (int)Math.Truncate(coeff) << 4; // First integer
            int i2 = (int)((coeff - Math.Truncate(coeff)) * 16); // Second integer
            if (coeff <= 1.1) // temporary fix for -1 error 
            {
                i2 += 1;
            }
            return (byte)(i1 + i2);
        }

        #region Neutron

        /// <summary>
        /// Reads neutron coefficient from the device
        /// </summary>
        /// <returns>Neutron coefficient</returns>
        public double ReadNeutronCoefficient()
        {
            return GetNeutronCoeffFromByte(ReadByteFromEEPROM(81)); //GetNeutronCoeffFromByte(SendCommandAndReceiveAndCheck("GetNeutronCoefficient", "Ok4")[6]);
        }

        /// <summary>
        /// Reads neutron coefficient from the device asynchronously
        /// </summary>
        /// <returns>Neutron coefficient</returns>
        public async Task<double> ReadNeutronCoefficientAsync(CancellationToken cancellationToken)
        {
            return GetNeutronCoeffFromByte(await ReadByteFromEEPROMAsync(81, cancellationToken));
        }

        /// <summary>
        /// Changes neutron coefficient for the device
        /// </summary>
        /// <param name="value">New neutron</param>
        public void WriteNeutronCoefficient(double value)
        {
            WriteByteToEEPROM(81, GetByteFromNeutronCoeff(value));
        }

        /// <summary>
        /// Changes neutron coefficient for the device asynchronously
        /// </summary>
        /// <param name="value">New neutron</param>
        public async Task WriteNeutronCoefficientAsync(double value, CancellationToken cancellationToken)
        {
            await WriteByteToEEPROMAsync(81, GetByteFromNeutronCoeff(value), cancellationToken);
        }

        #endregion

        /// <summary>
        /// Reads history from the device
        /// </summary>
        /// <returns>Array of readed history events in sequential order</returns>
        public HistoryEventRecord[] ReadHistory()
        {
            try
            {
                _dataSendSemaphore.Wait();
                List<PM1401GNEventRecord> records = new List<PM1401GNEventRecord>();
                byte[] received = UncheckedTransmit(CommunicationCommands["GetHistory"]);
                int recordsCount = 1;
                while (recordsCount > 0)
                {
                    recordsCount = CheckHistoryResult(received) / PM1401GNEventRecord.SingleRecordSize;
                    for (int i = 0; i < recordsCount; i++)
                    {
                        records.Add(PM1401GNEventRecord.ParseFromArray(received, PM1401GNEventRecord.SingleRecordSize * i + 6));
                    }
                    received = UncheckedTransmit(CommunicationCommands["GetHistoryNext"]);
                }
                return records.ToArray();
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
        }

        /// <summary>
        /// Reads history from the device asynchronously
        /// </summary>
        /// <returns>Array of readed history events in sequential order</returns>
        public async Task<HistoryEventRecord[]> ReadHistoryAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                List<PM1401GNEventRecord> records = new List<PM1401GNEventRecord>();
                byte[] received = await UncheckedTransmitAsync(CommunicationCommands["GetHistory"], cancellationToken);
                int recordsCount = 1;
                while (recordsCount > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    recordsCount = CheckHistoryResult(received) / PM1401GNEventRecord.SingleRecordSize;
                    for (int i = 0; i < recordsCount; i++)
                    {
                        records.Add(PM1401GNEventRecord.ParseFromArray(received, PM1401GNEventRecord.SingleRecordSize * i + 6));
                    }
                    received = await UncheckedTransmitAsync(CommunicationCommands["GetHistoryNext"], cancellationToken);
                }
                return records.ToArray();
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
        }

        // Ripped out from the PM1703Work
        // First 5? bytes: header
        // Each record is 8 bytes long
        /// <summary>
        /// Checks a history result and return an amount of bytes to read for records (in bytes)
        /// </summary>
        /// <param name="buff">Array to check</param>
        /// <returns>-1 if error otherwise amount of bytes to read</returns>
        private int CheckHistoryResult(IList<byte> buff)
        {
            byte[] array = CommunicationCommands["OkHistory"];
            if (buff.Count < array.Length)
            {
                return -1;
            }
            for (int i = 1; i < array.Length; i++)
            {
                if (i != 2 && buff[i] != array[i])
                {
                    return -1;
                }
            }
            //this.HistoryEnd = (buff[0] != 144);
            if (buff[0] == 144 || buff[0] == 160)
            {
                return 256 * buff[4] + buff[5] - 3;
            }
            return -1;
        }

        #region Safety alarm

        public byte ReadSafetyAlarm()
        {
            return ReadByteFromEEPROM(80);
        }

        public async Task<byte> ReadSafetyAlarmAsync(CancellationToken cancellationToken)
        {
            return await ReadByteFromEEPROMAsync(80, cancellationToken);
        }

        public void WriteSafetyAlarm(byte value)
        {
            WriteByteToEEPROM(80, value);
        }

        public async Task WriteSafetyAlarmAsync(byte value, CancellationToken cancellationToken)
        {
            await WriteByteToEEPROMAsync(80, value, cancellationToken);
        }

        #endregion

        #region Auto calibration enabled

        public bool ReadAutoCalibrationEnabled()
        {
            return ReadBitFromEEPROM(11, 4);
        }

        public async Task<bool> ReadAutoCalibrationEnabledAsync(CancellationToken cancellationToken)
        {
            return await ReadBitFromEEPROMAsync(11, 4, cancellationToken);
        }

        public void WriteAutoCalibrationEnabled(bool value)
        {
            WriteBitToEEPROM(11, 4, value);
        }

        public async Task WriteAutoCalibrationEnabledAsync(bool value, CancellationToken cancellationToken)
        {
            await WriteBitToEEPROMAsync(11, 4, value, cancellationToken);
        }

        #endregion

        private static bool IsAlarmChangeEnabled(byte modeValue) => ByteHelper.GetBit(modeValue, 6);//ByteHelper.GetBits(modeValue).ToArray()[6];

        private static bool IsCoefficientChangeEnabled(byte modeValue) => ByteHelper.GetBit(modeValue, 5);//ByteHelper.GetBits(modeValue).ToArray()[5];

        #region Alarm change enabled

        public bool ReadAlarmChangeEnabled()
        {
            return ReadBitFromEEPROM(9, 1);//IsAlarmChangeEnabled(ReadByteFromEEPROM(9));
        }

        public async Task<bool> ReadAlarmChangeEnabledAsync(CancellationToken cancellationToken)
        {
            return await ReadBitFromEEPROMAsync(9, 1, cancellationToken);//IsAlarmChangeEnabled(await ReadByteFromEEPROMAsync(9, cancellationToken));
        }

        public void WriteAlarmChangeEnabled(bool value)
        {
            WriteBitToEEPROM(9, 1, value);
        }

        public async Task WriteAlarmChangeEnabledAsync(bool value, CancellationToken cancellationToken)
        {
            await WriteBitToEEPROMAsync(9, 1, value, cancellationToken);
        }

        #endregion

        #region Coefficient change enabled

        public bool ReadCoefficientChangeEnabled()
        {
            return ReadBitFromEEPROM(9, 2);//return IsCoefficientChangeEnabled(ReadByteFromEEPROM(9));
        }

        public async Task<bool> ReadCoefficientChangeEnabledAsync(CancellationToken cancellationToken)
        {
            return await ReadBitFromEEPROMAsync(9, 2, cancellationToken);//return IsCoefficientChangeEnabled(await ReadByteFromEEPROMAsync(9, cancellationToken));
        }

        public void WriteCoefficientChangeEnabled(bool value)
        {
            WriteBitToEEPROM(9, 2, value);
        }

        public async Task WriteCoefficientChangeEnabledAsync(bool value, CancellationToken cancellationToken)
        {
           await WriteBitToEEPROMAsync(9, 2, value, cancellationToken);
        }

        #endregion

        private static bool IsAudioAlarmEnabled(byte modeValue) => ByteHelper.GetBits(modeValue).ToArray()[7];

        private static bool IsVibroAlarmEnabled(byte modeValue) => ByteHelper.GetBits(modeValue).ToArray()[6];

        #region Audio alarm enabled

        public bool ReadAudioAlarmEnabled()
        {
            return ReadBitFromEEPROM(10, 0);// IsAudioAlarmEnabled(ReadByteFromEEPROM(10));
        }

        public async Task<bool> ReadAudioAlarmEnabledAsync(CancellationToken cancellationToken)
        {
            return await ReadBitFromEEPROMAsync(10, 0, cancellationToken);//IsAudioAlarmEnabled(await ReadByteFromEEPROMAsync(10, cancellationToken));
        }

        public void WriteAudioAlarmEnabled(bool value)
        {
            WriteBitToEEPROM(10, 0, value);
        }

        public async Task WriteAudioAlarmEnabledAsync(bool value, CancellationToken cancellationToken)
        {
            await WriteBitToEEPROMAsync(10, 0, value, cancellationToken);
        }

        #endregion

        #region Vibro alarm enabled

        public bool ReadVibroAlarmEnabled()
        {
            return ReadBitFromEEPROM(10, 1);
        }

        public async Task<bool> ReadVibroAlarmEnabledAsync(CancellationToken cancellationToken)
        {
            return await ReadBitFromEEPROMAsync(10, 1, cancellationToken);//IsVibroAlarmEnabled(await ReadByteFromEEPROMAsync(10, cancellationToken));
        }

        public void WriteVibroAlarmEnabled(bool value)
        {
            WriteBitToEEPROM(10, 1, value);
        }

        public async Task WriteVibroAlarmEnabledAsync(bool value, CancellationToken cancellationToken)
        {
            await WriteBitToEEPROMAsync(10, 1, value, cancellationToken);
        }

        #endregion

        #region Calibration value

        public double ReadCalibrationValue()
        {
            byte[] array = ReadFromMC(201);
            return array[1] * 100 + array[0] * 100.0 / 255.0;
        }

        public async Task<double> ReadCalibrationValueAsync(CancellationToken cancellationToken)
        {

            byte[] array = await ReadFromMCAsync(201, cancellationToken);
            return array[1] * 100 + array[0] * 100.0 / 255.0;
        }

        public void WriteCalibrationValue(double value)
        {
            byte[] write = new byte[2];
            write[0] = (byte)(value % 100.0 / (100.0 / 255.0));
            write[1] = (byte)(value / 100);
            WriteBytesToEEPROM(14, write);
        }

        public async Task WriteCalibrationValueAsync(double value, CancellationToken cancellationToken)
        {
            byte[] write = new byte[2];
            write[0] = (byte)(value % 100.0 / (100.0 / 255.0));
            write[1] = (byte)(value / 100);
            await WriteBytesToEEPROMAsync(14, write, cancellationToken);
        }

        #endregion

        /**
        /// <summary>
        /// Reads history events directly from the memory, contains even deleted records
        /// </summary>
        private void BeginTotalHistoryDump()
        {
            PM1401GNEventRecord currentRecord;
            byte[] recordArray = new byte[PM1401GNEventRecord.SingleRecordSize];
            //980
            for (ushort i = 272 + PM1401GNEventRecord.SingleRecordSize * 0; i <= ushort.MaxValue; i += PM1401GNEventRecord.SingleRecordSize)
            {
                for (ushort offset = 0; offset < PM1401GNEventRecord.SingleRecordSize; offset += 2)
                {
                    byte[] readBuffer = ReadBytesFromEEPROM((ushort)(i + offset));
                    recordArray[offset] = readBuffer[0];
                    recordArray[offset + 1] = readBuffer[1];
                }
                currentRecord = PM1401GNEventRecord.ParseFromArray(recordArray);
                Console.WriteLine(currentRecord);
                Console.WriteLine();
            }
        }
        **/

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            IrDAClient.Dispose();
        }

        public sealed class PM1401GNEventRecord : HistoryEventRecord
        {

            /// <summary>
            /// Recorded event type
            /// </summary>
            public new PM1401GNEventType Type;

            public ushort Value1;

            public ushort Value2;

            /// <summary>
            /// Size of a single history record in bytes
            /// </summary>
            public const int SingleRecordSize = 8;

            public override string ToString()
            {
                string type = GetTypeAsString();
                string? string2 = null;
                string2 = Type switch
                {
                    PM1401GNEventType.CalibrationDoneCPS => string.Format("CPS gamma: {0}; CPS neutron: {1}", GetStandardizedValue1(), GetStandardizedValue2()),
                    PM1401GNEventType.BackgroundRecordCPS => string.Format("CPS gamma: {0}; CPS neutron: {1}", GetStandardizedValue1(), GetStandardizedValue2()),
                    PM1401GNEventType.NGammaChange => string.Format("{0} -> {1}", GetStandardizedValue1(), GetStandardizedValue2()),
                    PM1401GNEventType.AlarmNeutron => string.Format("CPS gamma: {0}; CPS neutron: {1}", GetStandardizedValue1(), GetStandardizedValue2()),
                    PM1401GNEventType.AlarmGammaCPS => string.Format("CPS gamma: {0}; CPS neutron: {1}", GetStandardizedValue1(), GetStandardizedValue2()),
                    PM1401GNEventType.AlarmChanged => string.Format("{0} -> {1}", GetStandardizedValue1(), GetStandardizedValue2()),
                    PM1401GNEventType.BackgroundRecordDoseRate => string.Format("Doserate gamma (uSv/H): {0}; CPS neutron: {1}", GetStandardizedValue1(), GetStandardizedValue2()),
                    PM1401GNEventType.AlarmGammaDoseRate => string.Format("Doserate gamma (uSv/H): {0}; CPS neutron: {1}", GetStandardizedValue1(), GetStandardizedValue2()),
                    _ => string.Format("First raw value: {0}; Second raw value: {1}", Value1, Value2),
                };
                return TimeStamp.ToString() + " - " + type + (string2 == null ? string.Empty : Environment.NewLine + string2);
            }

            private object? ToStandardizedValue(ushort value, bool isSecondValue)
            {
                switch (Type)
                {
                    case PM1401GNEventType.CalibrationDoneCPS:
                        return value / 2; //CPS
                    case PM1401GNEventType.CalibrationDoneDoseRate:
                        return null; // ? not sure
                    case PM1401GNEventType.BackgroundRecordCPS:
                        return value; //CPS
                    case PM1401GNEventType.NGammaChange:
                        return value / 10.0d; //MSE(n) for channel
                    case PM1401GNEventType.AlarmNeutron:
                        return value; //CPS
                    case PM1401GNEventType.AlarmGammaCPS:
                        return value; //CPS
                    case PM1401GNEventType.AlarmChanged:
                        bool audio = IsAudioAlarmEnabled((byte)value);
                        bool vibro = IsVibroAlarmEnabled((byte)value);
                        return string.Format("Aud={0};Vib={1}", audio ? 1 : 0, vibro ? 1 : 0);
                    case PM1401GNEventType.BackgroundRecordDoseRate:
                        return isSecondValue ? value : value / 100.0;
                    case PM1401GNEventType.AlarmGammaDoseRate:
                        return isSecondValue ? value : value / 100.0;
                    default:
                        return null;
                }
            }

            public object? GetStandardizedValue1()
            {
                return ToStandardizedValue(Value1, false);
            }

            public object? GetStandardizedValue2()
            {
                return ToStandardizedValue(Value2, true);
            }

            public static new int GetNumberOfSupportedValues()
            {
                return 2;
            }

            public override object?[] GetStandartizedValues()
            {
                return new object?[] { GetStandardizedValue1(), GetStandardizedValue2() };
            }

            public override string[] GetStandartizedValuesUnits()
            {
                string unit1 = string.Empty;
                string unit2 = string.Empty;
                switch (Type)
                {
                    case PM1401GNEventType.TurnOff:
                    case PM1401GNEventType.TurnOn:
                    case PM1401GNEventType.AlarmChanged:
                    case PM1401GNEventType.NGammaChange:
                        break;

                    case PM1401GNEventType.AlarmGammaCPS:
                    case PM1401GNEventType.CalibrationDoneCPS:
                    case PM1401GNEventType.AlarmNeutron:
                    case PM1401GNEventType.BackgroundRecordCPS:
                        unit1 = "CPS";
                        unit2 = "CPS";
                        break;

                    case PM1401GNEventType.AlarmGammaDoseRate:
                    case PM1401GNEventType.CalibrationDoneDoseRate:
                    case PM1401GNEventType.BackgroundRecordDoseRate:
                        unit1 = "uSv/H";
                        unit2 = "CPS";
                        break;

                    default:
                        break;
                }
                return new string[] { unit1, unit2 };
            }

            public static PM1401GNEventRecord ParseFromArray(byte[] array, int startIndex = 0)
            {
                PM1401GNEventType eventType = (PM1401GNEventType)array[startIndex + 6];
                DateTime timeStamp = RawBytesConverter.Bytes3ToTime(array, startIndex);
                ushort value1 = 0;
                byte value2 = 0;
                switch (eventType)
                {
                    case PM1401GNEventType.TurnOff:
                    case PM1401GNEventType.TurnOn:
                        break;
                    case PM1401GNEventType.AlarmGammaCPS:
                    case PM1401GNEventType.BackgroundRecordCPS:
                    case PM1401GNEventType.CalibrationDoneCPS:
                        value1 = (ushort)(array[startIndex + 5] << 8 | array[startIndex + 4]);
                        value2 = array[startIndex + 7];
                        break;

                    case PM1401GNEventType.AlarmNeutron:
                        value1 = (ushort)(array[startIndex + 5] << 8 | array[startIndex + 4]);
                        value2 = array[startIndex + 7];
                        break;

                    case PM1401GNEventType.NGammaChange:
                        value1 = array[startIndex + 5];
                        value2 = array[startIndex + 4];
                        break;

                    case PM1401GNEventType.CalibrationDoneDoseRate:
                        break;

                    case PM1401GNEventType.AlarmChanged:
                        value1 = array[startIndex + 5];
                        value2 = array[startIndex + 4];
                        break;

                    case PM1401GNEventType.BackgroundRecordDoseRate:
                        value1 = (ushort)(array[startIndex + 5] << 8 | array[startIndex + 4]);
                        value2 = array[startIndex + 7];
                        break;

                    case PM1401GNEventType.AlarmGammaDoseRate:
                        value1 = (ushort)(array[startIndex + 5] << 8 | array[startIndex + 4]);
                        value2 = array[startIndex + 7];
                        break;

                }

                PM1401GNEventRecord historyRecord = new PM1401GNEventRecord
                {
                    Type = eventType,
                    TimeStamp = timeStamp,
                    Value1 = value1,
                    Value2 = value2,
                };
                return historyRecord;
            }

            public override string GetTypeAsString()
            {
                return Type switch
                {
                    PM1401GNEventType.TurnOn => "Turn on",
                    PM1401GNEventType.TurnOff => "Turn off",
                    PM1401GNEventType.CalibrationDoneCPS => "Calibration done (CPS)",
                    PM1401GNEventType.CalibrationDoneDoseRate => "Calibration done (uSv/H)",
                    PM1401GNEventType.BackgroundRecordCPS => "Background record (CPS)",
                    PM1401GNEventType.NGammaChange => "Gamma n change",
                    PM1401GNEventType.AlarmNeutron => "Neutron alarm",
                    PM1401GNEventType.AlarmGammaCPS => "Gamma alarm (CPS)",
                    PM1401GNEventType.AlarmChanged => "Alarm mode switch",
                    PM1401GNEventType.BackgroundRecordDoseRate => "Background record (uSv/H)",
                    PM1401GNEventType.AlarmGammaDoseRate => "Gamma alarm (uSv/H)",
                    _ => string.Format("Unknown record type({0})", (byte)Type),
                };
            }
        }

        /// <summary>
        /// Types of history event records for PM1401GN
        /// </summary>
        public enum PM1401GNEventType : byte
        {
            TurnOn = 49,
            TurnOff = 48,
            CalibrationDoneCPS = 67,
            CalibrationDoneDoseRate = 99,
            BackgroundRecordCPS = 70,
            BackgroundRecordDoseRate = 102,
            NGammaChange = 83,
            AlarmGammaCPS = 65,
            AlarmGammaDoseRate = 97,
            AlarmNeutron = 78,
            AlarmChanged = 115,
        }

        public class PM1401GNDeviceParameterInfo : DeviceParameterInfo
        {
            public PM1401GNDeviceParameterInfo(PM1401GNDeviceParameterType type, string name, bool changable, Type valueType, string? unit = null) : base((int)type, name, changable, valueType, unit)
            {
            }

            public override string ToString()
            {
                return string.Format("{0} - {1}", Id, Name);
            }
        }

        public enum PM1401GNDeviceParameterType : int
        {
            SerialNumber = 0,
            NGamma = 1,
            NNeutron = 2,
            BatteryLevel = 3,
            BatteryCutoffLevel = 4,
            BatteryPercentage = 5,
            DateTime = 6,
            HistoryRecordingInterval = 7,
            AutoCalibration = 8,
            AlarmModeEditing = 9,
            AlarmCoefficientEditing = 10,
            AudioAlarm = 11,
            VibroAlarm = 12,
            CalibrationValue = 13,
            //Virtual parameters, they dont actually exist
            //StatDPS = 14, 
            //StatAlarm = 15,
            SafetyAlarm = 16,
            OverloadThreshold = 17, 
        }
    }
}
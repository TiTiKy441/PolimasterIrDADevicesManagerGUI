using InTheHand.Net.Sockets;
using InTheHand.Net;
using PolimasterIrDADevicesManagerGUI.Utils;
using System.Text;

namespace PolimasterIrDADevicesManagerGUI.Device.Protocols.Implementations
{

    /**
     * This is basically blind coding cause I dont have access to PM1703MO-1
     * I just kinda look at the disassembly of the code from Personal Dose Tracker app and try to replicate it's behaviour
     **/
    /**
     * DO NOT USE AT ALL.
     **/
    public sealed class PM1703MO1ProtocolDevice : PMSearchPagerGeneralProtocolDevice
    {



        public PM1703MO1ProtocolDevice(IrDAClient client, IrDAEndPoint deviceEndPoint) : base(client, deviceEndPoint)
        {
            BatteryReadoutCoefficient = 2.5d;
            SetupCommunicationCommands();
        }

        private void SetupCommunicationCommands()
        {
            CommunicationCommands["GetHistory"] = new byte[] { 131, 0, 5, 177, 64 };
            CommunicationCommands["DelHistory"] = new byte[] { 130, 0, 5, 177, 66 };
            CommunicationCommands["OkHistory"] = new byte[] { 144, 0, 54, 114, 0 };
            CommunicationCommands["GetHistoryNext"] = new byte[] { 131, 0, 3 };
        }

        #region Gamma search threshold

        public async Task<ushort> ReadGammaSearchThresholdAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromEEPROMAsync(54, cancellationToken);
        }

        public async Task WriteGammaSearchThresholdAsync(ushort value, CancellationToken cancellationToken)
        {
            await WriteUshortToEEPROMAsync(54, value, cancellationToken);
        }

        #endregion

        #region Neutron search threshold

        public async Task<double> ReadNeutronSearchThresholdAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromEEPROMAsync(56, cancellationToken) / 10.0d;
        }

        public async Task WriteNeutronSearchThreshold(double value, CancellationToken cancellationToken)
        {
            await WriteUshortToEEPROMAsync(56, (ushort)(value * 10.0d), cancellationToken);
        }

        #endregion

        #region DER1 Threshold

        public async Task<double> ReadDER1ThresholdAsync(CancellationToken cancellationToken)
        {
            return await ReadFloatFromEEPROMAsync(112, cancellationToken);
        }

        public async Task WriteDER1ThresholdAsync(double value, CancellationToken cancellationToken)
        {
            await WriteFloatToEEPROMAsync(112, value, cancellationToken);
        }

        #endregion

        #region DER2 Threshold

        public async Task<double> ReadDER2ThresholdAsync(CancellationToken cancellationToken)
        {
            return await ReadFloatFromEEPROMAsync(116, cancellationToken);
        }

        public async Task WriteDER2ThresholdAsync(double value, CancellationToken cancellationToken)
        {
            await WriteFloatToEEPROMAsync(116, value, cancellationToken);
        }

        #endregion

        #region DER Threshold

        public async Task<double> ReadDERThresholdAsync(CancellationToken cancellationToken)
        {
            return await ReadFloatFromEEPROMAsync(102, cancellationToken);
        }

        public async Task WriteDERThresholdAsync(double value, CancellationToken cancellationToken)
        {
            await WriteFloatToEEPROMAsync(102, value, cancellationToken);
        }

        #endregion

        #region DE Threshold

        public async Task<double> ReadDEThresholdAsync(CancellationToken cancellationToken)
        {
            return await ReadFloatFromEEPROMAsync(108, cancellationToken);
        }

        public async Task WriteDEThresholdAsync(double value, CancellationToken cancellationToken)
        {
            await WriteFloatToEEPROMAsync(108, value, cancellationToken);
        }

        #endregion

        #region Neutron N

        public async Task<double> ReadNeutronNCoefficientAsync(CancellationToken cancellationToken)
        {
            return (double)((await ReadUshortFromEEPROMAsync(32, cancellationToken)) >> 8) / 10.0;
        }

        public async Task WriteNeutronNCoefficientAsync(double value, CancellationToken cancellationToken)
        {
            try
            {
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                byte[] bytesRead = await UncheckedReadBytesFromEEPROMAsync(32, cancellationToken);
                ushort num = (ushort)((int)bytesRead[0] | (int)bytesRead[1] << 8);
                num = (ushort)((int)(num & 255) | (int)((byte)(value * 10.0)) << 8);
                await UncheckedWriteBytesToEEPROMAsync(32, new byte[2] { (byte)num, (byte)(num >> 8) }, cancellationToken);
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
        }

        #endregion

        #region History records count

        public async Task<ushort> ReadHistoryRecordsCountAsync(CancellationToken cancellationToken)
        {
            ushort read = await ReadUshortFromEEPROMAsync(256, cancellationToken);
            return (ushort)((read >= 32768) ? 990 : ((read - 272) / 8));
        }

        #endregion

        #region Time before first record

        public async Task<ushort> ReadTimeBeforeFirstRecordAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromMCAsync(214, cancellationToken);
        }

        public async Task WriteTimeBeforeFirstRecordAsync(ushort value, CancellationToken cancellationToken)
        {
            await WriteUshortToMCAsync(214, value, cancellationToken);
        }

        #endregion

        #region Pager type

        public async Task<string> ReadPagerTypeAsync(CancellationToken cancellationToken)
        {
            return RawBytesConverter.GetStringValue(await ReadFromMCAsync(215, cancellationToken));
        }

        #endregion

        #region Device version

        public async Task<string> ReadDeviceVersionAsync(CancellationToken cancellationTokens)
        {
            return ParseDeviceVersion(await ReadFromMCAsync(251, cancellationTokens));
        }

        private static string ParseDeviceVersion(byte[] bytes)
        {
            // Zero idea what this actually does(?)
            string text = RawBytesConverter.GetStringValue(bytes);
            List<char> list = new List<char>();
            for (int i = 6; i < text.Length; i++)
            {
                list.Add(text[i]);
            }
            string[] array5 = new StringBuilder().Append(list.ToArray()).ToString().Split(new char[]
            {
                    ' '
            }, StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(array5[1], " ", array5[2].AsSpan(0, 8));
        }

        #endregion

        private ushort Mask(ushort value, ushort mask, bool isEnabled)
        {
            if (isEnabled)
            {
                return (ushort)(value | mask);
            }
            else
            {
                return (ushort)(value & ~mask);
            }
        }

        private async Task WriteMask(ushort address, ushort mask, bool isEnabled, CancellationToken cancellationToken)
        {
            try
            {
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                await UncheckedWriteUshortTOEEPROMAsync(address, Mask(await UncheckedReadUshortFromEEPROMAsync(address, cancellationToken), mask, isEnabled), cancellationToken);
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
        }

        private bool GetMask(ushort value, ushort mask)
        {
            return ((value & mask) != 0);
        }

        #region Mode 0-9

        public async Task<bool> ReadMode09EnabledAsync(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(52, cancellationToken), 8199);
        }

        public async Task WriteMode09EnabledAsync(CancellationToken cancellationToken, bool value)
        {
            await WriteMask(52, 8199, value, cancellationToken);
        }

        #endregion

        #region Is Roentgen scale

        public async Task<bool> ReadIsRoentgenScaleEnabledAsync(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(211, cancellationToken), 32);
        }

        public async Task WriteIsRoetgenScaleEnabledAsync(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(211, 32, value, cancellationToken);
        }

        #endregion

        #region Show 2 after comma

        public async Task<bool> ReadShowTwoAfterCommaEnabledAsync(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(211, cancellationToken), 8);
        }

        public async Task WriteShowTwoAfterCommaEnabledAsync(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(211, 8, value, cancellationToken);
        }

        #endregion

        #region Quick off

        public async Task<bool> ReadQuickOffEnabled(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(211, cancellationToken), 8192);
        }

        public async Task WriteQuickOffEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(211, 8192, value, cancellationToken);
        }

        #endregion

        #region Allow dose reset

        public async Task<bool> ReadAllowDoseReset(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(211, cancellationToken), 1);
        }

        public async Task WriteAllowDoseReset(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(211, 1, value, cancellationToken);
        }

        #endregion

        #region Allow alarm change

        public async Task<bool> ReadAllowAlarmChangeEnabled(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(211, cancellationToken), 512);
        }

        public async Task WriteAllowAlarmChangeEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(211, 512, value, cancellationToken);
        }

        #endregion

        #region Allow N coeff change

        public async Task<bool> ReadAllowNChangeEnabled(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(211, cancellationToken), 1024);
        }

        public async Task WriteAllowNChangeEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(211, 1024, value, cancellationToken);
        }

        #endregion

        #region Show prefix 0s

        public async Task<bool> ReadShowPrefixZerosAsync(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(212, cancellationToken), 32768);
        }

        public async Task WriteShowPrefixZerosAsync(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(212, 32768, value, cancellationToken);
        }

        #endregion

        #region Sound enabled

        public async Task<bool> ReadSoundAlarmEnabled(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(212, cancellationToken), 1);
        }

        public async Task WriteSoundAlarmEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(212, 1, value, cancellationToken);
        }

        #endregion

        #region Vibro enabled

        public async Task<bool> ReadVibroAlarmEnabled(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(212, cancellationToken), 2);
        }

        public async Task WriteVibroAlarmEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(212, 2, value, cancellationToken);
        }

        #endregion

        #region Button click sound

        public async Task<bool> ReadButtonClickSoundEnabledAsync(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(212, cancellationToken), 8);
        }

        public async Task WriteButtonClickSoundEnabledAsync(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(212, 8, value, cancellationToken);
        }

        #endregion

        #region Calibration beep on start

        public async Task<bool> ReadCalibrationBeepOnStartEnabled(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(212, cancellationToken), 128);
        }

        public async Task WriteCalibrationBeepOnStartEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(212, 128, value, cancellationToken);
        }

        #endregion

        #region Sound with light

        public async Task<bool> ReadSoundWithLightEnabled(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(212, cancellationToken), 64);
        }

        public async Task WriteSoundWithLightEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(212, 64, value, cancellationToken);
        }

        #endregion

        #region Allow autocalibration

        public async Task<bool> ReadAllowAutocalibration(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(212, cancellationToken), 32768);
        }

        public async Task WriteAllowAutocalibration(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(212, 32768, value, cancellationToken);
        }

        #endregion

        #region Enable neutron registation

        public async Task<bool> ReadNeutronRegistrationEnabled(CancellationToken cancellationToken)
        {
            return GetMask(await ReadUshortFromEEPROMAsync(211, cancellationToken), 16);
        }

        public async Task WriteNeutronRegistrationEnabled(bool value, CancellationToken cancellationToken)
        {
            await WriteMask(211, 16, value, cancellationToken);
        }

        #endregion


    }


    public enum PM1703MO1DeviceParameterType : int
    {
        RoentgenScale = 0,
        ShowPrefixZeros = 1,
        ShowTwoAfterComma = 2,
        AllowSound = 3,
        AllowVibro = 4,
        AllowButtonSound = 5,
        AllowCalibrationBeepOnStart = 6,
        AllowSoundAsLight = 7,
        AllowQuickOff = 8,
        AllowDoseReset = 9,
        AllowAlarmSetup = 10,
        AllowCoefficientSetup = 11,
        Enable09Mode = 12,
        
        GammaNCoefficient = 13,
        SearchThreshold = 14,
        ThresholdDER1 = 15,
        ThresholdDER2 = 16,
        ThresholdDE = 17,
        HistoryRecordingInterval = 18,
        TimeBeforeFirstRecord = 19,
        NeutronNCoefficient = 20,
        SearchNeutronThreshold = 21,

    }
}

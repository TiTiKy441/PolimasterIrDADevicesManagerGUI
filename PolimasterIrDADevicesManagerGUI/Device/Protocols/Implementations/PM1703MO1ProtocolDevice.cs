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

        public ushort ReadGammaSearchThreshold()
        {
            return ReadUshortFromEEPROM(54);
        }

        public async Task<ushort> ReadGammaSearchThresholdAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromEEPROMAsync(54, cancellationToken);
        }

        public void WriteGammaSearchThreshold(ushort value)
        {
            WriteUshortToEEPROM(54, value);
        }

        public async Task WriteGammaSearchThresholdAsync(ushort value, CancellationToken cancellationToken)
        {
            await WriteUshortToEEPROMAsync(54, value, cancellationToken);
        }

        #endregion

        #region Neutron search threshold

        public double ReadNeutronSearchThreshold()
        {
            return ReadUshortFromEEPROM(56) / 10.0d;
        }

        public async Task<double> ReadNeutronSearchThresholdAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromEEPROMAsync(56, cancellationToken) / 10.0d;
        }

        public void WriteNeutronSearchThreshold(double value)
        {
            WriteUshortToEEPROM(56, (ushort)(value * 10.0d));
        }

        public async Task WriteNeutronSearchThreshold(double value, CancellationToken cancellationToken)
        {
            await WriteUshortToEEPROMAsync(56, (ushort)(value * 10.0d), cancellationToken);
        }

        #endregion

        #region DER1 Threshold

        public double ReadDER1Threshold()
        {
            return ReadFloatFromEEPROM(112);
        }

        public async Task<double> ReadDER1ThresholdAsync(CancellationToken cancellationToken)
        {
            return await ReadFloatFromEEPROMAsync(112, cancellationToken);
        }

        public void WriteDER1Threshold(double value)
        {
            WriteFloatToEEPROM(112, value);
        }

        public async Task WriteDER1ThresholdAsync(double value, CancellationToken cancellationToken)
        {
            await WriteFloatToEEPROMAsync(112, value, cancellationToken);
        }

        #endregion

        #region DER2 Threshold

        public double ReadDER2Threshold()
        {
            return ReadFloatFromEEPROM(116);
        }

        public async Task<double> ReadDER2ThresholdAsync(CancellationToken cancellationToken)
        {
            return await ReadFloatFromEEPROMAsync(116, cancellationToken);
        }

        public void WriteDER2Threshold(double value)
        {
            WriteFloatToEEPROM(116, value);
        }

        public async Task WriteDER2ThresholdAsync(double value, CancellationToken cancellationToken)
        {
            await WriteFloatToEEPROMAsync(116, value, cancellationToken);
        }

        #endregion

        #region DER Threshold

        public double ReadDERThreshold()
        {
            return ReadFloatFromEEPROM(102);
        }

        public async Task<double> ReadDERThresholdAsync(CancellationToken cancellationToken)
        {
            return await ReadFloatFromEEPROMAsync(102, cancellationToken);
        }

        public void WriteDERThreshold(double value)
        {
            WriteFloatToEEPROM(102, value);
        }

        public async Task WriteDERThresholdAsync(double value, CancellationToken cancellationToken)
        {
            await WriteFloatToEEPROMAsync(102, value, cancellationToken);
        }

        #endregion

        #region DE Threshold

        public double ReadDEThreshold()
        {
            return ReadFloatFromEEPROM(108);
        }

        public async Task<double> ReadDEThresholdAsync(CancellationToken cancellationToken)
        {
            return await ReadFloatFromEEPROMAsync(108, cancellationToken);
        }

        public void WriteDEThreshold(double value)
        {
            WriteFloatToEEPROM(108, value);
        }

        public async Task WriteDEThresholdAsync(double value, CancellationToken cancellationToken)
        {
            await WriteFloatToEEPROMAsync(108, value, cancellationToken);
        }

        #endregion

        #region Neutron N

        public double ReadNeutronNCoefficient()
        {
            return (double)(ReadUshortFromEEPROM(32) >> 8) / 10.0;
        }

        public async Task<double> ReadNeutronNCoefficientAsync(CancellationToken cancellationToken)
        {
            return (double)((await ReadUshortFromEEPROMAsync(32, cancellationToken)) >> 8) / 10.0;
        }

        public void WriteNeutronNCoefficient(double value)
        {
            try
            {
                _dataSendSemaphore.Wait();
                byte[] bytesRead = UncheckedReadBytesFromEEPROM(32);
                ushort num = (ushort)((int)bytesRead[0] | (int)bytesRead[1] << 8);
                num = (ushort)((int)(num & 255) | (int)((byte)(value * 10.0)) << 8);
                UncheckedWriteBytesToEEPROM(32, new byte[2] { (byte)num, (byte)(num >> 8) });
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
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

        public ushort ReadHistoryRecordsCount()
        {
            ushort read = ReadUshortFromEEPROM(256);
            return (ushort)((read >= 32768) ? 990 : ((read - 272) / 8));
        }
        public async Task<ushort> ReadHistoryRecordsCountAsync(CancellationToken cancellationToken)
        {
            ushort read = await ReadUshortFromEEPROMAsync(256, cancellationToken);
            return (ushort)((read >= 32768) ? 990 : ((read - 272) / 8));
        }

        #endregion

        #region Time before first record

        public ushort ReadTimeBeforeFirstRecord()
        {
            return ReadUshortFromMC(214);
        }

        public async Task<ushort> ReadTimeBeforeFirstRecordAsync(CancellationToken cancellationToken)
        {
            return await ReadUshortFromMCAsync(214, cancellationToken);
        }

        public void WriteTimeBeforeFirstRecord(ushort value)
        {
            WriteUshortToMC(214, value);
        }

        public async Task WriteTimeBeforeFirstRecordAsync(ushort value, CancellationToken cancellationToken)
        {
            await WriteUshortToMCAsync(214, value, cancellationToken);
        }

        #endregion

        #region Pager type

        public string ReadPagerType()
        {
            return RawBytesConverter.GetStringValue(ReadFromMC(215));
        }

        public async Task<string> ReadPagerTypeAsync(CancellationToken cancellationToken)
        {
            return RawBytesConverter.GetStringValue(await ReadFromMCAsync(215, cancellationToken));
        }

        #endregion

        #region Device version

        public string ReadDeviceVersion()
        {
            return ParseDeviceVersion(ReadFromMC(251));
        }

        public async Task<string> ReadDeviceVersionAsync(CancellationToken cancellationTokens)
        {
            return ParseDeviceVersion(await ReadFromMCAsync(251, cancellationTokens));
        }

        private static string ParseDeviceVersion(byte[] bytes)
        {
            // Zero idea what this actually does(?)
            // Ripped out from the disassembly!
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

        #region Mode 0-9

        public bool ReadMode09Enabled()
        {
            return ((ReadUshortFromEEPROM(52)  & 8199) != 0);
        }

        public async Task<bool> ReadMode09EnabledAsync(CancellationToken cancellationToken)
        {
            return (((await ReadUshortFromEEPROMAsync(52, cancellationToken)) & 8199) != 0);
        }

        #endregion

    }
}

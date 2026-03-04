using InTheHand.Net.Sockets;
using InTheHand.Net;
using PolimasterIrDADevicesManagerGUI.Utils;

namespace PolimasterIrDADevicesManagerGUI.Device.Protocols
{
    /**
     * We dont use PMDeviceBaseProtocol commands due to sync issues where as the implementation on top...
     * we need to execute SetAddress and GetWord/SetWord in a strict sequence without any commands in between
     * Due to these reasons we use SendAndReceiveSequence for placing two commands and executing them in sequence, cause these commands... 
     * are implemented with a direct semaphore on the IrDA I/O and no commands can get in between
     **/
    public class EEPROMAccessProtocolDevice : PMDeviceBaseProtocol, IDirectEEPROMAccessDevice
    {

        public EEPROMAccessProtocolDevice(IrDAClient irdaClient, IrDAEndPoint endpoint) : base(irdaClient, endpoint)
        {
            SetupCommunicationCommands();
        }

        private void SetupCommunicationCommands()
        {
            CommunicationCommands["SetAddress"] = new byte[] { 130, 0, 10, 177, 0, 114, 0, 5, 0, 0 }; //Ok3
            CommunicationCommands["GetWord"] =    new byte[] { 131, 0, 5, 177, 156 }; //Ok4
            CommunicationCommands["SetWord"] =    new byte[] { 130, 0, 10, 177, 156, 114, 0, 5, 0, 0 }; //Ok3
        }

        public byte[] GenerateSetAddressCommand(ushort address)
        {
            byte[] cmd = CommunicationCommands["SetAddress"].ToArray();
            cmd[8] = (byte)address;
            cmd[9] = (byte)(address >> 8);
            return cmd;
        }

        private byte[] GenerateSetWordCommand(ushort value)
        {
            byte[] cmd = CommunicationCommands["SetWord"].ToArray();
            cmd[8] = (byte)value;
            cmd[9] = (byte)(value >> 8);
            return cmd;
        }

        private byte[] GenerateSetWordCommand(byte b1, byte b2)
        {

            byte[] cmd = CommunicationCommands["SetWord"].ToArray();
            cmd[8] = b1;
            cmd[9] = b2;
            return cmd;
        }

        private byte[] GenerateGetWordCommand()
        {
            byte[] cmd = CommunicationCommands["GetWord"].ToArray();
            return cmd;
        }

        protected async Task<byte[]> UncheckedReadBytesFromEEPROMAsync(ushort addr, CancellationToken cancellationToken)
        {
            byte[] received = await UncheckedTransmitAsync(GenerateSetAddressCommand(addr), cancellationToken);
            CheckResultAndThrow(received, CommunicationCommands["Ok3"]);
            received = await UncheckedTransmitAsync(GenerateGetWordCommand(), cancellationToken);
            CheckResultAndThrow(received, CommunicationCommands["Ok4"]);
            return new byte[2] { received[6], received[7] };
        }

        protected async Task UncheckedWriteBytesToEEPROMAsync(ushort addr, byte[] bytes, CancellationToken cancellationToken)
        {
            byte[] received = await UncheckedTransmitAsync(GenerateSetAddressCommand(addr), cancellationToken);
            CheckResultAndThrow(received, CommunicationCommands["Ok3"]);
            received = await UncheckedTransmitAsync(GenerateSetWordCommand(bytes[0], bytes[1]), cancellationToken);
            CheckResultAndThrow(received, CommunicationCommands["Ok3"]);
        }

        public async Task<byte[]> ReadBytesFromEEPROMAsync(ushort addr, CancellationToken cancellationToken)
        {
            try
            {
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                return await UncheckedReadBytesFromEEPROMAsync(addr, cancellationToken);
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
        }

        public async Task<byte> ReadByteFromEEPROMAsync(ushort addr, CancellationToken cancellationToken)
        {
            return (await ReadBytesFromEEPROMAsync(addr, cancellationToken))[0];
        }

        public async Task WriteBytesToEEPROMAsync(ushort addr, byte[] bytes, CancellationToken cancellationToken)
        {
            try
            {
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                await UncheckedWriteBytesToEEPROMAsync(addr, bytes, cancellationToken);
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
        }

        public async Task WriteBytesToEEPROMAsync(ushort addr, byte b1, byte b2, CancellationToken cancellationToken)
        {
            await WriteBytesToEEPROMAsync(addr, new byte[] { b1, b2 }, cancellationToken);
        }

        public async Task WriteByteToEEPROMAsync(ushort addr, byte b, CancellationToken cancellationToken)
        {
            try
            {
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                byte[] read = await UncheckedReadBytesFromEEPROMAsync(addr, cancellationToken);
                read[0] = b;
                await UncheckedWriteBytesToEEPROMAsync(addr, read, cancellationToken);
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
        }

        public async Task<bool> ReadBitFromEEPROMAsync(ushort addr, int bit, CancellationToken cancellationToken)
        {
            byte[] read = await ReadBytesFromEEPROMAsync(addr, cancellationToken);
            return ByteHelper.GetBit(read, bit);
        }

        public async Task WriteBitToEEPROMAsync(ushort addr, int bit, bool value, CancellationToken cancellationToken)
        {
            try
            {
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                byte[] val = await UncheckedReadBytesFromEEPROMAsync(addr, cancellationToken);
                byte[] newVal = ByteHelper.SetBit(val, bit, value);
                await UncheckedWriteBytesToEEPROMAsync(addr, val, cancellationToken);

            }
            finally
            {
                _dataSendSemaphore.Release();
            }
        }

        public async Task<double> ReadFloatFromEEPROMAsync(ushort addr, CancellationToken cancellationToken)
        {
            try
            {
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                byte[] readBytes = new byte[4];
                byte[] read1 = await UncheckedReadBytesFromEEPROMAsync(addr, cancellationToken);
                byte[] read2 = await UncheckedReadBytesFromEEPROMAsync((ushort)(addr + 2), cancellationToken);
                readBytes[0] = read1[0];
                readBytes[1] = read1[1];
                readBytes[2] = read2[0];
                readBytes[3] = read2[1];
                return RawBytesConverter.ReadDouble(readBytes);
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
        }

        public async Task WriteFloatToEEPROMAsync(ushort addr, double value, CancellationToken cancellationToken)
        {
            try
            {
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                byte[] bytesValue = new byte[4];
                RawBytesConverter.WriteDouble(value, bytesValue, 0);
                await UncheckedWriteBytesToEEPROMAsync(addr, new byte[2] { bytesValue[0], bytesValue[1] }, cancellationToken);
                await UncheckedWriteBytesToEEPROMAsync(addr, new byte[2] { bytesValue[2], bytesValue[3] }, cancellationToken);
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
        }

        public async Task<ushort> UncheckedReadUshortFromEEPROMAsync(ushort address, CancellationToken cancellationToken)
        {
            byte[] read = await UncheckedReadBytesFromEEPROMAsync(address, cancellationToken);
            return (ushort)((int)read[0] | (int)read[1] << 8);
        }

        public async Task UncheckedWriteUshortTOEEPROMAsync(ushort address, ushort value, CancellationToken cancellationToken)
        {
            await UncheckedWriteBytesToEEPROMAsync(address, new byte[] { (byte)value, (byte)(value >> 8)}, cancellationToken);
        }

        public async Task<ushort> ReadUshortFromEEPROMAsync(ushort address, CancellationToken cancellationToken)
        {
            byte[] read = await ReadBytesFromEEPROMAsync(address, cancellationToken);
            return (ushort)((int)read[0] | (int)read[1] << 8);
        }
        public async Task WriteUshortToEEPROMAsync(ushort address, ushort value, CancellationToken cancellationToken)
        {
            await WriteBytesToEEPROMAsync(address, (byte)value, (byte)(value >> 8), cancellationToken);
        }
    }
}

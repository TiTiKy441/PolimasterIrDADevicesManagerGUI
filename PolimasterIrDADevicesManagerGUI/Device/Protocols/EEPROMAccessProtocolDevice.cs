using InTheHand.Net.Sockets;
using InTheHand.Net;
using PolimasterIrDADevicesManagerGUI.Utils;
using System.Threading.Tasks;
using System.Formats.Asn1;

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

        private SemaphoreSlim _memoryAccessSemaphore = new SemaphoreSlim(1);

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

        public ushort ReadWordFromEEPROM(ushort addr)
        {
            byte[] array = ReadBytesFromEEPROM(addr);
            return (ushort)(array[0] | array[1] << 8);
        }

        public async Task<ushort> ReadWordFromEEPROMAsync(ushort addr, CancellationToken cancellationToken)
        {
            byte[] array = await ReadBytesFromEEPROMAsync(addr, cancellationToken);
            return (ushort)(array[0] | array[1] << 8);
        }

        public byte[] ReadBytesFromEEPROM(ushort addr)
        {
            try
            {
                _memoryAccessSemaphore.Wait();
                _dataSendSemaphore.Wait();
                return UncheckedReadBytesFromEEPROM(addr);
            }
            finally
            {
                _dataSendSemaphore.Release();
                _memoryAccessSemaphore.Release();
            }
        }

        protected byte[] UncheckedReadBytesFromEEPROM(ushort addr)
        {
            byte[] received = UncheckedTransmit(GenerateSetAddressCommand(addr));
            CheckResultAndThrow(received, CommunicationCommands["Ok3"]);
            received = UncheckedTransmit(GenerateGetWordCommand());
            CheckResultAndThrow(received, CommunicationCommands["Ok4"]);
            return new byte[2] { received[6], received[7] };
        }

        protected async Task<byte[]> UncheckedReadBytesFromEEPROMAsync(ushort addr, CancellationToken cancellationToken)
        {
            byte[] received = await UncheckedTransmitAsync(GenerateSetAddressCommand(addr), cancellationToken);
            CheckResultAndThrow(received, CommunicationCommands["Ok3"]);
            received = await UncheckedTransmitAsync(GenerateGetWordCommand(), cancellationToken);
            CheckResultAndThrow(received, CommunicationCommands["Ok4"]);
            return new byte[2] { received[6], received[7] };
        }

        public async Task<byte[]> ReadBytesFromEEPROMAsync(ushort addr, CancellationToken cancellationToken)
        {
            try
            {
                await _memoryAccessSemaphore.WaitAsync(cancellationToken);
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                return await UncheckedReadBytesFromEEPROMAsync(addr, cancellationToken);
            }
            finally
            {
                _dataSendSemaphore.Release();
                _memoryAccessSemaphore.Release();
            }
        }

        public byte ReadByteFromEEPROM(ushort addr)
        {
            return ReadBytesFromEEPROM(addr)[0];
        }

        public async Task<byte> ReadByteFromEEPROMAsync(ushort addr, CancellationToken cancellationToken)
        {
            return (await ReadBytesFromEEPROMAsync(addr, cancellationToken))[0];
        }
        
        protected void UncheckedWriteBytesToEEPROM(ushort addr, byte[] bytes)
        {
            byte[] received = UncheckedTransmit(GenerateSetAddressCommand(addr));
            CheckResultAndThrow(received, CommunicationCommands["Ok3"]);
            received = UncheckedTransmit(GenerateSetWordCommand(bytes[0], bytes[1]));
            CheckResultAndThrow(received, CommunicationCommands["Ok3"]);
        }


        protected async Task UncheckedWriteBytesToEEPROMAsync(ushort addr, byte[] bytes, CancellationToken cancellationToken)
        {
            byte[] received = await UncheckedTransmitAsync(GenerateSetAddressCommand(addr), cancellationToken);
            CheckResultAndThrow(received, CommunicationCommands["Ok3"]);
            received = await UncheckedTransmitAsync(GenerateSetWordCommand(bytes[0], bytes[1]), cancellationToken);
            CheckResultAndThrow(received, CommunicationCommands["Ok3"]);
        }

        public void WriteBytesToEEPROM(ushort addr, byte[] bytes)
        {
            try
            {
                _memoryAccessSemaphore.Wait();
                _dataSendSemaphore.Wait();
                UncheckedWriteBytesToEEPROM(addr, bytes);
            }
            finally
            {
                _dataSendSemaphore.Release();
                _memoryAccessSemaphore.Release();
            }
        }

        public async Task WriteBytesToEEPROMAsync(ushort addr, byte[] bytes, CancellationToken cancellationToken)
        {
            try
            {
                await _memoryAccessSemaphore.WaitAsync(cancellationToken);
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                await UncheckedWriteBytesToEEPROMAsync(addr, bytes, cancellationToken);
            }
            finally
            {
                _dataSendSemaphore.Release();
                _memoryAccessSemaphore.Release();
            }
        }

        public void WriteBytesToEEPROM(ushort addr, byte b1, byte b2)
        {
            WriteBytesToEEPROM(addr, new byte[] { b1, b2 });
        }

        public async Task WriteBytesToEEPROMAsync(ushort addr, byte b1, byte b2, CancellationToken cancellationToken)
        {
            await WriteBytesToEEPROMAsync(addr, new byte[] { b1, b2 }, cancellationToken);
        }

        public void WriteWordToEEPROM(ushort addr, ushort word)
        {
            WriteBytesToEEPROM(addr, (byte)word, (byte)(word >> 8));
        }

        public async Task WriteWordToEEPROMAsync(ushort addr, ushort word, CancellationToken cancellationToken)
        {
            await WriteBytesToEEPROMAsync(addr, (byte)word, (byte)(word >> 8), cancellationToken);
        }

        public void WriteByteToEEPROM(ushort addr, byte b)
        {
            try
            {
                _memoryAccessSemaphore.Wait();
                _dataSendSemaphore.Wait();
                byte[] r = UncheckedReadBytesFromEEPROM(addr);
                r[0] = b;
                UncheckedWriteBytesToEEPROM(addr, r);
            }
            finally
            {
                _dataSendSemaphore.Release();
                _memoryAccessSemaphore.Release();
            }
        }

        public async Task WriteByteToEEPROMAsync(ushort addr, byte b, CancellationToken cancellationToken)
        {
            try
            {
                await _memoryAccessSemaphore.WaitAsync(cancellationToken);
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                byte[] read = await UncheckedReadBytesFromEEPROMAsync(addr, cancellationToken);
                read[0] = b;
                await UncheckedWriteBytesToEEPROMAsync(addr, read, cancellationToken);
            }
            finally
            {
                _dataSendSemaphore.Release();
                _memoryAccessSemaphore.Release();
            }
        }

        public bool ReadBitFromEEPROM(ushort addr, int bit)
        {
            byte[] read = ReadBytesFromEEPROM(addr);
            return ByteHelper.GetBit(read, bit);
        }

        public async Task<bool> ReadBitFromEEPROMAsync(ushort addr, int bit, CancellationToken cancellationToken)
        {
            byte[] read = await ReadBytesFromEEPROMAsync(addr, cancellationToken);
            return ByteHelper.GetBit(read, bit);
        }

        public void WriteBitToEEPROM(ushort addr, int bit, bool value)
        {
            try
            {
                _memoryAccessSemaphore.Wait();
                _dataSendSemaphore.Wait();
                byte[] val = ByteHelper.SetBit(UncheckedReadBytesFromEEPROM(addr), bit, value);
                UncheckedWriteBytesToEEPROM(addr, val);
            }
            finally
            {
                _dataSendSemaphore.Release();
                _memoryAccessSemaphore.Release();
            }
        }

        public async Task WriteBitToEEPROMAsync(ushort addr, int bit, bool value, CancellationToken cancellationToken)
        {
            try
            {
                await _memoryAccessSemaphore.WaitAsync(cancellationToken);
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                byte[] val = await UncheckedReadBytesFromEEPROMAsync(addr, cancellationToken);
                byte[] newVal = ByteHelper.SetBit(val, bit, value);
                await UncheckedWriteBytesToEEPROMAsync(addr, val, cancellationToken);

            }
            finally
            {
                _dataSendSemaphore.Release();
                _memoryAccessSemaphore.Release();
            }
        }

        // Why ReadFloat and return double?
        // I dont know
        public double ReadFloatFromEEPROM(ushort addr)
        {
            try
            {
                _memoryAccessSemaphore.Wait();
                _dataSendSemaphore.Wait();
                byte[] readBytes = new byte[4];
                byte[] read1 = UncheckedReadBytesFromEEPROM(addr);
                byte[] read2 = UncheckedReadBytesFromEEPROM((ushort)(addr + 2));
                readBytes[0] = read1[0];
                readBytes[1] = read1[1];
                readBytes[2] = read2[0];
                readBytes[3] = read2[1];
                return RawBytesConverter.ReadDouble(readBytes);
            }
            finally
            {
                _dataSendSemaphore.Release();
                _memoryAccessSemaphore.Release();
            }
        }

        public async Task<double> ReadFloatFromEEPROMAsync(ushort addr, CancellationToken cancellationToken)
        {
            try
            {
                await _memoryAccessSemaphore.WaitAsync(cancellationToken);
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
                _memoryAccessSemaphore.Release();
            }
        }

        public void WriteFloatToEEPROM(ushort addr, double value)
        {
            try
            {
                _memoryAccessSemaphore.Wait();
                _dataSendSemaphore.Wait();
                byte[] bytesValue = new byte[4];
                RawBytesConverter.WriteDouble(value, bytesValue, 0);
                UncheckedWriteBytesToEEPROM(addr, new byte[2] { bytesValue[0], bytesValue[1] });
                UncheckedWriteBytesToEEPROM(addr, new byte[2] { bytesValue[2], bytesValue[3] });
            }
            finally
            {
                _dataSendSemaphore.Release();
                _memoryAccessSemaphore.Release();
            }
        }

        public async Task WriteFloatToEEPROMAsync(ushort addr, double value, CancellationToken cancellationToken)
        {
            try
            {
                await _memoryAccessSemaphore.WaitAsync(cancellationToken);
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                byte[] bytesValue = new byte[4];
                RawBytesConverter.WriteDouble(value, bytesValue, 0);
                await UncheckedWriteBytesToEEPROMAsync(addr, new byte[2] { bytesValue[0], bytesValue[1] }, cancellationToken);
                await UncheckedWriteBytesToEEPROMAsync(addr, new byte[2] { bytesValue[2], bytesValue[3] }, cancellationToken);
            }
            finally
            {
                _dataSendSemaphore.Release();
                _memoryAccessSemaphore.Release();
            }
        }

        public ushort ReadUshortFromEEPROM(ushort addr)
        {
            byte[] read = ReadBytesFromEEPROM(addr);
            return (ushort)((int)read[0] | (int)read[1] << 8);
        }

        public async Task<ushort> ReadUshortFromEEPROMAsync(ushort addr, CancellationToken cancellationToken)
        {
            byte[] read = await ReadBytesFromEEPROMAsync(addr, cancellationToken);
            return (ushort)((int)read[0] | (int)read[1] << 8);
        }

        public void WriteUshortToEEPROM(ushort addr, ushort value)
        {
            byte[] write = new byte[2];
            write[0] = (byte)value;
            write[1] = (byte)(value >> 8);
            WriteBytesToEEPROM(addr, write);
        }

        public async Task WriteUshortToEEPROMAsync(ushort addr, ushort value, CancellationToken cancellationToken)
        {
            byte[] write = new byte[2];
            write[0] = (byte)value;
            write[1] = (byte)(value >> 8);
            await WriteBytesToEEPROMAsync(addr, write, cancellationToken);
        }
    }
}

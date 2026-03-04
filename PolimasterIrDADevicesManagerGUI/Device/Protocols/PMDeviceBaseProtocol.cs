using InTheHand.Net;
using InTheHand.Net.Sockets;
using PolimasterIrDADevicesManagerGUI.Exceptions;
using PolimasterIrDADevicesManagerGUI.IrDA;

namespace PolimasterIrDADevicesManagerGUI.Device.Protocols
{
    /// <summary>
    /// Base protocol
    /// </summary>
    /**
     * As far as I understand, microcontroller's commands are standartized, where there are different type values stored at different addresses
     * 
     * For example, EEPROMAccessProtocolDevice offers a access to device's EEPROM and it works by setting a pointer to EEPROM at address 0 and then
     * On every cycle the device reads the values from the eeprom and stores them at address 156, which then read by McRead at address 156
     **/
    public abstract class PMDeviceBaseProtocol : IrDADevice
    {


        public PMDeviceBaseProtocol(IrDAClient irdaClient, IrDAEndPoint endpoint) : base(irdaClient, endpoint)
        {
            SetupCommunicationCommands();
        }

        private void SetupCommunicationCommands()
        {
            //CommunicationCommands["Ok"] =       new byte[] { 160, 0, 5, 178 };
            CommunicationCommands["Ok1"] =        new byte[] { 160, 0, 12, 114, 0, 9 };
            CommunicationCommands["Ok2"] =        new byte[] { 160, 0, 8, 242 };
            CommunicationCommands["Ok3"] =        new byte[] { 160, 0, 3 };
            CommunicationCommands["Ok4"] =        new byte[] { 160, 0, 8, 114, 0, 5 };
            CommunicationCommands["Ok5"] =        new byte[] { 160, 0, 8, 114, 0 };
            CommunicationCommands["OkArr"] =      new byte[] { 160, 0, 0, 114, 0, 0, 0 };
            CommunicationCommands["OkNext"] =     new byte[] { 144, 0, 54, 114, 0 };

            CommunicationCommands["McRead"] =     new byte[] { 131, 0, 5, 177, 0 }; // Address at the end
            CommunicationCommands["McWrite2"] =   new byte[] { 130, 0, 10, 177, 0, 114, 0, 5, 0, 0 }; // Address after 177 and before 114
            CommunicationCommands["McWriteArr"] = new byte[] { 130, 0, 10, 177, 0, 242 }; // Address after 177 and before 242
            CommunicationCommands["McReadNext"] = new byte[] { 131, 0, 3, 177, 0 };
        }
        
        public async Task WriteArrToMCAsync(byte address, byte[] data, CancellationToken cancellationToken)
        {
            List<byte> toSend = new List<byte>(CommunicationCommands["McWriteArr"]);
            toSend.AddRange(data);
            toSend[4] = address;
            await TransmitAndCheckAsync(toSend.ToArray(), CommunicationCommands["Ok3"], cancellationToken);
        }

        public async Task Write2ToMCAsync(byte address, byte[] data, CancellationToken cancellationToken)
        {
            if (data.Length != 2)
            {
                throw new ArgumentException("Must be 2 bytes long", nameof(data));
            }
            byte[] send = CommunicationCommands["McWrite2"];
            send[4] = address;
            send[8] = data[0];
            send[9] = data[1];
            await TransmitAndCheckAsync(send, CommunicationCommands["Ok3"], cancellationToken);
        } 

        public async Task<byte[]> ReadFromMCAsync(byte address, CancellationToken cancellationToken)
        {
            byte[] send = CommunicationCommands["McRead"].ToArray();
            send[4] = address;
            byte[] received = await TransmitAsync(send, cancellationToken);
            return GetBytesFromResponse(received);
        }

        public async Task<byte[]> ReadArrFromMCAsync(byte address, CancellationToken cancellationToken)
        {
            try
            {
                await _dataSendSemaphore.WaitAsync(cancellationToken);

                List<byte> total = new List<byte>();
                byte[] send = CommunicationCommands["McRead"];
                send[4] = address;
                byte[] received = await UncheckedTransmitAsync(send, cancellationToken);
                int receiveCount = 1;
                while (receiveCount > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    receiveCount = GetSizeOfNextChunk(received);
                    for (int i = 5; i < receiveCount; i++)
                    {
                        total.Add(received[i]);
                    }
                    received = await UncheckedTransmitAsync(CommunicationCommands["McReadNext"], cancellationToken);
                }
                return total.ToArray();
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
        }
        

        private int GetSizeOfNextChunk(IList<byte> buff)
        {
            byte[] array = CommunicationCommands["OkNext"];
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
            if (buff[0] == 144 || buff[0] == 160)
            {
                return 256 * buff[4] + buff[5] - 3;
            }
            return -1;
        }

        /// <summary>
        /// Checks the response and returns the appropriate data section
        /// </summary>
        /// <param name="received">Response to parse</param>
        /// <returns>Returned data</returns>
        /// <exception cref="Exception">Reply was in bad format</exception>
        private byte[] GetBytesFromResponse(byte[] received)
        {
            if (CheckResult(received, CommunicationCommands["Ok5"])) // Ok5 resp
            {
                return new byte[2] { received[6], received[7] };
            }
            if (CheckResult(received, CommunicationCommands["Ok4"])) // Ok4 resp
            {
                return new byte[2] { received[6], received[7] };
            }
            if (CheckResult(received, CommunicationCommands["Ok2"])) // Ok2 resp
            {
                return new byte[4] { received[4], received[5], received[6], received[7] };
            }
            // OkArr resp

            try
            {
                bool flagOkArray = true;
                byte[] okArray = CommunicationCommands["OkArr"].ToArray();
                okArray[1] = (byte)((short)received.Length >> 8);
                okArray[2] = (byte)received.Length;
                okArray[4] = (byte)((short)(received.Length - 3) >> 8);
                okArray[5] = (byte)(received.Length - 3);
                for (int j = 0; j < 6; j++)
                {
                    if (received[j] != okArray[j])
                    {
                        flagOkArray = false;
                    }
                }
                if (flagOkArray)
                {
                    byte[] ret = new byte[received.Length - 6];
                    Buffer.BlockCopy(received, 6, ret, 0, ret.Length);
                    return ret;
                }
            }
            catch(IndexOutOfRangeException)
            {
                // Empty
            }

            throw new UnrecognizedReplyException("Unknown response format");
        }

        public async Task<ushort> ReadUshortFromMCAsync(byte address, CancellationToken cancellationToken)
        {
            byte[] array = await ReadFromMCAsync(address, cancellationToken);
            return (ushort)(array[0] | array[1] << 8);
        }

        public async Task WriteUshortToMCAsync(byte address, ushort value, CancellationToken cancellationToken)
        {
            await Write2ToMCAsync(address, new byte[2] { (byte)value, (byte)(value >> 8) }, cancellationToken);
        }
    }
}

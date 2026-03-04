using InTheHand.Net;
using InTheHand.Net.Sockets;
using PolimasterIrDADevicesManagerGUI.Exceptions;
using System.Diagnostics;
using System.Net.Sockets;

namespace PolimasterIrDADevicesManagerGUI.IrDA
{

    /**
     * Unchecked here means that the data stream has not been checked to be used.
     * All of the checks which would prevent other data to slip in are supposed to be implemented on top of the Unchedked functions
     **/
    /// <summary>
    /// Basic response-reply based IrDA transmission class
    /// </summary>
    public abstract class IrDADevice : IDisposable
    {

        public bool Disposed { get; private set; } = false;

        public readonly IrDAClient IrDAClient;

        public NetworkStream? IrDAStream { get; private set; } = null;

        public readonly IrDAEndPoint DeviceEndPoint;

        protected static readonly Dictionary<string, byte[]> CommunicationCommands = new Dictionary<string, byte[]>();

        protected SemaphoreSlim _dataSendSemaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Wait for response for this amount of milliseconds
        /// </summary>
        public static int ResponseTimeout = 750;

        /// <summary>
        /// Attempts to resend data this amount of times
        /// </summary>
        public static int ResendAttempts = 2;

        public static bool DebugIrda = false;

        public IrDADevice(IrDAClient irdaClient, IrDAEndPoint endpoint)
        {
            IrDAClient = irdaClient;
            DeviceEndPoint = endpoint;
            
            Task.Run(async () =>
            {
                while (!Disposed)
                {
                    try
                    {
                        ConnectIfNotConnected();
                    }
                    catch (Exception)
                    {
                    }
                    await Task.Delay(100);
                }
            });
        }

        // Send a sequence of commands, IN ORDER, FOLLOWED BY EACH OTHER!!
        public async Task<byte[][]> TransmitSequenceAsync(byte[][] sendSequence, CancellationToken cancellationToken)
        {
            try
            {
                List<byte[]> responses = new List<byte[]>(sendSequence.Length);
                if (DebugIrda) Console.Write("P");
                await _dataSendSemaphore.WaitAsync(cancellationToken);
                foreach (byte[] send in sendSequence)
                {
                    responses.Add(await UncheckedTransmitAsync(send, cancellationToken));
                    if (DebugIrda) Console.WriteLine();
                }
                return responses.ToArray();
            }
            finally
            {
                _dataSendSemaphore.Release();
            }
        }

        protected async Task<byte[]> UncheckedTransmitAsync(byte[] send, CancellationToken cancellationToken)
        {
            if (DebugIrda) Console.Write("! C");
            while (!IrDAClient.Connected)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(1);
            }

            if (DebugIrda) Console.Write("! F");
            int fCounter = 0;
            while (IrDAStream.DataAvailable) // Flush the stream if it already had any garbage data in it
            {
                cancellationToken.ThrowIfCancellationRequested();
                _ = IrDAStream.ReadByte();
                fCounter += 1;
            }

            if (DebugIrda) Console.Write("!({0}) S({1})", fCounter, string.Join(" ", send.ToList().ConvertAll(x => x.ToString()).ToArray()));
            await IrDAStream.WriteAsync(send, cancellationToken); // Write our data

            if (DebugIrda) Console.Write("! W");
            List<byte> receive = new List<byte>();
            Stopwatch watch = Stopwatch.StartNew();
            int elapsed = 0;
            int resendAttempt = 0;
            while (!IrDAStream.DataAvailable) // Waiting for the response (or for the cancelling), for me takes like 130 ms
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(1); // DataAvailable is slow, so the delay here works very good at reducing CPU usage
                elapsed++;
                // Timeouted
                if (elapsed >= ResponseTimeout)
                {
                    if (resendAttempt >= ResendAttempts)
                    {
                        throw new TimeoutException(string.Format("No response from device after {0} attemps", resendAttempt));
                    }
                    elapsed = 0;
                    resendAttempt++;
                    if (DebugIrda) Console.WriteLine("! S");
                    await IrDAStream.WriteAsync(send, cancellationToken);
                    if (DebugIrda) Console.WriteLine("!({0}) W", resendAttempt);
                }
            }
            watch.Stop();
            if (DebugIrda) Console.Write("!({0}) R", watch.ElapsedMilliseconds);
            while (IrDAStream.DataAvailable) // Reading data byte by byte
            {
                cancellationToken.ThrowIfCancellationRequested();
                receive.Add((byte)IrDAStream.ReadByte());
            }
            if (DebugIrda) Console.Write("!({0})", string.Join(" ", receive.ToList().ConvertAll(x => x.ToString()).ToArray()));
            return receive.ToArray();
        }
        public async Task<byte[]> TransmitAsync(byte[] send, CancellationToken cancellationToken)
        {
            /**
             * Note to all future edits: async here doesnt work with the IrDAClient.Client, only through the stream
             * Or at least it didnt work for me
             **/
            try
            {
                if (DebugIrda) Console.Write("P");
                await _dataSendSemaphore.WaitAsync(cancellationToken); // One operation at a time so we are waiting until we are cleared to use
                return await UncheckedTransmitAsync(send, cancellationToken);
            }
            finally
            {
                _dataSendSemaphore.Release();
                if (DebugIrda) Console.WriteLine();
            }
        }

        protected async Task<byte[]> TransmitCommandAsync(string commandName, CancellationToken cancellationToken)
        {
            return await TransmitAsync(CommunicationCommands[commandName], cancellationToken);
        }

        protected async Task<byte[]> TransmitCommandAndCheckAsync(string commandName, string okName, CancellationToken cancellationToken)
        {
            byte[] bytes = await TransmitCommandAsync(commandName, cancellationToken);
            CheckResultAndThrow(bytes, CommunicationCommands[okName]);
            return bytes;
        }

        protected async Task<byte[]> TransmitAndCheckAsync(byte[] send, byte[] check, CancellationToken cancellationToken)
        {
            byte[] bytes = await TransmitAsync(send, cancellationToken);
            CheckResultAndThrow(bytes, check);
            return bytes;
        }

        protected bool CheckResult(byte[] bytes, byte[] check)
        {
            for (int i = 0; i < check.Length; i++)
            {
                if (bytes[i] != check[i]) return false;
            }
            return true;
        }

        protected void CheckResultAndThrow(byte[] bytes, byte[] check)
        {
            if (!CheckResult(bytes, check))
            {
                throw new ResultCheckFailedException();
            }
        }

        public virtual void ConnectIfNotConnected()
        {
            if (!IrDAClient.Connected)
            {
                IrDAClient.Connect(DeviceEndPoint);
                IrDAStream = IrDAClient.GetStream();
            }
            if (IrDAStream == null || !IrDAStream.Socket.Connected)
            {
                IrDAStream?.Dispose();
                IrDAStream = IrDAClient.GetStream();
            }
        }

        public virtual void Close()
        {
            if (IrDAStream != null)
            {
                IrDAStream.Close();
            }
            if (IrDAClient.Connected)
            {
                IrDAClient.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (IrDAStream != null)
            {
                IrDAStream.Close();
                IrDAStream.Dispose();
            }

            if (IrDAClient.Connected)
            {
                IrDAClient.Client.Disconnect(false);
            }

            IrDAClient.Close();
            IrDAClient.Dispose();

            Disposed = true;
        }
    }
}

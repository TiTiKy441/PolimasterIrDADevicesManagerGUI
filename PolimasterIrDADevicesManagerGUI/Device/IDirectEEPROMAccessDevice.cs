using System.Runtime.Intrinsics.X86;

namespace PolimasterIrDADevicesManagerGUI.Device
{
    /// <summary>
    /// Represents a device with direct access to it's EEPROM via a user usable interface
    /// </summary>
    public interface IDirectEEPROMAccessDevice
    {

        public abstract ushort ReadWordFromEEPROM(ushort addr);

        Task<ushort> ReadWordFromEEPROMAsync(ushort addr, CancellationToken cancellationToken);

        public abstract byte[] ReadBytesFromEEPROM(ushort addr);

        Task<byte[]> ReadBytesFromEEPROMAsync(ushort addr, CancellationToken cancellationToken);

        public abstract byte ReadByteFromEEPROM(ushort addr);

        Task<byte> ReadByteFromEEPROMAsync(ushort addr, CancellationToken cancellationToken);

        public abstract void WriteBytesToEEPROM(ushort addr, byte[] bytes);

        Task WriteBytesToEEPROMAsync(ushort addr, byte[] bytes, CancellationToken cancellationToken);

        public abstract void WriteBytesToEEPROM(ushort addr, byte b1, byte b2);

        Task WriteBytesToEEPROMAsync(ushort addr, byte b1, byte b2, CancellationToken cancellationToken);

        public abstract void WriteWordToEEPROM(ushort addr, ushort word);

        Task WriteWordToEEPROMAsync(ushort addr, ushort word, CancellationToken cancellationToken);

        public abstract void WriteByteToEEPROM(ushort addr, byte b);

        Task WriteByteToEEPROMAsync(ushort addr, byte b, CancellationToken cancellationToken);


    }
}

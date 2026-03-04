using System.Runtime.Intrinsics.X86;

namespace PolimasterIrDADevicesManagerGUI.Device
{
    /// <summary>
    /// Represents a device with direct access to it's EEPROM via a user usable interface
    /// </summary>
    public interface IDirectEEPROMAccessDevice
    {

        Task<ushort> ReadUshortFromEEPROMAsync(ushort addr, CancellationToken cancellationToken);

        Task WriteUshortToEEPROMAsync(ushort addr, ushort word, CancellationToken cancellationToken);


        Task<byte[]> ReadBytesFromEEPROMAsync(ushort addr, CancellationToken cancellationToken);

        Task WriteBytesToEEPROMAsync(ushort addr, byte[] bytes, CancellationToken cancellationToken);



        Task<byte> ReadByteFromEEPROMAsync(ushort addr, CancellationToken cancellationToken);

        Task WriteByteToEEPROMAsync(ushort addr, byte b, CancellationToken cancellationToken);

    }
}

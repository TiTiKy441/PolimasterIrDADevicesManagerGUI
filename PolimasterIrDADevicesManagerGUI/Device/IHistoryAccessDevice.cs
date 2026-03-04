namespace PolimasterIrDADevicesManagerGUI.Device
{
    public interface IHistoryAccessDevice
    {

        Task<HistoryEventRecord[]> ReadHistoryAsync(CancellationToken cancellationToken);

        Task ClearHistoryAsync(CancellationToken cancellationToken);

    }
}

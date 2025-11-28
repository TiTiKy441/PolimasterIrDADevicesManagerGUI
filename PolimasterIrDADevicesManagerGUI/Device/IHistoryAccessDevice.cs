namespace PolimasterIrDADevicesManagerGUI.Device
{
    public interface IHistoryAccessDevice
    {

        public abstract HistoryEventRecord[] ReadHistory();

        Task<HistoryEventRecord[]> ReadHistoryAsync(CancellationToken cancellationToken);

        public abstract void ClearHistory();

        Task ClearHistoryAsync(CancellationToken cancellationToken);

    }
}

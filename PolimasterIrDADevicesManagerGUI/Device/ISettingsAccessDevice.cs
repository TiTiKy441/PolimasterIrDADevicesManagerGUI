using PolimasterIrDADevicesManagerGUI.Device;

namespace PolimasterIrDADevicesManagerGUI.Device
{
    public interface ISettingsAccessDevice : IDisposable
    {
        
        Task WriteParameterAsync(int parameter, object value, CancellationToken cancellationToken);

        Task WriteParameterAsStringAsync(int parameter, string value, CancellationToken cancellationToken);

        Task<object> ReadParameterAsync(int parameter, CancellationToken cancellationToken);

        public abstract DeviceParameterInfo[] GetSupportedParameters();

        public abstract DeviceParameterInfo FindParameterByType(int type);

    }
}

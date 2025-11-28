using PolimasterIrDADevicesManagerGUI.Device;

namespace PolimasterIrDADevicesManagerGUI.Device
{
    public interface ISettingsAccessDevice : IDisposable
    {

        public void WriteParameter(int parameter, object value);

        public void WriteParameterAsString(int parameter, string value);

        Task WriteParameterAsync(int parameter, object value, CancellationToken cancellationToken);

        Task WriteParameterAsStringAsync(int parameter, string value, CancellationToken cancellationToken);

        public object ReadParameter(int parameter);

        Task<object> ReadParameterAsync(int parameter, CancellationToken cancellationToken);

        public abstract DeviceParameterInfo[] GetSupportedParameters();

        public abstract DeviceParameterInfo FindParameterByType(int type);

    }
}

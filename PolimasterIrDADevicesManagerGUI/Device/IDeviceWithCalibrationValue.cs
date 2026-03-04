namespace PolimasterIrDADevicesManagerGUI.Device
{
    interface IDeviceWithCalibrationValue
    {

        public Task<double> ReadCalibrationValueAsync(CancellationToken cancellationToken);

        public Task WriteCalibrationValueAsync(double value, CancellationToken cancellationToken);

    }
}

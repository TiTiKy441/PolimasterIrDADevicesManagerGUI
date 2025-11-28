namespace PolimasterIrDADevicesManagerGUI.Device
{
    interface IDeviceWithCalibrationValue
    {

        public double ReadCalibrationValue();

        public Task<double> ReadCalibrationValueAsync(CancellationToken cancellationToken);

        public void WriteCalibrationValue(double value);

        public Task WriteCalibrationValueAsync(double value, CancellationToken cancellationToken);

    }
}

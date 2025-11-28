namespace PolimasterIrDADevicesManagerGUI.Device
{
    public abstract class HistoryEventRecord
    {
        /// <summary>
        /// Event timestamp
        /// </summary>
        public DateTime TimeStamp;

        public int Type;

        public int GeneralType;

        public virtual string GetTypeAsString()
        {
            return Type.ToString();
        }

        public static int GetNumberOfSupportedValues()
        {
            return 0;
        }

        public virtual object?[] GetStandartizedValues()
        {
            return Array.Empty<object?>();
        }

        public virtual string[] GetStandartizedValuesUnits()
        {
            return Array.Empty<string>();
        }
    }

    public enum GeneralEventType
    {
        Alarm = 0,
        SettingsChange = 1,
        TurnOn = 2,
        TurnOff = 3,
    }
}

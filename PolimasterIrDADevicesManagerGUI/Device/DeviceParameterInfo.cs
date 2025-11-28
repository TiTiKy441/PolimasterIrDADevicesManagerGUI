namespace PolimasterIrDADevicesManagerGUI.Device
{
    public class DeviceParameterInfo
    {

        public readonly int Id;

        public readonly string Name;

        public readonly bool Editable;

        public readonly Type ValueType;

        public readonly string? Units;

        public DeviceParameterInfo(int id, string name, bool editable, Type type, string? unit = null)
        {
            Id = id;
            Name = name;
            Editable = editable;
            ValueType = type;
            Units = unit;
        }
    }
}

using System.Management;
using System.Runtime.Versioning;

namespace DisplayDetective.Library.Common;

public sealed class Display : IDisplay
{
    public static IDisplay Create(string deviceID, string name, string manufacturer, string description)
    {
        return new Display(deviceID, name, manufacturer, description);
    }

    [SupportedOSPlatform("windows")]
    public static IDisplay Create(ManagementBaseObject instance)
    {
        var deviceID = instance[nameof(DeviceID)]?.ToString() ?? throw NewArgumentException(instance, nameof(DeviceID));
        var name = instance[nameof(Name)]?.ToString() ?? throw NewArgumentException(instance, nameof(Name));
        var manufacturer = instance[nameof(Manufacturer)]?.ToString() ?? throw NewArgumentException(instance, nameof(Manufacturer));
        var description = instance[nameof(Description)]?.ToString() ?? throw NewArgumentException(instance, nameof(Description));
        return new Display(deviceID, name, manufacturer, description);
    }

    private static ArgumentException NewArgumentException(ManagementBaseObject instance, string property)
    {
        return new ArgumentException($"{typeof(ManagementBaseObject)} {property} not found", nameof(instance));
    }

    public string DeviceID { get; }

    public string Name { get; }

    public string Manufacturer { get; }

    public string Description { get; }

    private Display(string deviceID, string name, string manufacturer, string description)
    {
        DeviceID = deviceID;
        Name = name;
        Manufacturer = manufacturer;
        Description = description;
    }

    public override string ToString() => $"Name={Name} Manufacturer={Manufacturer} Description={Description} DeviceID={DeviceID}";

    public string ToMultilineString()
    {
        return $"""
                Name = {Name}
        Manufacturer = {Manufacturer}
         Description = {Description}
            DeviceID = {DeviceID}
        """;
    }
}
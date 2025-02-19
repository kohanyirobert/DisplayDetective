namespace DisplayDetective.Library.Common;

public interface IDisplay
{
    string DeviceID { get; }
    string Name { get; }
    string Manufacturer { get; }
    string Description { get; }
    string ToString();
    string ToMultilineString();
}
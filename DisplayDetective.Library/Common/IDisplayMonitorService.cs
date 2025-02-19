namespace DisplayDetective.Library.Common;

public interface IDisplayMonitorService
{
    event EventHandler<IDisplay> OnDisplayCreated;

    event EventHandler<IDisplay> OnDisplayDeleted;
}
using System.Management;
using System.Runtime.Versioning;

using DisplayDetective.Library.Common;

namespace DisplayDetective.Library.Windows;

[SupportedOSPlatform("windows")]
internal sealed class WindowsDisplayMonitorService : IDisplayMonitorService
{
    public event EventHandler<IDisplay> OnDisplayCreated = delegate { };

    public event EventHandler<IDisplay> OnDisplayDeleted = delegate { };

    private static readonly ManagementScope Scope = new(ManagementPath.DefaultPath);

    private static readonly WqlEventQuery CreationQuery = new(@"
            SELECT * FROM __InstanceCreationEvent WITHIN 5
            WHERE TargetInstance ISA 'Win32_PnPEntity'
            AND TargetInstance.PNPClass = 'Monitor'");

    private static readonly WqlEventQuery DeletionQuery = new(@"
            SELECT * FROM __InstanceDeletionEvent WITHIN 5 
            WHERE TargetInstance ISA 'Win32_PnPEntity' 
            AND TargetInstance.PNPClass = 'Monitor'");

    private readonly ManagementEventWatcher _creationWatcher;
    private readonly ManagementEventWatcher _deletionWatcher;

    internal WindowsDisplayMonitorService()
    {
        Scope.Connect();

        _creationWatcher = new ManagementEventWatcher(Scope, CreationQuery);
        _creationWatcher.EventArrived += (sender, e) =>
        {
            var instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            OnDisplayCreated.Invoke(this, Display.Create(instance));
        };

        _deletionWatcher = new ManagementEventWatcher(Scope, DeletionQuery);
        _deletionWatcher.EventArrived += (sender, e) =>
        {
            var instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            OnDisplayDeleted.Invoke(this, Display.Create(instance));
        };

        _creationWatcher.Start();
        _deletionWatcher.Start();
    }
}
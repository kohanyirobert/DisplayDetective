using System.Diagnostics;

namespace DisplayDetective.Library.Common;

public interface ICommandRunnerService
{
    Process Run(string commandFileName, IEnumerable<string> commandArguments, CancellationToken token);
}
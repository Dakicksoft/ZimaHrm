using System;

namespace ZimaHrm.Core.Infrastructure.Versioning
{
    public interface IVersionProvider
    {
        string Name { get; }
        Guid ApplicationId { get; }
        Version CurrentVersion { get; }
    }
}

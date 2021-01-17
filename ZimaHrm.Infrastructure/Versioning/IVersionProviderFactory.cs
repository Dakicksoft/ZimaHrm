using System.Collections.Generic;

namespace ZimaHrm.Core.Infrastructure.Versioning
{
    public interface IVersionProviderFactory
    {
        IEnumerable<IVersionProvider> VersionProviders { get; }
        IVersionProvider Get(string name);

    }
}

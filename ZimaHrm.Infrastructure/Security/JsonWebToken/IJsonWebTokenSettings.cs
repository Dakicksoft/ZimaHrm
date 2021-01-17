using System;

namespace ZimaHrm.Core.Infrastructure.Security
{
    public interface IJsonWebTokenSettings
    {
        string Audience { get; }

        TimeSpan Expires { get; }

        string Issuer { get; }

        string Key { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ZimaHrm.Core.Infrastructure.Helper
{
    public static class IdHelper
    {
        public static Guid GenerateId(string guid = "")
        {
            return string.IsNullOrEmpty(guid) ? Guid.NewGuid() : new Guid(guid);
        }
    }
}

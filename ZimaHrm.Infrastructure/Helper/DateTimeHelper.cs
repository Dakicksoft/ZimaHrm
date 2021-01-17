using System;
using System.Collections.Generic;
using System.Text;

namespace ZimaHrm.Core.Infrastructure.Helper
{
    public static class DateTimeHelper
    {
        public static DateTime GenerateDateTime()
        {
            return DateTimeOffset.Now.UtcDateTime;
        }
    }
}

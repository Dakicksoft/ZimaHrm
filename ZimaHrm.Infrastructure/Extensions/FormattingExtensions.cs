using System;
using System.Collections.Generic;
using System.Text;

namespace ZimaHrm.Core.Infrastructure.Extensions
{
    public static class FormattingExtensions
    {
        public static string ToHumanReadableSize(this long len)
        {
            string[] sizes = { "Bytes", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }
            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }
    }
}

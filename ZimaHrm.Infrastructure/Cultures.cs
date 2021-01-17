﻿using System.Globalization;

namespace ZimaHrm.Core.Infrastructure
{
    /// <summary>
    /// Cultures supported by our framework.
    /// </summary>
    public static class Cultures
    {
        /// <summary>
        /// English (United States)
        /// </summary>
        public static CultureInfo EnglishUnitedStates { get { return CultureInfo.GetCultureInfo("en-US"); } }

        /// <summary>
        /// Spanish (Spain)
        /// </summary>
        public static CultureInfo TurkishSpain { get { return CultureInfo.GetCultureInfo("tr-TR"); } }
    }
}

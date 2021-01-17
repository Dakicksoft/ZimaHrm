﻿using System;

namespace ZimaHrm.Core.Infrastructure.Utility
{
    public static class EnumUtility
    {
        public static T SafeParse<T>(string value, T defaultValue)
            where T : struct
        {
            if (!Enum.TryParse(value, out T result))
            {
                result = defaultValue;
            }

            return result;
        }

        public static T SafeParseFlags<T>(string value, T defaultValue)
            where T : struct
        {
            return SafeParseFlags(value, defaultValue, ",");
        }

        public static T SafeParseFlags<T>(string value, T defaultValue, string separator)
            where T : struct
        {
            if (!typeof(T).IsDefined(typeof(FlagsAttribute), false))
            {
                throw new ArgumentException($"{typeof(T).FullName} type should have [Flags] attribute.");
            }

            var result = 0;
            var wasAssigned = false;

            if (!string.IsNullOrEmpty(value))
            {
                var parts = value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    if (Enum.TryParse(part.Trim(), true, out T partResult))
                    {
                        result |= Convert.ToInt32(partResult);
                        wasAssigned = true;
                    }
                }
            }

            return wasAssigned ? (T)Enum.ToObject(typeof(T), result) : defaultValue;
        }
    }
}
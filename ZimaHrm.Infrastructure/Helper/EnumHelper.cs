using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ZimaHrm.Core.Infrastructure.Helper
{
    public static class EnumHelper
    {
        public static IDictionary<Enum, string> ToDictionary(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var dics = new Dictionary<Enum, string>();
            var enumValues = Enum.GetValues(type);

            foreach (Enum value in enumValues)
            {
                dics.Add(value, GetDisplayName(value));
            }

            return dics;
        }

        public static string GetDisplayName(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var displayName = value.ToString();
            var fieldInfo = value.GetType().GetField(displayName);
            var attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes.Length > 0)
            {
                displayName = attributes[0].Description;
            }

            return displayName;
        }

        public static IEnumerable<KeyValueObject<TKey>> GetEnumKeyValue<TEnum, TKey>()
         where TKey : class
        {
            var metas = GetMetadata<TEnum, TKey>();
            var results = metas.Item1.Zip(metas.Item2, (key, value) =>
                new KeyValueObject<TKey>
                (
                    key,
                    value
                )
            );
            return results;
        }

        public static (IEnumerable<TKey>, IEnumerable<string>) GetMetadata<TEnum, TKey>()
        {
            var keyArray = (TKey[])Enum.GetValues(typeof(TEnum));
            var nameArray = Enum.GetNames(typeof(TEnum));

            IList<TKey> keys = new List<TKey>();
            foreach (var item in keyArray) keys.Add(item);

            IList<string> names = new List<string>();
            foreach (var item in nameArray) names.Add(item);

            return (keys, names);
        }

        /// <summary>
        /// Gets the values of the specified enumeration type.
        /// </summary>
        // C# doesn't allow constraining the type to an Enum.
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}

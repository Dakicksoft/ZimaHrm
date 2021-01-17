using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ZimaHrm.Core.Infrastructure.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool ContainsKeyAndValueIsNullOrEmpty(this Dictionary<string, object> map, string key)
        {
            return map.ContainsKey(key) && (map[key] == null || string.IsNullOrEmpty(map[key].ToString()));
        }

        public static void ApplyIf<TKey, TValue>(this Dictionary<TKey, TValue> map1, Dictionary<TKey, TValue> map2)
        {
            foreach (var pair in map2)
                if (!map1.ContainsKey(pair.Key))
                    map1.Add(pair.Key, pair.Value);
        }

        public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> map, IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            if (keys.Count() != values.Count())
                throw new ArgumentException("Keys and values must be matching length.");

            var keyEnumerator = keys.GetEnumerator();
            var valueEnumerator = values.GetEnumerator();

            while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
            {
                if (!map.ContainsKey(keyEnumerator.Current))
                    map.Add(keyEnumerator.Current, valueEnumerator.Current);
            }
        }

        public static void AddDataIfNotEmpty(this Dictionary<string, string> dictionary, XDocument document, string elementName)
        {
            var element = document.Root.Element(elementName);
            if (element != null)
                dictionary.AddItemIfNotEmpty(elementName, element.Value);
        }

        public static void AddItemIfNotEmpty(this IDictionary<string, string> dictionary, string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (!String.IsNullOrEmpty(value))
                dictionary[key] = value;
        }

        /// <summary>
        /// Adds or overwrites the existing value.
        /// </summary>
        public static void AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
        }

        public static IDictionary<T1, T2> Merge<T1, T2>(this IDictionary<T1, T2> first, IDictionary<T1, T2> second)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");

            var merged = new Dictionary<T1, T2>();
            first.ToList().ForEach(kv => merged[kv.Key] = kv.Value);
            second.ToList().ForEach(kv => merged[kv.Key] = kv.Value);

            return merged;
        }

        public static bool ContainsKeyWithValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, params TValue[] values)
        {
            if (dictionary == null || values == null || values.Length == 0)
                return false;

            TValue temp;
            try
            {
                if (!dictionary.TryGetValue(key, out temp))
                    return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }

            return values.Any(v => v.Equals(temp));
        }

        public static int GetCollectionHashCode<TValue>(this IDictionary<string, TValue> source, IList<string> exclusions = null)
        {
            var assemblyQualifiedName = typeof(TValue).AssemblyQualifiedName;
            int hashCode = assemblyQualifiedName?.GetHashCode() ?? 0;

            var keyValuePairHashes = new List<int>(source.Keys.Count);

            foreach (var key in source.Keys.OrderBy(x => x))
            {
                if (exclusions != null && exclusions.Contains(key))
                    continue;

                var item = source[key];
                unchecked
                {
                    var kvpHash = key.GetHashCode();
                    kvpHash = (kvpHash * 397) ^ item.GetHashCode();
                    keyValuePairHashes.Add(kvpHash);
                }
            }

            keyValuePairHashes.Sort();
            foreach (var kvpHash in keyValuePairHashes)
            {
                unchecked
                {
                    hashCode = (hashCode * 397) ^ kvpHash;
                }
            }

            return hashCode;
        }

        public static bool CollectionEquals<TValue>(this IDictionary<string, TValue> source, IDictionary<string, TValue> other)
        {
            if (source.Count != other.Count)
                return false;

            foreach (var key in source.Keys)
            {
                var sourceValue = source[key];

                TValue otherValue;
                if (!other.TryGetValue(key, out otherValue))
                    return false;

                if (sourceValue.Equals(otherValue))
                    return false;
            }

            return true;
        }
        public static void AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Empty collection");
            }

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
            }
        }

        public static TValue GetValueOrThrow<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, string exceptionMessage)
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                throw new KeyNotFoundException(exceptionMessage);
            }
            return value;
        }
    }
}

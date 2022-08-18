﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace ZimaHrm.Core.Infrastructure.Extensions
{
    /// <summary>
    ///     Extension methods applied to the <see cref="object"/> type.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        ///     Gets a hash of the current instance.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the Cryptographic Service Provider to use.
        /// </typeparam>
        /// <param name="instance">
        ///     The instance being extended.
        /// </param>
        /// <returns>
        ///     A base 64 encoded string representation of the hash.
        /// </returns>
        public static string GetHash<T>(this object instance) where T : HashAlgorithm, new()
        {
            T cryptoServiceProvider = new T();
            return computeHash(instance, cryptoServiceProvider);
        }

        /// <summary>
        ///     Gets a key based hash of the current instance.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the Cryptographic Service Provider to use.
        /// </typeparam>
        /// <param name="instance">
        ///     The instance being extended.
        /// </param>
        /// <param name="key">
        ///     The key passed into the Cryptographic Service Provider algorithm.
        /// </param>
        /// <returns>
        ///     A base 64 encoded string representation of the hash.
        /// </returns>
        public static string GetKeyedHash<T>(this object instance, byte[] key) where T : KeyedHashAlgorithm, new()
        {
            T cryptoServiceProvider = new T { Key = key };
            return computeHash(instance, cryptoServiceProvider);
        }


        private static string computeHash<T>(object instance, T cryptoServiceProvider) where T : HashAlgorithm, new()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(instance.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, instance);
                cryptoServiceProvider.ComputeHash(System.Text.Encoding.UTF8.GetBytes(textWriter.ToString()));
                return Convert.ToBase64String(cryptoServiceProvider.Hash);
            }
        }

        public static Nullable<T> ToNullable<T>(this object obj) where T : struct
        {
            if (obj == null)
            {
                return default(T);
            }
            var objString = obj as string;
            if (objString != null)
            {
                return objString.ToNullable<T>();
            }
            var objType = typeof(T);
            if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                objType = Nullable.GetUnderlyingType(objType);
            }
            return (T)Convert.ChangeType(obj, objType);
        }

        public static bool IsNullOrEmpty(this object obj)
        {
            return obj == null || string.IsNullOrWhiteSpace(obj.ToString());
        }
    }
}

using System;
using System.Collections.Generic;

namespace ZimaHrm.Core.Infrastructure.Extensions
{
  /// <summary>
  /// <see cref="IList{T}"/> extensions.
  /// </summary>
  public static class ListExtensions
  {
    /// <summary>
    /// Add object to <see cref="IList{T}"/> only when not null.
    /// </summary>
    /// <typeparam name="T">The type contained within the <see cref="IList{T}"/>.</typeparam>
    /// <param name="obj">The <see cref="IList{T}"/>.</param>
    /// <param name="item">The item to potentially add.</param>
    /// <exception cref="ArgumentNullException">Thrown if obj is null</exception>
    public static void AddIfNotNull<T>(this IList<T> obj, T item)
    {
      if (obj == null)
      {
        throw new ArgumentNullException($"{nameof(obj)} is null");
      }

      if (item == null)
      {
        return;
      }

      obj.Add(item);
    }

    /// <summary>
    /// Adds a <see cref="IEnumerable{T}"/> to an <see cref="IList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type contained within the <see cref="IList{T}"/>.</typeparam>
    /// <param name="obj">The <see cref="IList{T}"/>.</param>
    /// <param name="items">The items to potentially add.</param>
    /// <exception cref="ArgumentNullException">Thrown if obj is null</exception>
    public static void AddRangeIfNotNull<T>(this IList<T> obj, IEnumerable<T> items)
    {
      if (obj == null)
      {
        throw new ArgumentNullException($"{nameof(obj)} is null");
      }

      if (items == null)
      {
        return;
      }

      if (obj is List<T> objList)
      {
        objList.AddRange(items);
        return;
      }

      foreach (var item in items)
      {
        obj.Add(item);
      }
    }
  }
}

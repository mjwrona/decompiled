// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ReadOnlyEnumerableExtensions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData
{
  internal static class ReadOnlyEnumerableExtensions
  {
    internal static bool IsEmptyReadOnlyEnumerable<T>(this IEnumerable<T> source) => source == ReadOnlyEnumerable<T>.Empty();

    internal static ReadOnlyEnumerable<T> ToReadOnlyEnumerable<T>(
      this IEnumerable<T> source,
      string collectionName)
    {
      return source is ReadOnlyEnumerable<T> readOnlyEnumerable ? readOnlyEnumerable : throw new ODataException(Strings.ReaderUtils_EnumerableModified((object) collectionName));
    }

    internal static ReadOnlyEnumerable<T> GetOrCreateReadOnlyEnumerable<T>(
      this IEnumerable<T> source,
      string collectionName)
    {
      return source.IsEmptyReadOnlyEnumerable<T>() ? new ReadOnlyEnumerable<T>() : source.ToReadOnlyEnumerable<T>(collectionName);
    }

    internal static ReadOnlyEnumerable<T> ConcatToReadOnlyEnumerable<T>(
      this IEnumerable<T> source,
      string collectionName,
      T item)
    {
      ReadOnlyEnumerable<T> readOnlyEnumerable = source.GetOrCreateReadOnlyEnumerable<T>(collectionName);
      readOnlyEnumerable.AddToSourceList(item);
      return readOnlyEnumerable;
    }
  }
}

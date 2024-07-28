// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CollectionsExtensions
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class CollectionsExtensions
  {
    public static TCollection AddRange<T, TCollection>(
      this TCollection collection,
      IEnumerable<T> values)
      where TCollection : ICollection<T>
    {
      foreach (T obj in values)
        collection.Add(obj);
      return collection;
    }

    public static TCollection AddRangeIfRangeNotNull<T, TCollection>(
      this TCollection collection,
      IEnumerable<T> values)
      where TCollection : ICollection<T>
    {
      if (values != null)
        collection.AddRange<T, TCollection>(values);
      return collection;
    }

    public static TCollection Add<TCollection, TItem>(
      this TCollection destination,
      IEnumerable<TItem> source)
      where TCollection : ICollection<TItem>
    {
      if (destination is List<TItem> objList)
      {
        objList.AddRange(source);
        return destination;
      }
      foreach (TItem obj in source)
        destination.Add(obj);
      return destination;
    }
  }
}

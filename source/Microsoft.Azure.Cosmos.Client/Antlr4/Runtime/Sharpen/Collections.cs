// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Sharpen.Collections
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Antlr4.Runtime.Sharpen
{
  internal static class Collections
  {
    public static T[] EmptyList<T>() => Antlr4.Runtime.Sharpen.Collections.EmptyListImpl<T>.Instance;

    public static IDictionary<TKey, TValue> EmptyMap<TKey, TValue>() => Antlr4.Runtime.Sharpen.Collections.EmptyMapImpl<TKey, TValue>.Instance;

    public static ReadOnlyCollection<T> SingletonList<T>(T item) => new ReadOnlyCollection<T>((IList<T>) new T[1]
    {
      item
    });

    public static IDictionary<TKey, TValue> SingletonMap<TKey, TValue>(TKey key, TValue value) => (IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>()
    {
      {
        key,
        value
      }
    };

    private static class EmptyListImpl<T>
    {
      public static readonly T[] Instance = new T[0];
    }

    private static class EmptyMapImpl<TKey, TValue>
    {
      public static IDictionary<TKey, TValue> Instance => (IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>();
    }
  }
}

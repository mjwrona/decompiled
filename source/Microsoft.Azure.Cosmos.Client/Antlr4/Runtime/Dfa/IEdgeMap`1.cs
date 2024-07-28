// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Dfa.IEdgeMap`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System.Collections;
using System.Collections.Generic;

namespace Antlr4.Runtime.Dfa
{
  internal interface IEdgeMap<T> : IEnumerable<KeyValuePair<int, T>>, IEnumerable
  {
    int Count { get; }

    bool IsEmpty { get; }

    bool ContainsKey(int key);

    T this[int key] { get; }

    [return: NotNull]
    IEdgeMap<T> Put(int key, T value);

    [return: NotNull]
    IEdgeMap<T> Remove(int key);

    [return: NotNull]
    IEdgeMap<T> PutAll(IEdgeMap<T> m);

    [return: NotNull]
    IEdgeMap<T> Clear();

    [return: NotNull]
    IDictionary<int, T> ToMap();
  }
}

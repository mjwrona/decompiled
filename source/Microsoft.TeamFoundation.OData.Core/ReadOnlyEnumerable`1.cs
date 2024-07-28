// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ReadOnlyEnumerable`1
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.OData
{
  internal sealed class ReadOnlyEnumerable<T> : ReadOnlyEnumerable, IEnumerable<T>, IEnumerable
  {
    private readonly IList<T> sourceList;
    private static readonly SimpleLazy<ReadOnlyEnumerable<T>> EmptyInstance = new SimpleLazy<ReadOnlyEnumerable<T>>((Func<ReadOnlyEnumerable<T>>) (() => new ReadOnlyEnumerable<T>((IList<T>) new ReadOnlyCollection<T>((IList<T>) new List<T>(0)))), true);

    internal ReadOnlyEnumerable()
      : this((IList<T>) new List<T>())
    {
    }

    internal ReadOnlyEnumerable(IList<T> sourceList)
      : base((IEnumerable) sourceList)
    {
      this.sourceList = sourceList;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.sourceList.GetEnumerator();

    internal static ReadOnlyEnumerable<T> Empty() => ReadOnlyEnumerable<T>.EmptyInstance.Value;

    internal void AddToSourceList(T itemToAdd) => this.sourceList.Add(itemToAdd);
  }
}

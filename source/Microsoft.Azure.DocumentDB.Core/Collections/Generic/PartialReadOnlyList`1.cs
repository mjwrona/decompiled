// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Collections.Generic.PartialReadOnlyList`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Collections.Generic
{
  internal sealed class PartialReadOnlyList<T> : 
    IReadOnlyList<T>,
    IEnumerable<T>,
    IEnumerable,
    IReadOnlyCollection<T>
  {
    private readonly IReadOnlyList<T> list;
    private readonly int startIndex;
    private readonly int count;

    public PartialReadOnlyList(IReadOnlyList<T> list, int count)
      : this(list, 0, count)
    {
    }

    public PartialReadOnlyList(IReadOnlyList<T> list, int startIndex, int count)
    {
      if (list == null)
        throw new ArgumentNullException(nameof (list));
      if (count <= 0 || count > list.Count)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (startIndex < 0 || startIndex + count > list.Count)
        throw new ArgumentOutOfRangeException(nameof (startIndex));
      this.list = list;
      this.startIndex = startIndex;
      this.count = count;
    }

    public T this[int index]
    {
      get
      {
        if (index < 0 || index >= this.count)
          throw new ArgumentOutOfRangeException(nameof (index));
        return this.list[checked (this.startIndex + index)];
      }
    }

    public int Count => this.count;

    public IEnumerator<T> GetEnumerator()
    {
      for (int i = 0; i < this.count; ++i)
        yield return this.list[i + this.startIndex];
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}

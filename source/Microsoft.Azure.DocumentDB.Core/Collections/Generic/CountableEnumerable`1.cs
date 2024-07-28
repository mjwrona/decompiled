// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Collections.Generic.CountableEnumerable`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Collections.Generic
{
  internal sealed class CountableEnumerable<T> : IEnumerable<T>, IEnumerable
  {
    private readonly IEnumerable<T> enumerable;
    private readonly int count;

    public CountableEnumerable(IEnumerable<T> enumerable, int count)
    {
      if (enumerable == null)
        throw new ArgumentNullException(nameof (enumerable));
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      this.enumerable = enumerable;
      this.count = count;
    }

    public int Count => this.count;

    public IEnumerator<T> GetEnumerator()
    {
      int i = 0;
      foreach (T obj in this.enumerable)
      {
        if (i++ < this.count)
          yield return obj;
        else
          break;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}

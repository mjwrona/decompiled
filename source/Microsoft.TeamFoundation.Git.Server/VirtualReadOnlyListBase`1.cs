// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.VirtualReadOnlyListBase`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal abstract class VirtualReadOnlyListBase<T> : 
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    public T this[int index]
    {
      get
      {
        ArgumentUtility.CheckForOutOfRange(index, nameof (index), 0, this.Count - 1);
        return this.DoGet(index);
      }
    }

    public abstract int Count { get; }

    public IEnumerator<T> GetEnumerator()
    {
      int count = this.Count;
      for (int i = 0; i < count; ++i)
        yield return this.DoGet(i);
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    protected abstract T DoGet(int index);
  }
}

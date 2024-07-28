// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Common.ReadOnlySet`1
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics.Common
{
  public sealed class ReadOnlySet<T> : IReadOnlySet<T>, IEnumerable<T>, IEnumerable
  {
    private readonly ISet<T> set;

    public ReadOnlySet(ISet<T> set) => this.set = set;

    public bool Contains(T item) => this.set.Contains(item);

    public IEnumerator<T> GetEnumerator()
    {
      foreach (T obj in (IEnumerable<T>) this.set)
        yield return obj;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.set.GetEnumerator();
  }
}

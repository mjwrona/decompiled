// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.PagedEnumerator`1
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class PagedEnumerator<T> : IEnumerator<IEnumerable<T>>, IDisposable, IEnumerator
  {
    private readonly int pageSize;
    private readonly IEnumerator<T> enumerator;
    private bool complete;

    public PagedEnumerator(IEnumerator<T> enumerator, int pageSize)
    {
      this.enumerator = enumerator;
      this.pageSize = pageSize;
    }

    public IEnumerable<T> Current { get; private set; }

    object IEnumerator.Current => (object) this.Current;

    public void Dispose() => this.enumerator.Dispose();

    public bool MoveNext()
    {
      if (this.complete)
        return false;
      List<T> objList = (List<T>) null;
      while (objList == null || objList.Count < this.pageSize)
      {
        if (this.enumerator.MoveNext())
        {
          objList = objList ?? new List<T>();
          objList.Add(this.enumerator.Current);
        }
        else
        {
          this.complete = true;
          break;
        }
      }
      if (objList == null)
        return false;
      this.Current = (IEnumerable<T>) objList;
      return true;
    }

    public void Reset()
    {
      this.enumerator.Reset();
      this.complete = false;
    }
  }
}

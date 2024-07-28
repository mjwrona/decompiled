// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataEnumerator`1
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class ODataEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
  {
    private IEnumerator<T> enumerator;
    private ODataEnumerable<T> enumerable;

    public T Current { get; private set; }

    object IEnumerator.Current => (object) this.Current;

    public ODataEnumerator(IEnumerator<T> enumerator, ODataEnumerable<T> enumerable)
    {
      this.enumerable = enumerable;
      this.enumerator = enumerator;
    }

    public bool MoveNext()
    {
      if (!this.enumerator.MoveNext())
        return false;
      if (this.enumerable.ReachedPageSize())
      {
        this.enumerable.SetNextPagePresent();
        return false;
      }
      this.Current = this.enumerator.Current;
      this.enumerable.IncrementCurrentCount();
      return true;
    }

    public void Reset()
    {
      this.enumerable.Reset();
      this.enumerator.Reset();
    }

    public void Dispose() => this.enumerator.Dispose();
  }
}

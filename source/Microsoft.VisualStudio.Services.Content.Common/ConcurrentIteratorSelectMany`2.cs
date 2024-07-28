// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ConcurrentIteratorSelectMany`2
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class ConcurrentIteratorSelectMany<T1, T2> : IConcurrentIterator<T2>, IDisposable
  {
    private readonly IConcurrentIterator<T1> baseEnumerator;
    private IConcurrentIterator<T2> currentEnumerator;
    private readonly Func<T1, IConcurrentIterator<T2>> selector;

    public ConcurrentIteratorSelectMany(
      IConcurrentIterator<T1> baseEnumerator,
      Func<T1, IConcurrentIterator<T2>> selector)
    {
      baseEnumerator.AssertNotEnumerated<T1>();
      this.baseEnumerator = baseEnumerator;
      this.selector = selector;
    }

    public T2 Current => this.currentEnumerator.Current;

    public bool EnumerationStarted => this.baseEnumerator.EnumerationStarted;

    public async Task<bool> MoveNextAsync(CancellationToken cancellationToken)
    {
      bool flag = !this.EnumerationStarted;
      if (!flag)
        flag = !await this.currentEnumerator.MoveNextAsync(cancellationToken);
      if (flag)
      {
        do
        {
          if (!await this.baseEnumerator.MoveNextAsync(cancellationToken).ConfigureAwait(true))
            return false;
          this.currentEnumerator = this.selector(this.baseEnumerator.Current);
        }
        while (!await this.currentEnumerator.MoveNextAsync(cancellationToken));
      }
      return true;
    }

    public void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.baseEnumerator.Dispose();
    }

    public void Dispose() => this.Dispose(true);
  }
}

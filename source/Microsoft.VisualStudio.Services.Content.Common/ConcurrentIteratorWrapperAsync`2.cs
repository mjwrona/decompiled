// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ConcurrentIteratorWrapperAsync`2
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public abstract class ConcurrentIteratorWrapperAsync<T1, T2> : IConcurrentIterator<T2>, IDisposable
  {
    private readonly IConcurrentIterator<T1> baseEnumerator;

    protected ConcurrentIteratorWrapperAsync(IConcurrentIterator<T1> baseEnumerator)
    {
      baseEnumerator.AssertNotEnumerated<T1>();
      this.baseEnumerator = baseEnumerator;
    }

    public T2 Current { get; protected set; }

    public bool EnumerationStarted => this.baseEnumerator.EnumerationStarted;

    public void Dispose() => this.Dispose(true);

    public async Task<bool> MoveNextAsync(CancellationToken cancellationToken)
    {
      do
      {
        if (!await this.baseEnumerator.MoveNextAsync(cancellationToken).ConfigureAwait(true))
          return false;
      }
      while (!await this.OnBaseValueEnumeratedAsync(this.baseEnumerator.Current).ConfigureAwait(true));
      return true;
    }

    protected abstract Task<bool> OnBaseValueEnumeratedAsync(T1 value);

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.baseEnumerator.Dispose();
    }
  }
}

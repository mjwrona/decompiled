// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ConcurrentIteratorTake`1
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class ConcurrentIteratorTake<T1> : IConcurrentIterator<T1>, IDisposable
  {
    private readonly IConcurrentIterator<T1> innerEnumerator;
    private long count;
    private bool disposed;

    public ConcurrentIteratorTake(IConcurrentIterator<T1> innerEnumerator, long take)
    {
      innerEnumerator.AssertNotEnumerated<T1>();
      this.innerEnumerator = innerEnumerator;
      this.count = take;
    }

    public void Dispose() => this.Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      if (disposing)
        this.innerEnumerator.Dispose();
      this.disposed = true;
    }

    public T1 Current => this.innerEnumerator.Current;

    public bool EnumerationStarted => this.innerEnumerator.EnumerationStarted;

    public async Task<bool> MoveNextAsync(CancellationToken token)
    {
      if (this.count <= 0L)
        return false;
      if (!await this.innerEnumerator.MoveNextAsync(token).ConfigureAwait(true))
        return false;
      --this.count;
      return true;
    }
  }
}

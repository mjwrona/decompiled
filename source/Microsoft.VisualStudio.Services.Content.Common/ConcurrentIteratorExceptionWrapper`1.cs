// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ConcurrentIteratorExceptionWrapper`1
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class ConcurrentIteratorExceptionWrapper<T> : IConcurrentIterator<T>, IDisposable
  {
    private readonly IEnumeratorExceptionMapper exceptionMapper;
    private readonly IConcurrentIterator<T> baseEnumerator;

    public ConcurrentIteratorExceptionWrapper(
      IConcurrentIterator<T> baseEnumerator,
      IEnumeratorExceptionMapper exceptionMapper)
    {
      baseEnumerator.AssertNotEnumerated<T>();
      this.exceptionMapper = exceptionMapper;
      this.baseEnumerator = baseEnumerator;
    }

    public T Current => this.baseEnumerator.Current;

    public bool EnumerationStarted => this.baseEnumerator.EnumerationStarted;

    public async Task<bool> MoveNextAsync(CancellationToken cancellationToken)
    {
      bool flag;
      Exception mappedException;
      try
      {
        flag = await this.baseEnumerator.MoveNextAsync(cancellationToken).ConfigureAwait(false);
      }
      catch (Exception ex) when (this.exceptionMapper.TryMapException(ex, out mappedException))
      {
        throw mappedException;
      }
      return flag;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.baseEnumerator.Dispose();
    }

    public void Dispose() => this.Dispose(true);
  }
}

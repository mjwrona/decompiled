// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ResetStartConcurrentIterator`1
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class ResetStartConcurrentIterator<T> : IConcurrentIterator<T>, IDisposable
  {
    private readonly IConcurrentIterator<T> inner;

    public ResetStartConcurrentIterator(IConcurrentIterator<T> inner) => this.inner = inner;

    public T Current => this.inner.Current;

    public bool EnumerationStarted { get; private set; }

    public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
    {
      this.EnumerationStarted = true;
      return this.inner.MoveNextAsync(cancellationToken);
    }

    public void Dispose() => this.inner.Dispose();
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.BulkAllObserver
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Threading;

namespace Nest
{
  public class BulkAllObserver : CoordinatedRequestObserverBase<BulkAllResponse>
  {
    private long _totalNumberOfFailedBuffers;
    private long _totalNumberOfRetries;

    public BulkAllObserver(
      Action<BulkAllResponse> onNext = null,
      Action<Exception> onError = null,
      Action onCompleted = null)
      : base(onNext, onError, onCompleted)
    {
    }

    public long TotalNumberOfFailedBuffers => this._totalNumberOfFailedBuffers;

    public long TotalNumberOfRetries => this._totalNumberOfRetries;

    internal void IncrementTotalNumberOfRetries() => Interlocked.Increment(ref this._totalNumberOfRetries);

    internal void IncrementTotalNumberOfFailedBuffers() => Interlocked.Increment(ref this._totalNumberOfFailedBuffers);
  }
}

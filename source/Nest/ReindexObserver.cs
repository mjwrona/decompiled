// Decompiled with JetBrains decompiler
// Type: Nest.ReindexObserver
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Threading;

namespace Nest
{
  public class ReindexObserver : BulkAllObserver
  {
    private long _seenScrollDocuments;
    private long _seenScrollOperations;

    public ReindexObserver(
      Action<BulkAllResponse> onNext = null,
      Action<Exception> onError = null,
      Action onCompleted = null)
      : base(onNext, onError, onCompleted)
    {
    }

    public long SeenScrollDocuments => this._seenScrollDocuments;

    public long SeenScrollOperations => this._seenScrollOperations;

    internal void IncrementSeenScrollDocuments(long documentCount) => Interlocked.Add(ref this._seenScrollDocuments, documentCount);

    internal void IncrementSeenScrollOperations() => Interlocked.Increment(ref this._seenScrollOperations);
  }
}

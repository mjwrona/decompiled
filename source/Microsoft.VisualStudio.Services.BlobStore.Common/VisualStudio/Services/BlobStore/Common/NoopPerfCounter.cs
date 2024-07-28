// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.NoopPerfCounter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using System.Threading;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  internal class NoopPerfCounter : PerfCounter
  {
    private long value;
    private int lastBatchItemCount;

    internal NoopPerfCounter(
      PerfCounterSet set,
      int id,
      CounterType type,
      string name,
      int baseId)
      : base(set, id, type, name, baseId)
    {
    }

    public override long RawValue => this.value;

    internal override CounterData Data => throw new NotSupportedException("NoopPerfCounter is a mock counter and therefore has no CounterData");

    public override void CountAverageBatchItemDuration(Stopwatch batchDuration, int batchItemCount) => this.lastBatchItemCount = batchItemCount;

    public override void CountAverageBatchItemDuration(TimeSpan batchDuration, int batchItemCount) => this.lastBatchItemCount = batchItemCount;

    public override void Decrement() => Interlocked.Decrement(ref this.value);

    public override void Increment() => Interlocked.Increment(ref this.value);
  }
}

// Decompiled with JetBrains decompiler
// Type: HdrHistogram.LongConcurrentHistogram
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram.Utilities;
using System;
using System.Threading;

namespace HdrHistogram
{
  internal class LongConcurrentHistogram : HistogramBase
  {
    private readonly WriterReaderPhaser _wrp = new WriterReaderPhaser();
    private readonly AtomicLongArray _counts;
    private long _totalCount;

    public LongConcurrentHistogram(
      long lowestTrackableValue,
      long highestTrackableValue,
      int numberOfSignificantValueDigits)
      : base(lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits)
    {
      this._counts = new AtomicLongArray(this.CountsArrayLength);
    }

    public LongConcurrentHistogram(
      long instanceId,
      long lowestTrackableValue,
      long highestTrackableValue,
      int numberOfSignificantValueDigits)
      : base(instanceId, lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits)
    {
      this._counts = new AtomicLongArray(this.CountsArrayLength);
    }

    public override long TotalCount
    {
      get => Interlocked.Read(ref this._totalCount);
      protected set => Interlocked.Exchange(ref this._totalCount, value);
    }

    protected override int WordSizeInBytes => 8;

    protected override long MaxAllowableCount => long.MaxValue;

    public override HistogramBase Copy()
    {
      LongConcurrentHistogram concurrentHistogram = new LongConcurrentHistogram(this.InstanceId, this.LowestTrackableValue, this.HighestTrackableValue, this.NumberOfSignificantValueDigits);
      concurrentHistogram.Add((HistogramBase) this);
      return (HistogramBase) concurrentHistogram;
    }

    protected override long GetCountAtIndex(int index)
    {
      try
      {
        this._wrp.ReaderLock();
        return this._counts[index];
      }
      finally
      {
        this._wrp.ReaderUnlock();
      }
    }

    protected override void SetCountAtIndex(int index, long value) => throw new NotImplementedException();

    protected override void IncrementCountAtIndex(int index)
    {
      long criticalValueAtEnter = this._wrp.WriterCriticalSectionEnter();
      try
      {
        this._counts.IncrementAndGet(index);
        Interlocked.Increment(ref this._totalCount);
      }
      finally
      {
        this._wrp.WriterCriticalSectionExit(criticalValueAtEnter);
      }
    }

    protected override void AddToCountAtIndex(int index, long addend)
    {
      long criticalValueAtEnter = this._wrp.WriterCriticalSectionEnter();
      try
      {
        this._counts.AddAndGet(index, addend);
        Interlocked.Add(ref this._totalCount, addend);
      }
      finally
      {
        this._wrp.WriterCriticalSectionExit(criticalValueAtEnter);
      }
    }

    protected override void ClearCounts()
    {
      try
      {
        this._wrp.ReaderLock();
        for (int index = 0; index < this._counts.Length; ++index)
          this._counts[index] = 0L;
        this.TotalCount = 0L;
      }
      finally
      {
        this._wrp.ReaderUnlock();
      }
    }

    protected override void CopyCountsInto(long[] target)
    {
      for (int index = 0; index < target.Length; ++index)
        target[index] = this._counts[index];
    }
  }
}

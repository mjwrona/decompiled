// Decompiled with JetBrains decompiler
// Type: HdrHistogram.LongHistogram
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace HdrHistogram
{
  internal class LongHistogram : HistogramBase
  {
    private readonly long[] _counts;
    private long _totalCount;

    public LongHistogram(long highestTrackableValue, int numberOfSignificantValueDigits)
      : this(1L, highestTrackableValue, numberOfSignificantValueDigits)
    {
    }

    public LongHistogram(
      long lowestTrackableValue,
      long highestTrackableValue,
      int numberOfSignificantValueDigits)
      : base(lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits)
    {
      this._counts = new long[this.CountsArrayLength];
    }

    public LongHistogram(
      long instanceId,
      long lowestTrackableValue,
      long highestTrackableValue,
      int numberOfSignificantValueDigits)
      : base(instanceId, lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits)
    {
      this._counts = new long[this.CountsArrayLength];
    }

    public override long TotalCount
    {
      get => this._totalCount;
      protected set => this._totalCount = value;
    }

    protected override int WordSizeInBytes => 8;

    protected override long MaxAllowableCount => long.MaxValue;

    public override HistogramBase Copy()
    {
      LongHistogram longHistogram = new LongHistogram(this.LowestTrackableValue, this.HighestTrackableValue, this.NumberOfSignificantValueDigits);
      longHistogram.Add((HistogramBase) this);
      return (HistogramBase) longHistogram;
    }

    protected override long GetCountAtIndex(int index) => this._counts[index];

    protected override void SetCountAtIndex(int index, long value) => this._counts[index] = value;

    protected override void IncrementCountAtIndex(int index)
    {
      ++this._counts[index];
      ++this._totalCount;
    }

    protected override void AddToCountAtIndex(int index, long addend)
    {
      this._counts[index] += addend;
      this._totalCount += addend;
    }

    protected override void ClearCounts()
    {
      Array.Clear((Array) this._counts, 0, this._counts.Length);
      this._totalCount = 0L;
    }

    protected override void CopyCountsInto(long[] target) => Array.Copy((Array) this._counts, (Array) target, target.Length);
  }
}

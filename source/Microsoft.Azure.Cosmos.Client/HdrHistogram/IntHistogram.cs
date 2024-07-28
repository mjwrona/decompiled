// Decompiled with JetBrains decompiler
// Type: HdrHistogram.IntHistogram
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace HdrHistogram
{
  internal class IntHistogram : HistogramBase
  {
    private readonly int[] _counts;
    private long _totalCount;

    public IntHistogram(long highestTrackableValue, int numberOfSignificantValueDigits)
      : this(1L, highestTrackableValue, numberOfSignificantValueDigits)
    {
    }

    public IntHistogram(
      long lowestTrackableValue,
      long highestTrackableValue,
      int numberOfSignificantValueDigits)
      : base(lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits)
    {
      this._counts = new int[this.CountsArrayLength];
    }

    public IntHistogram(
      long instanceId,
      long lowestTrackableValue,
      long highestTrackableValue,
      int numberOfSignificantValueDigits)
      : base(instanceId, lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits)
    {
      this._counts = new int[this.CountsArrayLength];
    }

    public override long TotalCount
    {
      get => this._totalCount;
      protected set => this._totalCount = value;
    }

    protected override int WordSizeInBytes => 4;

    protected override long MaxAllowableCount => (long) int.MaxValue;

    public override HistogramBase Copy()
    {
      IntHistogram intHistogram = new IntHistogram(this.LowestTrackableValue, this.HighestTrackableValue, this.NumberOfSignificantValueDigits);
      intHistogram.Add((HistogramBase) this);
      return (HistogramBase) intHistogram;
    }

    protected override long GetCountAtIndex(int index) => (long) this._counts[index];

    protected override void SetCountAtIndex(int index, long value) => this._counts[index] = (int) value;

    protected override void IncrementCountAtIndex(int index)
    {
      ++this._counts[index];
      ++this._totalCount;
    }

    protected override void AddToCountAtIndex(int index, long addend)
    {
      this._counts[index] += (int) addend;
      this._totalCount += addend;
    }

    protected override void ClearCounts()
    {
      Array.Clear((Array) this._counts, 0, this._counts.Length);
      this._totalCount = 0L;
    }

    protected override void CopyCountsInto(long[] target)
    {
      for (int index = 0; index < target.Length; ++index)
        target[index] = (long) this._counts[index];
    }
  }
}

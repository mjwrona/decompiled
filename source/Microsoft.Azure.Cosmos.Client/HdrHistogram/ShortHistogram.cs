// Decompiled with JetBrains decompiler
// Type: HdrHistogram.ShortHistogram
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace HdrHistogram
{
  internal sealed class ShortHistogram : HistogramBase
  {
    private readonly short[] _counts;
    private long _totalCount;

    public ShortHistogram(long highestTrackableValue, int numberOfSignificantValueDigits)
      : this(1L, highestTrackableValue, numberOfSignificantValueDigits)
    {
    }

    public ShortHistogram(
      long lowestTrackableValue,
      long highestTrackableValue,
      int numberOfSignificantValueDigits)
      : base(lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits)
    {
      this._counts = new short[this.CountsArrayLength];
    }

    public ShortHistogram(
      long instanceId,
      long lowestTrackableValue,
      long highestTrackableValue,
      int numberOfSignificantValueDigits)
      : base(instanceId, lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits)
    {
      this._counts = new short[this.CountsArrayLength];
    }

    public override long TotalCount
    {
      get => this._totalCount;
      protected set => this._totalCount = value;
    }

    protected override int WordSizeInBytes => 2;

    protected override long MaxAllowableCount => (long) short.MaxValue;

    public override HistogramBase Copy()
    {
      ShortHistogram shortHistogram = new ShortHistogram(this.LowestTrackableValue, this.HighestTrackableValue, this.NumberOfSignificantValueDigits);
      shortHistogram.Add((HistogramBase) this);
      return (HistogramBase) shortHistogram;
    }

    protected override long GetCountAtIndex(int index) => (long) this._counts[index];

    protected override void SetCountAtIndex(int index, long value) => this._counts[index] = (short) value;

    protected override void IncrementCountAtIndex(int index)
    {
      ++this._counts[index];
      ++this._totalCount;
    }

    protected override void AddToCountAtIndex(int index, long addend)
    {
      this._counts[index] += (short) addend;
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

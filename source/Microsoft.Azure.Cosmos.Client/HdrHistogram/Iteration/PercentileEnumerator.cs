// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Iteration.PercentileEnumerator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace HdrHistogram.Iteration
{
  internal sealed class PercentileEnumerator : AbstractHistogramEnumerator
  {
    private readonly int _percentileTicksPerHalfDistance;
    private double _percentileLevelToIterateTo;
    private bool _reachedLastRecordedValue;

    public PercentileEnumerator(HistogramBase histogram, int percentileTicksPerHalfDistance)
      : base(histogram)
    {
      this._percentileTicksPerHalfDistance = percentileTicksPerHalfDistance;
      this._percentileLevelToIterateTo = 0.0;
      this._reachedLastRecordedValue = false;
    }

    protected override bool HasNext()
    {
      if (base.HasNext())
        return true;
      if (this._reachedLastRecordedValue || this.ArrayTotalCount <= 0L)
        return false;
      this._percentileLevelToIterateTo = 100.0;
      this._reachedLastRecordedValue = true;
      return true;
    }

    protected override void IncrementIterationLevel() => this._percentileLevelToIterateTo += 100.0 / (double) ((long) this._percentileTicksPerHalfDistance * (long) Math.Pow(2.0, (double) ((long) (Math.Log(100.0 / (100.0 - this._percentileLevelToIterateTo)) / Math.Log(2.0)) + 1L)));

    protected override bool ReachedIterationLevel() => this.CountAtThisValue != 0L && 100.0 * (double) this.TotalCountToCurrentIndex / (double) this.ArrayTotalCount >= this._percentileLevelToIterateTo;

    protected override double GetPercentileIteratedTo() => this._percentileLevelToIterateTo;
  }
}

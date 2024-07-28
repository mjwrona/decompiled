// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Iteration.AbstractHistogramEnumerator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace HdrHistogram.Iteration
{
  internal abstract class AbstractHistogramEnumerator : 
    IEnumerator<HistogramIterationValue>,
    IEnumerator,
    IDisposable
  {
    private readonly long _savedHistogramTotalRawCount;
    private readonly HistogramIterationValue _currentIterationValue;
    private int _nextBucketIndex;
    private int _nextSubBucketIndex;
    private long _prevValueIteratedTo;
    private long _totalCountToPrevIndex;
    private long _totalValueToCurrentIndex;
    private bool _freshSubBucket;
    private long _currentValueAtIndex;
    private long _nextValueAtIndex;

    protected HistogramBase SourceHistogram { get; }

    protected long ArrayTotalCount { get; }

    protected int CurrentBucketIndex { get; private set; }

    protected int CurrentSubBucketIndex { get; private set; }

    protected long TotalCountToCurrentIndex { get; private set; }

    protected long CountAtThisValue { get; private set; }

    public HistogramIterationValue Current { get; private set; }

    protected AbstractHistogramEnumerator(HistogramBase histogram)
    {
      this.SourceHistogram = histogram;
      this._savedHistogramTotalRawCount = histogram.TotalCount;
      this.ArrayTotalCount = histogram.TotalCount;
      this.CurrentBucketIndex = 0;
      this.CurrentSubBucketIndex = 0;
      this._currentValueAtIndex = 0L;
      this._nextBucketIndex = 0;
      this._nextSubBucketIndex = 1;
      this._nextValueAtIndex = 1L;
      this._prevValueIteratedTo = 0L;
      this._totalCountToPrevIndex = 0L;
      this.TotalCountToCurrentIndex = 0L;
      this._totalValueToCurrentIndex = 0L;
      this.CountAtThisValue = 0L;
      this._freshSubBucket = true;
      this._currentIterationValue = new HistogramIterationValue();
    }

    protected virtual bool HasNext()
    {
      if (this.SourceHistogram.TotalCount != this._savedHistogramTotalRawCount)
        throw new InvalidOperationException("Source has been modified during enumeration.");
      return this.TotalCountToCurrentIndex < this.ArrayTotalCount;
    }

    protected abstract void IncrementIterationLevel();

    protected abstract bool ReachedIterationLevel();

    protected virtual double GetPercentileIteratedTo() => 100.0 * (double) this.TotalCountToCurrentIndex / (double) this.ArrayTotalCount;

    protected virtual long GetValueIteratedTo() => this.SourceHistogram.HighestEquivalentValue(this._currentValueAtIndex);

    private HistogramIterationValue Next()
    {
      while (!this.ExhaustedSubBuckets())
      {
        this.CountAtThisValue = this.SourceHistogram.GetCountAt(this.CurrentBucketIndex, this.CurrentSubBucketIndex);
        if (this._freshSubBucket)
        {
          this.TotalCountToCurrentIndex += this.CountAtThisValue;
          this._totalValueToCurrentIndex += this.CountAtThisValue * this.SourceHistogram.MedianEquivalentValue(this._currentValueAtIndex);
          this._freshSubBucket = false;
        }
        if (this.ReachedIterationLevel())
        {
          long valueIteratedTo = this.GetValueIteratedTo();
          this._currentIterationValue.Set(valueIteratedTo, this._prevValueIteratedTo, this.CountAtThisValue, this.TotalCountToCurrentIndex - this._totalCountToPrevIndex, this.TotalCountToCurrentIndex, this._totalValueToCurrentIndex, 100.0 * (double) this.TotalCountToCurrentIndex / (double) this.ArrayTotalCount, this.GetPercentileIteratedTo());
          this._prevValueIteratedTo = valueIteratedTo;
          this._totalCountToPrevIndex = this.TotalCountToCurrentIndex;
          this.IncrementIterationLevel();
          if (this.SourceHistogram.TotalCount != this._savedHistogramTotalRawCount)
            throw new InvalidOperationException("Source has been modified during enumeration.");
          return this._currentIterationValue;
        }
        this.IncrementSubBucket();
      }
      throw new ArgumentOutOfRangeException();
    }

    private bool ExhaustedSubBuckets() => this.CurrentBucketIndex >= this.SourceHistogram.BucketCount;

    private void IncrementSubBucket()
    {
      this._freshSubBucket = true;
      this.CurrentBucketIndex = this._nextBucketIndex;
      this.CurrentSubBucketIndex = this._nextSubBucketIndex;
      this._currentValueAtIndex = this._nextValueAtIndex;
      ++this._nextSubBucketIndex;
      if (this._nextSubBucketIndex >= this.SourceHistogram.SubBucketCount)
      {
        this._nextSubBucketIndex = this.SourceHistogram.SubBucketHalfCount;
        ++this._nextBucketIndex;
      }
      this._nextValueAtIndex = this.SourceHistogram.ValueFromIndex(this._nextBucketIndex, this._nextSubBucketIndex);
    }

    object IEnumerator.Current => (object) this.Current;

    bool IEnumerator.MoveNext()
    {
      int num = this.HasNext() ? 1 : 0;
      if (num == 0)
        return num != 0;
      this.Current = this.Next();
      return num != 0;
    }

    void IEnumerator.Reset()
    {
    }

    void IDisposable.Dispose()
    {
    }
  }
}

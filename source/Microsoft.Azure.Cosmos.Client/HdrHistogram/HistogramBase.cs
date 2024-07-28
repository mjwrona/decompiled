// Decompiled with JetBrains decompiler
// Type: HdrHistogram.HistogramBase
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram.Encoding;
using HdrHistogram.Iteration;
using HdrHistogram.Persistence;
using HdrHistogram.Utilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace HdrHistogram
{
  internal abstract class HistogramBase : IRecorder
  {
    private static readonly Regex TagValidation = new Regex("[, \\r\\n]", RegexOptions.Compiled);
    private static long _instanceIdSequencer = -1;
    private readonly int _subBucketHalfCountMagnitude;
    private readonly int _unitMagnitude;
    private readonly long _subBucketMask;
    private readonly int _bucketIndexOffset;
    private long _maxValue;
    private long _minNonZeroValue;
    private string _tag;

    public long InstanceId { get; }

    public long HighestTrackableValue { get; }

    public long LowestTrackableValue { get; }

    public int NumberOfSignificantValueDigits { get; }

    public long StartTimeStamp { get; set; }

    public long EndTimeStamp { get; set; }

    public string Tag
    {
      get => this._tag;
      set => this._tag = string.IsNullOrEmpty(value) || !HistogramBase.TagValidation.IsMatch(value) ? value : throw new ArgumentException("Tag string cannot contain commas, spaces, or line breaks.");
    }

    public abstract long TotalCount { get; protected set; }

    public int BucketCount { get; }

    public int SubBucketCount { get; }

    internal int SubBucketHalfCount { get; }

    internal int CountsArrayLength { get; }

    protected abstract int WordSizeInBytes { get; }

    protected abstract long MaxAllowableCount { get; }

    protected HistogramBase(
      long lowestTrackableValue,
      long highestTrackableValue,
      int numberOfSignificantValueDigits)
      : this(Interlocked.Decrement(ref HistogramBase._instanceIdSequencer), lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits)
    {
    }

    protected HistogramBase(
      long instanceId,
      long lowestTrackableValue,
      long highestTrackableValue,
      int numberOfSignificantValueDigits)
    {
      if (lowestTrackableValue < 1L)
        throw new ArgumentException("lowestTrackableValue must be >= 1", nameof (lowestTrackableValue));
      if (highestTrackableValue < 2L * lowestTrackableValue)
        throw new ArgumentException("highestTrackableValue must be >= 2 * lowestTrackableValue", nameof (highestTrackableValue));
      if (numberOfSignificantValueDigits < 0 || numberOfSignificantValueDigits > 5)
        throw new ArgumentException("numberOfSignificantValueDigits must be between 0 and 5", nameof (numberOfSignificantValueDigits));
      this.InstanceId = instanceId;
      this.LowestTrackableValue = lowestTrackableValue;
      this.HighestTrackableValue = highestTrackableValue;
      this.NumberOfSignificantValueDigits = numberOfSignificantValueDigits;
      this._unitMagnitude = (int) Math.Floor(Math.Log((double) this.LowestTrackableValue) / Math.Log(2.0));
      int num = (int) Math.Ceiling(Math.Log((double) (2L * (long) Math.Pow(10.0, (double) this.NumberOfSignificantValueDigits))) / Math.Log(2.0));
      this._subBucketHalfCountMagnitude = (num > 1 ? num : 1) - 1;
      this.SubBucketCount = (int) Math.Pow(2.0, (double) (this._subBucketHalfCountMagnitude + 1));
      this.SubBucketHalfCount = this.SubBucketCount / 2;
      this._subBucketMask = (long) (this.SubBucketCount - 1 << this._unitMagnitude);
      this._bucketIndexOffset = 64 - this._unitMagnitude - (this._subBucketHalfCountMagnitude + 1);
      this.BucketCount = this.GetBucketsNeededToCoverValue(this.HighestTrackableValue);
      this.CountsArrayLength = this.GetLengthForNumberOfBuckets(this.BucketCount);
    }

    public abstract HistogramBase Copy();

    public void RecordValue(long value) => this.RecordSingleValue(value);

    public void RecordValueWithCount(long value, long count)
    {
      int bucketIndex = this.GetBucketIndex(value);
      int subBucketIndex = this.GetSubBucketIndex(value, bucketIndex);
      this.AddToCountAtIndex(this.CountsArrayIndex(bucketIndex, subBucketIndex), count);
    }

    public void RecordValueWithExpectedInterval(
      long value,
      long expectedIntervalBetweenValueSamples)
    {
      this.RecordValueWithCountAndExpectedInterval(value, 1L, expectedIntervalBetweenValueSamples);
    }

    public void Reset() => this.ClearCounts();

    public virtual void Add(HistogramBase fromHistogram)
    {
      if (this.HighestTrackableValue < fromHistogram.HighestTrackableValue)
        throw new ArgumentOutOfRangeException(nameof (fromHistogram), string.Format("The other histogram covers a wider range ({0} than this one ({1}).", (object) fromHistogram.HighestTrackableValue, (object) this.HighestTrackableValue));
      if (this.BucketCount == fromHistogram.BucketCount && this.SubBucketCount == fromHistogram.SubBucketCount && this._unitMagnitude == fromHistogram._unitMagnitude)
      {
        for (int index = 0; index < fromHistogram.CountsArrayLength; ++index)
          this.AddToCountAtIndex(index, fromHistogram.GetCountAtIndex(index));
      }
      else
      {
        for (int index = 0; index < fromHistogram.CountsArrayLength; ++index)
        {
          long countAtIndex = fromHistogram.GetCountAtIndex(index);
          this.RecordValueWithCount(fromHistogram.ValueFromIndex(index), countAtIndex);
        }
      }
    }

    public long SizeOfEquivalentValueRange(long value)
    {
      int bucketIndex = this.GetBucketIndex(value);
      if (this.GetSubBucketIndex(value, bucketIndex) >= this.SubBucketCount)
        ++bucketIndex;
      return (long) (1 << this._unitMagnitude + bucketIndex);
    }

    public long LowestEquivalentValue(long value)
    {
      int bucketIndex = this.GetBucketIndex(value);
      int subBucketIndex = this.GetSubBucketIndex(value, bucketIndex);
      return this.ValueFromIndex(bucketIndex, subBucketIndex);
    }

    public long MedianEquivalentValue(long value) => this.LowestEquivalentValue(value) + (this.SizeOfEquivalentValueRange(value) >> 1);

    public long NextNonEquivalentValue(long value) => this.LowestEquivalentValue(value) + this.SizeOfEquivalentValueRange(value);

    public long GetValueAtPercentile(double percentile)
    {
      long num1 = Math.Max((long) (Math.Min(percentile, 100.0) / 100.0 * (double) this.TotalCount + 0.5), 1L);
      long num2 = 0;
      for (int bucketIndex = 0; bucketIndex < this.BucketCount; ++bucketIndex)
      {
        for (int subBucketIndex = bucketIndex == 0 ? 0 : this.SubBucketCount / 2; subBucketIndex < this.SubBucketCount; ++subBucketIndex)
        {
          num2 += this.GetCountAt(bucketIndex, subBucketIndex);
          if (num2 >= num1)
            return this.HighestEquivalentValue(this.ValueFromIndex(bucketIndex, subBucketIndex));
        }
      }
      throw new ArgumentOutOfRangeException(nameof (percentile), "percentile value not found in range");
    }

    public long GetCountAtValue(long value)
    {
      int bucketIndex = this.GetBucketIndex(value);
      int subBucketIndex = this.GetSubBucketIndex(value, bucketIndex);
      return this.GetCountAt(bucketIndex, subBucketIndex);
    }

    public IEnumerable<HistogramIterationValue> RecordedValues() => (IEnumerable<HistogramIterationValue>) new RecordedValuesEnumerable(this);

    public IEnumerable<HistogramIterationValue> AllValues() => (IEnumerable<HistogramIterationValue>) new AllValueEnumerable(this);

    public int GetNeededByteBufferCapacity() => this.GetNeededByteBufferCapacity(this.CountsArrayLength);

    public int Encode(ByteBuffer targetBuffer, IEncoder encoder)
    {
      IRecordedData data = this.GetData();
      return encoder.Encode(data, targetBuffer);
    }

    public bool HasOverflowed()
    {
      long num = 0;
      for (int index = 0; index < this.CountsArrayLength; ++index)
      {
        num += this.GetCountAtIndex(index);
        if (num < 0L)
          return true;
      }
      return num != this.TotalCount;
    }

    public virtual int GetEstimatedFootprintInBytes() => 512 + this.WordSizeInBytes * this.CountsArrayLength;

    internal long GetCountAt(int bucketIndex, int subBucketIndex) => this.GetCountAtIndex(this.CountsArrayIndex(bucketIndex, subBucketIndex));

    internal long ValueFromIndex(int bucketIndex, int subBucketIndex) => (long) subBucketIndex << bucketIndex + this._unitMagnitude;

    internal int FillCountsFromBuffer(ByteBuffer buffer, int length, int wordSizeInBytes) => CountsDecoder.GetDecoderForWordSize(wordSizeInBytes).ReadCounts(buffer, length, this.CountsArrayLength, (Action<int, long>) ((idx, count) =>
    {
      if (count > this.MaxAllowableCount)
        throw new ArgumentException(string.Format("An encoded count ({0}) does not fit in the Histogram's ({1} bytes) was encountered in the source", (object) count, (object) this.WordSizeInBytes));
      this.SetCountAtIndex(idx, count);
    }));

    internal void EstablishInternalTackingValues(int lengthToCover)
    {
      this.ResetMaxValue(0L);
      this.ResetMinNonZeroValue(long.MaxValue);
      int index1 = -1;
      int index2 = -1;
      long num = 0;
      for (int index3 = 0; index3 < lengthToCover; ++index3)
      {
        long countAtIndex;
        if ((countAtIndex = this.GetCountAtIndex(index3)) > 0L)
        {
          num += countAtIndex;
          index1 = index3;
          if (index2 == -1 && index3 != 0)
            index2 = index3;
        }
      }
      if (index1 >= 0)
        this.UpdatedMaxValue(this.HighestEquivalentValue(this.ValueFromIndex(index1)));
      if (index2 >= 0)
        this.UpdateMinNonZeroValue(this.ValueFromIndex(index2));
      this.TotalCount = num;
    }

    protected abstract long GetCountAtIndex(int index);

    protected abstract void SetCountAtIndex(int index, long value);

    protected abstract void IncrementCountAtIndex(int index);

    protected abstract void AddToCountAtIndex(int index, long addend);

    protected abstract void ClearCounts();

    protected abstract void CopyCountsInto(long[] target);

    private void UpdatedMaxValue(long value)
    {
      while (value > this._maxValue)
        this._maxValue = value;
    }

    private void UpdateMinNonZeroValue(long value)
    {
      while (value < this._minNonZeroValue)
        this._minNonZeroValue = value;
    }

    private void ResetMinNonZeroValue(long minNonZeroValue) => this._minNonZeroValue = minNonZeroValue;

    private void ResetMaxValue(long maxValue) => this._maxValue = maxValue;

    private void RecordSingleValue(long value)
    {
      int bucketIndex = this.GetBucketIndex(value);
      int subBucketIndex = this.GetSubBucketIndex(value, bucketIndex);
      this.IncrementCountAtIndex(this.CountsArrayIndex(bucketIndex, subBucketIndex));
    }

    private void RecordValueWithCountAndExpectedInterval(
      long value,
      long count,
      long expectedIntervalBetweenValueSamples)
    {
      this.RecordValueWithCount(value, count);
      if (expectedIntervalBetweenValueSamples <= 0L)
        return;
      for (long index = value - expectedIntervalBetweenValueSamples; index >= expectedIntervalBetweenValueSamples; index -= expectedIntervalBetweenValueSamples)
        this.RecordValueWithCount(index, count);
    }

    private int GetNeededByteBufferCapacity(int relevantLength) => relevantLength * this.WordSizeInBytes + 32;

    private int GetBucketsNeededToCoverValue(long value)
    {
      long num = (long) (this.SubBucketCount - 1 << this._unitMagnitude);
      int neededToCoverValue = 1;
      while (num < value && num > 0L)
      {
        num <<= 1;
        ++neededToCoverValue;
      }
      return neededToCoverValue;
    }

    private int GetLengthForNumberOfBuckets(int numberOfBuckets) => (numberOfBuckets + 1) * (this.SubBucketCount / 2);

    private int CountsArrayIndex(int bucketIndex, int subBucketIndex) => (bucketIndex + 1 << this._subBucketHalfCountMagnitude) + (subBucketIndex - this.SubBucketHalfCount);

    private int GetBucketIndex(long value) => HistogramBase.GetBucketIndex(value, this._subBucketMask, this._bucketIndexOffset);

    private static int GetBucketIndex(long value, long subBucketMask, int bucketIndexOffset)
    {
      int num = Bitwise.NumberOfLeadingZeros(value | subBucketMask);
      return bucketIndexOffset - num;
    }

    private int GetSubBucketIndex(long value, int bucketIndex) => (int) (value >> bucketIndex + this._unitMagnitude);

    private long ValueFromIndex(int index)
    {
      int bucketIndex = (index >> this._subBucketHalfCountMagnitude) - 1;
      int subBucketIndex = (index & this.SubBucketHalfCount - 1) + this.SubBucketHalfCount;
      if (bucketIndex < 0)
      {
        subBucketIndex -= this.SubBucketHalfCount;
        bucketIndex = 0;
      }
      return this.ValueFromIndex(bucketIndex, subBucketIndex);
    }

    private IRecordedData GetData()
    {
      long[] relevantCounts = this.GetRelevantCounts();
      return (IRecordedData) new RecordedData(this.GetEncodingCookie(), 0, this.NumberOfSignificantValueDigits, this.LowestTrackableValue, this.HighestTrackableValue, 1.0, relevantCounts);
    }

    private long[] GetRelevantCounts()
    {
      long maxValue = this.GetMaxValue();
      int bucketIndex = this.GetBucketIndex(maxValue);
      int subBucketIndex = this.GetSubBucketIndex(maxValue, bucketIndex);
      long[] target = new long[this.CountsArrayIndex(bucketIndex, subBucketIndex) + 1];
      this.CopyCountsInto(target);
      return target;
    }
  }
}

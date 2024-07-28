// Decompiled with JetBrains decompiler
// Type: HdrHistogram.HistogramFactory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace HdrHistogram
{
  internal abstract class HistogramFactory
  {
    private HistogramFactory()
    {
    }

    protected long LowestTrackableValue { get; set; } = 1;

    protected long HighestTrackableValue { get; set; } = TimeStamp.Minutes(10L);

    protected int NumberOfSignificantValueDigits { get; set; } = 3;

    public abstract HistogramFactory WithThreadSafeWrites();

    public abstract HistogramFactory.RecorderFactory WithThreadSafeReads();

    public abstract HistogramBase Create(
      long lowestDiscernibleValue,
      long highestTrackableValue,
      int numberOfSignificantValueDigits);

    public abstract HistogramBase Create(
      long instanceId,
      long lowestDiscernibleValue,
      long highestTrackableValue,
      int numberOfSignificantValueDigits);

    public HistogramFactory WithValuesFrom(long lowestDiscernibleValue)
    {
      this.LowestTrackableValue = lowestDiscernibleValue;
      return this;
    }

    public HistogramFactory WithValuesUpTo(long highestTrackableValue)
    {
      this.HighestTrackableValue = highestTrackableValue;
      return this;
    }

    public HistogramFactory WithPrecisionOf(int numberOfSignificantValueDigits)
    {
      this.NumberOfSignificantValueDigits = numberOfSignificantValueDigits;
      return this;
    }

    public HistogramBase Create() => this.Create(this.LowestTrackableValue, this.HighestTrackableValue, this.NumberOfSignificantValueDigits);

    public static HistogramFactory With64BitBucketSize() => (HistogramFactory) new HistogramFactory.LongHistogramFactory();

    public static HistogramFactory With32BitBucketSize() => (HistogramFactory) new HistogramFactory.IntHistogramFactory();

    public static HistogramFactory With16BitBucketSize() => (HistogramFactory) new HistogramFactory.ShortHistogramFactory();

    private sealed class LongHistogramFactory : HistogramFactory
    {
      public override HistogramBase Create(
        long lowestDiscernibleValue,
        long highestTrackableValue,
        int numberOfSignificantValueDigits)
      {
        return (HistogramBase) new LongHistogram(lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits);
      }

      public override HistogramBase Create(
        long instanceId,
        long lowestDiscernibleValue,
        long highestTrackableValue,
        int numberOfSignificantValueDigits)
      {
        return (HistogramBase) new LongHistogram(instanceId, lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits);
      }

      public override HistogramFactory WithThreadSafeWrites() => (HistogramFactory) new HistogramFactory.LongConcurrentHistogramFactory((HistogramFactory) this);

      public override HistogramFactory.RecorderFactory WithThreadSafeReads() => new HistogramFactory.RecorderFactory((HistogramFactory) this);
    }

    private sealed class IntHistogramFactory : HistogramFactory
    {
      public override HistogramBase Create(
        long lowestDiscernibleValue,
        long highestTrackableValue,
        int numberOfSignificantValueDigits)
      {
        return (HistogramBase) new IntHistogram(lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits);
      }

      public override HistogramBase Create(
        long instanceId,
        long lowestDiscernibleValue,
        long highestTrackableValue,
        int numberOfSignificantValueDigits)
      {
        return (HistogramBase) new IntHistogram(instanceId, lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits);
      }

      public override HistogramFactory WithThreadSafeWrites() => (HistogramFactory) new HistogramFactory.IntConcurrentHistogramFactory((HistogramFactory) this);

      public override HistogramFactory.RecorderFactory WithThreadSafeReads() => new HistogramFactory.RecorderFactory((HistogramFactory) this);
    }

    private sealed class ShortHistogramFactory : HistogramFactory
    {
      public override HistogramBase Create(
        long lowestDiscernibleValue,
        long highestTrackableValue,
        int numberOfSignificantValueDigits)
      {
        return (HistogramBase) new ShortHistogram(lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits);
      }

      public override HistogramBase Create(
        long instanceId,
        long lowestDiscernibleValue,
        long highestTrackableValue,
        int numberOfSignificantValueDigits)
      {
        return (HistogramBase) new ShortHistogram(instanceId, lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits);
      }

      public override HistogramFactory WithThreadSafeWrites() => throw new NotSupportedException("Short(16bit) Histograms do not support thread safe writes.");

      public override HistogramFactory.RecorderFactory WithThreadSafeReads() => new HistogramFactory.RecorderFactory((HistogramFactory) this);
    }

    private sealed class LongConcurrentHistogramFactory : HistogramFactory
    {
      public LongConcurrentHistogramFactory(HistogramFactory histogramFactory)
      {
        this.LowestTrackableValue = histogramFactory.LowestTrackableValue;
        this.HighestTrackableValue = histogramFactory.HighestTrackableValue;
        this.NumberOfSignificantValueDigits = histogramFactory.NumberOfSignificantValueDigits;
      }

      public override HistogramFactory WithThreadSafeWrites() => (HistogramFactory) this;

      public override HistogramFactory.RecorderFactory WithThreadSafeReads() => new HistogramFactory.RecorderFactory((HistogramFactory) this);

      public override HistogramBase Create(
        long lowestDiscernibleValue,
        long highestTrackableValue,
        int numberOfSignificantValueDigits)
      {
        return (HistogramBase) new LongConcurrentHistogram(lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits);
      }

      public override HistogramBase Create(
        long instanceId,
        long lowestDiscernibleValue,
        long highestTrackableValue,
        int numberOfSignificantValueDigits)
      {
        return (HistogramBase) new LongConcurrentHistogram(instanceId, lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits);
      }
    }

    private sealed class IntConcurrentHistogramFactory : HistogramFactory
    {
      public IntConcurrentHistogramFactory(HistogramFactory histogramFactory)
      {
        this.LowestTrackableValue = histogramFactory.LowestTrackableValue;
        this.HighestTrackableValue = histogramFactory.HighestTrackableValue;
        this.NumberOfSignificantValueDigits = histogramFactory.NumberOfSignificantValueDigits;
      }

      public override HistogramFactory WithThreadSafeWrites() => (HistogramFactory) this;

      public override HistogramFactory.RecorderFactory WithThreadSafeReads() => new HistogramFactory.RecorderFactory((HistogramFactory) this);

      public override HistogramBase Create(
        long lowestDiscernibleValue,
        long highestTrackableValue,
        int numberOfSignificantValueDigits)
      {
        return (HistogramBase) new IntConcurrentHistogram(lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits);
      }

      public override HistogramBase Create(
        long instanceId,
        long lowestDiscernibleValue,
        long highestTrackableValue,
        int numberOfSignificantValueDigits)
      {
        return (HistogramBase) new IntConcurrentHistogram(instanceId, lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits);
      }
    }

    public sealed class RecorderFactory
    {
      private readonly HistogramFactory _histogramBuilder;

      internal RecorderFactory(HistogramFactory histogramBuilder) => this._histogramBuilder = histogramBuilder;

      public Recorder Create() => new Recorder(this._histogramBuilder.LowestTrackableValue, this._histogramBuilder.HighestTrackableValue, this._histogramBuilder.NumberOfSignificantValueDigits, new HistogramFactoryDelegate(this._histogramBuilder.Create));
    }
  }
}

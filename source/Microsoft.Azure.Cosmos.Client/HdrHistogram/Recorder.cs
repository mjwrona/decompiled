// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Recorder
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram.Utilities;
using System;
using System.Threading;

namespace HdrHistogram
{
  internal class Recorder : IRecorder
  {
    private static long _instanceIdSequencer = 1;
    private readonly object _gate = new object();
    private readonly long _instanceId = Interlocked.Increment(ref Recorder._instanceIdSequencer);
    private readonly WriterReaderPhaser _recordingPhaser = new WriterReaderPhaser();
    private readonly HistogramFactoryDelegate _histogramFactory;
    private HistogramBase _activeHistogram;
    private HistogramBase _inactiveHistogram;

    public Recorder(
      long lowestDiscernibleValue,
      long highestTrackableValue,
      int numberOfSignificantValueDigits,
      HistogramFactoryDelegate histogramFactory)
    {
      this._histogramFactory = histogramFactory;
      this._activeHistogram = histogramFactory(this._instanceId, lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits);
      this._inactiveHistogram = histogramFactory(this._instanceId, lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits);
      this._activeHistogram.StartTimeStamp = DateTime.Now.MillisecondsSinceUnixEpoch();
    }

    public void RecordValue(long value)
    {
      long criticalValueAtEnter = this._recordingPhaser.WriterCriticalSectionEnter();
      try
      {
        this._activeHistogram.RecordValue(value);
      }
      finally
      {
        this._recordingPhaser.WriterCriticalSectionExit(criticalValueAtEnter);
      }
    }

    public void RecordValueWithCount(long value, long count)
    {
      long criticalValueAtEnter = this._recordingPhaser.WriterCriticalSectionEnter();
      try
      {
        this._activeHistogram.RecordValueWithCount(value, count);
      }
      finally
      {
        this._recordingPhaser.WriterCriticalSectionExit(criticalValueAtEnter);
      }
    }

    public void RecordValueWithExpectedInterval(
      long value,
      long expectedIntervalBetweenValueSamples)
    {
      long criticalValueAtEnter = this._recordingPhaser.WriterCriticalSectionEnter();
      try
      {
        this._activeHistogram.RecordValueWithExpectedInterval(value, expectedIntervalBetweenValueSamples);
      }
      finally
      {
        this._recordingPhaser.WriterCriticalSectionExit(criticalValueAtEnter);
      }
    }

    public HistogramBase GetIntervalHistogram() => this.GetIntervalHistogram((HistogramBase) null);

    public HistogramBase GetIntervalHistogram(HistogramBase histogramToRecycle)
    {
      lock (this._gate)
      {
        this.ValidateFitAsReplacementHistogram(histogramToRecycle);
        this._inactiveHistogram = histogramToRecycle;
        this.PerformIntervalSample();
        HistogramBase inactiveHistogram = this._inactiveHistogram;
        this._inactiveHistogram = (HistogramBase) null;
        return inactiveHistogram;
      }
    }

    public void GetIntervalHistogramInto(HistogramBase targetHistogram)
    {
      lock (this._gate)
      {
        this.PerformIntervalSample();
        this._inactiveHistogram.CopyInto(targetHistogram);
      }
    }

    public void Reset()
    {
      lock (this._gate)
      {
        this.PerformIntervalSample();
        this.PerformIntervalSample();
      }
    }

    private void PerformIntervalSample()
    {
      try
      {
        this._recordingPhaser.ReaderLock();
        if (this._inactiveHistogram == null)
          this._inactiveHistogram = this._histogramFactory(this._instanceId, this._activeHistogram.LowestTrackableValue, this._activeHistogram.HighestTrackableValue, this._activeHistogram.NumberOfSignificantValueDigits);
        this._inactiveHistogram.Reset();
        HistogramBase inactiveHistogram = this._inactiveHistogram;
        this._inactiveHistogram = this._activeHistogram;
        this._activeHistogram = inactiveHistogram;
        long num = DateTime.Now.MillisecondsSinceUnixEpoch();
        this._activeHistogram.StartTimeStamp = num;
        this._inactiveHistogram.EndTimeStamp = num;
        this._recordingPhaser.FlipPhase(TimeSpan.FromMilliseconds(0.5));
      }
      finally
      {
        this._recordingPhaser.ReaderUnlock();
      }
    }

    private void ValidateFitAsReplacementHistogram(HistogramBase replacementHistogram)
    {
      if (replacementHistogram != null && replacementHistogram.InstanceId != this._activeHistogram.InstanceId)
        throw new InvalidOperationException("Replacement histogram must have been obtained via a previous getIntervalHistogram() call from this " + this.GetType().Name + " instance");
    }
  }
}

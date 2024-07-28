// Decompiled with JetBrains decompiler
// Type: HdrHistogram.HistogramExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram.Iteration;
using HdrHistogram.Output;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HdrHistogram
{
  internal static class HistogramExtensions
  {
    public static long GetMaxValue(this HistogramBase histogram)
    {
      long num = histogram.RecordedValues().Select<HistogramIterationValue, long>((Func<HistogramIterationValue, long>) (hiv => hiv.ValueIteratedTo)).LastOrDefault<long>();
      return histogram.HighestEquivalentValue(num);
    }

    public static double GetMean(this HistogramBase histogram) => (double) histogram.RecordedValues().Select<HistogramIterationValue, long>((Func<HistogramIterationValue, long>) (hiv => hiv.TotalValueToThisValue)).LastOrDefault<long>() * 1.0 / (double) histogram.TotalCount;

    public static double GetStdDeviation(this HistogramBase histogram)
    {
      double mean = histogram.GetMean();
      double num1 = 0.0;
      foreach (HistogramIterationValue recordedValue in histogram.RecordedValues())
      {
        double num2 = (double) histogram.MedianEquivalentValue(recordedValue.ValueIteratedTo) * 1.0 - mean;
        num1 += num2 * num2 * (double) recordedValue.CountAddedInThisIterationStep;
      }
      return Math.Sqrt(num1 / (double) histogram.TotalCount);
    }

    public static long HighestEquivalentValue(this HistogramBase histogram, long value) => histogram.NextNonEquivalentValue(value) - 1L;

    public static void CopyInto(this HistogramBase source, HistogramBase targetHistogram)
    {
      targetHistogram.Reset();
      targetHistogram.Add(source);
      targetHistogram.StartTimeStamp = source.StartTimeStamp;
      targetHistogram.EndTimeStamp = source.EndTimeStamp;
    }

    public static IEnumerable<HistogramIterationValue> Percentiles(
      this HistogramBase histogram,
      int percentileTicksPerHalfDistance)
    {
      return (IEnumerable<HistogramIterationValue>) new PercentileEnumerable(histogram, percentileTicksPerHalfDistance);
    }

    public static void OutputPercentileDistribution(
      this HistogramBase histogram,
      TextWriter writer,
      int percentileTicksPerHalfDistance = 5,
      double outputValueUnitScalingRatio = 1.0,
      bool useCsvFormat = false)
    {
      IOutputFormatter outputFormatter = useCsvFormat ? (IOutputFormatter) new CsvOutputFormatter(writer, histogram.NumberOfSignificantValueDigits, outputValueUnitScalingRatio) : (IOutputFormatter) new HgrmOutputFormatter(writer, histogram.NumberOfSignificantValueDigits, outputValueUnitScalingRatio);
      try
      {
        outputFormatter.WriteHeader();
        foreach (HistogramIterationValue percentile in histogram.Percentiles(percentileTicksPerHalfDistance))
          outputFormatter.WriteValue(percentile);
        outputFormatter.WriteFooter(histogram);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        if (histogram.HasOverflowed())
          writer.Write("# Histogram counts indicate OVERFLOW values");
        else
          throw;
      }
    }

    public static void Record(this IRecorder recorder, Action action)
    {
      long timestamp = ValueStopwatch.GetTimestamp();
      action();
      long num = ValueStopwatch.GetTimestamp() - timestamp;
      recorder.RecordValue(num);
    }

    public static IDisposable RecordScope(this IRecorder recorder) => (IDisposable) new HistogramExtensions.Timer(recorder);

    private sealed class Timer : IDisposable
    {
      private readonly IRecorder _recorder;
      private readonly long _start;

      public Timer(IRecorder recorder)
      {
        this._recorder = recorder;
        this._start = ValueStopwatch.GetTimestamp();
      }

      public void Dispose() => this._recorder.RecordValue(ValueStopwatch.GetTimestamp() - this._start);
    }
  }
}

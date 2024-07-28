// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Output.HgrmOutputFormatter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram.Iteration;
using System.IO;

namespace HdrHistogram.Output
{
  internal sealed class HgrmOutputFormatter : IOutputFormatter
  {
    private readonly TextWriter _printStream;
    private readonly double _outputValueUnitScalingRatio;
    private readonly string _percentileFormatString;
    private readonly string _lastLinePercentileFormatString;
    private readonly string _footerLine1FormatString;
    private readonly string _footerLine2FormatString;
    private readonly string _footerLine3FormatString;

    public HgrmOutputFormatter(
      TextWriter printStream,
      int significantDigits,
      double outputValueUnitScalingRatio)
    {
      this._printStream = printStream;
      this._outputValueUnitScalingRatio = outputValueUnitScalingRatio;
      this._percentileFormatString = "{0,12:F" + significantDigits.ToString() + "} {1,2:F12} {2,10} {3,14:F2}\n";
      this._lastLinePercentileFormatString = "{0,12:F" + significantDigits.ToString() + "} {1,2:F12} {2,10}\n";
      this._footerLine1FormatString = "#[Mean    = {0,12:F" + significantDigits.ToString() + "}, StdDeviation   = {1,12:F" + significantDigits.ToString() + "}]\n";
      this._footerLine2FormatString = "#[Max     = {0,12:F" + significantDigits.ToString() + "}, Total count    = {1,12}]\n";
      this._footerLine3FormatString = "#[Buckets = {0,12}, SubBuckets     = {1,12}]\n";
    }

    public void WriteHeader() => this._printStream.Write("{0,12} {1,14} {2,10} {3,14}\n\n", (object) "Value", (object) "Percentile", (object) "TotalCount", (object) "1/(1-Percentile)");

    public void WriteValue(HistogramIterationValue iterationValue)
    {
      double num1 = (double) iterationValue.ValueIteratedTo / this._outputValueUnitScalingRatio;
      double num2 = iterationValue.PercentileLevelIteratedTo / 100.0;
      if (iterationValue.IsLastValue())
        this._printStream.Write(this._lastLinePercentileFormatString, (object) num1, (object) num2, (object) iterationValue.TotalCountToThisValue);
      else
        this._printStream.Write(this._percentileFormatString, (object) num1, (object) num2, (object) iterationValue.TotalCountToThisValue, (object) (1.0 / (1.0 - num2)));
    }

    public void WriteFooter(HistogramBase histogram)
    {
      this._printStream.Write(this._footerLine1FormatString, (object) (histogram.GetMean() / this._outputValueUnitScalingRatio), (object) (histogram.GetStdDeviation() / this._outputValueUnitScalingRatio));
      this._printStream.Write(this._footerLine2FormatString, (object) ((double) histogram.GetMaxValue() / this._outputValueUnitScalingRatio), (object) histogram.TotalCount);
      this._printStream.Write(this._footerLine3FormatString, (object) histogram.BucketCount, (object) histogram.SubBucketCount);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Output.CsvOutputFormatter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram.Iteration;
using System.IO;

namespace HdrHistogram.Output
{
  internal sealed class CsvOutputFormatter : IOutputFormatter
  {
    private readonly string _percentileFormatString;
    private readonly string _lastLinePercentileFormatString;
    private readonly TextWriter _textWriter;
    private readonly double _outputValueUnitScalingRatio;

    public CsvOutputFormatter(
      TextWriter textWriter,
      int significantDigits,
      double outputValueUnitScalingRatio)
    {
      this._textWriter = textWriter;
      this._outputValueUnitScalingRatio = outputValueUnitScalingRatio;
      this._percentileFormatString = "{0:F" + significantDigits.ToString() + "},{1:F12},{2},{3:F2}\n";
      this._lastLinePercentileFormatString = "{0:F" + significantDigits.ToString() + "},{1:F12},{2},Infinity\n";
    }

    public void WriteHeader() => this._textWriter.Write("\"Value\",\"Percentile\",\"TotalCount\",\"1/(1-Percentile)\"\n");

    public void WriteValue(HistogramIterationValue iterationValue)
    {
      double num1 = (double) iterationValue.ValueIteratedTo / this._outputValueUnitScalingRatio;
      double num2 = iterationValue.PercentileLevelIteratedTo / 100.0;
      if (iterationValue.IsLastValue())
        this._textWriter.Write(this._lastLinePercentileFormatString, (object) num1, (object) num2, (object) iterationValue.TotalCountToThisValue);
      else
        this._textWriter.Write(this._percentileFormatString, (object) num1, (object) num2, (object) iterationValue.TotalCountToThisValue, (object) (1.0 / (1.0 - num2)));
    }

    public void WriteFooter(HistogramBase histogram)
    {
    }
  }
}

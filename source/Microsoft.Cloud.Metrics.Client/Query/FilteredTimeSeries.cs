// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.FilteredTimeSeries
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics;
using Microsoft.Online.Metrics.Serialization.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Cloud.Metrics.Client.Query
{
  public sealed class FilteredTimeSeries : IFilteredTimeSeries, IQueryResultV3
  {
    internal FilteredTimeSeries(
      MetricIdentifier metricIdentifier,
      IReadOnlyList<KeyValuePair<string, string>> dimensionList,
      double evaluatedResult,
      IReadOnlyList<KeyValuePair<SamplingType, double[]>> seriesValues)
    {
      this.MetricIdentifier = metricIdentifier;
      this.DimensionList = dimensionList;
      this.EvaluatedResult = evaluatedResult;
      this.TimeSeriesValues = seriesValues;
    }

    public MetricIdentifier MetricIdentifier { get; }

    public IReadOnlyList<KeyValuePair<string, string>> DimensionList { get; }

    public double EvaluatedResult { get; }

    public IReadOnlyList<KeyValuePair<SamplingType, double[]>> TimeSeriesValues { get; }

    public double[] GetTimeSeriesValues(SamplingType samplingType)
    {
      for (int index = 0; index < this.TimeSeriesValues.Count; ++index)
      {
        if (samplingType.Equals(this.TimeSeriesValues[index].Key))
          return this.TimeSeriesValues[index].Value;
      }
      throw new KeyNotFoundException(string.Format("Sampling type {0} not found in the query result.", (object) samplingType));
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format("EvaluatedResult: {0}", (object) this.EvaluatedResult));
      stringBuilder.Append("Dimensions:");
      foreach (KeyValuePair<string, string> dimension in (IEnumerable<KeyValuePair<string, string>>) this.DimensionList)
        stringBuilder.Append(dimension.Key + ": " + dimension.Value + ";");
      stringBuilder.AppendLine();
      if (this.TimeSeriesValues != null && this.TimeSeriesValues.Count > 0)
      {
        stringBuilder.Append("[");
        for (int index = 0; index < this.TimeSeriesValues.Count; ++index)
        {
          stringBuilder.Append("[");
          stringBuilder.Append(string.Format("{0}, ", (object) this.TimeSeriesValues[index].Key));
          stringBuilder.Append(string.Join<double>(", ", (IEnumerable<double>) this.TimeSeriesValues[index].Value));
          stringBuilder.AppendLine("]");
        }
        stringBuilder.AppendLine("]");
      }
      return stringBuilder.ToString();
    }
  }
}

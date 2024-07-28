// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.QueryResult
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Cloud.Metrics.Client.Query
{
  internal sealed class QueryResult : IQueryResult
  {
    [JsonConstructor]
    internal QueryResult(
      KeyValuePair<string, string>[] dimensionList,
      double? evaluatedResult,
      double?[] seriesValues)
    {
      this.DimensionList = (IReadOnlyList<KeyValuePair<string, string>>) dimensionList;
      this.EvaluatedResult = evaluatedResult;
      this.TimeSeries = seriesValues;
    }

    public IReadOnlyList<KeyValuePair<string, string>> DimensionList { get; private set; }

    public double? EvaluatedResult { get; private set; }

    public double?[] TimeSeries { get; private set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format("Result: {0}", (object) this.EvaluatedResult));
      stringBuilder.Append("Dimensions:");
      foreach (KeyValuePair<string, string> dimension in (IEnumerable<KeyValuePair<string, string>>) this.DimensionList)
        stringBuilder.Append(string.Format("{0}: {1};", (object) dimension.Key, (object) dimension.Value));
      stringBuilder.AppendLine();
      if (this.TimeSeries != null && this.TimeSeries.Length != 0)
      {
        stringBuilder.Append("[");
        stringBuilder.Append(string.Join<double?>(", ", (IEnumerable<double?>) this.TimeSeries));
        stringBuilder.AppendLine("]");
      }
      return stringBuilder.ToString();
    }
  }
}

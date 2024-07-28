// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.TraceData.SummaryDiagnostics
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Tracing.TraceData
{
  internal struct SummaryDiagnostics
  {
    public SummaryDiagnostics(ITrace trace)
      : this()
    {
      this.DirectRequestsSummary = new Lazy<Dictionary<(int, int), int>>((Func<Dictionary<(int, int), int>>) (() => new Dictionary<(int, int), int>()));
      this.GatewayRequestsSummary = new Lazy<Dictionary<(int, int), int>>((Func<Dictionary<(int, int), int>>) (() => new Dictionary<(int, int), int>()));
      this.AllRegionsContacted = new Lazy<HashSet<Uri>>((Func<HashSet<Uri>>) (() => new HashSet<Uri>()));
      this.CollectSummaryFromTraceTree(trace);
    }

    public Lazy<HashSet<Uri>> AllRegionsContacted { get; private set; }

    public Lazy<Dictionary<(int statusCode, int subStatusCode), int>> DirectRequestsSummary { get; private set; }

    public Lazy<Dictionary<(int statusCode, int subStatusCode), int>> GatewayRequestsSummary { get; private set; }

    private void CollectSummaryFromTraceTree(ITrace currentTrace)
    {
      foreach (object obj in currentTrace.Data.Values)
      {
        if (obj is ClientSideRequestStatisticsTraceDatum statisticsTraceDatum)
        {
          this.AggregateStatsFromStoreResults(statisticsTraceDatum.StoreResponseStatisticsList);
          this.AggregateGatewayStatistics(statisticsTraceDatum.HttpResponseStatisticsList);
          this.AggregateRegionsContacted(statisticsTraceDatum.RegionsContacted);
        }
      }
      foreach (ITrace child in (IEnumerable<ITrace>) currentTrace.Children)
        this.CollectSummaryFromTraceTree(child);
    }

    private void AggregateRegionsContacted(HashSet<(string, Uri)> regionsContacted)
    {
      foreach ((string, Uri) tuple in regionsContacted)
        this.AllRegionsContacted.Value.Add(tuple.Item2);
    }

    private void AggregateGatewayStatistics(
      IReadOnlyList<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics> httpResponseStatisticsList)
    {
      foreach (ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics responseStatistics in (IEnumerable<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics>) httpResponseStatisticsList)
      {
        int num = 0;
        int result = 0;
        if (responseStatistics.HttpResponseMessage != null)
        {
          num = (int) responseStatistics.HttpResponseMessage.StatusCode;
          if (!int.TryParse(new HttpResponseHeadersWrapper(responseStatistics.HttpResponseMessage.Headers, responseStatistics.HttpResponseMessage.Content?.Headers).SubStatus, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
            result = 0;
        }
        if (!this.GatewayRequestsSummary.Value.ContainsKey((num, result)))
          this.GatewayRequestsSummary.Value[(num, result)] = 1;
        else
          this.GatewayRequestsSummary.Value[(num, result)]++;
      }
    }

    private void AggregateStatsFromStoreResults(
      IReadOnlyList<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics> storeResponseStatisticsList)
    {
      foreach (ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics responseStatistics in (IEnumerable<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics>) storeResponseStatisticsList)
      {
        int statusCode = (int) responseStatistics.StoreResult.StatusCode;
        int subStatusCode = (int) responseStatistics.StoreResult.SubStatusCode;
        if (!this.DirectRequestsSummary.Value.ContainsKey((statusCode, subStatusCode)))
          this.DirectRequestsSummary.Value[(statusCode, subStatusCode)] = 1;
        else
          this.DirectRequestsSummary.Value[(statusCode, subStatusCode)]++;
      }
    }

    public void WriteSummaryDiagnostics(IJsonWriter jsonWriter)
    {
      jsonWriter.WriteObjectStart();
      if (this.DirectRequestsSummary.IsValueCreated)
      {
        jsonWriter.WriteFieldName("DirectCalls");
        jsonWriter.WriteObjectStart();
        foreach (KeyValuePair<(int statusCode, int subStatusCode), int> keyValuePair in this.DirectRequestsSummary.Value)
        {
          jsonWriter.WriteFieldName(keyValuePair.Key.ToString());
          jsonWriter.WriteNumber64Value((Number64) (long) keyValuePair.Value);
        }
        jsonWriter.WriteObjectEnd();
      }
      if (this.AllRegionsContacted.IsValueCreated)
      {
        jsonWriter.WriteFieldName("RegionsContacted");
        jsonWriter.WriteNumber64Value((Number64) (long) this.AllRegionsContacted.Value.Count);
      }
      if (this.GatewayRequestsSummary.IsValueCreated)
      {
        jsonWriter.WriteFieldName("GatewayCalls");
        jsonWriter.WriteObjectStart();
        foreach (KeyValuePair<(int statusCode, int subStatusCode), int> keyValuePair in this.GatewayRequestsSummary.Value)
        {
          jsonWriter.WriteFieldName(keyValuePair.Key.ToString());
          jsonWriter.WriteNumber64Value((Number64) (long) keyValuePair.Value);
        }
        jsonWriter.WriteObjectEnd();
      }
      jsonWriter.WriteObjectEnd();
    }
  }
}

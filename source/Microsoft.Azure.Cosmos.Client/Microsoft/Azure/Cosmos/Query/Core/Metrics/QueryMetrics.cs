// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.QueryMetrics
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal sealed class QueryMetrics
  {
    public static readonly QueryMetrics Empty = new QueryMetrics(string.Empty, IndexUtilizationInfo.Empty, ClientSideMetrics.Empty);

    public QueryMetrics(
      BackendMetrics backendMetrics,
      IndexUtilizationInfo indexUtilizationInfo,
      ClientSideMetrics clientSideMetrics)
    {
      this.BackendMetrics = backendMetrics ?? throw new ArgumentNullException(nameof (backendMetrics));
      this.IndexUtilizationInfo = indexUtilizationInfo ?? throw new ArgumentNullException(nameof (indexUtilizationInfo));
      this.ClientSideMetrics = clientSideMetrics ?? throw new ArgumentNullException(nameof (clientSideMetrics));
    }

    public QueryMetrics(
      string deliminatedString,
      IndexUtilizationInfo indexUtilizationInfo,
      ClientSideMetrics clientSideMetrics)
    {
      BackendMetrics backendMetrics;
      // ISSUE: explicit constructor call
      this.\u002Ector(string.IsNullOrWhiteSpace(deliminatedString) || !BackendMetricsParser.TryParse(deliminatedString, out backendMetrics) ? BackendMetrics.Empty : backendMetrics, indexUtilizationInfo, clientSideMetrics);
    }

    public BackendMetrics BackendMetrics { get; }

    public IndexUtilizationInfo IndexUtilizationInfo { get; }

    public ClientSideMetrics ClientSideMetrics { get; }

    public static QueryMetrics operator +(QueryMetrics queryMetrics1, QueryMetrics queryMetrics2)
    {
      QueryMetrics.Accumulator accumulator = new QueryMetrics.Accumulator();
      accumulator = accumulator.Accumulate(queryMetrics1);
      accumulator = accumulator.Accumulate(queryMetrics2);
      return QueryMetrics.Accumulator.ToQueryMetrics(accumulator);
    }

    public override string ToString() => this.ToTextString();

    private string ToTextString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      new QueryMetricsTextWriter(stringBuilder).WriteQueryMetrics(this);
      return stringBuilder.ToString();
    }

    public static QueryMetrics CreateFromIEnumerable(IEnumerable<QueryMetrics> queryMetricsList)
    {
      if (queryMetricsList == null)
        throw new ArgumentNullException(nameof (queryMetricsList));
      QueryMetrics.Accumulator accumulator = new QueryMetrics.Accumulator();
      foreach (QueryMetrics queryMetrics in queryMetricsList)
        accumulator = accumulator.Accumulate(queryMetrics);
      return QueryMetrics.Accumulator.ToQueryMetrics(accumulator);
    }

    public ref struct Accumulator
    {
      public Accumulator(
        BackendMetrics.Accumulator backendMetricsAccumulator,
        IndexUtilizationInfo.Accumulator indexUtilizationInfoAccumulator,
        ClientSideMetrics.Accumulator clientSideMetricsAccumulator)
      {
        this.BackendMetricsAccumulator = backendMetricsAccumulator;
        this.IndexUtilizationInfoAccumulator = indexUtilizationInfoAccumulator;
        this.ClientSideMetricsAccumulator = clientSideMetricsAccumulator;
      }

      public BackendMetrics.Accumulator BackendMetricsAccumulator { get; }

      public IndexUtilizationInfo.Accumulator IndexUtilizationInfoAccumulator { get; }

      public ClientSideMetrics.Accumulator ClientSideMetricsAccumulator { get; }

      public QueryMetrics.Accumulator Accumulate(QueryMetrics queryMetrics)
      {
        if (queryMetrics == null)
          throw new ArgumentNullException(nameof (queryMetrics));
        return new QueryMetrics.Accumulator(this.BackendMetricsAccumulator.Accumulate(queryMetrics.BackendMetrics), this.IndexUtilizationInfoAccumulator.Accumulate(queryMetrics.IndexUtilizationInfo), this.ClientSideMetricsAccumulator.Accumulate(queryMetrics.ClientSideMetrics));
      }

      public static QueryMetrics ToQueryMetrics(QueryMetrics.Accumulator accumulator) => new QueryMetrics(BackendMetrics.Accumulator.ToBackendMetrics(accumulator.BackendMetricsAccumulator), IndexUtilizationInfo.Accumulator.ToIndexUtilizationInfo(accumulator.IndexUtilizationInfoAccumulator), ClientSideMetrics.Accumulator.ToClientSideMetrics(accumulator.ClientSideMetricsAccumulator));
    }
  }
}

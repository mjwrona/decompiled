// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.TraceData.QueryMetricsTraceDatum
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Metrics;
using System;

namespace Microsoft.Azure.Cosmos.Tracing.TraceData
{
  internal sealed class QueryMetricsTraceDatum : TraceDatum
  {
    private readonly Lazy<QueryMetrics> LazyQueryMetrics;

    public QueryMetricsTraceDatum(Lazy<QueryMetrics> queryMetrics) => this.LazyQueryMetrics = queryMetrics ?? throw new ArgumentNullException(nameof (queryMetrics));

    public QueryMetrics QueryMetrics => this.LazyQueryMetrics.Value;

    internal override void Accept(ITraceDatumVisitor traceDatumVisitor) => traceDatumVisitor.Visit(this);
  }
}

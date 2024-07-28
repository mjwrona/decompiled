// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ClientSideMetrics
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Documents
{
  internal sealed class ClientSideMetrics
  {
    public static readonly ClientSideMetrics Zero = new ClientSideMetrics(0L, 0.0, (IEnumerable<FetchExecutionRange>) new List<FetchExecutionRange>(), (IEnumerable<Tuple<string, SchedulingTimeSpan>>) new List<Tuple<string, SchedulingTimeSpan>>());
    private readonly long retries;
    private readonly double requestCharge;
    private readonly IEnumerable<FetchExecutionRange> fetchExecutionRanges;
    private readonly IEnumerable<Tuple<string, SchedulingTimeSpan>> partitionSchedulingTimeSpans;

    [JsonConstructor]
    public ClientSideMetrics(
      long retries,
      double requestCharge,
      IEnumerable<FetchExecutionRange> fetchExecutionRanges,
      IEnumerable<Tuple<string, SchedulingTimeSpan>> partitionSchedulingTimeSpans)
    {
      if (fetchExecutionRanges == null)
        throw new ArgumentNullException(nameof (fetchExecutionRanges));
      if (partitionSchedulingTimeSpans == null)
        throw new ArgumentNullException(nameof (partitionSchedulingTimeSpans));
      this.retries = retries;
      this.requestCharge = requestCharge;
      this.fetchExecutionRanges = fetchExecutionRanges;
      this.partitionSchedulingTimeSpans = partitionSchedulingTimeSpans;
    }

    public long Retries => this.retries;

    public double RequestCharge => this.requestCharge;

    public IEnumerable<FetchExecutionRange> FetchExecutionRanges => this.fetchExecutionRanges;

    public IEnumerable<Tuple<string, SchedulingTimeSpan>> PartitionSchedulingTimeSpans => this.partitionSchedulingTimeSpans;

    public static ClientSideMetrics CreateFromIEnumerable(
      IEnumerable<ClientSideMetrics> clientSideMetricsList)
    {
      long retries = 0;
      double requestCharge = 0.0;
      IEnumerable<FetchExecutionRange> fetchExecutionRanges = (IEnumerable<FetchExecutionRange>) new List<FetchExecutionRange>();
      IEnumerable<Tuple<string, SchedulingTimeSpan>> tuples = (IEnumerable<Tuple<string, SchedulingTimeSpan>>) new List<Tuple<string, SchedulingTimeSpan>>();
      if (clientSideMetricsList == null)
        throw new ArgumentNullException("clientSideQueryMetricsList");
      foreach (ClientSideMetrics clientSideMetrics in clientSideMetricsList)
      {
        retries += clientSideMetrics.retries;
        requestCharge += clientSideMetrics.requestCharge;
        fetchExecutionRanges = fetchExecutionRanges.Concat<FetchExecutionRange>(clientSideMetrics.fetchExecutionRanges);
        tuples = tuples.Concat<Tuple<string, SchedulingTimeSpan>>(clientSideMetrics.partitionSchedulingTimeSpans);
      }
      return new ClientSideMetrics(retries, requestCharge, fetchExecutionRanges, tuples);
    }
  }
}

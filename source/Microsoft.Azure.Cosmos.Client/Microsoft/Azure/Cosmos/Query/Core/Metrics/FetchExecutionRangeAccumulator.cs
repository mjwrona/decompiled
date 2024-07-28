// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.FetchExecutionRangeAccumulator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal sealed class FetchExecutionRangeAccumulator
  {
    private readonly DateTime constructionTime;
    private ValueStopwatch stopwatch;
    private List<FetchExecutionRange> fetchExecutionRanges;
    private DateTime startTime;
    private DateTime endTime;
    private bool isFetching;

    public FetchExecutionRangeAccumulator()
    {
      this.constructionTime = DateTime.UtcNow;
      this.stopwatch = ValueStopwatch.StartNew();
      this.fetchExecutionRanges = new List<FetchExecutionRange>();
    }

    public IEnumerable<FetchExecutionRange> GetExecutionRanges()
    {
      List<FetchExecutionRange> fetchExecutionRanges = this.fetchExecutionRanges;
      this.fetchExecutionRanges = new List<FetchExecutionRange>();
      return (IEnumerable<FetchExecutionRange>) fetchExecutionRanges;
    }

    public void BeginFetchRange()
    {
      if (this.isFetching)
        return;
      this.startTime = this.constructionTime.Add(this.stopwatch.Elapsed);
      this.isFetching = true;
    }

    public void EndFetchRange(
      string partitionIdentifier,
      string activityId,
      long numberOfDocuments,
      long retryCount)
    {
      if (!this.isFetching)
        return;
      this.endTime = this.constructionTime.Add(this.stopwatch.Elapsed);
      this.fetchExecutionRanges.Add(new FetchExecutionRange(partitionIdentifier, activityId, this.startTime, this.endTime, numberOfDocuments, retryCount));
      this.isFetching = false;
    }
  }
}

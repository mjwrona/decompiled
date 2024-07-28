// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.FetchExecutionRangeAccumulator
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Azure.Documents
{
  internal sealed class FetchExecutionRangeAccumulator
  {
    private readonly DateTime constructionTime;
    private readonly Stopwatch stopwatch;
    private List<FetchExecutionRange> fetchExecutionRanges;
    private DateTime startTime;
    private DateTime endTime;
    private bool isFetching;

    public FetchExecutionRangeAccumulator()
    {
      this.constructionTime = DateTime.UtcNow;
      this.stopwatch = Stopwatch.StartNew();
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

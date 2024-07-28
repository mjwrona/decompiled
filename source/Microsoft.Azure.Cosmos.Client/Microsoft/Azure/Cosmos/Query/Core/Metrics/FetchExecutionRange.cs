// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.FetchExecutionRange
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal sealed class FetchExecutionRange
  {
    public FetchExecutionRange(
      string partitionKeyRangeId,
      string activityId,
      DateTime startTime,
      DateTime endTime,
      long numberOfDocuments,
      long retryCount)
    {
      this.PartitionId = partitionKeyRangeId;
      this.ActivityId = activityId;
      this.StartTime = startTime;
      this.EndTime = endTime;
      this.NumberOfDocuments = numberOfDocuments;
      this.RetryCount = retryCount;
    }

    public string PartitionId { get; }

    public string ActivityId { get; }

    public DateTime StartTime { get; }

    public DateTime EndTime { get; }

    public long NumberOfDocuments { get; }

    public long RetryCount { get; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats.AggregatedStorageLogStatsResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats
{
  public class AggregatedStorageLogStatsResult
  {
    public DateTimeOffset StartTime { get; private set; }

    public DateTimeOffset EndTime { get; private set; }

    public Dictionary<string, OperationAggregate> AggregatedOperations { get; } = new Dictionary<string, OperationAggregate>();

    public AggregatedStorageLogStatsResult(DateTimeOffset startTime, DateTimeOffset endTime)
    {
      this.StartTime = startTime;
      this.EndTime = endTime;
    }
  }
}

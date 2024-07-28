// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ReferenceAuditJobInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using System;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [Serializable]
  public class ReferenceAuditJobInfo
  {
    public int TotalPartitions { get; set; }

    public int PartitionId { get; set; }

    public string DomainId { get; set; }

    public int CpuThreshold { get; set; }

    public int MaxParallelism { get; set; }

    public string RunId { get; set; }

    public int JobRetryCount { get; set; }

    public string ErrorDetails { get; set; }

    public ConcurrentDictionary<string, ReferenceCrawlStatus> CrawlStatuses { get; } = new ConcurrentDictionary<string, ReferenceCrawlStatus>();
  }
}

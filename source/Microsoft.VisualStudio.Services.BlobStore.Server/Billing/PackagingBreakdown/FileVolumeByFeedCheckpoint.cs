// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Billing.PackagingBreakdown.FileVolumeByFeedCheckpoint
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Billing.PackagingBreakdown
{
  public class FileVolumeByFeedCheckpoint : IJobCheckpoint
  {
    public FileVolumeByFeedResult Result { get; set; }

    public string Version { get; set; }

    public string RunId { get; set; }

    public int PartitionId { get; set; }

    public string DomainId { get; set; }

    public int TotalPartitions { get; set; }

    public DateTimeOffset? FirstJobStartTime { get; set; }

    public bool IsResumedFromCheckpoint { get; set; }

    public bool IsCompleteResult { get; set; }
  }
}

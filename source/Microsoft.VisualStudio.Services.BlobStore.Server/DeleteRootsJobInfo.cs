// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DeleteRootsJobInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [Serializable]
  public class DeleteRootsJobInfo
  {
    public DeleteRootsJobResult Result = new DeleteRootsJobResult();

    public string Version { get; set; }

    public int PartitionId { get; set; }

    public string DomainId { get; set; }

    public int TotalPartitions { get; set; }

    public int ParallelismDegree { get; set; }

    public string RunId { get; set; }

    public int CpuThreshold { get; set; }

    public TimeSpan SoftDeleteRetentionTime { get; set; }

    public long NumDeleteFailuresThreshold { get; set; }

    public TimeSpan TotalThrottleDuration { get; set; }
  }
}

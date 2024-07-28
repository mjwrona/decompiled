// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.FileStorageJobInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [Serializable]
  public class FileStorageJobInfo : 
    FileStorageJobBase,
    IJobCheckpoint,
    IFileStorageSizeJobInfo,
    IDomainJobInfo
  {
    public string Version { get; set; }

    public int PartitionId { get; set; }

    public FileStorageJobMode JobMode { get; set; }

    public string LastProcessedBlobId { get; set; }

    public int JobCompletePerMille { get; set; }

    public int BlobIdPartitionSize { get; set; }

    public DateTimeOffset? FirstJobStartTime { get; set; }

    public bool IsResumedFromCheckpoint { get; set; }

    public bool IsCompleteResult { get; set; }

    [IgnoreDataMember]
    public TimeSpan CheckpointValidityPeriod { get; set; }
  }
}

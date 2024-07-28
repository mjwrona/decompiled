// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.ArtifactsUsage.ArtifactsUsageBreakdown
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.ArtifactsUsage
{
  [DataContract]
  public class ArtifactsUsageBreakdown
  {
    [DataMember]
    public StorageUsage StorageUsage { get; set; }

    [DataMember]
    public double? LogicalDropStorageSizeInBytes { get; set; }

    [DataMember]
    public double? LogicalPipelineArtifactStorageSizeInBytes { get; set; }

    [DataMember]
    public double? LogicalPipelineCacheStorageSizeInBytes { get; set; }

    [DataMember]
    public IEnumerable<FeedUsage> FeedsUsage { get; set; }

    [DataMember]
    public Dictionary<Guid, double> LogicalSymbolsUsageInBytes { get; set; }

    [DataMember]
    public Dictionary<Guid, double> LogicalProjectsUsageInBytes { get; set; }
  }
}

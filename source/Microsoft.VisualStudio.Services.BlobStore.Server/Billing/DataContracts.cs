// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Billing.DataContracts
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Billing
{
  public sealed class DataContracts
  {
    [Serializable]
    public class MeterUsageInfo
    {
      public double UsedQty { get; set; }

      public double MaxQty { get; set; }
    }

    [Serializable]
    public class UsageInfo
    {
      public double LogicalFileStorageSize { get; set; }

      public double PhysicalFileStorageSize { get; set; }

      public double LogicalChunkStorageSize { get; set; }

      public double PhysicalChunkStorageSize { get; set; }

      public double LogicalFileStorageNonExemptedSize { get; set; }

      public double LogicalChunkStorageNonExemptedSize { get; set; }

      public DataContracts.LogicalFileUsageBreakdownInfo LogicalFileUsageBreakdownInfo { get; set; }

      public DataContracts.LogicalChunkUsageBreakdownInfo LogicalChunkUsageBreakdownInfo { get; set; }

      public DataContracts.PhysicalFileUsageBreakdownInfo PhysicalFileUsageBreakdownInfo { get; set; }

      public DataContracts.PhysicalChunkUsageBreakdownInfo PhysicalChunkUsageBreakdownInfo { get; set; }
    }

    [Serializable]
    public class RawStorageBreakdownInfo
    {
      public double LogicalDropStorageSize { get; set; }

      public double LogicalSymbolStorageSize { get; set; }

      public double LogicalPipelineArtifactStorageSize { get; set; }

      public double LogicalPipelineCacheStorageSize { get; set; }

      public double LogicalPackagingStorageSize { get; set; }

      public double LogicalUPackStorageSize { get; set; }

      public double LogicalNpmStorageSize { get; set; }

      public double LogicalPyPiStorageSize { get; set; }

      public double LogicalNuGetStorageSize { get; set; }

      public double LogicalIvyStorageSize { get; set; }

      public double LogicalMavenStorageSize { get; set; }

      public double LogicalCargoStorageSize { get; set; }

      public double LogicalOthersStorageSize { get; set; }

      public double LogicalBuildArtifactsStorageSize { get; set; }

      public double LogicalBuildLogsStorageSize { get; set; }
    }

    [Serializable]
    public class LogicalFileUsageBreakdownInfo
    {
      public double LogicalDropStorageSize { get; set; }

      public double LogicalSymbolStorageSize { get; set; }

      public double LogicalPipelineArtifactStorageSize { get; set; }

      public double LogicalPipelineCacheStorageSize { get; set; }

      public double LogicalOthersStorageSize { get; set; }

      public double LogicalBuildArtifactsStorageSize { get; set; }

      public double LogicalBuildLogsStorageSize { get; set; }

      public DataContracts.LogicalFilePackagingUsageBreakdownInfo LogicalFilePackagingUsageBreakdownInfo { get; set; }

      [JsonIgnore]
      public string DomainId { get; set; } = WellKnownDomainIds.DefaultDomainId.Serialize();
    }

    [Serializable]
    public class LogicalChunkUsageBreakdownInfo
    {
      public double LogicalDropStorageSize { get; set; }

      public double LogicalSymbolStorageSize { get; set; }

      public double LogicalPipelineArtifactStorageSize { get; set; }

      public double LogicalPipelineCacheStorageSize { get; set; }

      public double LogicalOthersStorageSize { get; set; }

      public double LogicalBuildArtifactsStorageSize { get; set; }

      public double LogicalBuildLogsStorageSize { get; set; }

      public DataContracts.LogicalChunkPackagingUsageBreakdownInfo LogicalChunkPackagingUsageBreakdownInfo { get; set; }

      [JsonIgnore]
      public string DomainId { get; set; } = WellKnownDomainIds.DefaultDomainId.Serialize();
    }

    [Serializable]
    public class PhysicalFileUsageBreakdownInfo
    {
      public double PhysicalDropStorageSize { get; set; }

      public double PhysicalSymbolStorageSize { get; set; }

      public double PhysicalPipelineArtifactStorageSize { get; set; }

      public double PhysicalPipelineCacheStorageSize { get; set; }

      public double PhysicalOthersStorageSize { get; set; }

      public double PhysicalBuildArtifactsStorageSize { get; set; }

      public double PhysicalBuildLogsStorageSize { get; set; }

      public DataContracts.PhysicalFilePackagingUsageBreakdownInfo PhysicalFilePackagingUsageBreakdownInfo { get; set; }

      [JsonIgnore]
      public string DomainId { get; set; } = WellKnownDomainIds.DefaultDomainId.Serialize();
    }

    [Serializable]
    public class PhysicalChunkUsageBreakdownInfo
    {
      public double PhysicalDropStorageSize { get; set; }

      public double PhysicalSymbolStorageSize { get; set; }

      public double PhysicalPipelineArtifactStorageSize { get; set; }

      public double PhysicalPipelineCacheStorageSize { get; set; }

      public double PhysicalOthersStorageSize { get; set; }

      public double PhysicalBuildArtifactsStorageSize { get; set; }

      public double PhysicalBuildLogsStorageSize { get; set; }

      public DataContracts.PhysicalChunkPackagingUsageBreakdownInfo PhysicalChunkPackagingUsageBreakdownInfo { get; set; }

      [JsonIgnore]
      public string DomainId { get; set; } = WellKnownDomainIds.DefaultDomainId.Serialize();
    }

    [Serializable]
    public class LogicalFilePackagingUsageBreakdownInfo
    {
      public double LogicalPackagingStorageSize { get; set; }

      public double LogicalUPackStorageSize { get; set; }

      public double LogicalNpmStorageSize { get; set; }

      public double LogicalPyPiStorageSize { get; set; }

      public double LogicalNuGetStorageSize { get; set; }

      public double LogicalIvyStorageSize { get; set; }

      public double LogicalMavenStorageSize { get; set; }

      public double LogicalCargoStorageSize { get; set; }
    }

    [Serializable]
    public class LogicalChunkPackagingUsageBreakdownInfo
    {
      public double LogicalPackagingStorageSize { get; set; }

      public double LogicalUPackStorageSize { get; set; }

      public double LogicalNpmStorageSize { get; set; }

      public double LogicalPyPiStorageSize { get; set; }

      public double LogicalNuGetStorageSize { get; set; }

      public double LogicalIvyStorageSize { get; set; }

      public double LogicalMavenStorageSize { get; set; }

      public double LogicalCargoStorageSize { get; set; }
    }

    [Serializable]
    public class PhysicalFilePackagingUsageBreakdownInfo
    {
      public double PhysicalPackagingStorageSize { get; set; }

      public double PhysicalUPackStorageSize { get; set; }

      public double PhysicalNpmStorageSize { get; set; }

      public double PhysicalPyPiStorageSize { get; set; }

      public double PhysicalNuGetStorageSize { get; set; }

      public double PhysicalIvyStorageSize { get; set; }

      public double PhysicalMavenStorageSize { get; set; }

      public double PhysicalCargoStorageSize { get; set; }
    }

    [Serializable]
    public class PhysicalChunkPackagingUsageBreakdownInfo
    {
      public double PhysicalPackagingStorageSize { get; set; }

      public double PhysicalUPackStorageSize { get; set; }

      public double PhysicalNpmStorageSize { get; set; }

      public double PhysicalPyPiStorageSize { get; set; }

      public double PhysicalNuGetStorageSize { get; set; }

      public double PhysicalIvyStorageSize { get; set; }

      public double PhysicalMavenStorageSize { get; set; }

      public double PhysicalCargoStorageSize { get; set; }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.StorageVolumeMeterJobResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Billing;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [Serializable]
  public class StorageVolumeMeterJobResult
  {
    public DateTimeOffset TimeStamp;
    public double DiscountMultiplier;
    public double TotalLogicalVolumeInGiB;
    public double TotalPhysicalVolumeInGiB;
    public double TotalLogicalVolumeNonExemptedInGiB;
    public double TotalMaxVolumeInGiB;
    public double TotalFileLogicalVolumeInGiB;
    public double TotalFilePhysicalVolumeInGiB;
    public double TotalChunkLogicalVolumeInGiB;
    public double TotalChunkPhysicalVolumeInGiB;
    public DataContracts.LogicalFileUsageBreakdownInfo LogicalFileUsageBreakdownInfo;
    public DataContracts.PhysicalFileUsageBreakdownInfo PhysicalFileUsageBreakdownInfo;
    public DataContracts.LogicalChunkUsageBreakdownInfo LogicalChunkUsageBreakdownInfo;
    public DataContracts.PhysicalChunkUsageBreakdownInfo PhysicalChunkUsageBreakdownInfo;
    public int TotalRetryCount;
    public DateTimeOffset ReportTime;
    public Guid EventId;
  }
}

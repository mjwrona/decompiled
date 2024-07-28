// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework.JobFanoutSettings
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework
{
  public static class JobFanoutSettings
  {
    public const int MaxPartitionLimit = 4100;
    public static Dictionary<Guid, int[]> ValidTotalPartitionsByJobId = new Dictionary<Guid, int[]>()
    {
      {
        ArtifactReservedJobIds.ChunkDedupLogicalSizeJobId,
        new int[4]{ 1, 16, 256, 4096 }
      },
      {
        ArtifactReservedJobIds.ChunkDedupPhysicalSizeJobId,
        new int[4]{ 1, 16, 256, 4096 }
      },
      {
        ArtifactReservedJobIds.FileStorageSizeJobId,
        new int[5]{ 2, 24, 30, 60, 90 }
      },
      {
        ArtifactReservedJobIds.DeleteExpiredBlobsJobId,
        new int[5]{ 2, 24, 30, 60, 90 }
      }
    };
    public static Dictionary<Guid, int[]> SubPartitionMap = new Dictionary<Guid, int[]>()
    {
      {
        ArtifactReservedJobIds.ChunkDedupPhysicalSizeJobId,
        new int[9]{ 1, 2, 4, 8, 16, 32, 64, 128, 256 }
      },
      {
        ArtifactReservedJobIds.FileStorageSizeJobId,
        new int[6]{ 1, 2, 4, 8, 16, 32 }
      },
      {
        ArtifactReservedJobIds.DeleteExpiredBlobsJobId,
        new int[9]{ 1, 2, 4, 8, 16, 32, 64, 128, 256 }
      }
    };
  }
}

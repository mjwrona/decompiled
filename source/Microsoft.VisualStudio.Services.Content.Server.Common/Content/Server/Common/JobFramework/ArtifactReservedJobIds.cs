// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework.ArtifactReservedJobIds
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework
{
  public static class ArtifactReservedJobIds
  {
    public static readonly Guid ChunkDedupLogicalSizeJobId = Guid.Parse("C1619FC8-8A2C-446D-9D9B-4DAA3A59F7F2");
    public static readonly Guid FileStorageSizeJobId = Guid.Parse("3125F7B8-FDA0-43D0-A664-837535644DC4");
    public static readonly Guid ChunkDedupPhysicalSizeJobId = Guid.Parse("09166CD2-A998-48EF-9AAB-3C34E96492B2");
    public static readonly Guid DeleteExpiredBlobsJobId = Guid.Parse("4D00CECD-C79B-483D-AECC-E20070BE6C16");
    public static readonly Guid HardDeleteRootsJobId = Guid.Parse("4096d9f2-dfd7-440b-863a-50a3a50b36cf");
  }
}

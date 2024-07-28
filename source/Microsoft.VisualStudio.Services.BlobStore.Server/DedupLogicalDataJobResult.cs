// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupLogicalDataJobResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [Serializable]
  public class DedupLogicalDataJobResult : IDedupLogicalDataJobResult
  {
    public ulong TotalBytesOutOfScope;
    public ConcurrentDictionary<ArtifactScopeType, ulong> SizeByScope = new ConcurrentDictionary<ArtifactScopeType, ulong>();

    public ulong TotalRootsEvaluated { get; set; }

    public ulong TotalRootsDiscovered { get; set; }

    public ulong TotalChunkRoots { get; set; }

    public ulong TotalRootsSoftDeleted { get; set; }

    public ulong TotalBytes { get; set; }

    public string ErrorDetails { get; set; }

    public TimeSpan TotalThrottleDuration { get; set; }

    [IgnoreDataMember]
    public ConcurrentDictionary<string, ulong> LogicalSizeByFeed { get; set; } = new ConcurrentDictionary<string, ulong>();
  }
}

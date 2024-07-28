// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.VisitDedupResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class VisitDedupResult
  {
    public long NodesVisited { get; }

    public long NonChunksVisited { get; }

    public long CachedNodes { get; }

    public long AlreadyMarkedNodes { get; }

    public DedupIdentifier RootId { get; }

    public HashSet<DedupIdentifier> MissingNodes { get; }

    public VisitDedupResult(
      DedupIdentifier rootId,
      long nodesVisited,
      long nonChunksVisited,
      long cachedNodes,
      long alreadyMarkedNodes,
      HashSet<DedupIdentifier> missingNodes)
    {
      this.RootId = rootId;
      this.NodesVisited = nodesVisited;
      this.NonChunksVisited = nonChunksVisited;
      this.CachedNodes = cachedNodes;
      this.AlreadyMarkedNodes = alreadyMarkedNodes;
      this.MissingNodes = missingNodes;
    }
  }
}

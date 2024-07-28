// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.FileItemExtensions
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  [CLSCompliant(false)]
  public static class FileItemExtensions
  {
    public static DedupNode ToDedupNode(this FileItem fileItem)
    {
      DedupIdentifier dedupIdentifier = fileItem.BlobIdentifier.ToDedupIdentifier();
      ulong length = (ulong) fileItem.Length;
      if (ChunkerHelper.IsChunk(dedupIdentifier.AlgorithmId))
        return new DedupNode(new ChunkInfo(0UL, (uint) length, dedupIdentifier.AlgorithmResult));
      if (ChunkerHelper.IsNode(dedupIdentifier.AlgorithmId))
        return new DedupNode(DedupNode.NodeType.InnerNode, length, dedupIdentifier.AlgorithmResult, new uint?());
      throw new ArgumentException(string.Format("Unknown algorithm: {0}", (object) dedupIdentifier.AlgorithmId));
    }
  }
}

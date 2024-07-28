// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.BlobStorageShardManagerExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public static class BlobStorageShardManagerExtensions
  {
    public static VirtualNodeContext<BlobStoragePhysicalNode> FindNode(
      this IShardManager<BlobStoragePhysicalNode> shardManager,
      BlobIdentifier blobId)
    {
      return shardManager.FindNode(blobId.GetKey());
    }

    public static IEnumerable<PhysicalNodeContext<BlobIdentifier, BlobStoragePhysicalNode>> FindNodes(
      this IShardManager<BlobStoragePhysicalNode> shardManager,
      IEnumerable<BlobIdentifier> blobIds)
    {
      return shardManager.FindNodes<BlobIdentifier>(blobIds.Select<BlobIdentifier, KeyValuePair<BlobIdentifier, ulong>>((Func<BlobIdentifier, KeyValuePair<BlobIdentifier, ulong>>) (blobId => new KeyValuePair<BlobIdentifier, ulong>(blobId, blobId.GetKey()))));
    }

    public static List<BlobStoragePhysicalNode> GetAllPhysicalNodes(
      this IShardManager<BlobStoragePhysicalNode> shardManager)
    {
      return shardManager.VirtualNodes.Select<VirtualNode<BlobStoragePhysicalNode>, BlobStoragePhysicalNode>((Func<VirtualNode<BlobStoragePhysicalNode>, BlobStoragePhysicalNode>) (vn => vn.PhysicalNode)).Distinct<BlobStoragePhysicalNode>().ToList<BlobStoragePhysicalNode>();
    }

    public static IBlobProvider GetBlobProvider(
      this VirtualNodeContext<BlobStoragePhysicalNode> nodeContext)
    {
      return nodeContext.VirtualNode.PhysicalNode.BlobProvider;
    }

    public static IBlobProvider GetBlobProvider(
      this PhysicalNodeContext<BlobIdentifier, BlobStoragePhysicalNode> nodeContext)
    {
      return nodeContext.PhysicalNode.BlobProvider;
    }

    public static ulong GetKey(this BlobIdentifier blobId) => BitConverter.ToUInt64(((IEnumerable<byte>) blobId.Bytes).Take<byte>(8).Reverse<byte>().ToArray<byte>(), 0);

    public static ulong GetKey(this DedupIdentifier dedupId) => BitConverter.ToUInt64(((IEnumerable<byte>) dedupId.Value).Take<byte>(8).Reverse<byte>().ToArray<byte>(), 0);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains.ShardedBlobContainerDomain
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains
{
  public class ShardedBlobContainerDomain : IBlobContainerDomain, IDomain
  {
    private readonly IShardManager<BlobContainerNode> shardManager;

    public ShardedBlobContainerDomain(IDomainId id, IShardManager<BlobContainerNode> shardManager)
    {
      this.Id = id ?? throw new ArgumentNullException("Must provide a valid domain ID");
      this.shardManager = shardManager;
    }

    public IDomainId Id { get; }

    public ICloudBlobContainer Find(BlobIdentifier blobId)
    {
      using (VirtualNodeContext<BlobContainerNode> node = this.shardManager.FindNode(ShardedBlobContainerDomain.GetKey(blobId)))
        return node.VirtualNode.PhysicalNode.Container;
    }

    public ICloudBlobContainer Find(DedupIdentifier dedupId)
    {
      using (VirtualNodeContext<BlobContainerNode> node = this.shardManager.FindNode(ShardedBlobContainerDomain.GetKey(dedupId)))
        return node.VirtualNode.PhysicalNode.Container;
    }

    public IEnumerable<(IEnumerable<BlobIdentifier> BlobIds, ICloudBlobContainer BlobContainer)> FindBlobContainers(
      IEnumerable<BlobIdentifier> blobIds)
    {
      IEnumerable<PhysicalNodeContext<BlobIdentifier, BlobContainerNode>> nodes = this.shardManager.FindNodes<BlobIdentifier>(blobIds.Select<BlobIdentifier, KeyValuePair<BlobIdentifier, ulong>>((Func<BlobIdentifier, KeyValuePair<BlobIdentifier, ulong>>) (id => new KeyValuePair<BlobIdentifier, ulong>(id, ShardedBlobContainerDomain.GetKey(id)))));
      try
      {
        return nodes.Select<PhysicalNodeContext<BlobIdentifier, BlobContainerNode>, (IEnumerable<BlobIdentifier>, ICloudBlobContainer)>((Func<PhysicalNodeContext<BlobIdentifier, BlobContainerNode>, (IEnumerable<BlobIdentifier>, ICloudBlobContainer)>) (context => (context.AllKeys, context.PhysicalNode.Container)));
      }
      finally
      {
        nodes.ForEach<PhysicalNodeContext<BlobIdentifier, BlobContainerNode>>((Action<PhysicalNodeContext<BlobIdentifier, BlobContainerNode>>) (context => context.Dispose()));
      }
    }

    public IEnumerable<(IEnumerable<DedupIdentifier> DedupIds, ICloudBlobContainer BlobContainer)> FindBlobContainers(
      IEnumerable<DedupIdentifier> dedupIds)
    {
      IEnumerable<PhysicalNodeContext<DedupIdentifier, BlobContainerNode>> nodes = this.shardManager.FindNodes<DedupIdentifier>(dedupIds.Select<DedupIdentifier, KeyValuePair<DedupIdentifier, ulong>>((Func<DedupIdentifier, KeyValuePair<DedupIdentifier, ulong>>) (id => new KeyValuePair<DedupIdentifier, ulong>(id, ShardedBlobContainerDomain.GetKey(id)))));
      try
      {
        return nodes.Select<PhysicalNodeContext<DedupIdentifier, BlobContainerNode>, (IEnumerable<DedupIdentifier>, ICloudBlobContainer)>((Func<PhysicalNodeContext<DedupIdentifier, BlobContainerNode>, (IEnumerable<DedupIdentifier>, ICloudBlobContainer)>) (context => (context.AllKeys, context.PhysicalNode.Container)));
      }
      finally
      {
        nodes.ForEach<PhysicalNodeContext<DedupIdentifier, BlobContainerNode>>((Action<PhysicalNodeContext<DedupIdentifier, BlobContainerNode>>) (context => context.Dispose()));
      }
    }

    private static ulong GetKey(BlobIdentifier blobId) => BitConverter.ToUInt64(((IEnumerable<byte>) blobId.Bytes).Take<byte>(8).Reverse<byte>().ToArray<byte>(), 0);

    private static ulong GetKey(DedupIdentifier dedupId) => BitConverter.ToUInt64(((IEnumerable<byte>) dedupId.Value).Take<byte>(8).Reverse<byte>().ToArray<byte>(), 0);

    public IEnumerable<ICloudBlobContainer> GetAllContainers() => this.shardManager.VirtualNodes.Select<VirtualNode<BlobContainerNode>, BlobContainerNode>((Func<VirtualNode<BlobContainerNode>, BlobContainerNode>) (vnode => vnode.PhysicalNode)).Distinct<BlobContainerNode>().Select<BlobContainerNode, ICloudBlobContainer>((Func<BlobContainerNode, ICloudBlobContainer>) (node => node.Container));
  }
}

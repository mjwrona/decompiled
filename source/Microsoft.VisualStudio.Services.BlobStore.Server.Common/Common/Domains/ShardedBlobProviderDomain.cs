// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains.ShardedBlobProviderDomain
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains
{
  public class ShardedBlobProviderDomain : IBlobProviderDomain, IDomain
  {
    private readonly IShardManager<BlobStoragePhysicalNode> shardManager;

    public ShardedBlobProviderDomain(
      IDomainId id,
      IShardManager<BlobStoragePhysicalNode> shardManager)
    {
      this.Id = id;
      this.shardManager = shardManager;
    }

    public IDomainId Id { get; }

    public IBlobProvider FindProvider(BlobIdentifier blobId)
    {
      using (VirtualNodeContext<BlobStoragePhysicalNode> node = this.shardManager.FindNode(ShardedBlobProviderDomain.GetKey(blobId)))
        return node.VirtualNode.PhysicalNode.BlobProvider;
    }

    public IEnumerable<(IEnumerable<BlobIdentifier> BlobIds, IBlobProvider BlobProvider)> FindProviders(
      IEnumerable<BlobIdentifier> blobIds)
    {
      IEnumerable<PhysicalNodeContext<BlobIdentifier, BlobStoragePhysicalNode>> nodes = this.shardManager.FindNodes<BlobIdentifier>(blobIds.Select<BlobIdentifier, KeyValuePair<BlobIdentifier, ulong>>((Func<BlobIdentifier, KeyValuePair<BlobIdentifier, ulong>>) (id => new KeyValuePair<BlobIdentifier, ulong>(id, ShardedBlobProviderDomain.GetKey(id)))));
      try
      {
        return nodes.Select<PhysicalNodeContext<BlobIdentifier, BlobStoragePhysicalNode>, (IEnumerable<BlobIdentifier>, IBlobProvider)>((Func<PhysicalNodeContext<BlobIdentifier, BlobStoragePhysicalNode>, (IEnumerable<BlobIdentifier>, IBlobProvider)>) (context => (context.AllKeys, context.PhysicalNode.BlobProvider)));
      }
      finally
      {
        nodes.ForEach<PhysicalNodeContext<BlobIdentifier, BlobStoragePhysicalNode>>((Action<PhysicalNodeContext<BlobIdentifier, BlobStoragePhysicalNode>>) (context => context.Dispose()));
      }
    }

    private static ulong GetKey(BlobIdentifier blobId) => BitConverter.ToUInt64(((IEnumerable<byte>) blobId.Bytes).Take<byte>(8).Reverse<byte>().ToArray<byte>(), 0);

    public IEnumerable<IBlobProvider> GetAllProviders() => this.shardManager.VirtualNodes.Select<VirtualNode<BlobStoragePhysicalNode>, BlobStoragePhysicalNode>((Func<VirtualNode<BlobStoragePhysicalNode>, BlobStoragePhysicalNode>) (virtualNode => virtualNode.PhysicalNode)).Distinct<BlobStoragePhysicalNode>().Select<BlobStoragePhysicalNode, IBlobProvider>((Func<BlobStoragePhysicalNode, IBlobProvider>) (node => node.BlobProvider));

    public IDictionary<ulong, IBlobProvider> GetVirtualMappings() => (IDictionary<ulong, IBlobProvider>) this.shardManager.VirtualNodes.ToDictionary<VirtualNode<BlobStoragePhysicalNode>, ulong, IBlobProvider>((Func<VirtualNode<BlobStoragePhysicalNode>, ulong>) (virtualNode => virtualNode.NodeId), (Func<VirtualNode<BlobStoragePhysicalNode>, IBlobProvider>) (virtualNode => virtualNode.PhysicalNode.BlobProvider));

    public IDictionary<string, IBlobProvider> MapShardToProvider() => (IDictionary<string, IBlobProvider>) this.shardManager.VirtualNodes.Select<VirtualNode<BlobStoragePhysicalNode>, BlobStoragePhysicalNode>((Func<VirtualNode<BlobStoragePhysicalNode>, BlobStoragePhysicalNode>) (vnode => vnode.PhysicalNode)).Distinct<BlobStoragePhysicalNode>().ToDictionary<BlobStoragePhysicalNode, string, IBlobProvider>((Func<BlobStoragePhysicalNode, string>) (node => node.UniqueName), (Func<BlobStoragePhysicalNode, IBlobProvider>) (node => node.BlobProvider));

    public IDictionary<string, IEnumerable<ulong>> MapIdToShard() => (IDictionary<string, IEnumerable<ulong>>) this.shardManager.VirtualNodes.GroupBy<VirtualNode<BlobStoragePhysicalNode>, string, ulong>((Func<VirtualNode<BlobStoragePhysicalNode>, string>) (vnode => vnode.PhysicalNode.UniqueName), (Func<VirtualNode<BlobStoragePhysicalNode>, ulong>) (vnode => vnode.NodeId)).ToDictionary<IGrouping<string, ulong>, string, IEnumerable<ulong>>((Func<IGrouping<string, ulong>, string>) (group => group.Key), (Func<IGrouping<string, ulong>, IEnumerable<ulong>>) (group => group.AsEnumerable<ulong>()));
  }
}

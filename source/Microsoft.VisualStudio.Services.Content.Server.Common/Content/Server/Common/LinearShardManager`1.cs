// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.LinearShardManager`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public sealed class LinearShardManager<TPhysicalNode> : IShardManager<TPhysicalNode> where TPhysicalNode : class, IPhysicalNode
  {
    private readonly IReadOnlyList<VirtualNode<TPhysicalNode>> virtualNodes;
    private readonly IReadOnlyList<TPhysicalNode> physicalNodes;
    private ulong rangeLength;

    public LinearShardManager(IEnumerable<TPhysicalNode> physicalNodeEnumerable)
    {
      LinearShardManager<TPhysicalNode> linearShardManager = this;
      this.physicalNodes = (IReadOnlyList<TPhysicalNode>) physicalNodeEnumerable.ToList<TPhysicalNode>();
      this.rangeLength = ulong.MaxValue / (ulong) this.physicalNodes.Count;
      ulong nodeId = 0;
      this.virtualNodes = (IReadOnlyList<VirtualNode<TPhysicalNode>>) this.physicalNodes.Select<TPhysicalNode, VirtualNode<TPhysicalNode>>((Func<TPhysicalNode, VirtualNode<TPhysicalNode>>) (n => new VirtualNode<TPhysicalNode>(n, 0, 1, nodeId * linearShardManager.rangeLength))).ToList<VirtualNode<TPhysicalNode>>();
    }

    public List<VirtualNode<TPhysicalNode>> VirtualNodes => this.virtualNodes.ToList<VirtualNode<TPhysicalNode>>();

    public List<MigratedKeyRange<TPhysicalNode>> AddNode(TPhysicalNode node, int virtualNodeCount) => throw new NotImplementedException();

    public void MarkMigrationComplete(MigratedKeyRange<TPhysicalNode> completedKeyRange) => throw new NotImplementedException();

    public VirtualNodeContext<TPhysicalNode> FindNode(ulong key) => new VirtualNodeContext<TPhysicalNode>(this.FindNodeInternal(key), (VirtualNode<TPhysicalNode>) null);

    public IEnumerable<PhysicalNodeContext<TKey, TPhysicalNode>> FindNodes<TKey>(
      IEnumerable<KeyValuePair<TKey, ulong>> keys)
    {
      return keys.GroupBy<KeyValuePair<TKey, ulong>, VirtualNode<TPhysicalNode>>((Func<KeyValuePair<TKey, ulong>, VirtualNode<TPhysicalNode>>) (key => this.FindNodeInternal(key.Value))).GroupBy<IGrouping<VirtualNode<TPhysicalNode>, KeyValuePair<TKey, ulong>>, TPhysicalNode, VirtualNodeContextWithKeys<TKey, TPhysicalNode>>((Func<IGrouping<VirtualNode<TPhysicalNode>, KeyValuePair<TKey, ulong>>, TPhysicalNode>) (virtualNodeGroup => virtualNodeGroup.Key.PhysicalNode), (Func<IGrouping<VirtualNode<TPhysicalNode>, KeyValuePair<TKey, ulong>>, VirtualNodeContextWithKeys<TKey, TPhysicalNode>>) (virtualNodeGroup => new VirtualNodeContextWithKeys<TKey, TPhysicalNode>(virtualNodeGroup.Key, (VirtualNode<TPhysicalNode>) null, (IEnumerable<KeyValuePair<TKey, ulong>>) virtualNodeGroup.ToDictionary<KeyValuePair<TKey, ulong>, TKey, ulong>((Func<KeyValuePair<TKey, ulong>, TKey>) (kvp => kvp.Key), (Func<KeyValuePair<TKey, ulong>, ulong>) (kvp => kvp.Value))))).Select<IGrouping<TPhysicalNode, VirtualNodeContextWithKeys<TKey, TPhysicalNode>>, PhysicalNodeContext<TKey, TPhysicalNode>>((Func<IGrouping<TPhysicalNode, VirtualNodeContextWithKeys<TKey, TPhysicalNode>>, PhysicalNodeContext<TKey, TPhysicalNode>>) (physicalNodeGrouping => new PhysicalNodeContext<TKey, TPhysicalNode>(physicalNodeGrouping.Key, physicalNodeGrouping.ToList<VirtualNodeContextWithKeys<TKey, TPhysicalNode>>())));
    }

    private VirtualNode<TPhysicalNode> FindNodeInternal(ulong key) => this.virtualNodes[Math.Min(this.virtualNodes.Count - 1, (int) (key / this.rangeLength))];
  }
}

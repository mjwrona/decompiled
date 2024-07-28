// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.VirtualNode`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public sealed class VirtualNode<TPhysicalNode> : IEquatable<VirtualNode<TPhysicalNode>> where TPhysicalNode : class, IPhysicalNode
  {
    public static readonly IComparer<VirtualNode<TPhysicalNode>> ComparerInstance = (IComparer<VirtualNode<TPhysicalNode>>) new VirtualNode<TPhysicalNode>.NodeIdComparer();
    public readonly TPhysicalNode PhysicalNode;
    public readonly int VirtualNodeIndex;
    public readonly int VirtualNodeCount;
    public readonly ulong NodeId;

    public VirtualNode(
      TPhysicalNode physicalNode,
      int virtualNodeIndex,
      int virtualNodeCount,
      ulong nodeId)
    {
      this.PhysicalNode = physicalNode;
      this.VirtualNodeIndex = virtualNodeIndex;
      this.VirtualNodeCount = virtualNodeCount;
      this.NodeId = nodeId;
    }

    public bool Equals(VirtualNode<TPhysicalNode> other) => VirtualNode<TPhysicalNode>.ComparerInstance.Compare(this, other) == 0;

    private class NodeIdComparer : IComparer<VirtualNode<TPhysicalNode>>
    {
      public int Compare(VirtualNode<TPhysicalNode> x, VirtualNode<TPhysicalNode> y) => x.NodeId.CompareTo(y.NodeId);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.PhysicalNodeContext`2
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public struct PhysicalNodeContext<TKey, TPhysicalNode> : IDisposable where TPhysicalNode : class, IPhysicalNode
  {
    public readonly TPhysicalNode PhysicalNode;
    public readonly List<VirtualNodeContextWithKeys<TKey, TPhysicalNode>> VirtualNodeContexts;

    public PhysicalNodeContext(
      TPhysicalNode physicalNode,
      List<VirtualNodeContextWithKeys<TKey, TPhysicalNode>> virtualNodeContexts)
    {
      if (virtualNodeContexts.Any<VirtualNodeContextWithKeys<TKey, TPhysicalNode>>((Func<VirtualNodeContextWithKeys<TKey, TPhysicalNode>, bool>) (virtualNodeContext => (object) virtualNodeContext.VirtualNode.PhysicalNode != (object) physicalNode)))
        throw new ArgumentException("Virtual node does not belong to given physical node.");
      this.PhysicalNode = physicalNode;
      this.VirtualNodeContexts = virtualNodeContexts;
    }

    public IEnumerable<TKey> AllKeys => this.VirtualNodeContexts.SelectMany<VirtualNodeContextWithKeys<TKey, TPhysicalNode>, TKey>((Func<VirtualNodeContextWithKeys<TKey, TPhysicalNode>, IEnumerable<TKey>>) (virtualNodeContextWithKeys => virtualNodeContextWithKeys.Keys.Select<KeyValuePair<TKey, ulong>, TKey>((Func<KeyValuePair<TKey, ulong>, TKey>) (kvp => kvp.Key))));

    public void Dispose()
    {
      foreach (VirtualNodeContext<TPhysicalNode> virtualNodeContext in this.VirtualNodeContexts)
        virtualNodeContext.Dispose();
    }
  }
}

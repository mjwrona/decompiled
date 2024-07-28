// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.IShardManager`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public interface IShardManager<TPhysicalNode> where TPhysicalNode : class, IPhysicalNode
  {
    List<VirtualNode<TPhysicalNode>> VirtualNodes { get; }

    List<MigratedKeyRange<TPhysicalNode>> AddNode(TPhysicalNode node, int virtualNodeCount);

    VirtualNodeContext<TPhysicalNode> FindNode(ulong key);

    IEnumerable<PhysicalNodeContext<TKey, TPhysicalNode>> FindNodes<TKey>(
      IEnumerable<KeyValuePair<TKey, ulong>> keys);

    void MarkMigrationComplete(MigratedKeyRange<TPhysicalNode> completedKeyRange);
  }
}

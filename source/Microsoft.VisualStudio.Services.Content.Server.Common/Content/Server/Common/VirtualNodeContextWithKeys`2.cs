// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.VirtualNodeContextWithKeys`2
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class VirtualNodeContextWithKeys<TKey, TPhysicalNode> : VirtualNodeContext<TPhysicalNode> where TPhysicalNode : class, IPhysicalNode
  {
    public readonly IEnumerable<KeyValuePair<TKey, ulong>> Keys;

    public VirtualNodeContextWithKeys(
      Microsoft.VisualStudio.Services.Content.Server.Common.VirtualNode<TPhysicalNode> virtualNode,
      Microsoft.VisualStudio.Services.Content.Server.Common.VirtualNode<TPhysicalNode> previousNode,
      IEnumerable<KeyValuePair<TKey, ulong>> keys)
      : base(virtualNode, previousNode)
    {
      this.Keys = keys;
    }
  }
}

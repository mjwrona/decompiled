// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.VirtualNodeContext`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class VirtualNodeContext<TPhysicalNode> : IDisposable where TPhysicalNode : class, IPhysicalNode
  {
    public readonly Microsoft.VisualStudio.Services.Content.Server.Common.VirtualNode<TPhysicalNode> VirtualNode;
    public readonly Microsoft.VisualStudio.Services.Content.Server.Common.VirtualNode<TPhysicalNode> PreviousNode;

    public VirtualNodeContext(
      Microsoft.VisualStudio.Services.Content.Server.Common.VirtualNode<TPhysicalNode> virtualNode,
      Microsoft.VisualStudio.Services.Content.Server.Common.VirtualNode<TPhysicalNode> previousNode)
    {
      this.VirtualNode = virtualNode;
      this.PreviousNode = previousNode;
    }

    public void Dispose() => this.Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
    }
  }
}

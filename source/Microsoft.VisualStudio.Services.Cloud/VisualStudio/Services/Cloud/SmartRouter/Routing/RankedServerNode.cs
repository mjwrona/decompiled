// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Routing.RankedServerNode
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Routing
{
  public class RankedServerNode : IComparable<RankedServerNode>
  {
    public RankedServerNode(uint rank, ServerNodeWithHash server)
    {
      this.Rank = rank;
      this.ServerNode = server.CheckArgumentIsNotNull<ServerNodeWithHash>(nameof (server));
    }

    public uint Rank { get; set; }

    public ServerNodeWithHash ServerNode { get; }

    int IComparable<RankedServerNode>.CompareTo(RankedServerNode? other)
    {
      if (other == null || this.Rank > other.Rank)
        return 1;
      return this.Rank >= other.Rank ? 0 : -1;
    }
  }
}

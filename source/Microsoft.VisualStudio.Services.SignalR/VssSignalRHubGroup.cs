// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.VssSignalRHubGroup
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.SignalR
{
  public class VssSignalRHubGroup
  {
    private HashSet<VssSignalRHubGroupConnection> m_connections;

    public VssSignalRHubGroupId GroupId { get; set; }

    public ISet<VssSignalRHubGroupConnection> Connections
    {
      get
      {
        if (this.m_connections == null)
          this.m_connections = new HashSet<VssSignalRHubGroupConnection>();
        return (ISet<VssSignalRHubGroupConnection>) this.m_connections;
      }
    }

    public override bool Equals(object obj) => obj is VssSignalRHubGroup vssSignalRhubGroup && this.GroupId.Equals(vssSignalRhubGroup.GroupId);

    public override int GetHashCode() => this.GroupId.GetHashCode();
  }
}

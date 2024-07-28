// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.VssSignalRHubGroupConnection
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using System;

namespace Microsoft.VisualStudio.Services.SignalR
{
  public class VssSignalRHubGroupConnection
  {
    public VssSignalRHubGroupId GroupId { get; set; }

    public string ConnectionId { get; set; }

    public DateTime ConnectedOn { get; set; }

    public Guid UserId { get; set; }

    public override bool Equals(object obj) => obj is VssSignalRHubGroupConnection rhubGroupConnection && rhubGroupConnection.GroupId.Equals(this.GroupId) && StringComparer.Ordinal.Equals(rhubGroupConnection.ConnectionId, this.ConnectionId);

    public override int GetHashCode() => this.ConnectionId.GetHashCode() ^ StringComparer.Ordinal.GetHashCode(this.ConnectionId);
  }
}

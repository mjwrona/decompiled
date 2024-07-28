// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.VssSignalRHubGroupId
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using System;

namespace Microsoft.VisualStudio.Services.SignalR
{
  public struct VssSignalRHubGroupId : IEquatable<VssSignalRHubGroupId>
  {
    public VssSignalRHubGroupId(string hubName, string groupName)
    {
      this.HubName = hubName;
      this.GroupName = groupName;
    }

    public string HubName { get; set; }

    public string GroupName { get; set; }

    public override bool Equals(object obj) => obj is VssSignalRHubGroupId other && this.Equals(other);

    public bool Equals(VssSignalRHubGroupId other) => other.HubName.Equals(this.HubName, StringComparison.Ordinal) && other.GroupName.Equals(this.GroupName, StringComparison.Ordinal);

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(this.HubName) ^ StringComparer.Ordinal.GetHashCode(this.GroupName);

    public static bool operator ==(VssSignalRHubGroupId left, VssSignalRHubGroupId right) => left.Equals(right);

    public static bool operator !=(VssSignalRHubGroupId left, VssSignalRHubGroupId right) => !left.Equals(right);
  }
}

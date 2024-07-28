// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.DataAccess.VssSignalRHubComponent4
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.SignalR.DataAccess
{
  internal class VssSignalRHubComponent4 : VssSignalRHubComponent3
  {
    public override void UpdateConnections(IEnumerable<string> connectionIds)
    {
      this.TraceEnter(0, nameof (UpdateConnections));
      this.PrepareStoredProcedure("SignalR.prc_UpdateConnections");
      this.BindConnectionIdTable("@connectionIds", connectionIds);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (UpdateConnections));
    }
  }
}

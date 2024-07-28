// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.DataAccess.VssSignalRHubComponent3
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.SignalR.DataAccess
{
  internal class VssSignalRHubComponent3 : VssSignalRHubComponent2
  {
    public override IList<VssSignalRHubGroupConnection> CleanupConnections(
      TimeSpan groupTimeout,
      TimeSpan connectionTimeout)
    {
      this.TraceEnter(0, nameof (CleanupConnections));
      this.PrepareStoredProcedure("SignalR.prc_CleanupConnections");
      this.BindInt("@groupTimeoutInSeconds", Convert.ToInt32(groupTimeout.TotalSeconds));
      this.BindInt("@connectionTimeoutInSeconds", Convert.ToInt32(connectionTimeout.TotalSeconds));
      IList<VssSignalRHubGroupConnection> rhubGroupConnectionList = (IList<VssSignalRHubGroupConnection>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<VssSignalRHubGroupConnection>((ObjectBinder<VssSignalRHubGroupConnection>) new VssSignalRHubGroupConnectionBinder());
        rhubGroupConnectionList = (IList<VssSignalRHubGroupConnection>) resultCollection.GetCurrent<VssSignalRHubGroupConnection>().Items;
      }
      this.TraceLeave(0, nameof (CleanupConnections));
      return rhubGroupConnectionList;
    }
  }
}

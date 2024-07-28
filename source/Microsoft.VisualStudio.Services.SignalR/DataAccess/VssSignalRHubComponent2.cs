// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.DataAccess.VssSignalRHubComponent2
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SignalR.DataAccess
{
  internal class VssSignalRHubComponent2 : VssSignalRHubComponent
  {
    public override VssSignalRHubGroupConnection AddConnectionToGroup(
      string hubName,
      string groupName,
      string connectionId,
      Guid userId)
    {
      this.TraceEnter(0, nameof (AddConnectionToGroup));
      this.PrepareStoredProcedure("SignalR.prc_AddConnectionToGroup");
      this.BindString("@hubName", hubName, 160, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@groupName", groupName, 160, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@connectionId", connectionId, 64, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@userId", userId);
      this.BindGuid("@writerId", this.Author);
      VssSignalRHubGroupConnection group = (VssSignalRHubGroupConnection) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<VssSignalRHubGroupConnection>((ObjectBinder<VssSignalRHubGroupConnection>) new VssSignalRHubGroupConnectionBinder());
        group = resultCollection.GetCurrent<VssSignalRHubGroupConnection>().SingleOrDefault<VssSignalRHubGroupConnection>();
      }
      this.TraceLeave(0, nameof (AddConnectionToGroup));
      return group;
    }

    public override IList<VssSignalRHubGroupConnection> CleanupConnections(
      TimeSpan groupTimeout,
      TimeSpan connectionTimeout)
    {
      this.TraceEnter(0, nameof (CleanupConnections));
      this.PrepareStoredProcedure("SignalR.prc_CleanupConnections");
      this.BindInt("@groupTimeoutInSeconds", Convert.ToInt32(groupTimeout.TotalSeconds));
      this.BindInt("@connectionTimeoutInSeconds", Convert.ToInt32(connectionTimeout.TotalSeconds));
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (CleanupConnections));
      return (IList<VssSignalRHubGroupConnection>) new List<VssSignalRHubGroupConnection>();
    }

    public override VssSignalRHubGroup GetGroup(string hubName, string groupName)
    {
      this.TraceEnter(0, nameof (GetGroup));
      this.PrepareStoredProcedure("SignalR.prc_GetHubGroup");
      this.BindString("@hubName", hubName, 160, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@groupName", groupName, 160, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      VssSignalRHubGroup group = (VssSignalRHubGroup) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<VssSignalRHubGroup>((ObjectBinder<VssSignalRHubGroup>) new VssSignalRHubGroupBinder());
        resultCollection.AddBinder<VssSignalRHubGroupConnection>((ObjectBinder<VssSignalRHubGroupConnection>) new VssSignalRHubGroupConnectionBinder());
        group = resultCollection.GetCurrent<VssSignalRHubGroup>().FirstOrDefault<VssSignalRHubGroup>();
        if (group != null)
        {
          resultCollection.NextResult();
          foreach (VssSignalRHubGroupConnection rhubGroupConnection in resultCollection.GetCurrent<VssSignalRHubGroupConnection>())
            group.Connections.Add(rhubGroupConnection);
        }
      }
      this.TraceLeave(0, nameof (GetGroup));
      return group;
    }

    public override IList<string> GetConnectionIds()
    {
      this.TraceEnter(0, nameof (GetConnectionIds));
      this.PrepareStoredProcedure("SignalR.prc_GetConnectionIds");
      IList<string> connectionIds = (IList<string>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<string>((ObjectBinder<string>) new VssSignalRConnectionIdBinder());
        connectionIds = (IList<string>) resultCollection.GetCurrent<string>().Items;
      }
      this.TraceLeave(0, nameof (GetConnectionIds));
      return connectionIds;
    }

    public override void InstallData()
    {
      this.TraceEnter(0, nameof (InstallData));
      this.PrepareStoredProcedure("SignalR.prc_InstallData");
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (InstallData));
    }

    public override IList<VssSignalRHubGroupConnection> RemoveConnectionFromGroup(
      string hubName,
      string groupName,
      string connectionId)
    {
      this.TraceEnter(0, nameof (RemoveConnectionFromGroup));
      this.PrepareStoredProcedure("SignalR.prc_RemoveConnectionFromGroup");
      this.BindString("@hubName", hubName, 160, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@groupName", groupName, 160, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@connectionId", connectionId, 64, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@writerId", this.Author);
      IList<VssSignalRHubGroupConnection> rhubGroupConnectionList = (IList<VssSignalRHubGroupConnection>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<VssSignalRHubGroupConnection>((ObjectBinder<VssSignalRHubGroupConnection>) new VssSignalRHubGroupConnectionBinder());
        rhubGroupConnectionList = (IList<VssSignalRHubGroupConnection>) resultCollection.GetCurrent<VssSignalRHubGroupConnection>().Items;
      }
      this.TraceLeave(0, nameof (RemoveConnectionFromGroup));
      return rhubGroupConnectionList;
    }

    public override void UpdateConnections(IEnumerable<string> connectionIds)
    {
      this.TraceEnter(0, nameof (UpdateConnections));
      this.PrepareStoredProcedure("SignalR.prc_UpdateConnections");
      this.BindStringTable("@connectionIds", connectionIds);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (UpdateConnections));
    }
  }
}

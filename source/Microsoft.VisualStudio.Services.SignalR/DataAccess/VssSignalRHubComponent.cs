// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.DataAccess.VssSignalRHubComponent
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SignalR.DataAccess
{
  internal class VssSignalRHubComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<VssSignalRHubComponent>(1, true),
      (IComponentCreator) new ComponentCreator<VssSignalRHubComponent2>(2),
      (IComponentCreator) new ComponentCreator<VssSignalRHubComponent3>(3),
      (IComponentCreator) new ComponentCreator<VssSignalRHubComponent4>(4)
    }, "SignalR_Hubs");
    protected static readonly SqlMetaData[] typ_UniqueConnectionId = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.NVarChar, 64L)
    };

    public VssSignalRHubComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public virtual VssSignalRHubGroupConnection AddConnectionToGroup(
      string hubName,
      string groupName,
      string connectionId,
      Guid userId)
    {
      return (VssSignalRHubGroupConnection) null;
    }

    public virtual VssSignalRHubGroup GetGroup(string hubName, string groupName) => (VssSignalRHubGroup) null;

    public virtual IList<VssSignalRHubGroupConnection> CleanupConnections(
      TimeSpan groupTimeout,
      TimeSpan connectionTimeout)
    {
      return (IList<VssSignalRHubGroupConnection>) new List<VssSignalRHubGroupConnection>();
    }

    public virtual IList<string> GetConnectionIds() => (IList<string>) Array.Empty<string>();

    public virtual void InstallData()
    {
    }

    public virtual IList<VssSignalRHubGroupConnection> RemoveConnectionFromGroup(
      string hubName,
      string groupName,
      string connectionId)
    {
      return (IList<VssSignalRHubGroupConnection>) Array.Empty<VssSignalRHubGroupConnection>();
    }

    public virtual void UpdateConnections(IEnumerable<string> connectionIds)
    {
    }

    protected virtual SqlParameter BindConnectionIdTable(
      string parameterName,
      IEnumerable<string> rows)
    {
      rows = rows ?? Enumerable.Empty<string>();
      System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (name =>
      {
        SqlDataRecord record = new SqlDataRecord(VssSignalRHubComponent.typ_UniqueConnectionId);
        record.SetString(0, name, BindStringBehavior.Unchanged);
        return record;
      });
      return this.BindTable(parameterName, "SignalR.typ_UniqueConnectionId", rows.Select<string, SqlDataRecord>(selector));
    }
  }
}

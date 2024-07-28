// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.DashboardGroupBinder
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Dashboards.DataAccess
{
  internal class DashboardGroupBinder : ObjectBinder<DashboardGroupEntry>
  {
    private SqlColumnBinder m_groupIdColumn = new SqlColumnBinder("GroupId");
    private SqlColumnBinder m_idColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder m_nameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder m_positionColumn = new SqlColumnBinder("Position");

    protected override DashboardGroupEntry Bind()
    {
      DashboardGroupEntry dashboardGroupEntry = new DashboardGroupEntry();
      dashboardGroupEntry.OwnerId = this.m_groupIdColumn.GetGuid((IDataReader) this.Reader, false);
      dashboardGroupEntry.GroupId = this.m_groupIdColumn.GetGuid((IDataReader) this.Reader, false);
      dashboardGroupEntry.Id = new Guid?(this.m_idColumn.GetGuid((IDataReader) this.Reader));
      dashboardGroupEntry.Name = this.m_nameColumn.GetString((IDataReader) this.Reader, false);
      dashboardGroupEntry.Position = this.m_positionColumn.GetInt32((IDataReader) this.Reader);
      return dashboardGroupEntry;
    }
  }
}

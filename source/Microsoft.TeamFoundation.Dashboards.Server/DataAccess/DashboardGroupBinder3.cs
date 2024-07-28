// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.DashboardGroupBinder3
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Dashboards.DataAccess
{
  internal class DashboardGroupBinder3 : DashboardGroupBinder2
  {
    private SqlColumnBinder m_eTagColumn = new SqlColumnBinder("ETag");
    private SqlColumnBinder m_descriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder m_ownerIdColumn = new SqlColumnBinder("OwnerId");
    private SqlColumnBinder m_ScopeColumn = new SqlColumnBinder("Scope");
    private SqlColumnBinder m_modifiedByColumn = new SqlColumnBinder("ModifiedBy");
    private SqlColumnBinder m_modifiedDateColumn = new SqlColumnBinder("ModifiedDate");
    private SqlColumnBinder m_lastAccessedDateColumn = new SqlColumnBinder("LastAccessedDate");

    protected override DashboardGroupEntry Bind()
    {
      DashboardGroupEntry dashboardGroupEntry = base.Bind();
      dashboardGroupEntry.ETag = this.m_eTagColumn.GetString((IDataReader) this.Reader, true);
      dashboardGroupEntry.Description = this.m_descriptionColumn.GetString((IDataReader) this.Reader, (string) null);
      dashboardGroupEntry.DashboardScope = (DashboardScope) this.m_ScopeColumn.GetInt32((IDataReader) this.Reader);
      dashboardGroupEntry.ModifiedBy = this.m_modifiedByColumn.GetGuid((IDataReader) this.Reader, true, Guid.Empty);
      dashboardGroupEntry.ModifiedDate = this.m_modifiedDateColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      dashboardGroupEntry.LastAccessedDate = this.m_lastAccessedDateColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      if (dashboardGroupEntry.DashboardScope == DashboardScope.Project)
        dashboardGroupEntry.OwnerId = this.m_ownerIdColumn.GetGuid((IDataReader) this.Reader, true, Guid.Empty);
      return dashboardGroupEntry;
    }
  }
}

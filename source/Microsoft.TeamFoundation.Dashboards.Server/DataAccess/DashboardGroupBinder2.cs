// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.DashboardGroupBinder2
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Dashboards.DataAccess
{
  internal class DashboardGroupBinder2 : DashboardGroupBinder
  {
    private SqlColumnBinder m_refreshIntervalColumn = new SqlColumnBinder("RefreshInterval");

    protected override DashboardGroupEntry Bind()
    {
      DashboardGroupEntry dashboardGroupEntry = base.Bind();
      dashboardGroupEntry.RefreshInterval = new int?(this.m_refreshIntervalColumn.GetInt32((IDataReader) this.Reader, 0));
      return dashboardGroupEntry;
    }
  }
}

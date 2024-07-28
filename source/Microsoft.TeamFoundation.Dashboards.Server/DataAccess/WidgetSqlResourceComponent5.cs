// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.WidgetSqlResourceComponent5
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.Model;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Dashboards.DataAccess
{
  public class WidgetSqlResourceComponent5 : WidgetSqlResourceComponent4
  {
    protected override void BindETag(string eTag) => this.BindString("@eTag", eTag, 256, true, SqlDbType.VarChar);

    public override DashboardWidgetsDataModel UpdateWidgets(
      Guid dataspaceId,
      DashboardWidgetsDataModel dashboard)
    {
      this.PrepareStoredProcedure("Dashboards.prc_UpdateDashboardWidgets");
      this.BindGuid("@dashboardId", dashboard.DashboardId);
      this.BindWidgetTable("@widgetTable", dashboard.Widgets);
      this.BindDataspaceId(dataspaceId);
      this.BindETag(dashboard.ETag);
      string str = (string) this.ExecuteScalar();
      dashboard.ETag = str;
      return dashboard;
    }

    public override DashboardWidgetsDataModel ReplaceWidgets(
      Guid dataspaceId,
      DashboardWidgetsDataModel dashboard)
    {
      return this.UpdateWidgets(dataspaceId, dashboard);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.WidgetSqlResourceComponent6
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Dashboards.Model;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Dashboards.DataAccess
{
  public class WidgetSqlResourceComponent6 : WidgetSqlResourceComponent5
  {
    private static readonly SqlMetaData[] typ_WidgetTable3 = new SqlMetaData[14]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DashboardId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("RowNumber", SqlDbType.Int),
      new SqlMetaData("ColumnNumber", SqlDbType.Int),
      new SqlMetaData("RowLength", SqlDbType.Int),
      new SqlMetaData("ColumnLength", SqlDbType.Int),
      new SqlMetaData("WidgetTypeId", SqlDbType.NVarChar, 1000L),
      new SqlMetaData("Settings", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 256L),
      new SqlMetaData("SettingsVersion", SqlDbType.NVarChar, 256L),
      new SqlMetaData("WidgetETag", SqlDbType.NVarChar, 256L),
      new SqlMetaData("DashboardETag", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ReplaceWidget", SqlDbType.Bit)
    };

    protected override SqlParameter BindWidgetTable(
      string parameterName,
      IEnumerable<WidgetDataModel> rows)
    {
      rows = rows ?? Enumerable.Empty<WidgetDataModel>();
      System.Func<WidgetDataModel, SqlDataRecord> selector = (System.Func<WidgetDataModel, SqlDataRecord>) (widget =>
      {
        ArgumentUtility.CheckForNull<WidgetDataModel>(widget, nameof (widget));
        SqlDataRecord record = new SqlDataRecord(WidgetSqlResourceComponent6.typ_WidgetTable3);
        int ordinal1 = 0;
        int num1 = ordinal1 + 1;
        record.SetGuid(ordinal1, widget.Id.Value);
        int ordinal2 = num1;
        int num2 = ordinal2 + 1;
        record.SetGuid(ordinal2, widget.DashboardId);
        int ordinal3 = num2;
        int num3 = ordinal3 + 1;
        record.SetString(ordinal3, widget.Name);
        int ordinal4 = num3;
        int num4 = ordinal4 + 1;
        record.SetInt32(ordinal4, widget.Position.Row);
        int ordinal5 = num4;
        int num5 = ordinal5 + 1;
        record.SetInt32(ordinal5, widget.Position.Column);
        int ordinal6 = num5;
        int num6 = ordinal6 + 1;
        record.SetInt32(ordinal6, widget.Size.RowSpan);
        int ordinal7 = num6;
        int num7 = ordinal7 + 1;
        record.SetInt32(ordinal7, widget.Size.ColumnSpan);
        int ordinal8 = num7;
        int num8 = ordinal8 + 1;
        record.SetString(ordinal8, widget.ContributionId);
        int ordinal9 = num8;
        int num9 = ordinal9 + 1;
        record.SetNullableString(ordinal9, widget.Settings);
        int ordinal10 = num9;
        int num10 = ordinal10 + 1;
        record.SetNullableString(ordinal10, widget.ArtifactId);
        int ordinal11 = num10;
        int num11 = ordinal11 + 1;
        record.SetNullableString(ordinal11, widget.SettingsVersion != null ? widget.SettingsVersion.ToString() : SemanticVersion.Default.ToString());
        int ordinal12 = num11;
        int num12 = ordinal12 + 1;
        record.SetNullableString(ordinal12, widget.ETag);
        int ordinal13 = num12;
        int num13 = ordinal13 + 1;
        record.SetNullableString(ordinal13, widget.Dashboard != null ? widget.Dashboard.ETag : "");
        int ordinal14 = num13;
        int num14 = ordinal14 + 1;
        record.SetBoolean(ordinal14, widget.ReplaceWidget);
        return record;
      });
      return this.BindTable(parameterName, "Dashboards.typ_WidgetTable3", rows.Select<WidgetDataModel, SqlDataRecord>(selector));
    }

    public override DashboardWidgetsDataModel UpdateWidgets(
      Guid dataspaceId,
      DashboardWidgetsDataModel dashboard)
    {
      this.PrepareStoredProcedure("Dashboards.prc_UpdateDashboardWidgetsV2");
      this.BindGuid("@dashboardId", dashboard.DashboardId);
      this.BindWidgetTable("@widgetTable", dashboard.Widgets);
      this.BindDataspaceId(dataspaceId);
      this.BindETag(dashboard.ETag);
      if (this.Version >= 12)
        this.BindGuid("@modifiedBy", dashboard.ModifiedBy);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Widget>((ObjectBinder<Widget>) new WidgetBinder2());
      resultCollection.AddBinder<string>((ObjectBinder<string>) new WidgetsETagBinder());
      List<Widget> items = resultCollection.GetCurrent<Widget>().Items;
      resultCollection.NextResult();
      string str = resultCollection.GetCurrent<string>().Items.FirstOrDefault<string>();
      DashboardWidgetsDataModel resultDashboard = new DashboardWidgetsDataModel()
      {
        DashboardId = dashboard.DashboardId,
        ETag = str
      };
      resultDashboard.Widgets = items.Select<Widget, WidgetDataModel>((System.Func<Widget, WidgetDataModel>) (widget => new WidgetDataModel(resultDashboard.DashboardId, widget)));
      return resultDashboard;
    }

    public override DashboardWidgetsDataModel ReplaceWidgets(
      Guid dataspaceId,
      DashboardWidgetsDataModel dashboard)
    {
      this.PrepareStoredProcedure("Dashboards.prc_UpdateDashboardWidgetsV2");
      this.BindGuid("@dashboardId", dashboard.DashboardId);
      this.BindWidgetTable("@widgetTable", dashboard.Widgets);
      this.BindBoolean("@replace", true);
      this.BindDataspaceId(dataspaceId);
      this.BindETag(dashboard.ETag);
      if (this.Version >= 12)
        this.BindGuid("@modifiedBy", dashboard.ModifiedBy);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Widget>((ObjectBinder<Widget>) new WidgetBinder2());
      resultCollection.AddBinder<string>((ObjectBinder<string>) new WidgetsETagBinder());
      List<Widget> items = resultCollection.GetCurrent<Widget>().Items;
      resultCollection.NextResult();
      string str = resultCollection.GetCurrent<string>().Items.FirstOrDefault<string>();
      DashboardWidgetsDataModel resultDashboard = new DashboardWidgetsDataModel()
      {
        DashboardId = dashboard.DashboardId,
        ETag = str
      };
      resultDashboard.Widgets = items.Select<Widget, WidgetDataModel>((System.Func<Widget, WidgetDataModel>) (widget => new WidgetDataModel(resultDashboard.DashboardId, widget)));
      return resultDashboard;
    }

    public override DashboardWidgetsDataModel GetWidgetsForDashboard(
      Guid dataspaceId,
      Guid dashboardId)
    {
      this.PrepareStoredProcedure("Dashboards.prc_GetWidgetByDashboardId");
      this.BindGuid("@dashboardId", dashboardId);
      this.BindDataspaceId(dataspaceId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Widget>((ObjectBinder<Widget>) new WidgetBinder2());
      resultCollection.AddBinder<string>((ObjectBinder<string>) new WidgetsETagBinder());
      List<Widget> items = resultCollection.GetCurrent<Widget>().Items;
      resultCollection.NextResult();
      string str = resultCollection.GetCurrent<string>().Items.FirstOrDefault<string>();
      DashboardWidgetsDataModel resultDashboard = new DashboardWidgetsDataModel()
      {
        DashboardId = dashboardId,
        ETag = str
      };
      resultDashboard.Widgets = items.Select<Widget, WidgetDataModel>((System.Func<Widget, WidgetDataModel>) (widget => new WidgetDataModel(resultDashboard.DashboardId, widget)));
      return resultDashboard;
    }

    public override Widget GetWidgetById(Guid dataspaceId, Guid id)
    {
      this.PrepareStoredProcedure("Dashboards.prc_GetWidgetById");
      this.BindGuid("@id", id);
      this.BindDataspaceId(dataspaceId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Widget>((ObjectBinder<Widget>) new WidgetBinder2());
      resultCollection.AddBinder<string>((ObjectBinder<string>) new WidgetsETagBinder());
      List<Widget> items = resultCollection.GetCurrent<Widget>().Items;
      Widget widgetById = items == null || items.Count <= 0 ? (Widget) null : items[0];
      if (widgetById != null)
      {
        resultCollection.NextResult();
        string ETag = resultCollection.GetCurrent<string>().Items.FirstOrDefault<string>();
        widgetById.Dashboard = new Dashboard(ETag);
      }
      return widgetById;
    }
  }
}

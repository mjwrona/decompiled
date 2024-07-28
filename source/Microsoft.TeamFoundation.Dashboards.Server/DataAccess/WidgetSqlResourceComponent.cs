// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.WidgetSqlResourceComponent
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
  public class WidgetSqlResourceComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[10]
    {
      (IComponentCreator) new ComponentCreator<WidgetSqlResourceComponent>(1),
      (IComponentCreator) new ComponentCreator<WidgetSqlResourceComponent4>(4),
      (IComponentCreator) new ComponentCreator<WidgetSqlResourceComponent5>(5),
      (IComponentCreator) new ComponentCreator<WidgetSqlResourceComponent6>(6),
      (IComponentCreator) new ComponentCreator<WidgetSqlResourceComponent6>(7),
      (IComponentCreator) new ComponentCreator<WidgetSqlResourceComponent6>(8),
      (IComponentCreator) new ComponentCreator<WidgetSqlResourceComponent9>(9),
      (IComponentCreator) new ComponentCreator<WidgetSqlResourceComponent9>(10),
      (IComponentCreator) new ComponentCreator<WidgetSqlResourceComponent11>(11),
      (IComponentCreator) new ComponentCreator<WidgetSqlResourceComponent12>(12)
    }, "Dashboards");
    private static readonly SqlMetaData[] typ_WidgetTable = new SqlMetaData[10]
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
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 256L)
    };
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1600301,
        new SqlExceptionFactory(typeof (WidgetExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new WidgetExistsException(Guid.Parse(sqEr.ExtractString("id")).ToString())))
      },
      {
        1600302,
        new SqlExceptionFactory(typeof (WidgetDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new WidgetDoesNotExistException(Guid.Parse(sqEr.ExtractString("id")))))
      },
      {
        1600303,
        new SqlExceptionFactory(typeof (WidgetETagConflictException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new WidgetETagConflictException()))
      },
      {
        1600304,
        new SqlExceptionFactory(typeof (WidgetConflictOnCopyException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new WidgetConflictOnCopyException()))
      }
    };

    public WidgetSqlResourceComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected virtual SqlParameter BindWidgetTable(string parameterName, WidgetDataModel row) => this.BindWidgetTable(parameterName, (IEnumerable<WidgetDataModel>) new List<WidgetDataModel>()
    {
      row
    });

    protected virtual SqlParameter BindWidgetTable(
      string parameterName,
      IEnumerable<WidgetDataModel> rows)
    {
      rows = rows ?? Enumerable.Empty<WidgetDataModel>();
      System.Func<WidgetDataModel, SqlDataRecord> selector = (System.Func<WidgetDataModel, SqlDataRecord>) (widget =>
      {
        ArgumentUtility.CheckForNull<WidgetDataModel>(widget, nameof (widget));
        SqlDataRecord record = new SqlDataRecord(WidgetSqlResourceComponent.typ_WidgetTable);
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
        return record;
      });
      return this.BindTable(parameterName, "Dashboards.typ_WidgetTable", rows.Select<WidgetDataModel, SqlDataRecord>(selector));
    }

    protected virtual void BindETag(string ETag)
    {
    }

    public virtual Widget AddWidget(Guid dataspaceId, WidgetDataModel widget)
    {
      this.PrepareStoredProcedure("Dashboards.prc_AddWidget");
      this.BindWidgetTable("@widgetTable", widget);
      this.BindDataspaceId(dataspaceId);
      this.ExecuteNonQuery();
      return (Widget) widget;
    }

    public virtual WidgetDataModel UpdateWidget(Guid dataspaceId, WidgetDataModel widget)
    {
      this.PrepareStoredProcedure("Dashboards.prc_UpdateWidget");
      this.BindWidgetTable("@widgetTable", widget);
      this.BindDataspaceId(dataspaceId);
      this.ExecuteNonQuery();
      return widget;
    }

    public virtual DashboardWidgetsDataModel UpdateWidgets(
      Guid dataspaceId,
      DashboardWidgetsDataModel dashboard)
    {
      this.PrepareStoredProcedure("Dashboards.prc_UpdateDashboardWidgets");
      this.BindGuid("@dashboardId", dashboard.DashboardId);
      this.BindWidgetTable("@widgetTable", dashboard.Widgets);
      this.BindDataspaceId(dataspaceId);
      this.ExecuteNonQuery();
      return dashboard;
    }

    public virtual void DeleteWidget(Guid dataspaceId, Guid id)
    {
      this.PrepareStoredProcedure("Dashboards.prc_DeleteWidget");
      this.BindGuid("@id", id);
      this.BindDataspaceId(dataspaceId);
      this.ExecuteNonQuery();
    }

    public virtual Widget GetWidgetById(Guid dataspaceId, Guid id)
    {
      this.PrepareStoredProcedure("Dashboards.prc_GetWidgetById");
      this.BindGuid("@id", id);
      this.BindDataspaceId(dataspaceId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Widget>((ObjectBinder<Widget>) new WidgetBinder());
      List<Widget> items = resultCollection.GetCurrent<Widget>().Items;
      return items != null && items.Count > 0 ? items[0] : (Widget) null;
    }

    public virtual IEnumerable<Widget> GetWidgetsByDashboardId(Guid dataspaceId, Guid dashboardId)
    {
      this.PrepareStoredProcedure("Dashboards.prc_GetWidgetByDashboardId");
      this.BindGuid("@dashboardId", dashboardId);
      this.BindDataspaceId(dataspaceId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Widget>((ObjectBinder<Widget>) new WidgetBinder());
      return (IEnumerable<Widget>) resultCollection.GetCurrent<Widget>().Items;
    }

    public virtual int GetWidgetCount(Guid dataspaceId, Guid id)
    {
      IEnumerable<Widget> widgetsByDashboardId = this.GetWidgetsByDashboardId(dataspaceId, id);
      return widgetsByDashboardId == null ? 0 : widgetsByDashboardId.Count<Widget>();
    }

    public virtual DashboardWidgetsDataModel ReplaceWidgets(
      Guid dataspaceId,
      DashboardWidgetsDataModel dashboard)
    {
      return (DashboardWidgetsDataModel) null;
    }

    public virtual DashboardWidgetsDataModel CreateDashboardWithWidgets(
      Guid dataspaceId,
      DashboardGroupEntryDataModel2 dashboard,
      DashboardWidgetsDataModel dashboardWidgets)
    {
      return (DashboardWidgetsDataModel) null;
    }

    public virtual DashboardWidgetsDataModel GetWidgetsForDashboard(
      Guid dataspaceId,
      Guid dashboardId)
    {
      this.PrepareStoredProcedure("Dashboards.prc_GetWidgetByDashboardId");
      this.BindGuid("@dashboardId", dashboardId);
      this.BindDataspaceId(dataspaceId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Widget>((ObjectBinder<Widget>) new WidgetBinder2());
      List<Widget> items = resultCollection.GetCurrent<Widget>().Items;
      DashboardWidgetsDataModel resultDashboard = new DashboardWidgetsDataModel()
      {
        DashboardId = dashboardId
      };
      resultDashboard.Widgets = items.Select<Widget, WidgetDataModel>((System.Func<Widget, WidgetDataModel>) (widget => new WidgetDataModel(resultDashboard.DashboardId, widget)));
      return resultDashboard;
    }

    protected virtual SqlParameter BindDataspaceId(Guid dataspaceId) => this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceId));

    protected override string TraceArea => "WidgetComponent";

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) WidgetSqlResourceComponent.s_sqlExceptionFactories;
  }
}

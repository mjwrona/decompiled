// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.DashboardGroupSqlResourceComponent
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
  public class DashboardGroupSqlResourceComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[10]
    {
      (IComponentCreator) new ComponentCreator<DashboardGroupSqlResourceComponent>(1),
      (IComponentCreator) new ComponentCreator<DashboardGroupSqlResourceComponent2>(2),
      (IComponentCreator) new ComponentCreator<DashboardGroupSqlResourceComponent3>(3),
      (IComponentCreator) new ComponentCreator<DashboardGroupSqlResourceComponent5>(5),
      (IComponentCreator) new ComponentCreator<DashboardGroupSqlResourceComponent7>(7),
      (IComponentCreator) new ComponentCreator<DashboardGroupSqlResourceComponent8>(8),
      (IComponentCreator) new ComponentCreator<DashboardGroupSqlResourceComponent8>(9),
      (IComponentCreator) new ComponentCreator<DashboardGroupSqlResourceComponent9>(10),
      (IComponentCreator) new ComponentCreator<DashboardGroupSqlResourceComponent9>(11),
      (IComponentCreator) new ComponentCreator<DashboardGroupSqlResourceComponent12>(12)
    }, "Dashboards");
    private static readonly SqlMetaData[] typ_DashboardGroupTable = new SqlMetaData[4]
    {
      new SqlMetaData("GroupId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 32L),
      new SqlMetaData("Position", SqlDbType.Int)
    };
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1600207,
        new SqlExceptionFactory(typeof (DashboardEntryExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new DashboardEntryExistsException(Guid.Parse(sqEr.ExtractString("id")).ToString())))
      },
      {
        1600208,
        new SqlExceptionFactory(typeof (DashboardDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new DashboardDoesNotExistException(Guid.Parse(sqEr.ExtractString("id")))))
      }
    };

    public DashboardGroupSqlResourceComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected virtual SqlParameter BindDashboardGroupTable(
      string parameterName,
      DashboardGroupEntryDataModel2 row)
    {
      return this.BindDashboardGroupTable(parameterName, (IEnumerable<DashboardGroupEntryDataModel2>) new DashboardGroupEntryDataModel2[1]
      {
        row
      });
    }

    protected virtual SqlParameter BindDashboardGroupTable(
      string parameterName,
      IEnumerable<DashboardGroupEntryDataModel2> rows)
    {
      rows = rows ?? Enumerable.Empty<DashboardGroupEntryDataModel2>();
      System.Func<DashboardGroupEntryDataModel, SqlDataRecord> selector = (System.Func<DashboardGroupEntryDataModel, SqlDataRecord>) (dashboardGroup =>
      {
        ArgumentUtility.CheckForNull<DashboardGroupEntryDataModel>(dashboardGroup, nameof (dashboardGroup));
        SqlDataRecord sqlDataRecord = new SqlDataRecord(DashboardGroupSqlResourceComponent.typ_DashboardGroupTable);
        int ordinal1 = 0;
        int num1 = ordinal1 + 1;
        sqlDataRecord.SetGuid(ordinal1, dashboardGroup.GroupId);
        int ordinal2 = num1;
        int num2 = ordinal2 + 1;
        sqlDataRecord.SetGuid(ordinal2, dashboardGroup.Id.Value);
        int ordinal3 = num2;
        int num3 = ordinal3 + 1;
        sqlDataRecord.SetString(ordinal3, dashboardGroup.Name);
        int ordinal4 = num3;
        int num4 = ordinal4 + 1;
        sqlDataRecord.SetInt32(ordinal4, dashboardGroup.Position);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Dashboards.typ_DashboardGroupTable", ((IEnumerable<DashboardGroupEntryDataModel>) rows).Select<DashboardGroupEntryDataModel, SqlDataRecord>(selector));
    }

    public virtual DashboardGroupEntry AddDashboard(
      Guid dataspaceId,
      DashboardGroupEntryDataModel2 entry)
    {
      this.PrepareStoredProcedure("Dashboards.prc_AddDashboardsToGroup");
      this.BindDashboardGroupTable("@dashboardGroupTable", entry);
      this.BindDataspaceId(dataspaceId);
      this.ExecuteNonQuery();
      return (DashboardGroupEntry) entry;
    }

    public virtual void UpdateDashboards(
      Guid dataspaceId,
      Guid groupId,
      DashboardScope scope,
      IEnumerable<DashboardGroupEntryDataModel2> entries)
    {
      this.PrepareStoredProcedure("Dashboards.prc_UpdateDashboards");
      this.BindGuid("@groupId", groupId);
      this.BindScope(scope);
      this.BindDashboardGroupTable("@dashboardGroupTable", entries);
      this.BindDataspaceId(dataspaceId);
      this.ExecuteNonQuery();
    }

    public virtual void DeleteDashboard(
      Guid dataspaceId,
      Guid groupId,
      DashboardScope scope,
      Guid dashboardId)
    {
      this.PrepareStoredProcedure("Dashboards.prc_DeleteDashboardFromGroup");
      this.BindGuid("@groupId", groupId);
      this.BindScope(scope);
      this.BindGuid("@dashboardId", dashboardId);
      this.BindDataspaceId(dataspaceId);
      this.ExecuteNonQuery();
    }

    public virtual DashboardGroupEntry GetDashboardById(
      Guid dataspaceId,
      Guid groupId,
      DashboardScope scope,
      Guid id)
    {
      this.PrepareStoredProcedure("Dashboards.prc_GetDashboard");
      this.BindGuid("@groupId", groupId);
      this.BindScope(scope);
      this.BindGuid("@id", id);
      this.BindDataspaceId(dataspaceId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DashboardGroupEntry>((ObjectBinder<DashboardGroupEntry>) new DashboardGroupBinder());
      return resultCollection.GetCurrent<DashboardGroupEntry>().SingleOrDefault<DashboardGroupEntry>();
    }

    public virtual List<DashboardGroupEntry> GetDashboardsByGroupId(
      Guid dataspaceId,
      Guid groupId,
      DashboardScope scope)
    {
      this.PrepareStoredProcedure("Dashboards.prc_GetDashboardGroup");
      this.BindGuid("@groupId", groupId);
      this.BindScope(scope);
      this.BindDataspaceId(dataspaceId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DashboardGroupEntry>((ObjectBinder<DashboardGroupEntry>) new DashboardGroupBinder());
      return resultCollection.GetCurrent<DashboardGroupEntry>().Items;
    }

    public virtual void UpdateDashboardLastAccessedDate(
      Guid dashboardId,
      Guid dataspaceId,
      Guid groupId)
    {
    }

    public virtual List<DashboardGroupEntry> GetDashboardsByProjectId(Guid projectId) => throw new NotSupportedException();

    public virtual List<DashboardGroupEntry> GetDashboardsByIds(
      Guid projectId,
      IEnumerable<Guid> dashboardIds)
    {
      throw new NotSupportedException();
    }

    protected SqlParameter BindDataspaceId(Guid dataspaceId) => this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceId));

    protected virtual void BindScope(DashboardScope scope)
    {
    }

    protected override string TraceArea => "DashboardGroupComponent";

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) DashboardGroupSqlResourceComponent.s_sqlExceptionFactories;
  }
}

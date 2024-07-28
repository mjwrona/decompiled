// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.DashboardGroupSqlResourceComponent3
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
  public class DashboardGroupSqlResourceComponent3 : DashboardGroupSqlResourceComponent2
  {
    private static readonly SqlMetaData[] typ_DashboardGroupTable3 = new SqlMetaData[6]
    {
      new SqlMetaData("GroupId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 32L),
      new SqlMetaData("Position", SqlDbType.Int),
      new SqlMetaData("Scope", SqlDbType.Int),
      new SqlMetaData("RefreshInterval", SqlDbType.Int)
    };

    protected override SqlParameter BindDashboardGroupTable(
      string parameterName,
      IEnumerable<DashboardGroupEntryDataModel2> rows)
    {
      rows = rows ?? Enumerable.Empty<DashboardGroupEntryDataModel2>();
      System.Func<DashboardGroupEntryDataModel2, SqlDataRecord> selector = (System.Func<DashboardGroupEntryDataModel2, SqlDataRecord>) (dashboardGroup =>
      {
        ArgumentUtility.CheckForNull<DashboardGroupEntryDataModel2>(dashboardGroup, nameof (dashboardGroup));
        SqlDataRecord sqlDataRecord = new SqlDataRecord(DashboardGroupSqlResourceComponent3.typ_DashboardGroupTable3);
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
        int ordinal5 = num4;
        int num5 = ordinal5 + 1;
        sqlDataRecord.SetInt32(ordinal5, (int) dashboardGroup.Scope);
        int ordinal6 = num5;
        int num6 = ordinal6 + 1;
        sqlDataRecord.SetInt32(ordinal6, dashboardGroup.RefreshInterval.GetValueOrDefault(0));
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Dashboards.typ_DashboardGroupTable3", rows.Select<DashboardGroupEntryDataModel2, SqlDataRecord>(selector));
    }

    public override DashboardGroupEntry AddDashboard(
      Guid dataspaceId,
      DashboardGroupEntryDataModel2 entry)
    {
      this.PrepareStoredProcedure("Dashboards.prc_AddDashboardsToGroup");
      this.BindDashboardGroupTable("@dashboardGroupTable", entry);
      this.BindDataspaceId(dataspaceId);
      this.ExecuteNonQuery();
      return (DashboardGroupEntry) entry;
    }

    public override DashboardGroupEntry GetDashboardById(
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
      resultCollection.AddBinder<DashboardGroupEntry>((ObjectBinder<DashboardGroupEntry>) new DashboardGroupBinder2());
      return resultCollection.GetCurrent<DashboardGroupEntry>().SingleOrDefault<DashboardGroupEntry>();
    }

    public override List<DashboardGroupEntry> GetDashboardsByGroupId(
      Guid dataspaceId,
      Guid groupId,
      DashboardScope scope)
    {
      this.PrepareStoredProcedure("Dashboards.prc_GetDashboardGroup");
      this.BindGuid("@groupId", groupId);
      this.BindScope(scope);
      this.BindDataspaceId(dataspaceId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DashboardGroupEntry>((ObjectBinder<DashboardGroupEntry>) new DashboardGroupBinder2());
      return resultCollection.GetCurrent<DashboardGroupEntry>().Items;
    }
  }
}

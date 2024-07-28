// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.DashboardGroupSqlResourceComponent12
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Dashboards.Model;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Dashboards.DataAccess
{
  public class DashboardGroupSqlResourceComponent12 : DashboardGroupSqlResourceComponent9
  {
    private static readonly SqlMetaData[] typ_DashboardGroupTable7 = new SqlMetaData[12]
    {
      new SqlMetaData("GroupId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 32L),
      new SqlMetaData("Position", SqlDbType.Int),
      new SqlMetaData("Scope", SqlDbType.Int),
      new SqlMetaData("RefreshInterval", SqlDbType.Int),
      new SqlMetaData("ETag", SqlDbType.VarChar, 256L),
      new SqlMetaData("Description", SqlDbType.NVarChar, 128L),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ModifiedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ModifiedDate", SqlDbType.DateTime),
      new SqlMetaData("LastAccessedDate", SqlDbType.DateTime)
    };

    public override void UpdateDashboardLastAccessedDate(
      Guid dashboardId,
      Guid dataspaceId,
      Guid groupId)
    {
      this.PrepareStoredProcedure("Dashboards.prc_UpdateDashboardLastAccessedDate");
      this.BindGuid("@dashboardId", dashboardId);
      this.BindDataspaceId(dataspaceId);
      this.BindGuid("@groupId", groupId);
      this.ExecuteNonQuery();
    }

    protected override SqlParameter BindDashboardGroupTable(
      string parameterName,
      IEnumerable<DashboardGroupEntryDataModel2> rows)
    {
      rows = rows ?? Enumerable.Empty<DashboardGroupEntryDataModel2>();
      System.Func<DashboardGroupEntryDataModel2, SqlDataRecord> selector = (System.Func<DashboardGroupEntryDataModel2, SqlDataRecord>) (dashboardGroup =>
      {
        ArgumentUtility.CheckForNull<DashboardGroupEntryDataModel2>(dashboardGroup, nameof (dashboardGroup));
        SqlDataRecord record = new SqlDataRecord(DashboardGroupSqlResourceComponent12.typ_DashboardGroupTable7);
        int ordinal1 = 0;
        int num1 = ordinal1 + 1;
        record.SetGuid(ordinal1, dashboardGroup.GroupId);
        int ordinal2 = num1;
        int num2 = ordinal2 + 1;
        record.SetGuid(ordinal2, dashboardGroup.Id.Value);
        int ordinal3 = num2;
        int num3 = ordinal3 + 1;
        record.SetString(ordinal3, dashboardGroup.Name);
        int ordinal4 = num3;
        int num4 = ordinal4 + 1;
        record.SetInt32(ordinal4, dashboardGroup.Position);
        int ordinal5 = num4;
        int num5 = ordinal5 + 1;
        record.SetInt32(ordinal5, (int) dashboardGroup.Scope);
        int ordinal6 = num5;
        int num6 = ordinal6 + 1;
        record.SetInt32(ordinal6, dashboardGroup.RefreshInterval.GetValueOrDefault(0));
        int ordinal7 = num6;
        int num7 = ordinal7 + 1;
        record.SetString(ordinal7, dashboardGroup.ETag, BindStringBehavior.Unchanged);
        int ordinal8 = num7;
        int num8 = ordinal8 + 1;
        record.SetString(ordinal8, dashboardGroup.Description, BindStringBehavior.Unchanged);
        int ordinal9 = num8;
        int num9 = ordinal9 + 1;
        record.SetGuid(ordinal9, dashboardGroup.OwnerId);
        int ordinal10 = num9;
        int num10 = ordinal10 + 1;
        record.SetGuid(ordinal10, dashboardGroup.ModifiedBy);
        int ordinal11 = num10;
        int num11 = ordinal11 + 1;
        record.SetNullableDateTime(ordinal11, dashboardGroup.ModifiedDate);
        int ordinal12 = num11;
        int num12 = ordinal12 + 1;
        record.SetNullableDateTime(ordinal12, dashboardGroup.LastAccessedDate);
        return record;
      });
      return this.BindTable(parameterName, "Dashboards.typ_DashboardGroupTable7", rows.Select<DashboardGroupEntryDataModel2, SqlDataRecord>(selector));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component70
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component70 : Build2Component69
  {
    protected static readonly SqlMetaData[] typ_MinimalRetentionLeaseTable = new SqlMetaData[3]
    {
      new SqlMetaData("OwnerId", SqlDbType.NVarChar, 400L),
      new SqlMetaData("RunId", SqlDbType.Int),
      new SqlMetaData("DefinitionId", SqlDbType.Int)
    };
    protected static readonly SqlMetaData[] typ_RetentionLeaseTable = new SqlMetaData[5]
    {
      new SqlMetaData("OwnerId", SqlDbType.NVarChar, 400L),
      new SqlMetaData("RunId", SqlDbType.Int),
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("ValidUntil", SqlDbType.Date),
      new SqlMetaData("HighPriority", SqlDbType.Bit)
    };

    public override async Task<IList<BuildSchedule>> GetSchedulesByDefinitionId(
      Guid projectId,
      int definitionId)
    {
      Build2Component70 build2Component70 = this;
      IList<BuildSchedule> list;
      using (build2Component70.TraceScope(method: nameof (GetSchedulesByDefinitionId)))
      {
        build2Component70.PrepareStoredProcedure("Build.prc_GetSchedulesByDefinitionId");
        build2Component70.BindInt("@dataspaceId", build2Component70.GetDataspaceId(projectId));
        build2Component70.BindInt("@definitionId", definitionId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component70.ExecuteReaderAsync(), build2Component70.ProcedureName, build2Component70.RequestContext))
        {
          resultCollection.AddBinder<BuildSchedule>(build2Component70.GetBuildScheduleBinder());
          list = (IList<BuildSchedule>) resultCollection.GetCurrent<BuildSchedule>().Items.Distinct<BuildSchedule>().ToList<BuildSchedule>();
        }
      }
      return list;
    }

    protected virtual ObjectBinder<BuildSchedule> GetBuildScheduleBinder() => (ObjectBinder<BuildSchedule>) new BuildScheduleBinder(this.RequestContext, (BuildSqlComponentBase) this);

    public override async Task<IReadOnlyList<RetentionLease>> AddRetentionLeases(
      Guid projectId,
      IEnumerable<RetentionLease> leases)
    {
      Build2Component70 build2Component70 = this;
      IReadOnlyList<RetentionLease> retentionLeaseList;
      using (build2Component70.TraceScope(method: nameof (AddRetentionLeases)))
      {
        build2Component70.PrepareStoredProcedure("Build.prc_AddRetentionLeases");
        build2Component70.BindInt("@dataspaceId", build2Component70.GetDataspaceId(projectId));
        build2Component70.BindRetentionLeaseTable("@leases", leases);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component70.ExecuteReaderAsync(), build2Component70.ProcedureName, build2Component70.RequestContext))
        {
          resultCollection.AddBinder<RetentionLease>(build2Component70.GetRetentionLeaseBinder());
          retentionLeaseList = (IReadOnlyList<RetentionLease>) resultCollection.GetCurrent<RetentionLease>().Items.AsReadOnly();
        }
      }
      return retentionLeaseList;
    }

    public override async Task<IReadOnlyList<RetentionLease>> DeleteRetentionLeases(
      Guid projectId,
      HashSet<int> leaseIds)
    {
      Build2Component70 component = this;
      IReadOnlyList<RetentionLease> leases = await component.GetRetentionLeasesByIds(projectId, leaseIds);
      using (component.TraceScope(method: nameof (DeleteRetentionLeases)))
      {
        component.PrepareStoredProcedure("Build.prc_DeleteRetentionLeases");
        component.BindInt("@dataspaceID", component.GetDataspaceId(projectId));
        component.BindUniqueInt32Table("@leaseIds", (IEnumerable<int>) leaseIds);
        object obj = await component.ExecuteNonQueryAsync(false);
      }
      IReadOnlyList<RetentionLease> retentionLeaseList = leases;
      leases = (IReadOnlyList<RetentionLease>) null;
      return retentionLeaseList;
    }

    public override sealed async Task<RetentionLease> GetRetentionLeaseById(
      Guid projectId,
      int leaseId)
    {
      return (await this.GetRetentionLeasesByIds(projectId, ((IEnumerable<int>) new int[1]
      {
        leaseId
      }).ToHashSet<int>())).SingleOrDefault<RetentionLease>();
    }

    public override async Task<IReadOnlyList<RetentionLease>> GetRetentionLeasesByIds(
      Guid projectId,
      HashSet<int> leaseIds)
    {
      Build2Component70 component = this;
      IReadOnlyList<RetentionLease> retentionLeasesByIds;
      using (component.TraceScope(method: nameof (GetRetentionLeasesByIds)))
      {
        component.PrepareStoredProcedure("Build.prc_GetRetentionLeasesByIds");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindUniqueInt32Table("@leaseIds", (IEnumerable<int>) leaseIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<RetentionLease>(component.GetRetentionLeaseBinder());
          retentionLeasesByIds = (IReadOnlyList<RetentionLease>) resultCollection.GetCurrent<RetentionLease>().Items.AsReadOnly();
        }
      }
      return retentionLeasesByIds;
    }

    public override async Task<IReadOnlyList<RetentionLease>> GetRetentionLeases(
      Guid projectId,
      IEnumerable<MinimalRetentionLease> leases,
      bool useOptimizedSelect = false)
    {
      Build2Component70 build2Component70 = this;
      IReadOnlyList<RetentionLease> retentionLeases;
      using (build2Component70.TraceScope(method: nameof (GetRetentionLeases)))
      {
        build2Component70.PrepareStoredProcedure("Build.prc_GetRetentionLeases");
        build2Component70.BindInt("@dataspaceId", build2Component70.GetDataspaceId(projectId));
        if (useOptimizedSelect)
          build2Component70.BindBoolean("@useOptimizedSelect", useOptimizedSelect);
        build2Component70.BindMinimalRetentionLeaseTable("@leases", leases);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component70.ExecuteReaderAsync(), build2Component70.ProcedureName, build2Component70.RequestContext))
        {
          resultCollection.AddBinder<RetentionLease>(build2Component70.GetRetentionLeaseBinder());
          retentionLeases = (IReadOnlyList<RetentionLease>) resultCollection.GetCurrent<RetentionLease>().Items.AsReadOnly();
        }
      }
      return retentionLeases;
    }

    protected virtual SqlParameter BindMinimalRetentionLeaseTable(
      string parameterName,
      IEnumerable<MinimalRetentionLease> leases)
    {
      leases = leases ?? Enumerable.Empty<MinimalRetentionLease>();
      return this.BindTable(parameterName, "Build.typ_MinimalRetentionLeaseTable", (IEnumerable<SqlDataRecord>) leases.Select<MinimalRetentionLease, SqlDataRecord>(new System.Func<MinimalRetentionLease, SqlDataRecord>(rowBinder)).ToArray<SqlDataRecord>());

      static SqlDataRecord rowBinder(MinimalRetentionLease lease)
      {
        SqlDataRecord record = new SqlDataRecord(Build2Component70.typ_MinimalRetentionLeaseTable);
        record.SetNullableString(0, string.IsNullOrWhiteSpace(lease.OwnerId) ? (string) null : lease.OwnerId);
        record.SetNullableInt32(1, lease.RunId);
        record.SetNullableInt32(2, lease.DefinitionId);
        return record;
      }
    }

    protected virtual SqlParameter BindRetentionLeaseTable(
      string parameterName,
      IEnumerable<RetentionLease> leases)
    {
      leases = leases ?? Enumerable.Empty<RetentionLease>();
      return this.BindTable(parameterName, "Build.typ_RetentionLeaseTable", (IEnumerable<SqlDataRecord>) leases.Select<RetentionLease, SqlDataRecord>(new System.Func<RetentionLease, SqlDataRecord>(rowBinder)).ToArray<SqlDataRecord>());

      static SqlDataRecord rowBinder(RetentionLease lease)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(Build2Component70.typ_RetentionLeaseTable);
        sqlDataRecord.SetString(0, lease.OwnerId);
        sqlDataRecord.SetInt32(1, lease.RunId);
        sqlDataRecord.SetInt32(2, lease.DefinitionId);
        sqlDataRecord.SetDateTime(3, lease.ValidUntil);
        sqlDataRecord.SetBoolean(4, lease.HighPriority);
        return sqlDataRecord;
      }
    }
  }
}

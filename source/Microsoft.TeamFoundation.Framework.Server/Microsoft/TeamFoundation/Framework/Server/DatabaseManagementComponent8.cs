// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementComponent8
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseManagementComponent8 : DatabaseManagementComponent7
  {
    public override ITeamFoundationDatabaseProperties AcquireDatabasePartition(
      int databaseIdToAcquire,
      AcquirePartitionOptions acquireOptions)
    {
      if (databaseIdToAcquire == -2)
        throw new VirtualServiceHostException();
      this.PrepareStoredProcedure("prc_AcquireDatabasePartition");
      this.BindInt("@databaseIdToAcquire", databaseIdToAcquire);
      SqlDataReader dataReader = this.ExecuteReader();
      if (!dataReader.Read())
        return (ITeamFoundationDatabaseProperties) null;
      InternalDatabaseProperties result;
      this.CreatePropertiesBinder(dataReader, this.ProcedureName).Bind(out result);
      return (ITeamFoundationDatabaseProperties) result;
    }

    public override ITeamFoundationDatabaseProperties RegisterDatabase(
      string connectionString,
      string databaseName,
      string serviceLevel,
      string poolName,
      int tenants,
      int maxTenants,
      TeamFoundationDatabaseStatus status,
      DateTime statusChangedDate,
      string statusReason,
      DateTime? lastTenantAdded,
      bool registerCredential,
      string userId,
      byte[] passwordEncrypted,
      Guid signingKeyId,
      TeamFoundationDatabaseFlags flags,
      string serviceObjective)
    {
      if (string.IsNullOrEmpty(databaseName))
        databaseName = TeamFoundationDatabaseManagementService.ParseUniqueConnectionStringFields(connectionString);
      this.PrepareStoredProcedure("prc_RegisterDatabase");
      connectionString = SqlConnectionHelper.SanitizeConnectionString(connectionString);
      this.BindString("@connectionString", connectionString, 520, false, SqlDbType.NVarChar);
      this.BindString("@databaseName", databaseName, 520, true, SqlDbType.NVarChar);
      this.BindString("@serviceLevel", serviceLevel, (int) byte.MaxValue, true, SqlDbType.VarChar);
      this.BindString("@poolName", poolName, 256, false, SqlDbType.VarChar);
      this.BindInt("@tenants", tenants);
      this.BindInt("@maxTenants", maxTenants);
      this.BindInt("@status", (int) status);
      this.BindDateTime("@statusChangedDate", statusChangedDate);
      this.BindString("@statusReason", statusReason, int.MaxValue, true, SqlDbType.VarChar);
      if (lastTenantAdded.HasValue)
      {
        DateTime? nullable = lastTenantAdded;
        DateTime minValue = DateTime.MinValue;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != minValue ? 1 : 0) : 0) : 1) != 0)
        {
          this.BindDateTime("@lastTenantAdded", lastTenantAdded.Value);
          goto label_6;
        }
      }
      this.BindNullValue("@lastTenantAdded", SqlDbType.DateTime);
label_6:
      this.BindBoolean("@registerCredential", registerCredential);
      this.BindString("@userId", userId, 128, true, SqlDbType.NVarChar);
      if (this.Version >= 10)
      {
        this.BindGuid("@signingKeyId", signingKeyId);
        this.BindBinary("@passwordEncrypted", passwordEncrypted, 1024, SqlDbType.VarBinary);
      }
      else
        this.BindBinary("@passwordEncrypted", passwordEncrypted, 256, SqlDbType.Binary);
      if (this.Version >= 13)
        this.BindInt("@flags", (int) flags);
      SqlDataReader dataReader = this.ExecuteReader();
      if (!dataReader.Read())
        throw new DatabaseNotFoundException();
      InternalDatabaseProperties result;
      this.CreatePropertiesBinder(dataReader, this.ProcedureName).Bind(out result);
      return (ITeamFoundationDatabaseProperties) result;
    }

    public override ITeamFoundationDatabaseProperties UpdateDatabase(
      TeamFoundationDatabaseProperties properties)
    {
      if (!properties.IsUpdateRequired())
        return properties.GetCachedProperties();
      if (properties.DatabaseId == -2)
        throw new VirtualServiceHostException();
      this.PrepareStoredProcedure("prc_UpdateDatabase");
      this.BindInt("@databaseId", properties.DatabaseId);
      this.BindLong("@version", properties.Version);
      if (properties.IsConnectionInfoDirty)
        this.BindString("@connectionString", properties.ConnectionInfoWrapper.ConnectionString, 520, false, SqlDbType.NVarChar);
      if (properties.IsServiceLevelDirty)
        this.BindString("@serviceLevel", properties.ServiceLevel, (int) byte.MaxValue, true, SqlDbType.VarChar);
      if (properties.IsPoolNameDirty)
        this.BindString("@poolName", this.GetLegacyPoolName(properties.PoolName), 256, false, SqlDbType.VarChar);
      if (properties.IsTenantsDirty)
        this.BindInt("@tenants", properties.Tenants);
      if (properties.IsMaxTenantsDirty)
        this.BindInt("@maxTenants", properties.MaxTenants);
      if (properties.IsFlagsDirty)
        this.BindInt("@flags", (int) properties.Flags);
      if (properties.IsTenantsPendingDeleteDirty)
        this.BindInt("@tenantsPendingDelete", properties.TenantsPendingDelete);
      if (properties.IsAcquisitionOrderDirty)
        this.BindDateTime("@acquisitionOrder", properties.AcquisitionOrder);
      if (properties.IsDatabaseSizeDirty)
        this.BindInt("@databaseSize", properties.DatabaseSize);
      if (properties.IsDatabaseCapacityDirty)
        this.BindInt("@databaseCapacity", properties.DatabaseCapacity);
      if (properties.IsStatusDirty)
        this.BindInt("@status", (int) properties.Status);
      if (properties.IsStatusReasonDirty)
        this.BindString("@statusReason", properties.StatusReason, int.MaxValue, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      SqlDataReader dataReader = this.ExecuteReader();
      if (!dataReader.Read())
        throw new DatabaseNotFoundException();
      InternalDatabaseProperties result;
      this.CreatePropertiesBinder(dataReader, this.ProcedureName).Bind(out result);
      TeamFoundationTracingService.TraceRaw(99117, TraceLevel.Info, "DatabaseManagement", "Component", "Updated database properties. New version: {0}. Database Properties:  {1}", (object) result.Version, (object) properties);
      properties.Updated();
      return (ITeamFoundationDatabaseProperties) result;
    }

    public override void FlushDatabaseCache()
    {
      this.PrepareStoredProcedure("prc_FlushDatabaseCache");
      this.ExecuteNonQuery();
    }

    protected override DatabasePropertiesBinder CreatePropertiesBinder() => (DatabasePropertiesBinder) new DatabasePropertiesBinder5();

    protected override DatabasePropertiesBinder CreatePropertiesBinder(
      SqlDataReader dataReader,
      string storedProcedure)
    {
      return (DatabasePropertiesBinder) new DatabasePropertiesBinder5(dataReader, storedProcedure);
    }
  }
}

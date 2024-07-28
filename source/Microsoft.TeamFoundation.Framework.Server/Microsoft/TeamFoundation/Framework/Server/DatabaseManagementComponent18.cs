// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementComponent18
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseManagementComponent18 : DatabaseManagementComponent17
  {
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
        this.BindString("@connectionString", SqlConnectionHelper.SanitizeConnectionString(properties.ConnectionInfoWrapper.ConnectionString), 520, false, SqlDbType.NVarChar);
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
      if (properties.IsMinServiceObjectiveDirty)
        this.BindString("@minServiceObjective", properties.MinServiceObjective, 20, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      if (properties.IsMaxServiceObjectiveDirty)
        this.BindString("@maxServiceObjective", properties.MaxServiceObjective, 20, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      if (properties.IsRequestTimeoutDirty)
        this.BindInt("@requestTimeout", properties.RequestTimeout);
      if (properties.IsDeadlockRetriesDirty)
        this.BindInt("@deadlockRetries", properties.DeadlockRetries);
      if (properties.IsDeadlockPauseDirty)
        this.BindInt("@deadlockPause", properties.DeadlockPause);
      if (properties.IsLoggingOptionsDirty)
        this.BindByte("@loggingOptions", (byte) properties.LoggingOptions);
      if (properties.IsExecutionTimeThresholdDirty)
        this.BindInt("@executionTimeThreshold", (int) properties.ExecutionTimeThreshold.TotalSeconds);
      if (properties.IsBreakerDisabledDirty)
        this.BindBoolean("@breakerDisabled", properties.BreakerDisabled);
      if (properties.IsBreakerErrorThresholdPercDirty)
        this.BindByte("@breakerErrorThresholdPerc", properties.BreakerErrorThresholdPerc);
      if (properties.IsBreakerForceClosedDirty)
        this.BindBoolean("@breakerForceClosed", properties.BreakerForceClosed);
      if (properties.IsBreakerForceOpenDirty)
        this.BindBoolean("@breakerForceOpen", properties.BreakerForceOpen);
      if (properties.IsBreakerMaxBackoffDirty)
        this.BindInt("@breakerMaxBackoff", properties.BreakerMaxBackoff);
      if (properties.IsBreakerRequestVolumeThresholdDirty)
        this.BindInt("@breakerRequestVolumeThreshold", properties.BreakerRequestVolumeThreshold);
      if (properties.IsBreakerExecutionTimeoutDirty)
        this.BindInt("@breakerExecutionTimeout", properties.BreakerExecutionTimeout);
      if (properties.IsBreakerMaxExecConcurrentRequestsDirty)
        this.BindInt("@breakerMaxExecConcurrentRequests", properties.BreakerMaxExecConcurrentRequests);
      if (properties.IsBreakerMaxFallbackConcurrentRequestsDirty)
        this.BindInt("@breakerMaxFallbackConcurrentRequests", properties.BreakerMaxFallbackConcurrentRequests);
      SqlDataReader dataReader = this.ExecuteReader();
      if (!dataReader.Read())
        throw new DatabaseNotFoundException();
      InternalDatabaseProperties result;
      this.CreatePropertiesBinder(dataReader, this.ProcedureName).Bind(out result);
      TeamFoundationTracingService.TraceRaw(99117, TraceLevel.Info, "DatabaseManagement", "Component", "Updated database properties. New version: {0}. Database Properties:  {1}", (object) result.Version, (object) properties);
      properties.Updated();
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
      string minServiceObjective;
      string maxServiceObjective;
      DatabaseManagementComponent18.ComputeMinMaxServiceObjective(poolName, serviceObjective, out minServiceObjective, out maxServiceObjective);
      this.BindString("@minServiceObjective", minServiceObjective, 20, true, SqlDbType.VarChar);
      this.BindString("@maxServiceObjective", maxServiceObjective, 20, true, SqlDbType.VarChar);
      SqlDataReader dataReader = this.ExecuteReader();
      if (!dataReader.Read())
        throw new DatabaseNotFoundException();
      InternalDatabaseProperties result;
      this.CreatePropertiesBinder(dataReader, this.ProcedureName).Bind(out result);
      return (ITeamFoundationDatabaseProperties) result;
    }

    public static void ComputeMinMaxServiceObjective(
      string poolName,
      string serviceObjective,
      out string minServiceObjective,
      out string maxServiceObjective)
    {
      minServiceObjective = string.Empty;
      maxServiceObjective = string.Empty;
      if (string.IsNullOrEmpty(serviceObjective) || !string.Equals(poolName, DatabaseManagementConstants.ConfigurationPoolName) && !string.Equals(poolName, DatabaseManagementConstants.DefaultPartitionPoolName))
        return;
      if (serviceObjective.StartsWith("S"))
      {
        minServiceObjective = "S2";
        int result;
        if (int.TryParse(serviceObjective.Substring(1), out result) && result > 7)
          maxServiceObjective = serviceObjective;
        else
          maxServiceObjective = "S7";
      }
      else
      {
        if (!serviceObjective.StartsWith("P") || serviceObjective.StartsWith("PRS"))
          return;
        minServiceObjective = "P1";
        int result;
        if (int.TryParse(serviceObjective.Substring(1), out result) && result > 6)
          maxServiceObjective = serviceObjective;
        else
          maxServiceObjective = "P6";
      }
    }

    protected override DatabasePropertiesBinder CreatePropertiesBinder() => (DatabasePropertiesBinder) new DatabasePropertiesBinder9();

    protected override DatabasePropertiesBinder CreatePropertiesBinder(
      SqlDataReader dataReader,
      string storedProcedure)
    {
      return (DatabasePropertiesBinder) new DatabasePropertiesBinder9(dataReader, storedProcedure);
    }
  }
}

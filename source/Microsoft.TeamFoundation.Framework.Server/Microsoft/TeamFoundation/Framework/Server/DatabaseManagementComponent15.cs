// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementComponent15
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseManagementComponent15 : DatabaseManagementComponent12
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

    protected override DatabasePropertiesBinder CreatePropertiesBinder() => (DatabasePropertiesBinder) new DatabasePropertiesBinder8();

    protected override DatabasePropertiesBinder CreatePropertiesBinder(
      SqlDataReader dataReader,
      string storedProcedure)
    {
      return (DatabasePropertiesBinder) new DatabasePropertiesBinder8(dataReader, storedProcedure);
    }
  }
}

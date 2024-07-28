// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.InternalDatabaseProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.DatabaseReplication;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class InternalDatabaseProperties : ITeamFoundationDatabaseProperties
  {
    public static readonly int DefaultDatabaseRequestTimeout = 300;
    private VssRefreshCache<DatabaseResourceStats> m_databaseResourceStatsCache;
    private static readonly string s_layer = "PropertiesCache";
    private static readonly string s_area = "DatabaseManagement";

    public InternalDatabaseProperties()
    {
      this.RequestTimeout = InternalDatabaseProperties.DefaultDatabaseRequestTimeout;
      this.DeadlockRetries = 20;
      this.DeadlockPause = 200;
      this.LoggingOptions = DatabaseManagementConstants.DefaultDatabaseLoggingOptions;
      this.ExecutionTimeThreshold = new TimeSpan(0, 0, 30);
      this.m_databaseResourceStatsCache = new VssRefreshCache<DatabaseResourceStats>(TimeSpan.FromSeconds(DatabaseManagementConstants.PerfCacheRefreshInterval), new Func<IVssRequestContext, DatabaseResourceStats>(this.FetchDatabaseResourceStats), true);
    }

    private TeamFoundationDatabaseProperties GetEditableCopy() => new TeamFoundationDatabaseProperties(this);

    internal void Initialize(
      int databaseId,
      string connectionString,
      string databaseName,
      string serviceLevel,
      string poolName,
      int tenants,
      int maxTenants,
      TeamFoundationDatabaseStatus status,
      DateTime statusChangedDate,
      string statusReason,
      DateTime lastTenantAdded,
      DateTime acquisitionOrder)
    {
      this.Initialize(databaseId, connectionString, databaseName, serviceLevel, poolName, tenants, maxTenants, 0, status, statusChangedDate, statusReason, lastTenantAdded, acquisitionOrder, TeamFoundationDatabaseFlags.None);
    }

    internal void Initialize(
      int databaseId,
      string connectionString,
      string databaseName,
      string serviceLevel,
      string poolName,
      int tenants,
      int maxTenants,
      int tenantsPendingDelete,
      TeamFoundationDatabaseStatus status,
      DateTime statusChangedDate,
      string statusReason,
      DateTime lastTenantAdded,
      DateTime acquisitionOrder,
      TeamFoundationDatabaseFlags flags)
    {
      this.Initialize(databaseId, connectionString, databaseName, serviceLevel, poolName, tenants, maxTenants, tenantsPendingDelete, 0, 0, DateTime.MinValue, status, statusChangedDate, statusReason, lastTenantAdded, acquisitionOrder, flags);
    }

    internal void Initialize(
      int databaseId,
      string connectionString,
      string databaseName,
      string serviceLevel,
      string poolName,
      int tenants,
      int maxTenants,
      int tenantsPendingDelete,
      int databaseSize,
      int databaseCapacity,
      DateTime sizeChangedDate,
      TeamFoundationDatabaseStatus status,
      DateTime statusChangedDate,
      string statusReason,
      DateTime lastTenantAdded,
      DateTime acquisitionOrder,
      TeamFoundationDatabaseFlags flags)
    {
      int databaseId1 = databaseId;
      SqlConnectionInfoWrapper connectionInfo = new SqlConnectionInfoWrapper();
      connectionInfo.ConnectionString = connectionString;
      string databaseName1 = databaseName;
      string serviceLevel1 = serviceLevel;
      string poolName1 = poolName;
      int tenants1 = tenants;
      int maxTenants1 = maxTenants;
      int tenantsPendingDelete1 = tenantsPendingDelete;
      int databaseSize1 = databaseSize;
      int databaseCapacity1 = databaseCapacity;
      DateTime sizeChangedDate1 = sizeChangedDate;
      int status1 = (int) status;
      DateTime statusChangedDate1 = statusChangedDate;
      string statusReason1 = statusReason;
      DateTime lastTenantAdded1 = lastTenantAdded;
      DateTime acquisitionOrder1 = acquisitionOrder;
      int flags1 = (int) flags;
      long invalidDatabaseVersion = (long) DatabaseManagementConstants.InvalidDatabaseVersion;
      this.Initialize(databaseId1, connectionInfo, databaseName1, serviceLevel1, poolName1, tenants1, maxTenants1, tenantsPendingDelete1, databaseSize1, databaseCapacity1, sizeChangedDate1, (TeamFoundationDatabaseStatus) status1, statusChangedDate1, statusReason1, lastTenantAdded1, acquisitionOrder1, (TeamFoundationDatabaseFlags) flags1, invalidDatabaseVersion);
    }

    internal void Initialize(
      int databaseId,
      SqlConnectionInfoWrapper connectionInfo,
      string databaseName,
      string serviceLevel,
      string poolName,
      int tenants,
      int maxTenants,
      int tenantsPendingDelete,
      int databaseSize,
      int databaseCapacity,
      DateTime sizeChangedDate,
      TeamFoundationDatabaseStatus status,
      DateTime statusChangedDate,
      string statusReason,
      DateTime lastTenantAdded,
      DateTime acquisitionOrder,
      TeamFoundationDatabaseFlags flags,
      long version)
    {
      this.Initialize(databaseId, connectionInfo, databaseName, serviceLevel, poolName, tenants, maxTenants, tenantsPendingDelete, databaseSize, databaseCapacity, sizeChangedDate, status, statusChangedDate, statusReason, lastTenantAdded, acquisitionOrder, flags, version, InternalDatabaseProperties.DefaultDatabaseRequestTimeout, 20, 200, TeamFoundationDatabaseLoggingOptions.None, new TimeSpan(0, 0, 30));
    }

    internal void Initialize(
      int databaseId,
      SqlConnectionInfoWrapper connectionInfo,
      string databaseName,
      string serviceLevel,
      string poolName,
      int tenants,
      int maxTenants,
      int tenantsPendingDelete,
      int databaseSize,
      int databaseCapacity,
      DateTime sizeChangedDate,
      TeamFoundationDatabaseStatus status,
      DateTime statusChangedDate,
      string statusReason,
      DateTime lastTenantAdded,
      DateTime acquisitionOrder,
      TeamFoundationDatabaseFlags flags,
      long version,
      int requestTimeout,
      int deadlockRetries,
      int deadlockPause,
      TeamFoundationDatabaseLoggingOptions loggingOptions,
      TimeSpan executionTimeThreshold)
    {
      this.Initialize(databaseId, connectionInfo, databaseName, serviceLevel, poolName, tenants, maxTenants, tenantsPendingDelete, databaseSize, databaseCapacity, sizeChangedDate, status, statusChangedDate, statusReason, lastTenantAdded, acquisitionOrder, flags, version, requestTimeout, deadlockRetries, deadlockPause, loggingOptions, executionTimeThreshold, false, byte.MaxValue, false, false, -1, -1, -1, -1, -1);
    }

    internal void Initialize(
      int databaseId,
      SqlConnectionInfoWrapper connectionInfo,
      string databaseName,
      string serviceLevel,
      string poolName,
      int tenants,
      int maxTenants,
      int tenantsPendingDelete,
      int databaseSize,
      int databaseCapacity,
      DateTime sizeChangedDate,
      TeamFoundationDatabaseStatus status,
      DateTime statusChangedDate,
      string statusReason,
      DateTime lastTenantAdded,
      DateTime acquisitionOrder,
      TeamFoundationDatabaseFlags flags,
      long version,
      int requestTimeout,
      int deadlockRetries,
      int deadlockPause,
      TeamFoundationDatabaseLoggingOptions loggingOptions,
      TimeSpan executionTimeThreshold,
      bool breakerDisabled,
      byte breakerErrorThresholdPerc,
      bool breakerForceClosed,
      bool breakerForceOpen,
      int breakerMaxBackoff,
      int breakerRequestVolumeThreshold,
      int breakerExecutionTimeout,
      int breakerMaxExecConcurrentRequests,
      int breakerMaxFallbackConcurrentRequests)
    {
      this.Initialize(databaseId, connectionInfo, connectionInfo, databaseName, serviceLevel, poolName, tenants, maxTenants, tenantsPendingDelete, databaseSize, databaseCapacity, sizeChangedDate, status, statusChangedDate, statusReason, lastTenantAdded, acquisitionOrder, flags, version, requestTimeout, deadlockRetries, deadlockPause, loggingOptions, executionTimeThreshold, breakerDisabled, breakerErrorThresholdPerc, breakerForceClosed, breakerForceOpen, breakerMaxBackoff, breakerRequestVolumeThreshold, breakerExecutionTimeout, breakerMaxExecConcurrentRequests, breakerMaxFallbackConcurrentRequests);
    }

    internal void Initialize(
      int databaseId,
      SqlConnectionInfoWrapper defaultConnectionInfoWrapper,
      SqlConnectionInfoWrapper dboConnectionInfoWrapper,
      string databaseName,
      string serviceLevel,
      string poolName,
      int tenants,
      int maxTenants,
      int tenantsPendingDelete,
      int databaseSize,
      int databaseCapacity,
      DateTime sizeChangedDate,
      TeamFoundationDatabaseStatus status,
      DateTime statusChangedDate,
      string statusReason,
      DateTime lastTenantAdded,
      DateTime acquisitionOrder,
      TeamFoundationDatabaseFlags flags,
      long version,
      int requestTimeout,
      int deadlockRetries,
      int deadlockPause,
      TeamFoundationDatabaseLoggingOptions loggingOptions,
      TimeSpan executionTimeThreshold,
      bool breakerDisabled,
      byte breakerErrorThresholdPerc,
      bool breakerForceClosed,
      bool breakerForceOpen,
      int breakerMaxBackoff,
      int breakerRequestVolumeThreshold,
      int breakerExecutionTimeout,
      int breakerMaxExecConcurrentRequests,
      int breakerMaxFallbackConcurrentRequests)
    {
      this.DatabaseId = databaseId;
      this.DatabaseName = databaseName;
      this.ServiceLevel = serviceLevel;
      this.PoolName = poolName;
      this.Tenants = tenants;
      this.MaxTenants = maxTenants;
      this.TenantsPendingDelete = tenantsPendingDelete;
      this.DatabaseSize = databaseSize;
      this.DatabaseCapacity = databaseCapacity;
      this.SizeChangedDate = sizeChangedDate;
      this.Status = status;
      this.StatusChangedDate = statusChangedDate;
      this.StatusReason = statusReason;
      this.LastTenantAdded = lastTenantAdded;
      this.AcquisitionOrder = acquisitionOrder;
      this.Flags = flags;
      this.ConnectionInfoWrapper = defaultConnectionInfoWrapper;
      this.DboConnectionInfoWrapper = dboConnectionInfoWrapper;
      this.ReadOnlyConnectionInfoWrapper = new SqlConnectionInfoWrapper()
      {
        ConnectionString = TeamFoundationDatabaseProperties.GetReadOnlyConnectionString(defaultConnectionInfoWrapper.ConnectionString),
        UserId = defaultConnectionInfoWrapper.UserId,
        PasswordEncrypted = defaultConnectionInfoWrapper.PasswordEncrypted,
        SigningKeyId = defaultConnectionInfoWrapper.SigningKeyId
      };
      this.Version = version;
      this.RequestTimeout = requestTimeout;
      this.DeadlockRetries = deadlockRetries;
      this.DeadlockPause = deadlockPause;
      this.LoggingOptions = loggingOptions;
      this.ExecutionTimeThreshold = executionTimeThreshold;
      this.BreakerDisabled = breakerDisabled;
      this.BreakerErrorThresholdPerc = breakerErrorThresholdPerc;
      this.BreakerForceClosed = breakerForceClosed;
      this.BreakerForceOpen = breakerForceOpen;
      this.BreakerMaxBackoff = breakerMaxBackoff;
      this.BreakerRequestVolumeThreshold = breakerRequestVolumeThreshold;
      this.BreakerExecutionTimeout = breakerExecutionTimeout;
      this.BreakerMaxExecConcurrentRequests = breakerMaxExecConcurrentRequests;
      this.BreakerMaxFallbackConcurrentRequests = breakerMaxFallbackConcurrentRequests;
    }

    internal void Initialize(
      int databaseId,
      SqlConnectionInfoWrapper defaultConnectionInfoWrapper,
      SqlConnectionInfoWrapper dboConnectionInfoWrapper,
      string databaseName,
      string serviceLevel,
      string poolName,
      int tenants,
      int maxTenants,
      int tenantsPendingDelete,
      int databaseSize,
      int databaseCapacity,
      DateTime sizeChangedDate,
      TeamFoundationDatabaseStatus status,
      DateTime statusChangedDate,
      string statusReason,
      DateTime lastTenantAdded,
      DateTime acquisitionOrder,
      TeamFoundationDatabaseFlags flags,
      long version,
      int requestTimeout,
      int deadlockRetries,
      int deadlockPause,
      TeamFoundationDatabaseLoggingOptions loggingOptions,
      TimeSpan executionTimeThreshold,
      bool breakerDisabled,
      byte breakerErrorThresholdPerc,
      bool breakerForceClosed,
      bool breakerForceOpen,
      int breakerMaxBackoff,
      int breakerRequestVolumeThreshold,
      int breakerExecutionTimeout,
      int breakerMaxExecConcurrentRequests,
      int breakerMaxFallbackConcurrentRequests,
      string minServiceObjective,
      string maxServiceObjective)
    {
      this.DatabaseId = databaseId;
      this.DatabaseName = databaseName;
      this.ServiceLevel = serviceLevel;
      this.PoolName = poolName;
      this.Tenants = tenants;
      this.MaxTenants = maxTenants;
      this.TenantsPendingDelete = tenantsPendingDelete;
      this.DatabaseSize = databaseSize;
      this.DatabaseCapacity = databaseCapacity;
      this.SizeChangedDate = sizeChangedDate;
      this.Status = status;
      this.StatusChangedDate = statusChangedDate;
      this.StatusReason = statusReason;
      this.LastTenantAdded = lastTenantAdded;
      this.AcquisitionOrder = acquisitionOrder;
      this.Flags = flags;
      this.ConnectionInfoWrapper = defaultConnectionInfoWrapper;
      this.DboConnectionInfoWrapper = dboConnectionInfoWrapper;
      this.ReadOnlyConnectionInfoWrapper = new SqlConnectionInfoWrapper()
      {
        ConnectionString = TeamFoundationDatabaseProperties.GetReadOnlyConnectionString(defaultConnectionInfoWrapper.ConnectionString),
        UserId = defaultConnectionInfoWrapper.UserId,
        PasswordEncrypted = defaultConnectionInfoWrapper.PasswordEncrypted,
        SigningKeyId = defaultConnectionInfoWrapper.SigningKeyId
      };
      this.Version = version;
      this.RequestTimeout = requestTimeout;
      this.DeadlockRetries = deadlockRetries;
      this.DeadlockPause = deadlockPause;
      this.LoggingOptions = loggingOptions;
      this.ExecutionTimeThreshold = executionTimeThreshold;
      this.BreakerDisabled = breakerDisabled;
      this.BreakerErrorThresholdPerc = breakerErrorThresholdPerc;
      this.BreakerForceClosed = breakerForceClosed;
      this.BreakerForceOpen = breakerForceOpen;
      this.BreakerMaxBackoff = breakerMaxBackoff;
      this.BreakerRequestVolumeThreshold = breakerRequestVolumeThreshold;
      this.BreakerExecutionTimeout = breakerExecutionTimeout;
      this.BreakerMaxExecConcurrentRequests = breakerMaxExecConcurrentRequests;
      this.BreakerMaxFallbackConcurrentRequests = breakerMaxFallbackConcurrentRequests;
      this.MinServiceObjective = minServiceObjective;
      this.MaxServiceObjective = maxServiceObjective;
    }

    internal void Update(
      IVssRequestContext requestContext,
      SerializableDatabaseProperties updatedProperties)
    {
      this.DatabaseId = updatedProperties.DatabaseId;
      if (updatedProperties.DatabaseName != null)
        this.DatabaseName = updatedProperties.DatabaseName;
      if (updatedProperties.ServiceLevel != null)
        this.ServiceLevel = updatedProperties.ServiceLevel;
      if (updatedProperties.PoolName != null)
        this.PoolName = updatedProperties.PoolName;
      int? nullable1 = updatedProperties.Tenants;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.Tenants;
        this.Tenants = nullable1.Value;
      }
      nullable1 = updatedProperties.MaxTenants;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.MaxTenants;
        this.MaxTenants = nullable1.Value;
      }
      nullable1 = updatedProperties.TenantsPendingDelete;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.TenantsPendingDelete;
        this.TenantsPendingDelete = nullable1.Value;
      }
      nullable1 = updatedProperties.DatabaseSize;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.DatabaseSize;
        this.DatabaseSize = nullable1.Value;
      }
      nullable1 = updatedProperties.DatabaseCapacity;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.DatabaseCapacity;
        this.DatabaseCapacity = nullable1.Value;
      }
      DateTime? nullable2 = updatedProperties.SizeChangedDate;
      if (nullable2.HasValue)
      {
        nullable2 = updatedProperties.SizeChangedDate;
        this.SizeChangedDate = nullable2.Value;
      }
      nullable1 = updatedProperties.Status;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.Status;
        this.Status = (TeamFoundationDatabaseStatus) nullable1.Value;
      }
      nullable2 = updatedProperties.StatusChangedDate;
      if (nullable2.HasValue)
      {
        nullable2 = updatedProperties.StatusChangedDate;
        this.StatusChangedDate = nullable2.Value;
      }
      if (updatedProperties.StatusReason != null)
        this.StatusReason = updatedProperties.StatusReason;
      nullable2 = updatedProperties.LastTenantAdded;
      if (nullable2.HasValue)
      {
        nullable2 = updatedProperties.LastTenantAdded;
        this.LastTenantAdded = nullable2.Value;
      }
      nullable2 = updatedProperties.AcquisitionOrder;
      if (nullable2.HasValue)
      {
        nullable2 = updatedProperties.AcquisitionOrder;
        this.AcquisitionOrder = nullable2.Value;
      }
      if (updatedProperties.ConnectionInfoWrapper != null)
      {
        this.ConnectionInfoWrapper = updatedProperties.ConnectionInfoWrapper;
        if (updatedProperties.ConnectionInfoWrapper.IsValidSecurityConfiguration)
          this.SqlConnectionInfo = updatedProperties.ConnectionInfoWrapper.ToSqlConnectionInfo(requestContext);
        this.ReadOnlyConnectionInfoWrapper = new SqlConnectionInfoWrapper()
        {
          ConnectionString = TeamFoundationDatabaseProperties.GetReadOnlyConnectionString(this.ConnectionInfoWrapper.ConnectionString),
          UserId = this.ConnectionInfoWrapper.UserId,
          PasswordEncrypted = this.ConnectionInfoWrapper.PasswordEncrypted,
          SigningKeyId = this.ConnectionInfoWrapper.SigningKeyId
        };
        if (this.ReadOnlyConnectionInfoWrapper.IsValidSecurityConfiguration)
          this.ReadOnlyConnectionInfo = this.ReadOnlyConnectionInfoWrapper.ToSqlConnectionInfo(requestContext);
      }
      if (updatedProperties.DboConnectionInfoWrapper != null)
      {
        this.DboConnectionInfoWrapper = updatedProperties.DboConnectionInfoWrapper;
        if (updatedProperties.DboConnectionInfoWrapper.IsValidSecurityConfiguration)
          this.DboConnectionInfo = updatedProperties.DboConnectionInfoWrapper.ToSqlConnectionInfo(requestContext);
      }
      if (updatedProperties.MinServiceObjective != null)
        this.MinServiceObjective = updatedProperties.MinServiceObjective;
      if (updatedProperties.MaxServiceObjective != null)
        this.MaxServiceObjective = updatedProperties.MaxServiceObjective;
      nullable1 = updatedProperties.Flags;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.Flags;
        this.Flags = (TeamFoundationDatabaseFlags) nullable1.Value;
      }
      long? version = updatedProperties.Version;
      if (version.HasValue)
      {
        version = updatedProperties.Version;
        this.Version = version.Value;
      }
      nullable1 = updatedProperties.RequestTimeout;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.RequestTimeout;
        this.RequestTimeout = nullable1.Value;
      }
      nullable1 = updatedProperties.DeadlockRetries;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.DeadlockRetries;
        this.DeadlockRetries = nullable1.Value;
      }
      nullable1 = updatedProperties.DeadlockPause;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.DeadlockPause;
        this.DeadlockPause = nullable1.Value;
      }
      byte? nullable3 = updatedProperties.LoggingOptions;
      if (nullable3.HasValue)
      {
        nullable3 = updatedProperties.LoggingOptions;
        this.LoggingOptions = (TeamFoundationDatabaseLoggingOptions) nullable3.Value;
      }
      nullable1 = updatedProperties.ExecutionTimeThreshold;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.ExecutionTimeThreshold;
        this.ExecutionTimeThreshold = new TimeSpan(0, 0, nullable1.Value);
      }
      bool? nullable4 = updatedProperties.BreakerDisabled;
      if (nullable4.HasValue)
      {
        nullable4 = updatedProperties.BreakerDisabled;
        this.BreakerDisabled = nullable4.Value;
      }
      nullable3 = updatedProperties.BreakerErrorThresholdPerc;
      if (nullable3.HasValue)
      {
        nullable3 = updatedProperties.BreakerErrorThresholdPerc;
        this.BreakerErrorThresholdPerc = nullable3.Value;
      }
      nullable4 = updatedProperties.BreakerForceClosed;
      if (nullable4.HasValue)
      {
        nullable4 = updatedProperties.BreakerForceClosed;
        this.BreakerForceClosed = nullable4.Value;
      }
      nullable4 = updatedProperties.BreakerForceOpen;
      if (nullable4.HasValue)
      {
        nullable4 = updatedProperties.BreakerForceOpen;
        this.BreakerForceOpen = nullable4.Value;
      }
      nullable1 = updatedProperties.BreakerMaxBackoff;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.BreakerMaxBackoff;
        this.BreakerMaxBackoff = nullable1.Value;
      }
      nullable1 = updatedProperties.BreakerRequestVolumeThreshold;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.BreakerRequestVolumeThreshold;
        this.BreakerRequestVolumeThreshold = nullable1.Value;
      }
      nullable1 = updatedProperties.BreakerExecutionTimeout;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.BreakerExecutionTimeout;
        this.BreakerExecutionTimeout = nullable1.Value;
      }
      nullable1 = updatedProperties.BreakerMaxExecConcurrentRequests;
      if (nullable1.HasValue)
      {
        nullable1 = updatedProperties.BreakerMaxExecConcurrentRequests;
        this.BreakerMaxExecConcurrentRequests = nullable1.Value;
      }
      nullable1 = updatedProperties.BreakerMaxFallbackConcurrentRequests;
      if (!nullable1.HasValue)
        return;
      nullable1 = updatedProperties.BreakerMaxFallbackConcurrentRequests;
      this.BreakerMaxFallbackConcurrentRequests = nullable1.Value;
    }

    internal void Update(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties updatedProperties)
    {
      this.AcquisitionOrder = updatedProperties.AcquisitionOrder;
      this.ConnectionInfoWrapper = updatedProperties.ConnectionInfoWrapper;
      this.DboConnectionInfoWrapper = updatedProperties.DboConnectionInfoWrapper;
      this.ReadOnlyConnectionInfoWrapper = updatedProperties.ReadOnlyConnectionInfoWrapper;
      this.DatabaseId = updatedProperties.DatabaseId;
      this.DatabaseName = updatedProperties.DatabaseName;
      this.Flags = updatedProperties.Flags;
      this.ServiceLevel = updatedProperties.ServiceLevel;
      this.PoolName = updatedProperties.PoolName;
      this.Tenants = updatedProperties.Tenants;
      this.MaxTenants = updatedProperties.MaxTenants;
      this.TenantsPendingDelete = updatedProperties.TenantsPendingDelete;
      this.DatabaseSize = updatedProperties.DatabaseSize;
      this.DatabaseCapacity = updatedProperties.DatabaseCapacity;
      this.SizeChangedDate = updatedProperties.SizeChangedDate;
      this.Status = updatedProperties.Status;
      this.StatusChangedDate = updatedProperties.StatusChangedDate;
      this.StatusReason = updatedProperties.StatusReason;
      this.LastTenantAdded = updatedProperties.LastTenantAdded;
      this.MinServiceObjective = updatedProperties.MinServiceObjective;
      this.MaxServiceObjective = updatedProperties.MaxServiceObjective;
      this.Version = updatedProperties.Version;
      this.RequestTimeout = updatedProperties.RequestTimeout;
      this.DeadlockRetries = updatedProperties.DeadlockRetries;
      this.DeadlockPause = updatedProperties.DeadlockPause;
      this.LoggingOptions = updatedProperties.LoggingOptions;
      this.ExecutionTimeThreshold = updatedProperties.ExecutionTimeThreshold;
      this.BreakerDisabled = updatedProperties.BreakerDisabled;
      this.BreakerErrorThresholdPerc = updatedProperties.BreakerErrorThresholdPerc;
      this.BreakerForceClosed = updatedProperties.BreakerForceClosed;
      this.BreakerForceOpen = updatedProperties.BreakerForceOpen;
      this.BreakerMaxBackoff = updatedProperties.BreakerMaxBackoff;
      this.BreakerRequestVolumeThreshold = updatedProperties.BreakerRequestVolumeThreshold;
      this.BreakerExecutionTimeout = updatedProperties.BreakerExecutionTimeout;
      this.BreakerMaxExecConcurrentRequests = updatedProperties.BreakerMaxExecConcurrentRequests;
      this.BreakerMaxFallbackConcurrentRequests = updatedProperties.BreakerMaxFallbackConcurrentRequests;
      try
      {
        if (this.ConnectionInfoWrapper != null)
        {
          if (this.ConnectionInfoWrapper.IsValidSecurityConfiguration)
            this.SqlConnectionInfo = this.ConnectionInfoWrapper.ToSqlConnectionInfo(requestContext);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(98011, TraceLevel.Error, InternalDatabaseProperties.s_area, InternalDatabaseProperties.s_layer, "Exception getting SqlConnectionInfo for default connection string for database {0}. Exception details: {1}", (object) this.DatabaseId, (object) ex);
        this.SqlConnectionInfo = (ISqlConnectionInfo) null;
      }
      try
      {
        if (this.DboConnectionInfoWrapper != null)
        {
          if (this.DboConnectionInfoWrapper.IsValidSecurityConfiguration)
            this.DboConnectionInfo = this.DboConnectionInfoWrapper.ToSqlConnectionInfo(requestContext);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(98011, TraceLevel.Error, InternalDatabaseProperties.s_area, InternalDatabaseProperties.s_layer, "Exception getting SqlConnectionInfo for DBO connection string for database {0}. Exception details: {1}", (object) this.DatabaseId, (object) ex);
        this.DboConnectionInfo = (ISqlConnectionInfo) null;
      }
      try
      {
        if (this.ReadOnlyConnectionInfoWrapper == null || !this.ReadOnlyConnectionInfoWrapper.IsValidSecurityConfiguration)
          return;
        this.ReadOnlyConnectionInfo = this.ReadOnlyConnectionInfoWrapper.ToSqlConnectionInfo(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(98011, TraceLevel.Error, InternalDatabaseProperties.s_area, InternalDatabaseProperties.s_layer, "Exception getting SqlConnectionInfo for readOnly connection string for database {0}. Exception details: {1}", (object) this.DatabaseId, (object) ex);
        this.ReadOnlyConnectionInfo = (ISqlConnectionInfo) null;
      }
    }

    public DateTime AcquisitionOrder { get; private set; }

    public SqlConnectionInfoWrapper ConnectionInfoWrapper { get; private set; }

    public ISqlConnectionInfo SqlConnectionInfo { get; internal set; }

    public SqlConnectionInfoWrapper DboConnectionInfoWrapper { get; private set; }

    public ISqlConnectionInfo DboConnectionInfo { get; private set; }

    public SqlConnectionInfoWrapper ReadOnlyConnectionInfoWrapper { get; private set; }

    public ISqlConnectionInfo ReadOnlyConnectionInfo { get; private set; }

    public int DatabaseCapacity { get; private set; }

    public int DatabaseId { get; private set; }

    public string DatabaseName { get; private set; }

    public int DatabaseSize { get; private set; }

    public TeamFoundationDatabaseFlags Flags { get; private set; }

    public DateTime LastTenantAdded { get; private set; }

    public int MaxTenants { get; private set; }

    public string PoolName { get; private set; }

    public string ServiceLevel { get; private set; }

    public DateTime SizeChangedDate { get; private set; }

    public TeamFoundationDatabaseStatus Status { get; private set; }

    public DateTime StatusChangedDate { get; private set; }

    public string StatusReason { get; private set; }

    public int Tenants { get; private set; }

    public int TenantsPendingDelete { get; private set; }

    public string MinServiceObjective { get; private set; }

    public string MaxServiceObjective { get; private set; }

    public long Version { get; private set; }

    public int RequestTimeout { get; private set; }

    public int DeadlockRetries { get; private set; }

    public int DeadlockPause { get; private set; }

    public TeamFoundationDatabaseLoggingOptions LoggingOptions { get; private set; }

    public TimeSpan ExecutionTimeThreshold { get; private set; }

    public bool BreakerDisabled { get; private set; }

    public byte BreakerErrorThresholdPerc { get; private set; }

    public bool BreakerForceClosed { get; private set; }

    public bool BreakerForceOpen { get; private set; }

    public int BreakerMaxBackoff { get; private set; }

    public int BreakerRequestVolumeThreshold { get; private set; }

    public int BreakerExecutionTimeout { get; private set; }

    public int BreakerMaxExecConcurrentRequests { get; private set; }

    public int BreakerMaxFallbackConcurrentRequests { get; private set; }

    public Guid MaintenanceJobId => TeamFoundationDatabaseProperties.GetMaintenanceJobId(this.DatabaseId);

    internal void UpdateSqlConnectionInfo(IVssRequestContext requestContext)
    {
      if (this.ConnectionInfoWrapper != null)
        this.SqlConnectionInfo = this.UpdateSqlConnectionInfo(this.ConnectionInfoWrapper, requestContext);
      if (this.DboConnectionInfoWrapper != null)
        this.DboConnectionInfo = this.UpdateSqlConnectionInfo(this.DboConnectionInfoWrapper, requestContext);
      if (this.ReadOnlyConnectionInfoWrapper == null)
        return;
      this.ReadOnlyConnectionInfo = this.UpdateSqlConnectionInfo(this.ReadOnlyConnectionInfoWrapper, requestContext);
    }

    private ISqlConnectionInfo UpdateSqlConnectionInfo(
      SqlConnectionInfoWrapper connectionInfoWrapper,
      IVssRequestContext requestContext)
    {
      try
      {
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionInfoWrapper.ConnectionString);
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(FrameworkResources.BadConnectionStringFormat((object) this.DatabaseId), ex);
        return (ISqlConnectionInfo) null;
      }
      return connectionInfoWrapper.IsValidSecurityConfiguration ? connectionInfoWrapper.ToSqlConnectionInfo(requestContext) : (ISqlConnectionInfo) null;
    }

    internal void UpdateSqlConnectionInfoRaw(ISqlConnectionInfo configConnectionInfo)
    {
      if (this.ConnectionInfoWrapper != null && this.ConnectionInfoWrapper.IsValidSecurityConfiguration)
        this.SqlConnectionInfo = this.ConnectionInfoWrapper.ToSqlConnectionInfoRaw(configConnectionInfo);
      if (this.DboConnectionInfoWrapper != null && this.DboConnectionInfoWrapper.IsValidSecurityConfiguration)
        this.DboConnectionInfo = this.DboConnectionInfoWrapper.ToSqlConnectionInfoRaw(configConnectionInfo);
      if (this.ReadOnlyConnectionInfoWrapper == null || !this.ReadOnlyConnectionInfoWrapper.IsValidSecurityConfiguration)
        return;
      this.ReadOnlyConnectionInfo = this.ReadOnlyConnectionInfoWrapper.ToSqlConnectionInfoRaw(configConnectionInfo);
    }

    public TeamFoundationDatabaseProperties GetEditableProperties() => new TeamFoundationDatabaseProperties(this);

    public ITeamFoundationDatabaseProperties GetCachedProperties() => (ITeamFoundationDatabaseProperties) this;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, string.Format("DatabaseProperties\r\n[\r\n    DatabaseId:                             {0}\r\n    Version:                                {1}\r\n    DatabaseName:                           {2}\r\n    ServiceLevel:                           {3}\r\n    PoolName:                               {4}\r\n    Tenants:                                {5}\r\n    MaxTenants:                             {6}\r\n    TenantsPendingDelete:                   {7}\r\n    Status:                                 {8}\r\n    StatusChangedDate:                      {9}\r\n    StatusReason:                           {10}\r\n    LastTenantAdded:                        {11}\r\n    AcquisitionOrder:                       {12}\r\n    Flags:                                  {13}\r\n    DatabaseSize:                           {14}\r\n    DatabaseCapacity:                       {15}\r\n    SizeChangedDate:                        {16}\r\n    MinServiceObjective:                    {17}\r\n    MaxServiceObjective:                    {18}\r\n    RequestTimeout:                         {19}\r\n    DeadlockRetries:                        {20}\r\n    DeadlockPause:                          {21}\r\n    LoggingOptions:                         {22}\r\n    ExecutionTimeThreshold:                 {23}\r\n    BreakerDisabled:                        {24}\r\n    BreakerErrorThresholdPerc:              {25}\r\n    BreakerForceClosed:                     {26}\r\n    BreakerForceOpen:                       {27}\r\n    BreakerMaxBackoff:                      {28}\r\n    BreakerRequestVolumeThreshold:          {29}\r\n    BreakerExecutionTimeout:                {30}\r\n    BreakerMaxExecConcurrentRequests:       {31}\r\n    BreakerMaxFallbackConcurrentRequests:   {32}\r\n]", (object) this.DatabaseId, (object) this.Version, (object) this.DatabaseName, (object) this.ServiceLevel, (object) this.PoolName, (object) this.Tenants, (object) this.MaxTenants, (object) this.TenantsPendingDelete, (object) this.Status, (object) this.StatusChangedDate, (object) this.StatusReason, (object) this.LastTenantAdded, (object) this.AcquisitionOrder, (object) this.Flags, (object) this.DatabaseSize, (object) this.DatabaseCapacity, (object) this.SizeChangedDate, (object) this.MinServiceObjective, (object) this.MaxServiceObjective, (object) this.RequestTimeout, (object) this.DeadlockRetries, (object) this.DeadlockPause, (object) this.LoggingOptions, (object) this.ExecutionTimeThreshold, (object) this.BreakerDisabled, (object) this.BreakerErrorThresholdPerc, (object) this.BreakerForceClosed, (object) this.BreakerForceOpen, (object) this.BreakerMaxBackoff, (object) this.BreakerRequestVolumeThreshold, (object) this.BreakerExecutionTimeout, (object) this.BreakerMaxExecConcurrentRequests, (object) this.BreakerMaxFallbackConcurrentRequests));

    public DatabaseResourceStats GetDatabaseResourceStats(IVssRequestContext requestContext) => this.m_databaseResourceStatsCache.Get(requestContext);

    public ISqlConnectionInfo GetDboConnectionInfo() => this.DboConnectionInfo ?? this.SqlConnectionInfo;

    private DatabaseResourceStats FetchDatabaseResourceStats(IVssRequestContext requestContext)
    {
      if (this.SqlConnectionInfo == null)
        return (DatabaseResourceStats) null;
      DatabaseResourceStats databaseResourceStats;
      try
      {
        using (DiagnosticComponent componentRaw = this.GetDboConnectionInfo().CreateComponentRaw<DiagnosticComponent>())
        {
          databaseResourceStats = componentRaw.QueryDbmsResourceStats(4);
          if (databaseResourceStats != null)
            databaseResourceStats.DatabaseId = this.DatabaseId;
        }
      }
      catch (ServiceNotRegisteredException ex) when (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        return (DatabaseResourceStats) null;
      }
      catch (DatabaseConfigurationException ex)
      {
        requestContext.TraceException(98010, InternalDatabaseProperties.s_area, InternalDatabaseProperties.s_layer, (Exception) ex);
        return (DatabaseResourceStats) null;
      }
      return databaseResourceStats;
    }

    public string GetActualServerName(IVssRequestContext requestContext)
    {
      string actualServerName = TeamFoundationDataTierService.ManipulateDataSource(this.SqlConnectionInfo.DataSource, DataSourceOptions.RemoveProtocol);
      DatabaseReplicationContext replicationContext = this.GetDatabaseReplicationContext(requestContext);
      if (replicationContext != DatabaseReplicationContext.Default)
        actualServerName = replicationContext.PrimaryServerName + AzureDomainConstants.DatabaseWindowsNet;
      return actualServerName;
    }

    public string GetFailoverGroupListenerName(IVssRequestContext requestContext) => this.GetDatabaseReplicationContext(requestContext) != DatabaseReplicationContext.Default ? TeamFoundationDataTierService.ManipulateDataSource(this.SqlConnectionInfo.DataSource, DataSourceOptions.RemoveProtocol) : string.Empty;

    private DatabaseReplicationContext GetDatabaseReplicationContext(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled(FrameworkServerConstants.SqlFailoverGroupEnabled) ? requestContext.GetService<IDatabaseFailoverGroupService>().GetDatabaseReplicationContext(requestContext, (ITeamFoundationDatabaseProperties) this) : DatabaseReplicationContext.Default;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationDatabaseProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TeamFoundationDatabaseProperties : ITeamFoundationDatabaseProperties
  {
    private int m_databaseId;
    private SqlConnectionInfoWrapper m_connectionInfo;
    private SqlConnectionInfoWrapper m_dboConnectionInfo;
    private SqlConnectionInfoWrapper m_readOnlyConnectionInfo;
    private string m_serviceLevel;
    private string m_poolName;
    private int m_tenants;
    private int m_maxTenants;
    private TeamFoundationDatabaseStatus m_status;
    private string m_statusReason;
    private DateTime m_acquisitionOrder;
    private TeamFoundationDatabaseFlags m_flags;
    private int m_tenantsPendingDelete;
    private int m_databaseSize;
    private int m_databaseCapacity;
    private long m_version;
    private int m_requestTimeout;
    private int m_deadlockRetries;
    private int m_deadlockPause;
    private TeamFoundationDatabaseLoggingOptions m_loggingOptions;
    private TimeSpan m_executionTimeThreshold;
    private string m_minServiceObjective;
    private string m_maxServiceObjective;
    private bool m_breakerDisabled;
    private byte m_breakerErrorThresholdPerc;
    private bool m_breakerForceClosed;
    private bool m_breakerForceOpen;
    private int m_breakerMaxBackoff;
    private int m_breakerRequestVolumeThreshold;
    private int m_breakerExecutionTimeout;
    private int m_breakerMaxExecConcurrentRequests;
    private int m_breakerMaxFallbackConcurrentRequests;
    private InternalDatabaseProperties m_master;

    public TeamFoundationDatabaseProperties(InternalDatabaseProperties master)
    {
      ArgumentUtility.CheckForNull<InternalDatabaseProperties>(master, nameof (master));
      this.m_master = master;
    }

    public TeamFoundationDatabaseProperties(int databaseId) => this.m_databaseId = databaseId;

    public int DatabaseId => this.m_master.DatabaseId;

    public SqlConnectionInfoWrapper ConnectionInfoWrapper => !this.IsConnectionInfoDirty ? this.m_master.ConnectionInfoWrapper : this.m_connectionInfo;

    public ISqlConnectionInfo SqlConnectionInfo => this.m_master.SqlConnectionInfo;

    public SqlConnectionInfoWrapper DboConnectionInfoWrapper => !this.IsConnectionInfoDirty ? this.m_master.DboConnectionInfoWrapper : this.m_dboConnectionInfo;

    public ISqlConnectionInfo DboConnectionInfo => this.m_master.DboConnectionInfo;

    public SqlConnectionInfoWrapper ReadOnlyConnectionInfoWrapper => !this.IsConnectionInfoDirty ? this.m_master.ReadOnlyConnectionInfoWrapper : this.m_readOnlyConnectionInfo;

    public ISqlConnectionInfo ReadOnlyConnectionInfo => this.m_master.ReadOnlyConnectionInfo;

    public void UpdateConnectionString(string connectionString)
    {
      this.m_connectionInfo = new SqlConnectionInfoWrapper()
      {
        PasswordEncrypted = this.m_master.ConnectionInfoWrapper.PasswordEncrypted,
        UserId = this.m_master.ConnectionInfoWrapper.UserId,
        ConnectionString = connectionString
      };
      if (this.m_master.DboConnectionInfoWrapper != null)
        this.m_dboConnectionInfo = new SqlConnectionInfoWrapper()
        {
          PasswordEncrypted = this.m_master.DboConnectionInfoWrapper.PasswordEncrypted,
          UserId = this.m_master.DboConnectionInfoWrapper.UserId,
          ConnectionString = connectionString
        };
      if (this.m_master.ReadOnlyConnectionInfoWrapper != null)
        this.m_readOnlyConnectionInfo = new SqlConnectionInfoWrapper()
        {
          PasswordEncrypted = this.m_master.ReadOnlyConnectionInfoWrapper.PasswordEncrypted,
          UserId = this.m_master.ReadOnlyConnectionInfoWrapper.UserId,
          ConnectionString = TeamFoundationDatabaseProperties.GetReadOnlyConnectionString(connectionString)
        };
      this.LockVersion();
      this.IsConnectionInfoDirty = true;
    }

    internal bool IsConnectionInfoDirty { get; private set; }

    public string DatabaseName => this.m_master.DatabaseName;

    public string ServiceLevel
    {
      get => this.IsServiceLevelDirty ? this.m_serviceLevel : this.m_master.ServiceLevel;
      set
      {
        if (!this.IsServiceLevelDirty && string.Equals(value, this.m_master.ServiceLevel, StringComparison.OrdinalIgnoreCase))
          return;
        this.LockVersion();
        this.m_serviceLevel = value;
        this.IsServiceLevelDirty = true;
      }
    }

    internal bool IsServiceLevelDirty { get; private set; }

    public string PoolName
    {
      get => this.IsPoolNameDirty ? this.m_poolName : this.m_master.PoolName;
      set
      {
        if (!this.IsPoolNameDirty && string.Equals(value, this.m_master.PoolName, StringComparison.OrdinalIgnoreCase))
          return;
        this.LockVersion();
        this.m_poolName = value;
        this.IsPoolNameDirty = true;
      }
    }

    internal bool IsPoolNameDirty { get; private set; }

    public int Tenants
    {
      get => this.IsTenantsDirty ? this.m_tenants : this.m_master.Tenants;
      set
      {
        if (!this.IsTenantsDirty && value == this.m_master.Tenants)
          return;
        this.LockVersion();
        this.m_tenants = value;
        this.IsTenantsDirty = true;
      }
    }

    internal bool IsTenantsDirty { get; private set; }

    public int MaxTenants
    {
      get => this.IsMaxTenantsDirty ? this.m_maxTenants : this.m_master.MaxTenants;
      set
      {
        if (!this.IsMaxTenantsDirty && value == this.m_master.MaxTenants)
          return;
        this.LockVersion();
        this.m_maxTenants = value;
        this.IsMaxTenantsDirty = true;
      }
    }

    internal bool IsMaxTenantsDirty { get; private set; }

    public int TenantsPendingDelete
    {
      get => this.IsTenantsPendingDeleteDirty ? this.m_tenantsPendingDelete : this.m_master.TenantsPendingDelete;
      internal set
      {
        if (!this.IsTenantsPendingDeleteDirty && value == this.m_master.TenantsPendingDelete)
          return;
        this.LockVersion();
        this.m_tenantsPendingDelete = value;
        this.IsTenantsPendingDeleteDirty = true;
      }
    }

    internal bool IsTenantsPendingDeleteDirty { get; set; }

    public int DatabaseSize
    {
      get => this.IsDatabaseSizeDirty ? this.m_databaseSize : this.m_master.DatabaseSize;
      internal set
      {
        if (!this.IsDatabaseSizeDirty && value == this.m_master.DatabaseSize)
          return;
        this.LockVersion();
        this.m_databaseSize = value;
        this.IsDatabaseSizeDirty = true;
      }
    }

    internal bool IsDatabaseSizeDirty { get; set; }

    public int DatabaseCapacity
    {
      get => this.IsDatabaseCapacityDirty ? this.m_databaseCapacity : this.m_master.DatabaseCapacity;
      set
      {
        if (!this.IsDatabaseCapacityDirty && value == this.m_master.DatabaseCapacity)
          return;
        this.LockVersion();
        this.m_databaseCapacity = value;
        this.IsDatabaseCapacityDirty = true;
      }
    }

    internal bool IsDatabaseCapacityDirty { get; set; }

    public DateTime SizeChangedDate => this.m_master.SizeChangedDate;

    public TeamFoundationDatabaseStatus Status
    {
      get => this.IsStatusDirty ? this.m_status : this.m_master.Status;
      set
      {
        if (!this.IsStatusDirty && value == this.m_master.Status)
          return;
        this.LockVersion();
        this.m_status = value;
        this.IsStatusDirty = true;
      }
    }

    public bool IsStatusDirty { get; set; }

    public DateTime StatusChangedDate => this.m_master.StatusChangedDate;

    public string StatusReason
    {
      get => this.IsStatusReasonDirty ? this.m_statusReason : this.m_master.StatusReason;
      set
      {
        if (!this.IsStatusReasonDirty && string.Equals(value, this.m_master.StatusReason))
          return;
        this.LockVersion();
        this.m_statusReason = value;
        this.IsStatusReasonDirty = true;
      }
    }

    public bool IsStatusReasonDirty { get; set; }

    public DateTime LastTenantAdded => this.m_master.LastTenantAdded;

    public TeamFoundationDatabaseFlags Flags
    {
      get => this.IsFlagsDirty ? this.m_flags : this.m_master.Flags;
      set
      {
        if (!this.IsFlagsDirty && value == this.m_master.Flags)
          return;
        this.LockVersion();
        this.IsFlagsDirty = true;
        this.m_flags = value;
      }
    }

    internal bool IsFlagsDirty { get; private set; }

    public DateTime AcquisitionOrder
    {
      get => this.IsAcquisitionOrderDirty ? this.m_acquisitionOrder : this.m_master.AcquisitionOrder;
      set
      {
        if (!this.IsAcquisitionOrderDirty && !(value != this.m_master.AcquisitionOrder))
          return;
        this.LockVersion();
        this.m_acquisitionOrder = value;
        this.IsAcquisitionOrderDirty = true;
      }
    }

    internal bool IsAcquisitionOrderDirty { get; private set; }

    public long Version => this.IsUpdateRequired() ? this.m_version : this.m_master.Version;

    public int RequestTimeout
    {
      get => this.IsRequestTimeoutDirty ? this.m_requestTimeout : this.m_master.RequestTimeout;
      set
      {
        if (!this.IsRequestTimeoutDirty && value == this.m_master.RequestTimeout)
          return;
        this.LockVersion();
        this.m_requestTimeout = value;
        this.IsRequestTimeoutDirty = true;
      }
    }

    internal bool IsRequestTimeoutDirty { get; private set; }

    public int DeadlockRetries
    {
      get => this.IsDeadlockRetriesDirty ? this.m_deadlockRetries : this.m_master.DeadlockRetries;
      set
      {
        if (!this.IsDeadlockRetriesDirty && value == this.m_master.DeadlockRetries)
          return;
        this.LockVersion();
        this.m_deadlockRetries = value;
        this.IsDeadlockRetriesDirty = true;
      }
    }

    internal bool IsDeadlockRetriesDirty { get; private set; }

    public int DeadlockPause
    {
      get => this.IsDeadlockPauseDirty ? this.m_deadlockPause : this.m_master.DeadlockPause;
      set
      {
        if (!this.IsDeadlockPauseDirty && value == this.m_master.DeadlockPause)
          return;
        this.LockVersion();
        this.m_deadlockPause = value;
        this.IsDeadlockPauseDirty = true;
      }
    }

    internal bool IsDeadlockPauseDirty { get; private set; }

    public TeamFoundationDatabaseLoggingOptions LoggingOptions
    {
      get => this.IsLoggingOptionsDirty ? this.m_loggingOptions : this.m_master.LoggingOptions;
      set
      {
        if (!this.IsLoggingOptionsDirty && value == this.m_master.LoggingOptions)
          return;
        this.LockVersion();
        this.m_loggingOptions = value;
        this.IsLoggingOptionsDirty = true;
      }
    }

    internal bool IsLoggingOptionsDirty { get; private set; }

    public TimeSpan ExecutionTimeThreshold
    {
      get => this.IsExecutionTimeThresholdDirty ? this.m_executionTimeThreshold : this.m_master.ExecutionTimeThreshold;
      set
      {
        if (!this.IsExecutionTimeThresholdDirty && !(value != this.m_master.ExecutionTimeThreshold))
          return;
        this.LockVersion();
        this.m_executionTimeThreshold = value;
        this.IsExecutionTimeThresholdDirty = true;
      }
    }

    internal bool IsExecutionTimeThresholdDirty { get; private set; }

    public string MinServiceObjective
    {
      get => this.IsMinServiceObjectiveDirty ? this.m_minServiceObjective : this.m_master.MinServiceObjective;
      set
      {
        if (!this.IsMinServiceObjectiveDirty && !(value != this.m_master.MinServiceObjective))
          return;
        this.LockVersion();
        this.m_minServiceObjective = value;
        this.IsMinServiceObjectiveDirty = true;
      }
    }

    internal bool IsMinServiceObjectiveDirty { get; private set; }

    public string MaxServiceObjective
    {
      get => this.IsMaxServiceObjectiveDirty ? this.m_maxServiceObjective : this.m_master.MaxServiceObjective;
      set
      {
        if (!this.IsMaxServiceObjectiveDirty && !(value != this.m_master.MaxServiceObjective))
          return;
        this.LockVersion();
        this.m_maxServiceObjective = value;
        this.IsMaxServiceObjectiveDirty = true;
      }
    }

    internal bool IsMaxServiceObjectiveDirty { get; private set; }

    public bool BreakerDisabled
    {
      get => this.IsBreakerDisabledDirty ? this.m_breakerDisabled : this.m_master.BreakerDisabled;
      set
      {
        if (!this.IsBreakerDisabledDirty && value == this.m_master.BreakerDisabled)
          return;
        this.LockVersion();
        this.m_breakerDisabled = value;
        this.IsBreakerDisabledDirty = true;
      }
    }

    internal bool IsBreakerDisabledDirty { get; private set; }

    public byte BreakerErrorThresholdPerc
    {
      get => this.IsBreakerErrorThresholdPercDirty ? this.m_breakerErrorThresholdPerc : this.m_master.BreakerErrorThresholdPerc;
      set
      {
        if (!this.IsBreakerErrorThresholdPercDirty && (int) value == (int) this.m_master.BreakerErrorThresholdPerc)
          return;
        this.LockVersion();
        this.m_breakerErrorThresholdPerc = value;
        this.IsBreakerErrorThresholdPercDirty = true;
      }
    }

    internal bool IsBreakerErrorThresholdPercDirty { get; private set; }

    public bool BreakerForceClosed
    {
      get => this.IsBreakerForceClosedDirty ? this.m_breakerForceClosed : this.m_master.BreakerForceClosed;
      set
      {
        if (!this.IsBreakerForceClosedDirty && value == this.m_master.BreakerForceClosed)
          return;
        this.LockVersion();
        this.m_breakerForceClosed = value;
        this.IsBreakerForceClosedDirty = true;
      }
    }

    internal bool IsBreakerForceClosedDirty { get; private set; }

    public bool BreakerForceOpen
    {
      get => this.IsBreakerForceOpenDirty ? this.m_breakerForceOpen : this.m_master.BreakerForceOpen;
      set
      {
        if (!this.IsBreakerForceOpenDirty && value == this.m_master.BreakerForceOpen)
          return;
        this.LockVersion();
        this.m_breakerForceOpen = value;
        this.IsBreakerForceOpenDirty = true;
      }
    }

    internal bool IsBreakerForceOpenDirty { get; private set; }

    public int BreakerMaxBackoff
    {
      get => this.IsBreakerMaxBackoffDirty ? this.m_breakerMaxBackoff : this.m_master.BreakerMaxBackoff;
      set
      {
        if (!this.IsBreakerMaxBackoffDirty && value == this.m_master.BreakerMaxBackoff)
          return;
        this.LockVersion();
        this.m_breakerMaxBackoff = value;
        this.IsBreakerMaxBackoffDirty = true;
      }
    }

    internal bool IsBreakerMaxBackoffDirty { get; private set; }

    public int BreakerRequestVolumeThreshold
    {
      get => this.IsBreakerRequestVolumeThresholdDirty ? this.m_breakerRequestVolumeThreshold : this.m_master.BreakerRequestVolumeThreshold;
      set
      {
        if (!this.IsBreakerRequestVolumeThresholdDirty && value == this.m_master.BreakerRequestVolumeThreshold)
          return;
        this.LockVersion();
        this.m_breakerRequestVolumeThreshold = value;
        this.IsBreakerRequestVolumeThresholdDirty = true;
      }
    }

    internal bool IsBreakerRequestVolumeThresholdDirty { get; private set; }

    public int BreakerExecutionTimeout
    {
      get => this.IsBreakerExecutionTimeoutDirty ? this.m_breakerExecutionTimeout : this.m_master.BreakerExecutionTimeout;
      set
      {
        if (!this.IsBreakerExecutionTimeoutDirty && value == this.m_master.BreakerExecutionTimeout)
          return;
        this.LockVersion();
        this.m_breakerExecutionTimeout = value;
        this.IsBreakerExecutionTimeoutDirty = true;
      }
    }

    internal bool IsBreakerExecutionTimeoutDirty { get; private set; }

    public int BreakerMaxExecConcurrentRequests
    {
      get => this.IsBreakerMaxExecConcurrentRequestsDirty ? this.m_breakerMaxExecConcurrentRequests : this.m_master.BreakerMaxExecConcurrentRequests;
      set
      {
        if (!this.IsBreakerMaxExecConcurrentRequestsDirty && value == this.m_master.BreakerMaxExecConcurrentRequests)
          return;
        this.LockVersion();
        this.m_breakerMaxExecConcurrentRequests = value;
        this.IsBreakerMaxExecConcurrentRequestsDirty = true;
      }
    }

    internal bool IsBreakerMaxExecConcurrentRequestsDirty { get; private set; }

    public int BreakerMaxFallbackConcurrentRequests
    {
      get => this.IsBreakerMaxFallbackConcurrentRequestsDirty ? this.m_breakerMaxFallbackConcurrentRequests : this.m_master.BreakerMaxFallbackConcurrentRequests;
      set
      {
        if (!this.IsBreakerMaxFallbackConcurrentRequestsDirty && value == this.m_master.BreakerMaxFallbackConcurrentRequests)
          return;
        this.LockVersion();
        this.m_breakerMaxFallbackConcurrentRequests = value;
        this.IsBreakerMaxFallbackConcurrentRequestsDirty = true;
      }
    }

    internal bool IsBreakerMaxFallbackConcurrentRequestsDirty { get; private set; }

    public DatabaseResourceStats GetDatabaseResourceStats(IVssRequestContext requestContext) => this.m_master.GetDatabaseResourceStats(requestContext);

    public Guid MaintenanceJobId => TeamFoundationDatabaseProperties.GetMaintenanceJobId(this.DatabaseId);

    private static Guid GetMaintenanceJobId(Guid baseGuid, int databaseId)
    {
      byte[] byteArray = baseGuid.ToByteArray();
      byte[] bytes = BitConverter.GetBytes(databaseId);
      Array.Copy((Array) bytes, (Array) byteArray, bytes.Length);
      return new Guid(byteArray);
    }

    internal static Guid GetMaintenanceJobId(int databaseId) => TeamFoundationDatabaseProperties.GetMaintenanceJobId(DatabaseManagementConstants.DatabaseMaintenanceBaseJobId, databaseId);

    internal static int GetDatabaseId(Guid maintenanceJobId) => !(TeamFoundationDatabaseProperties.GetMaintenanceJobId(DatabaseManagementConstants.DatabaseMaintenanceBaseJobId, 0) != TeamFoundationDatabaseProperties.GetMaintenanceJobId(maintenanceJobId, 0)) ? BitConverter.ToInt32(maintenanceJobId.ToByteArray(), 0) : throw new ArgumentException("Invalid database maintenance JobId");

    internal static string GetReadOnlyConnectionString(string connectionString)
    {
      if (connectionString == null)
        return connectionString;
      int startIndex = connectionString.IndexOf("ApplicationIntent", StringComparison.OrdinalIgnoreCase);
      if (startIndex == -1)
        return connectionString + ";ApplicationIntent=ReadOnly";
      int num = connectionString.IndexOf(';', startIndex);
      if (num == -1)
        num = connectionString.Length;
      return connectionString.Replace(connectionString.Substring(startIndex, num - startIndex), "ApplicationIntent=ReadOnly");
    }

    public override string ToString() => string.Format("[DatabaseId:                           {0}\r\nBase Version:                          {1}\r\nName:                                  {2}\r\nServiceLevel:                          {3} {4}\r\nPoolName:                              {5} {6}\r\nTenants:                               {7} {8}\r\nMaxTenants:                            {9} {10}\r\nTenantsPendingDelete:                  {11} {12}\r\nStatus:                                {13} {14}\r\nStatusReason:                          {15} {16}\r\nStatusChangedDate:                     {17}\r\nAcquisitionOrder:                      {18} {19}\r\nFlags:                                 {20} {21}\r\nDatabaseSize:                          {22} {23}\r\nDatabaseCapacity:                      {24} {25}\r\nMinServiceObjective:                   {26} {27}\r\nMaxServiceObjective:                   {28} {29}\r\nRequestTimeout:                        {30} {31}\r\nDeadlockRetries:                       {32} {33}\r\nDeadlockPause:                         {34} {35}\r\nLoggingOptions:                        {36} {37}\r\nExecutionTimeThreshold:                {38} {39}\r\nBreakerDisabled:                       {40} {41}\r\nBreakerErrorThresholdPerc:             {42} {43}\r\nBreakerForceClosed:                    {44} {45}\r\nBreakerForceOpen:                      {46} {47}\r\nBreakerMaxBackoff:                     {48} {49}\r\nBreakerRequestVolumeThreshold:         {50} {51}\r\nBreakerExecutionTimeout:               {52} {53}\r\nBreakerMaxExecConcurrentRequests:      {54} {55}\r\nBreakerMaxFallbackConcurrentRequests:  {56} {57}]", (object) this.DatabaseId, (object) this.Version, (object) this.DatabaseName, (object) this.ServiceLevel, (object) this.IsServiceLevelDirty, (object) this.PoolName, (object) this.IsPoolNameDirty, (object) this.Tenants, (object) this.IsTenantsDirty, (object) this.MaxTenants, (object) this.IsMaxTenantsDirty, (object) this.TenantsPendingDelete, (object) this.IsTenantsPendingDeleteDirty, (object) this.Status, (object) this.IsStatusDirty, (object) this.StatusReason, (object) this.IsStatusReasonDirty, (object) this.StatusChangedDate, (object) this.AcquisitionOrder, (object) this.IsAcquisitionOrderDirty, (object) this.Flags, (object) this.IsFlagsDirty, (object) this.DatabaseSize, (object) this.IsDatabaseSizeDirty, (object) this.DatabaseCapacity, (object) this.IsDatabaseCapacityDirty, (object) this.MinServiceObjective, (object) this.IsMinServiceObjectiveDirty, (object) this.MaxServiceObjective, (object) this.IsMaxServiceObjectiveDirty, (object) this.RequestTimeout, (object) this.IsRequestTimeoutDirty, (object) this.DeadlockRetries, (object) this.IsDeadlockRetriesDirty, (object) this.DeadlockPause, (object) this.IsDeadlockPauseDirty, (object) this.LoggingOptions, (object) this.IsLoggingOptionsDirty, (object) this.ExecutionTimeThreshold, (object) this.IsExecutionTimeThresholdDirty, (object) this.BreakerDisabled, (object) this.IsBreakerDisabledDirty, (object) this.BreakerErrorThresholdPerc, (object) this.IsBreakerErrorThresholdPercDirty, (object) this.BreakerForceClosed, (object) this.IsBreakerForceClosedDirty, (object) this.BreakerForceOpen, (object) this.IsBreakerForceOpenDirty, (object) this.BreakerMaxBackoff, (object) this.IsBreakerMaxBackoffDirty, (object) this.BreakerRequestVolumeThreshold, (object) this.IsBreakerRequestVolumeThresholdDirty, (object) this.BreakerExecutionTimeout, (object) this.IsBreakerExecutionTimeoutDirty, (object) this.BreakerMaxExecConcurrentRequests, (object) this.IsBreakerMaxExecConcurrentRequestsDirty, (object) this.BreakerMaxFallbackConcurrentRequests, (object) this.IsBreakerMaxFallbackConcurrentRequestsDirty);

    private void LockVersion()
    {
      if (this.IsUpdateRequired())
        return;
      this.m_version = this.m_master.Version;
    }

    internal void Updated()
    {
      this.IsAcquisitionOrderDirty = false;
      this.IsFlagsDirty = false;
      this.IsMaxTenantsDirty = false;
      this.IsPoolNameDirty = false;
      this.IsServiceLevelDirty = false;
      this.IsTenantsDirty = false;
      this.IsTenantsPendingDeleteDirty = false;
      this.IsDatabaseSizeDirty = false;
      this.IsDatabaseCapacityDirty = false;
      this.IsConnectionInfoDirty = false;
      this.IsStatusDirty = false;
      this.IsStatusReasonDirty = false;
      this.IsMinServiceObjectiveDirty = false;
      this.IsMaxServiceObjectiveDirty = false;
      this.IsRequestTimeoutDirty = false;
      this.IsDeadlockRetriesDirty = false;
      this.IsDeadlockPauseDirty = false;
      this.IsLoggingOptionsDirty = false;
      this.IsExecutionTimeThresholdDirty = false;
      this.IsBreakerDisabledDirty = false;
      this.IsBreakerErrorThresholdPercDirty = false;
      this.IsBreakerForceClosedDirty = false;
      this.IsBreakerForceOpenDirty = false;
      this.IsBreakerMaxBackoffDirty = false;
      this.IsBreakerRequestVolumeThresholdDirty = false;
      this.IsBreakerExecutionTimeoutDirty = false;
      this.IsBreakerMaxExecConcurrentRequestsDirty = false;
      this.IsBreakerMaxFallbackConcurrentRequestsDirty = false;
      this.m_connectionInfo = (SqlConnectionInfoWrapper) null;
      this.m_version = -1L;
    }

    internal bool IsUpdateRequired() => this.IsAcquisitionOrderDirty || this.IsFlagsDirty || this.IsMaxTenantsDirty || this.IsPoolNameDirty || this.IsServiceLevelDirty || this.IsTenantsDirty || this.IsTenantsPendingDeleteDirty || this.IsDatabaseSizeDirty || this.IsDatabaseCapacityDirty || this.IsConnectionInfoDirty || this.IsStatusDirty || this.IsStatusReasonDirty || this.IsMinServiceObjectiveDirty || this.IsMaxServiceObjectiveDirty || this.IsRequestTimeoutDirty || this.IsDeadlockRetriesDirty || this.IsDeadlockPauseDirty || this.IsLoggingOptionsDirty || this.IsExecutionTimeThresholdDirty || this.IsBreakerDisabledDirty || this.IsBreakerErrorThresholdPercDirty || this.IsBreakerForceClosedDirty || this.IsBreakerForceOpenDirty || this.IsBreakerMaxBackoffDirty || this.IsBreakerRequestVolumeThresholdDirty || this.IsBreakerExecutionTimeoutDirty || this.IsBreakerMaxExecConcurrentRequestsDirty || this.IsBreakerMaxFallbackConcurrentRequestsDirty;

    public TeamFoundationDatabaseProperties GetEditableProperties() => this;

    public ITeamFoundationDatabaseProperties GetCachedProperties() => (ITeamFoundationDatabaseProperties) this.m_master;

    public ISqlConnectionInfo GetDboConnectionInfo() => this.DboConnectionInfo ?? this.SqlConnectionInfo;

    public string GetActualServerName(IVssRequestContext requestContext) => this.m_master.GetActualServerName(requestContext);

    public string GetFailoverGroupListenerName(IVssRequestContext requestContext) => this.m_master.GetFailoverGroupListenerName(requestContext);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationDatabaseProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface ITeamFoundationDatabaseProperties
  {
    DateTime AcquisitionOrder { get; }

    SqlConnectionInfoWrapper ConnectionInfoWrapper { get; }

    SqlConnectionInfoWrapper DboConnectionInfoWrapper { get; }

    SqlConnectionInfoWrapper ReadOnlyConnectionInfoWrapper { get; }

    int DatabaseCapacity { get; }

    int DatabaseId { get; }

    string DatabaseName { get; }

    int DatabaseSize { get; }

    TeamFoundationDatabaseFlags Flags { get; }

    DateTime LastTenantAdded { get; }

    int MaxTenants { get; }

    string PoolName { get; }

    string ServiceLevel { get; }

    DateTime SizeChangedDate { get; }

    ISqlConnectionInfo SqlConnectionInfo { get; }

    ISqlConnectionInfo DboConnectionInfo { get; }

    ISqlConnectionInfo ReadOnlyConnectionInfo { get; }

    TeamFoundationDatabaseStatus Status { get; }

    DateTime StatusChangedDate { get; }

    string StatusReason { get; }

    int Tenants { get; }

    int TenantsPendingDelete { get; }

    string MinServiceObjective { get; }

    string MaxServiceObjective { get; }

    long Version { get; }

    int RequestTimeout { get; }

    int DeadlockRetries { get; }

    int DeadlockPause { get; }

    TeamFoundationDatabaseLoggingOptions LoggingOptions { get; }

    TimeSpan ExecutionTimeThreshold { get; }

    bool BreakerDisabled { get; }

    byte BreakerErrorThresholdPerc { get; }

    bool BreakerForceClosed { get; }

    bool BreakerForceOpen { get; }

    int BreakerMaxBackoff { get; }

    int BreakerRequestVolumeThreshold { get; }

    int BreakerExecutionTimeout { get; }

    int BreakerMaxExecConcurrentRequests { get; }

    int BreakerMaxFallbackConcurrentRequests { get; }

    Guid MaintenanceJobId { get; }

    DatabaseResourceStats GetDatabaseResourceStats(IVssRequestContext requestContext);

    TeamFoundationDatabaseProperties GetEditableProperties();

    ITeamFoundationDatabaseProperties GetCachedProperties();

    ISqlConnectionInfo GetDboConnectionInfo();

    string GetActualServerName(IVssRequestContext requestContext);

    string GetFailoverGroupListenerName(IVssRequestContext requestContext);
  }
}

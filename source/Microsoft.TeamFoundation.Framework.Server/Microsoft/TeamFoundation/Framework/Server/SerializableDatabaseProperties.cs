// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SerializableDatabaseProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SerializableDatabaseProperties
  {
    public DateTime? AcquisitionOrder { get; set; }

    public SqlConnectionInfoWrapper ConnectionInfoWrapper { get; set; }

    public SqlConnectionInfoWrapper DboConnectionInfoWrapper { get; set; }

    public SqlConnectionInfoWrapper ReadOnlyConnectionInfoWrapper { get; set; }

    public int? DatabaseCapacity { get; set; }

    public int DatabaseId { get; set; }

    public string DatabaseName { get; set; }

    public int? DatabaseSize { get; set; }

    public int? Flags { get; set; }

    public DateTime? LastTenantAdded { get; set; }

    public int? MaxTenants { get; set; }

    public string PoolName { get; set; }

    public string ServiceLevel { get; set; }

    public DateTime? SizeChangedDate { get; set; }

    public int? Status { get; set; }

    public DateTime? StatusChangedDate { get; set; }

    public string StatusReason { get; set; }

    public int? Tenants { get; set; }

    public int? TenantsPendingDelete { get; set; }

    public string MinServiceObjective { get; set; }

    public string MaxServiceObjective { get; set; }

    public long? Version { get; set; }

    public int? RequestTimeout { get; set; }

    public int? DeadlockPause { get; set; }

    public int? DeadlockRetries { get; set; }

    public byte? LoggingOptions { get; set; }

    public int? ExecutionTimeThreshold { get; set; }

    public bool? BreakerDisabled { get; set; }

    public byte? BreakerErrorThresholdPerc { get; set; }

    public bool? BreakerForceClosed { get; set; }

    public bool? BreakerForceOpen { get; set; }

    public int? BreakerMaxBackoff { get; set; }

    public int? BreakerRequestVolumeThreshold { get; set; }

    public int? BreakerExecutionTimeout { get; set; }

    public int? BreakerMaxExecConcurrentRequests { get; set; }

    public int? BreakerMaxFallbackConcurrentRequests { get; set; }
  }
}

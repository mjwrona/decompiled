// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationDatabasePool
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationDatabasePool
  {
    private int m_poolId;
    private TeamFoundationDatabaseType m_databaseType;
    private string m_collation;
    private string m_poolName;
    private bool m_isPoolNameDirty;
    private TeamFoundationDatabaseCredentialPolicy m_credentialPolicy;
    private bool m_isCredentialPolicyDirty;
    private int m_initialCapacity;
    private bool m_isInitialCapacityDirty;
    private int m_createThreshold;
    private bool m_isCreateThresholdDirty;
    private int m_growBy;
    private bool m_isGrowByDirty;
    private string m_servicingOperations;
    private bool m_isServicingOperationsDirty;
    private int m_maxDatabaseLimit;
    private bool m_isMaxDatabaseLimitDirty;
    private string m_deltaOperationPrefixes;
    private bool m_isDeltaOperationPrefixesDirty;
    private TeamFoundationDatabasePoolFlags m_flags;
    private bool m_areFlagsDirty;
    private string m_serviceObjective;
    private bool m_isServiceObjectiveDirty;

    public int PoolId => this.m_poolId;

    public TeamFoundationDatabaseType DatabaseType => this.m_databaseType;

    public string Collation => this.m_collation;

    public string PoolName
    {
      get => this.m_poolName;
      set
      {
        this.m_poolName = value;
        this.m_isPoolNameDirty = true;
      }
    }

    public bool IsPoolNameDirty => this.m_isPoolNameDirty;

    public TeamFoundationDatabaseCredentialPolicy CredentialPolicy
    {
      get => this.m_credentialPolicy;
      set
      {
        this.m_credentialPolicy = value;
        this.m_isCredentialPolicyDirty = true;
      }
    }

    internal bool IsCredentialPolicyDirty => this.m_isCredentialPolicyDirty;

    public int InitialCapacity
    {
      get => this.m_initialCapacity;
      set
      {
        this.m_initialCapacity = value;
        this.m_isInitialCapacityDirty = true;
      }
    }

    internal bool IsInitialCapacityDirty => this.m_isInitialCapacityDirty;

    public int CreateThreshold
    {
      get => this.m_createThreshold;
      set
      {
        this.m_createThreshold = value;
        this.m_isCreateThresholdDirty = true;
      }
    }

    public bool IsCreateThresholdDirty => this.m_isCreateThresholdDirty;

    public int GrowBy
    {
      get => this.m_growBy;
      set
      {
        this.m_growBy = value;
        this.m_isGrowByDirty = true;
      }
    }

    internal bool IsGrowByDirty => this.m_isGrowByDirty;

    public string ServicingOperations
    {
      get => this.m_servicingOperations;
      set
      {
        this.m_servicingOperations = value;
        this.m_isServicingOperationsDirty = true;
      }
    }

    internal bool IsServicingOperationsDirty => this.m_isServicingOperationsDirty;

    public int MaxDatabaseLimit
    {
      get => this.m_maxDatabaseLimit;
      set
      {
        this.m_maxDatabaseLimit = value;
        this.m_isMaxDatabaseLimitDirty = true;
      }
    }

    internal bool IsMaxDatabaseLimitDirty => this.m_isMaxDatabaseLimitDirty;

    public string DeltaOperationPrefixes
    {
      get => this.m_deltaOperationPrefixes;
      set
      {
        this.m_deltaOperationPrefixes = value;
        this.m_isDeltaOperationPrefixesDirty = true;
      }
    }

    public bool DisableTenantCountFixup
    {
      get => (this.m_flags & TeamFoundationDatabasePoolFlags.DisableTenantCountFixup) == TeamFoundationDatabasePoolFlags.DisableTenantCountFixup;
      set
      {
        if (value)
          this.m_flags |= TeamFoundationDatabasePoolFlags.DisableTenantCountFixup;
        else
          this.m_flags &= ~TeamFoundationDatabasePoolFlags.DisableTenantCountFixup;
        this.m_areFlagsDirty = true;
      }
    }

    internal bool IsDeltaOperationPrefixesDirty => this.m_isDeltaOperationPrefixesDirty;

    internal bool AreFlagsDirty() => this.m_areFlagsDirty;

    internal bool IsServiceObjectiveDirty => this.m_isServiceObjectiveDirty;

    public string ServiceObjective
    {
      get => this.m_serviceObjective;
      set
      {
        this.m_serviceObjective = value;
        this.m_isServiceObjectiveDirty = true;
      }
    }

    internal TeamFoundationDatabasePoolFlags GetFlags() => this.m_flags;

    public void Initialize(
      int poolId,
      TeamFoundationDatabaseType databaseType,
      TeamFoundationDatabaseCredentialPolicy credentialPolicy,
      string collation,
      string poolName,
      int initialCapacity,
      int createThreshold,
      int growBy,
      string servicingOperations,
      int maxDatabaseLimit,
      string deltaOperationPrefixes,
      TeamFoundationDatabasePoolFlags flags,
      string serviceObjective)
    {
      this.m_poolId = poolId;
      this.m_databaseType = databaseType;
      this.m_credentialPolicy = credentialPolicy;
      this.m_collation = collation;
      this.m_poolName = poolName;
      this.m_initialCapacity = initialCapacity;
      this.m_createThreshold = createThreshold;
      this.m_growBy = growBy;
      this.m_servicingOperations = servicingOperations;
      this.m_maxDatabaseLimit = maxDatabaseLimit;
      this.m_deltaOperationPrefixes = deltaOperationPrefixes;
      this.m_flags = flags;
      this.m_serviceObjective = serviceObjective;
    }

    internal void Updated()
    {
      this.m_isCreateThresholdDirty = false;
      this.m_isCredentialPolicyDirty = false;
      this.m_isGrowByDirty = false;
      this.m_isInitialCapacityDirty = false;
      this.m_isMaxDatabaseLimitDirty = false;
      this.m_isPoolNameDirty = false;
      this.m_isServicingOperationsDirty = false;
      this.m_isDeltaOperationPrefixesDirty = false;
      this.m_areFlagsDirty = false;
      this.m_isServiceObjectiveDirty = false;
    }

    internal bool IsUpdateRequired() => this.m_isCreateThresholdDirty || this.m_isCredentialPolicyDirty || this.m_isGrowByDirty || this.m_isInitialCapacityDirty || this.m_isMaxDatabaseLimitDirty || this.m_isPoolNameDirty || this.m_isServicingOperationsDirty || this.m_isDeltaOperationPrefixesDirty || this.m_areFlagsDirty || this.m_isServiceObjectiveDirty;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TeamFoundationDatabasePool properties\r\n[\r\n    PoolName:                {0}\r\n    PoolId:                  {1}\r\n    DatabaseType:            {2}\r\n    Collation:               {3}\r\n    InitialCapacity:         {4}\r\n    CreateThreshold:         {5}\r\n    GrowBy:                  {6}\r\n    ServicingOperations:     {7}\r\n    DeltaOperationPrefixes:  {8}\r\n    MaxDatabaseLimit:        {9}\r\n    CredentialPolicy:        {10}\r\n    DisableTenantCountFixup: {11}\r\n    ServiceObjective:        {12}\r\n]", (object) this.PoolName, (object) this.PoolId, (object) this.DatabaseType, (object) this.Collation, (object) this.InitialCapacity, (object) this.CreateThreshold, (object) this.GrowBy, (object) this.ServicingOperations, (object) this.DeltaOperationPrefixes, (object) this.MaxDatabaseLimit, (object) this.CredentialPolicy, (object) this.DisableTenantCountFixup, (object) this.ServiceObjective);
  }
}

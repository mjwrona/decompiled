// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ElasticPool
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class ElasticPool
  {
    public ElasticPool()
    {
    }

    private ElasticPool(ElasticPool poolToBeCloned)
    {
      this.PoolId = poolToBeCloned.PoolId;
      this.ServiceEndpointId = poolToBeCloned.ServiceEndpointId;
      this.ServiceEndpointScope = poolToBeCloned.ServiceEndpointScope;
      this.AzureId = poolToBeCloned.AzureId;
      this.MaxCapacity = poolToBeCloned.MaxCapacity;
      this.DesiredIdle = poolToBeCloned.DesiredIdle;
      this.RecycleAfterEachUse = poolToBeCloned.RecycleAfterEachUse;
      this.MaxSavedNodeCount = poolToBeCloned.MaxSavedNodeCount;
      this.OsType = poolToBeCloned.OsType;
      this.OrchestrationType = poolToBeCloned.OrchestrationType;
      this.State = poolToBeCloned.State;
      this.OfflineSince = poolToBeCloned.OfflineSince;
      this.DesiredSize = poolToBeCloned.DesiredSize;
      this.SizingAttempts = poolToBeCloned.SizingAttempts;
      this.AgentInteractiveUI = poolToBeCloned.AgentInteractiveUI;
      this.TimeToLiveMinutes = poolToBeCloned.TimeToLiveMinutes;
    }

    public void Validate()
    {
      ArgumentUtility.CheckForNonPositiveInt(this.PoolId, "PoolId");
      ArgumentUtility.CheckForEmptyGuid(this.ServiceEndpointId, "ServiceEndpointId");
      ArgumentUtility.CheckForEmptyGuid(this.ServiceEndpointScope, "ServiceEndpointScope");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(this.AzureId, "AzureId");
      ArgumentUtility.CheckForOutOfRange(this.MaxCapacity, "MaxCapacity", 0, 1000);
      ArgumentUtility.CheckForOutOfRange(this.DesiredIdle, "DesiredIdle", 0, this.MaxCapacity);
      ArgumentUtility.CheckForOutOfRange(this.MaxSavedNodeCount, "MaxSavedNodeCount", 0, this.MaxCapacity);
      ArgumentUtility.CheckForNonnegativeInt(this.DesiredSize, "DesiredSize");
      ArgumentUtility.CheckForNonnegativeInt(this.SizingAttempts, "SizingAttempts");
      ArgumentUtility.CheckForNonnegativeInt(this.TimeToLiveMinutes, "TimeToLiveMinutes");
      ArgumentUtility.CheckForOutOfRange(this.TimeToLiveMinutes, "TimeToLiveMinutes", 0, 10000);
    }

    public void MergeChanges(NullableElasticPool nep)
    {
      this.PoolId = nep.PoolId;
      Guid? nullable1 = nep.ServiceEndpointId;
      this.ServiceEndpointId = nullable1 ?? this.ServiceEndpointId;
      nullable1 = nep.ServiceEndpointScope;
      this.ServiceEndpointScope = nullable1 ?? this.ServiceEndpointScope;
      this.AzureId = nep.AzureId ?? this.AzureId;
      int? nullable2 = nep.MaxCapacity;
      this.MaxCapacity = nullable2 ?? this.MaxCapacity;
      nullable2 = nep.DesiredIdle;
      this.DesiredIdle = nullable2 ?? this.DesiredIdle;
      bool? nullable3 = nep.RecycleAfterEachUse;
      this.RecycleAfterEachUse = ((int) nullable3 ?? (this.RecycleAfterEachUse ? 1 : 0)) != 0;
      nullable2 = nep.MaxSavedNodeCount;
      this.MaxSavedNodeCount = nullable2 ?? this.MaxSavedNodeCount;
      this.OsType = (OperatingSystemType) ((int) nep.OsType ?? (int) this.OsType);
      this.OrchestrationType = (OrchestrationType) ((int) nep.OrchestrationType ?? (int) this.OrchestrationType);
      ElasticPoolState? state = nep.State;
      this.State = (ElasticPoolState) ((int) state ?? (int) this.State);
      state = nep.State;
      ElasticPoolState elasticPoolState = ElasticPoolState.Online;
      DateTime? nullable4;
      DateTime? nullable5;
      if (state.GetValueOrDefault() == elasticPoolState & state.HasValue)
      {
        nullable4 = nep.OfflineSince;
        if (!nullable4.HasValue)
        {
          nullable4 = new DateTime?();
          nullable5 = nullable4;
          goto label_4;
        }
      }
      nullable4 = nep.OfflineSince;
      nullable5 = nullable4 ?? this.OfflineSince;
label_4:
      this.OfflineSince = nullable5;
      nullable2 = nep.DesiredSize;
      this.DesiredSize = nullable2 ?? this.DesiredSize;
      nullable2 = nep.SizingAttempts;
      this.SizingAttempts = nullable2 ?? this.SizingAttempts;
      nullable3 = nep.AgentInteractiveUI;
      this.AgentInteractiveUI = ((int) nullable3 ?? (this.AgentInteractiveUI ? 1 : 0)) != 0;
      nullable2 = nep.TimeToLiveMinutes;
      this.TimeToLiveMinutes = nullable2 ?? this.TimeToLiveMinutes;
    }

    [DataMember]
    public int PoolId { get; set; }

    [DataMember]
    public Guid ServiceEndpointId { get; set; }

    [DataMember]
    public Guid ServiceEndpointScope { get; set; }

    [DataMember]
    public string AzureId { get; set; }

    [DataMember]
    public int MaxCapacity { get; set; }

    [DataMember]
    public int DesiredIdle { get; set; }

    [DataMember]
    public bool RecycleAfterEachUse { get; set; }

    [DataMember]
    public int MaxSavedNodeCount { get; set; }

    [DataMember]
    public OperatingSystemType OsType { get; set; }

    [DataMember]
    public OrchestrationType OrchestrationType { get; set; }

    [DataMember]
    public ElasticPoolState State { get; set; }

    [DataMember]
    public DateTime? OfflineSince { get; set; }

    [DataMember]
    public int DesiredSize { get; set; }

    [DataMember]
    public int SizingAttempts { get; set; }

    [DataMember]
    public bool AgentInteractiveUI { get; set; }

    [DataMember]
    public int TimeToLiveMinutes { get; set; }

    public int SavedNodeCount { get; set; }

    public ElasticPool Clone() => new ElasticPool(this);
  }
}

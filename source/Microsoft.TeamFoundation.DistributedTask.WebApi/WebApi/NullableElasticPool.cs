// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.NullableElasticPool
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public class NullableElasticPool
  {
    public NullableElasticPool()
    {
    }

    public NullableElasticPool(int poolId) => this.PoolId = poolId;

    public NullableElasticPool(ElasticPool ep)
    {
      this.PoolId = ep.PoolId;
      this.ServiceEndpointId = new Guid?(ep.ServiceEndpointId);
      this.ServiceEndpointScope = new Guid?(ep.ServiceEndpointScope);
      this.AzureId = ep.AzureId;
      this.MaxCapacity = new int?(ep.MaxCapacity);
      this.DesiredIdle = new int?(ep.DesiredIdle);
      this.RecycleAfterEachUse = new bool?(ep.RecycleAfterEachUse);
      this.MaxSavedNodeCount = new int?(ep.MaxSavedNodeCount);
      this.OsType = new OperatingSystemType?(ep.OsType);
      this.OrchestrationType = new Microsoft.TeamFoundation.DistributedTask.WebApi.OrchestrationType?(ep.OrchestrationType);
      this.State = new ElasticPoolState?(ep.State);
      this.OfflineSince = ep.OfflineSince;
      this.DesiredSize = new int?(ep.DesiredSize);
      this.SizingAttempts = new int?(ep.SizingAttempts);
      this.AgentInteractiveUI = new bool?(ep.AgentInteractiveUI);
      this.TimeToLiveMinutes = new int?(ep.TimeToLiveMinutes);
    }

    public NullableElasticPool(int poolId, ElasticPoolSettings eps)
    {
      this.PoolId = poolId;
      this.ServiceEndpointId = eps.ServiceEndpointId;
      this.ServiceEndpointScope = eps.ServiceEndpointScope;
      this.AzureId = eps.AzureId;
      this.MaxCapacity = eps.MaxCapacity;
      this.DesiredIdle = eps.DesiredIdle;
      this.RecycleAfterEachUse = eps.RecycleAfterEachUse;
      this.MaxSavedNodeCount = eps.MaxSavedNodeCount;
      this.OsType = eps.OsType;
      this.OrchestrationType = eps.OrchestrationType;
      this.AgentInteractiveUI = eps.AgentInteractiveUI;
      this.TimeToLiveMinutes = eps.TimeToLiveMinutes;
    }

    public int PoolId { get; set; }

    public Guid? ServiceEndpointId { get; set; }

    public Guid? ServiceEndpointScope { get; set; }

    public string AzureId { get; set; }

    public int? MaxCapacity { get; set; }

    public int? DesiredIdle { get; set; }

    public bool? RecycleAfterEachUse { get; set; }

    public int? MaxSavedNodeCount { get; set; }

    public OperatingSystemType? OsType { get; set; }

    public Microsoft.TeamFoundation.DistributedTask.WebApi.OrchestrationType? OrchestrationType { get; set; }

    public ElasticPoolState? State { get; set; }

    public DateTime? OfflineSince { get; set; }

    public int? DesiredSize { get; set; }

    public int? SizingAttempts { get; set; }

    public bool? AgentInteractiveUI { get; set; }

    public int? TimeToLiveMinutes { get; set; }
  }
}

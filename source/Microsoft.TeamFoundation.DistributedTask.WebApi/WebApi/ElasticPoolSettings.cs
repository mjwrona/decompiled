// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ElasticPoolSettings
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class ElasticPoolSettings
  {
    public ElasticPoolSettings()
    {
    }

    private ElasticPoolSettings(ElasticPoolSettings poolToBeCloned)
    {
      this.ServiceEndpointId = poolToBeCloned.ServiceEndpointId;
      this.ServiceEndpointScope = poolToBeCloned.ServiceEndpointScope;
      this.AzureId = poolToBeCloned.AzureId;
      this.MaxCapacity = poolToBeCloned.MaxCapacity;
      this.DesiredIdle = poolToBeCloned.DesiredIdle;
      this.RecycleAfterEachUse = poolToBeCloned.RecycleAfterEachUse;
      this.MaxSavedNodeCount = poolToBeCloned.MaxSavedNodeCount;
      this.OsType = poolToBeCloned.OsType;
      this.AgentInteractiveUI = poolToBeCloned.AgentInteractiveUI;
      this.TimeToLiveMinutes = poolToBeCloned.TimeToLiveMinutes;
    }

    [DataMember]
    public Guid? ServiceEndpointId { get; set; }

    [DataMember]
    public Guid? ServiceEndpointScope { get; set; }

    [DataMember]
    public string AzureId { get; set; }

    [DataMember]
    public int? MaxCapacity { get; set; }

    [DataMember]
    public int? DesiredIdle { get; set; }

    [DataMember]
    public bool? RecycleAfterEachUse { get; set; }

    [DataMember]
    public int? MaxSavedNodeCount { get; set; }

    [DataMember]
    public OperatingSystemType? OsType { get; set; }

    [DataMember]
    public Microsoft.TeamFoundation.DistributedTask.WebApi.OrchestrationType? OrchestrationType { get; set; }

    [DataMember]
    public bool? AgentInteractiveUI { get; set; }

    [DataMember]
    public int? TimeToLiveMinutes { get; set; }

    public ElasticPoolSettings Clone() => new ElasticPoolSettings(this);
  }
}

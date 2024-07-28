// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskOrchestrationPlanReference
  {
    [DataMember]
    public Guid ScopeIdentifier { get; set; }

    [DataMember]
    public string PlanType { get; set; }

    [DataMember]
    public int Version { get; set; }

    [DataMember]
    public Guid PlanId { get; set; }

    [DataMember]
    public string PlanGroup { get; set; }

    [DataMember]
    public Uri ArtifactUri { get; set; }

    [DataMember]
    public Uri ArtifactLocation { get; set; }

    [IgnoreDataMember]
    internal long ContainerId { get; set; }

    [IgnoreDataMember]
    public OrchestrationProcessType ProcessType { get; internal set; }

    [DataMember]
    public TaskOrchestrationOwner Definition { get; set; }

    [DataMember]
    public TaskOrchestrationOwner Owner { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.EnvironmentDeploymentExecutionRecord
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class EnvironmentDeploymentExecutionRecord
  {
    [DataMember]
    public long Id { get; set; }

    [DataMember]
    public string RequestIdentifier { get; set; }

    [DataMember]
    public int EnvironmentId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ServiceOwner { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ScopeId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? ResourceId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PlanType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid PlanId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StageName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string JobName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int StageAttempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int JobAttempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskOrchestrationOwner Definition { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskOrchestrationOwner Owner { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskResult? Result { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime QueueTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }
  }
}

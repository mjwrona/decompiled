// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Data.Model.DeploymentExecutionRecordObject
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Data.Model
{
  [DataContract]
  public class DeploymentExecutionRecordObject
  {
    [DataMember]
    public long Id { get; set; }

    [DataMember]
    public string RequestIdentifier { get; set; }

    [DataMember]
    public EnvironmentReference EnvironmentReference { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ServiceOwner { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ScopeId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public EnvironmentResourceReference ResourceReference { get; set; }

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

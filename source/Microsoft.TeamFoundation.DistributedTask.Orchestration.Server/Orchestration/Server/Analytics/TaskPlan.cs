// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.TaskPlan
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  [DataContract]
  public class TaskPlan : ShallowTaskPlan
  {
    [DataMember(EmitDefaultValue = false)]
    public Guid? PlanGuid { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? TimelineGuid { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? PlanVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PlanType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PlanGroup { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? ProcessType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? TemplateType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? ChangeId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ArtifactUri { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Result { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResultCode { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DefinitionName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string OwnerName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? RequestedByGuid { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? RequestedForGuid { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }
  }
}

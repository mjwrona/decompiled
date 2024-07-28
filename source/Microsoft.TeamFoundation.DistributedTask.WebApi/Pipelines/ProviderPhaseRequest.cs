// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ProviderPhaseRequest
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class ProviderPhaseRequest
  {
    [DataMember(IsRequired = true)]
    public Guid PlanId { get; set; }

    [DataMember(IsRequired = true)]
    public string PlanType { get; set; }

    [DataMember(IsRequired = false)]
    public int PlanVersion { get; set; }

    [DataMember(IsRequired = true)]
    public Guid ServiceOwner { get; set; }

    [DataMember(IsRequired = true)]
    public string PhaseOrchestrationId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ProviderPhase ProviderPhase { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ProjectReference Project { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskOrchestrationOwner Pipeline { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskOrchestrationOwner Run { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PipelineGraphNodeReference Stage { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PipelineGraphNodeReference Phase { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, VariableValue> Variables { get; set; }
  }
}

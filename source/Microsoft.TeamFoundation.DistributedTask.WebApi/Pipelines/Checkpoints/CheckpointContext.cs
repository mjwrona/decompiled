// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Checkpoints.CheckpointContext
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Checkpoints
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  [ClientIgnore]
  public class CheckpointContext
  {
    [DataMember(IsRequired = true)]
    public Guid Id { get; set; }

    [DataMember(IsRequired = true)]
    public string OrchestrationId { get; set; }

    [DataMember(IsRequired = true)]
    public Guid PlanId { get; set; }

    [DataMember(IsRequired = true)]
    public string HubName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CheckpointScope Project { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PipelineScope Pipeline { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GraphNodeScope GraphNode { get; set; }
  }
}

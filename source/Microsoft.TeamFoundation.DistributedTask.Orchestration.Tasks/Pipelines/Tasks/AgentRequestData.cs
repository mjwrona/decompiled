// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.AgentRequestData
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  public sealed class AgentRequestData
  {
    [DataMember(EmitDefaultValue = false)]
    public int PoolId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public long RequestId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? ReceiveTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentReference ReservedAgent { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public JObject AgentSpecification { get; set; }
  }
}

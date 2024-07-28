// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunPlanInput
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  [DataContract]
  [JsonConverter(typeof (RunPlanInputConverter))]
  public sealed class RunPlanInput
  {
    [DataMember]
    public int PoolId { get; set; }

    [DataMember]
    public Guid HostId { get; set; }

    [DataMember]
    public Guid PlanId { get; set; }

    [DataMember]
    public Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy.JobEnvironment Environment { get; set; }

    [DataMember]
    public TaskOrchestrationContainer Implementation { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunAgentJobInput
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  public sealed class RunAgentJobInput
  {
    [DataMember(Name = "AgentIds", EmitDefaultValue = false)]
    private List<int> m_agentIds;

    public RunAgentJobInput()
    {
      this.StageAttempt = 1;
      this.PhaseAttempt = 1;
    }

    [DataMember]
    public Guid ScopeId { get; set; }

    [DataMember]
    public Guid PlanId { get; set; }

    [DataMember]
    public int PlanVersion { get; set; }

    [DataMember]
    public int QueueId { get; set; }

    [DataMember]
    public int PoolId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int ActivityDispatcherShardsCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PipelineActivityShardKey ShardKey { get; set; }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int StageAttempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StageName { get; set; }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int PhaseAttempt { get; set; }

    [DataMember]
    public string PhaseName { get; set; }

    [DataMember]
    public JobExecutionState Job { get; set; }

    public List<int> AgentIds
    {
      get
      {
        if (this.m_agentIds == null)
          this.m_agentIds = new List<int>();
        return this.m_agentIds;
      }
    }

    [DataMember]
    public bool NotifyJobAssigned { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool NotifyProviderJobStarted { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public JObject AgentSpecification { get; set; }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<int> agentIds = this.m_agentIds;
      // ISSUE: explicit non-virtual call
      if ((agentIds != null ? (__nonvirtual (agentIds.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_agentIds = (List<int>) null;
    }
  }
}

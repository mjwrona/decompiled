// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.RunDeploymentLifeCycleHookInput
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DataContract]
  internal class RunDeploymentLifeCycleHookInput
  {
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private List<IVariable> m_variables;

    [DataMember]
    public Guid ScopeId { get; set; }

    [DataMember]
    public Guid PlanId { get; set; }

    [DataMember]
    public string PlanType { get; set; }

    [DataMember]
    public PipelineGraphNodeReference Stage { get; set; }

    [DataMember]
    public PipelineGraphNodeReference Phase { get; set; }

    [DataMember]
    public string HookInstaceName { get; set; }

    [DataMember]
    public int Attempt { get; set; }

    [DataMember]
    public int Order { get; set; }

    [DataMember]
    public string HookInstaceDisplayName { get; set; }

    [DataMember]
    public DeploymentLifeCycleHookBase LifeCycleHook { get; set; }

    [DataMember]
    public int Version { get; set; }

    public IList<IVariable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new List<IVariable>();
        return (IList<IVariable>) this.m_variables;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<IVariable> variables = this.m_variables;
      // ISSUE: explicit non-virtual call
      if ((variables != null ? (__nonvirtual (variables.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_variables = (List<IVariable>) null;
    }
  }
}

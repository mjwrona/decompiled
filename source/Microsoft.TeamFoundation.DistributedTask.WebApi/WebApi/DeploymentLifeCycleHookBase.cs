// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentLifeCycleHookBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal abstract class DeploymentLifeCycleHookBase
  {
    private List<Step> m_steps;

    protected DeploymentLifeCycleHookBase(DeploymentLifeCycleHookType type)
    {
      this.Type = type;
      this.m_steps = new List<Step>();
    }

    protected DeploymentLifeCycleHookBase(DeploymentLifeCycleHookBase hookToClone)
    {
      this.Type = hookToClone.Type;
      this.Target = hookToClone.Target?.Clone();
      List<Step> steps = hookToClone.m_steps;
      // ISSUE: explicit non-virtual call
      if ((steps != null ? (__nonvirtual (steps.Count) > 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_steps = new List<Step>(hookToClone.m_steps.Select<Step, Step>((Func<Step, Step>) (x => x.Clone())));
    }

    [DataMember]
    public DeploymentLifeCycleHookType Type { get; set; }

    [DataMember]
    public PhaseTarget Target { get; set; }

    [DataMember]
    public List<Step> Steps
    {
      get
      {
        if (this.m_steps == null)
          this.m_steps = new List<Step>();
        return this.m_steps;
      }
    }

    public abstract DeploymentLifeCycleHookBase Clone();

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<Step> steps = this.m_steps;
      // ISSUE: explicit non-virtual call
      if ((steps != null ? (__nonvirtual (steps.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_steps = (List<Step>) null;
    }
  }
}

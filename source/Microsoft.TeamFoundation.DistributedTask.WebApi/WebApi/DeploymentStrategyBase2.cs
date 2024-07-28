// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentStrategyBase2
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal abstract class DeploymentStrategyBase2
  {
    private IList<DeploymentLifeCycleHookBase> m_hooks;

    protected DeploymentStrategyBase2(DeploymentStrategyType type)
    {
      this.Type = type;
      this.m_hooks = (IList<DeploymentLifeCycleHookBase>) new List<DeploymentLifeCycleHookBase>();
    }

    [DataMember]
    public DeploymentStrategyType Type { get; set; }

    [DataMember]
    public IList<DeploymentLifeCycleHookBase> Hooks
    {
      get
      {
        if (this.m_hooks == null)
          this.m_hooks = (IList<DeploymentLifeCycleHookBase>) new List<DeploymentLifeCycleHookBase>();
        return this.m_hooks;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IList<DeploymentLifeCycleHookBase> hooks = this.m_hooks;
      if ((hooks != null ? (hooks.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_hooks = (IList<DeploymentLifeCycleHookBase>) null;
    }
  }
}

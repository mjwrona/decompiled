// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DeploymentLifeCycleExecutor
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class DeploymentLifeCycleExecutor
  {
    private TaskResult m_currentResult;
    private TaskResult? m_lifeCycleResult;
    private bool m_lifeCycleHookInProgress;
    private List<DeploymentLifeCycleHookBase> m_lifeCycleHooks;
    private DeploymentLifeCycleHookType m_nextHookType;

    public DeploymentLifeCycleExecutor(IList<DeploymentLifeCycleHookBase> hooks)
    {
      this.m_lifeCycleHooks = new List<DeploymentLifeCycleHookBase>();
      this.m_lifeCycleHooks.AddRange((IEnumerable<DeploymentLifeCycleHookBase>) hooks);
      this.m_currentResult = TaskResult.Succeeded;
      this.m_lifeCycleResult = new TaskResult?();
      this.m_nextHookType = this.m_lifeCycleHooks.Count == 1 ? this.m_lifeCycleHooks[0].Type : DeploymentLifeCycleHookType.PreDeploy;
    }

    public DeploymentLifeCycleHookBase GetNextLifeCycleHook()
    {
      if (this.IsDeploymentCompleted() || this.m_lifeCycleHookInProgress)
        return (DeploymentLifeCycleHookBase) null;
      DeploymentLifeCycleHookBase nextLifeCycleHook = this.GetNextLifeCycleHook(this.m_nextHookType);
      this.m_nextHookType = this.GetNextLifeCycleHookType(nextLifeCycleHook != null ? nextLifeCycleHook.Type : DeploymentLifeCycleHookType.Undefined);
      if (nextLifeCycleHook == null)
        return nextLifeCycleHook;
      this.m_lifeCycleHookInProgress = true;
      return nextLifeCycleHook;
    }

    public void OnHookCompleted(TaskResult result)
    {
      this.m_currentResult = PipelineUtilities.MergeResult(this.m_currentResult, result);
      if (this.m_currentResult == TaskResult.Failed && this.m_nextHookType != DeploymentLifeCycleHookType.Undefined)
        this.m_nextHookType = DeploymentLifeCycleHookType.OnFailure;
      if (!this.CanExecuteHooks() || this.m_currentResult == TaskResult.Canceled)
        this.m_lifeCycleResult = new TaskResult?(this.m_currentResult);
      this.m_lifeCycleHookInProgress = false;
    }

    public bool IsDeploymentCompleted() => this.m_lifeCycleResult.HasValue;

    public TaskResult? GetDeploymentResult() => this.m_lifeCycleResult;

    private DeploymentLifeCycleHookBase GetNextLifeCycleHook(DeploymentLifeCycleHookType hookType)
    {
      DeploymentLifeCycleHookBase lifeCycleHookBase = this.m_lifeCycleHooks.FirstOrDefault<DeploymentLifeCycleHookBase>((Func<DeploymentLifeCycleHookBase, bool>) (h => h.Type == hookType));
      return lifeCycleHookBase == null && hookType != DeploymentLifeCycleHookType.Undefined ? this.GetNextLifeCycleHook(this.GetNextLifeCycleHookType(hookType)) : lifeCycleHookBase;
    }

    private DeploymentLifeCycleHookType GetNextLifeCycleHookType(
      DeploymentLifeCycleHookType hookType)
    {
      switch (hookType)
      {
        case DeploymentLifeCycleHookType.Deploy:
          return DeploymentLifeCycleHookType.RouteTraffic;
        case DeploymentLifeCycleHookType.PreDeploy:
          return DeploymentLifeCycleHookType.Deploy;
        case DeploymentLifeCycleHookType.RouteTraffic:
          return DeploymentLifeCycleHookType.PostRouteTraffic;
        case DeploymentLifeCycleHookType.PostRouteTraffic:
          return DeploymentLifeCycleHookType.OnSuccess;
        case DeploymentLifeCycleHookType.OnSuccess:
        case DeploymentLifeCycleHookType.OnFailure:
          return DeploymentLifeCycleHookType.Undefined;
        default:
          return DeploymentLifeCycleHookType.Undefined;
      }
    }

    private bool CanExecuteHooks() => this.GetNextLifeCycleHook(this.m_nextHookType) != null;
  }
}

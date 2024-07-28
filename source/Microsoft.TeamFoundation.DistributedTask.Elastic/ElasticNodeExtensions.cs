// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticNodeExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  public static class ElasticNodeExtensions
  {
    public static void TransitionState(
      this ElasticNode node,
      ElasticNodeState state,
      StringBuilder sb = null,
      string message = null,
      bool forceIdle = false)
    {
      if (node.State == state && (!forceIdle || state != ElasticNodeState.Idle))
      {
        node.HasStateChanged = false;
      }
      else
      {
        sb?.AppendLine(string.Format("{0} ({1}), ({2}), {3} -> {4}. {5}", (object) node.Name, (object) node.ComputeChangeLog, (object) node.AgentChangeLog, (object) node.State, (object) state, (object) message));
        node.State = state;
        node.HasStateChanged = true;
        node.StateChangedOn = DateTime.UtcNow;
      }
    }

    public static async Task TransitionStateAndLogAsync(
      this ElasticNode node,
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      ElasticNodeState state,
      StringBuilder sb,
      string message)
    {
      await requestContext.GetService<IElasticPoolLogService>().AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Warning, OperationType.SizingJob, message);
      node.TransitionState(state, sb);
    }

    public static async Task TransitionOrSaveAsync(
      this ElasticNode node,
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      ElasticNodeState state,
      StringBuilder sb,
      bool isNodeUnhealthy,
      string message)
    {
      if (isNodeUnhealthy)
      {
        IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Warning, OperationType.SizingJob, message);
        if (elasticPool.SavedNodeCount < elasticPool.MaxSavedNodeCount)
        {
          state = ElasticNodeState.Saved;
          ++elasticPool.SavedNodeCount;
          await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Info, OperationType.SizingJob, ElasticResources.SavedNode((object) node.Name));
        }
        els = (IElasticPoolLogService) null;
      }
      node.TransitionState(state, sb);
    }
  }
}

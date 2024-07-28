// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Sharding.ShardingStrategyBuilder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Sharding
{
  internal static class ShardingStrategyBuilder
  {
    public static IActivityShardLocator Build(
      IVssRequestContext requestContext,
      string hubName,
      int dispatcherShardsCount,
      int serverDispatcherShardsCount,
      ICollection<PipelineShardOverride> shardOverrides)
    {
      IList<IActivityShardLocator> activityShardLocators = (IList<IActivityShardLocator>) new List<IActivityShardLocator>()
      {
        (IActivityShardLocator) new CompatShardLocator(dispatcherShardsCount)
      };
      if (shardOverrides != null && shardOverrides.Count > 0)
        activityShardLocators.Add((IActivityShardLocator) new ThrottledPipelinesShardLocator(shardOverrides));
      if (dispatcherShardsCount <= 1)
      {
        requestContext.Trace(0, TraceLevel.Info, "Orchestration", nameof (ShardingStrategyBuilder), "Sharding not enabled for the Hub: " + hubName);
        return (IActivityShardLocator) new CompositeActivityShardLocator(activityShardLocators);
      }
      if (dispatcherShardsCount != serverDispatcherShardsCount)
        activityShardLocators.Add((IActivityShardLocator) new ServerTaskShardLocatorRandom(serverDispatcherShardsCount));
      activityShardLocators.Add((IActivityShardLocator) new ActivityShardLocatorRandom(dispatcherShardsCount));
      requestContext.Trace(0, TraceLevel.Info, "Orchestration", nameof (ShardingStrategyBuilder), string.Format("Found {0} sharding strategies for the Hub: {1}", (object) activityShardLocators.Count, (object) hubName));
      return (IActivityShardLocator) new CompositeActivityShardLocator(activityShardLocators);
    }
  }
}

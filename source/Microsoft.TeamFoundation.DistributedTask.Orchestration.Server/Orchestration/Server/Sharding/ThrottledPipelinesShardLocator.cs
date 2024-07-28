// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Sharding.ThrottledPipelinesShardLocator
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Sharding
{
  internal class ThrottledPipelinesShardLocator : IActivityShardLocator
  {
    private readonly 
    #nullable disable
    IDictionary<string, PipelineShardOverride> m_shardOverridesLookup;

    public ThrottledPipelinesShardLocator(ICollection<PipelineShardOverride> shardOverrides) => this.m_shardOverridesLookup = (IDictionary<string, PipelineShardOverride>) shardOverrides.ToImmutableDictionary<PipelineShardOverride, string, PipelineShardOverride>((Func<PipelineShardOverride, string>) (x => x.GetIdentifier()), (Func<PipelineShardOverride, PipelineShardOverride>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public bool TryGetShardValue(
      IActivityShardKey key,
      out string shardValue,
      string dispatcherType = null)
    {
      PipelineShardOverride pipelineShardOverride;
      if (this.m_shardOverridesLookup.TryGetValue(key.GetGroupKey(), out pipelineShardOverride))
      {
        shardValue = (string.IsNullOrEmpty(dispatcherType) ? (object) "" : (object) (dispatcherType + " ")).ToString() + (object) pipelineShardOverride.ShardId;
        return true;
      }
      shardValue = (string) null;
      return false;
    }
  }
}

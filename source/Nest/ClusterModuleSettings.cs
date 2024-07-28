// Decompiled with JetBrains decompiler
// Type: Nest.ClusterModuleSettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class ClusterModuleSettings : IClusterModuleSettings
  {
    public IAllocationAwarenessSettings AllocationAwareness { get; set; }

    public IAllocationFilteringSettings AllocationFiltering { get; set; }

    public IDiskBasedShardAllocationSettings DiskBasedShardAllocation { get; set; }

    public IDictionary<string, LogLevel> Logger { get; set; }

    public bool? ReadOnly { get; set; }

    public IShardAllocationSettings ShardAllocation { get; set; }

    public IShardBalancingHeuristicsSettings ShardBalancingHeuristics { get; set; }

    public IShardRebalancingSettings ShardRebalancing { get; set; }
  }
}

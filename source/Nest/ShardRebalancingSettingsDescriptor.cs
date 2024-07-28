// Decompiled with JetBrains decompiler
// Type: Nest.ShardRebalancingSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ShardRebalancingSettingsDescriptor : 
    DescriptorBase<ShardRebalancingSettingsDescriptor, IShardRebalancingSettings>,
    IShardRebalancingSettings
  {
    Nest.AllowRebalance? IShardRebalancingSettings.AllowRebalance { get; set; }

    int? IShardRebalancingSettings.ConcurrentRebalance { get; set; }

    Nest.RebalanceEnable? IShardRebalancingSettings.RebalanceEnable { get; set; }

    public ShardRebalancingSettingsDescriptor RebalanceEnable(Nest.RebalanceEnable? enable) => this.Assign<Nest.RebalanceEnable?>(enable, (Action<IShardRebalancingSettings, Nest.RebalanceEnable?>) ((a, v) => a.RebalanceEnable = v));

    public ShardRebalancingSettingsDescriptor AllowRebalance(Nest.AllowRebalance? enable) => this.Assign<Nest.AllowRebalance?>(enable, (Action<IShardRebalancingSettings, Nest.AllowRebalance?>) ((a, v) => a.AllowRebalance = v));

    public ShardRebalancingSettingsDescriptor ConcurrentRebalance(int? concurrent) => this.Assign<int?>(concurrent, (Action<IShardRebalancingSettings, int?>) ((a, v) => a.ConcurrentRebalance = v));
  }
}

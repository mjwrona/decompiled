// Decompiled with JetBrains decompiler
// Type: Nest.ShardBalancingHeuristicsSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ShardBalancingHeuristicsSettingsDescriptor : 
    DescriptorBase<ShardBalancingHeuristicsSettingsDescriptor, IShardBalancingHeuristicsSettings>,
    IShardBalancingHeuristicsSettings
  {
    float? IShardBalancingHeuristicsSettings.BalanceIndex { get; set; }

    float? IShardBalancingHeuristicsSettings.BalanceShard { get; set; }

    float? IShardBalancingHeuristicsSettings.BalanceThreshold { get; set; }

    public ShardBalancingHeuristicsSettingsDescriptor BalanceShard(float? balance) => this.Assign<float?>(balance, (Action<IShardBalancingHeuristicsSettings, float?>) ((a, v) => a.BalanceShard = v));

    public ShardBalancingHeuristicsSettingsDescriptor BalanceIndex(float? balance) => this.Assign<float?>(balance, (Action<IShardBalancingHeuristicsSettings, float?>) ((a, v) => a.BalanceIndex = v));

    public ShardBalancingHeuristicsSettingsDescriptor BalanceThreshold(float? balance) => this.Assign<float?>(balance, (Action<IShardBalancingHeuristicsSettings, float?>) ((a, v) => a.BalanceThreshold = v));
  }
}

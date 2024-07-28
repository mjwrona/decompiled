// Decompiled with JetBrains decompiler
// Type: Nest.ClusterModuleSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ClusterModuleSettingsDescriptor : 
    DescriptorBase<ClusterModuleSettingsDescriptor, IClusterModuleSettings>,
    IClusterModuleSettings
  {
    IAllocationAwarenessSettings IClusterModuleSettings.AllocationAwareness { get; set; }

    IAllocationFilteringSettings IClusterModuleSettings.AllocationFiltering { get; set; }

    IDiskBasedShardAllocationSettings IClusterModuleSettings.DiskBasedShardAllocation { get; set; }

    IDictionary<string, LogLevel> IClusterModuleSettings.Logger { get; set; }

    bool? IClusterModuleSettings.ReadOnly { get; set; }

    IShardAllocationSettings IClusterModuleSettings.ShardAllocation { get; set; }

    IShardBalancingHeuristicsSettings IClusterModuleSettings.ShardBalancingHeuristics { get; set; }

    IShardRebalancingSettings IClusterModuleSettings.ShardRebalancing { get; set; }

    public ClusterModuleSettingsDescriptor ShardRebalancing(bool? readOnly = true) => this.Assign<bool?>(readOnly, (Action<IClusterModuleSettings, bool?>) ((a, v) => a.ReadOnly = v));

    public ClusterModuleSettingsDescriptor Logger(IDictionary<string, LogLevel> logger) => this.Assign<IDictionary<string, LogLevel>>(logger, (Action<IClusterModuleSettings, IDictionary<string, LogLevel>>) ((a, v) => a.Logger = v));

    public ClusterModuleSettingsDescriptor Logger(
      Func<FluentDictionary<string, LogLevel>, FluentDictionary<string, LogLevel>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, LogLevel>, FluentDictionary<string, LogLevel>>>(selector, (Action<IClusterModuleSettings, Func<FluentDictionary<string, LogLevel>, FluentDictionary<string, LogLevel>>>) ((a, v) => a.Logger = v != null ? (IDictionary<string, LogLevel>) v(new FluentDictionary<string, LogLevel>()) : (IDictionary<string, LogLevel>) null));
    }

    public ClusterModuleSettingsDescriptor AllocationAwareness(
      Func<AllocationAwarenessSettings, IAllocationAwarenessSettings> selector)
    {
      return this.Assign<Func<AllocationAwarenessSettings, IAllocationAwarenessSettings>>(selector, (Action<IClusterModuleSettings, Func<AllocationAwarenessSettings, IAllocationAwarenessSettings>>) ((a, v) => a.AllocationAwareness = v != null ? v(new AllocationAwarenessSettings()) : (IAllocationAwarenessSettings) null));
    }

    public ClusterModuleSettingsDescriptor AllocationFiltering(
      Func<AllocationFilteringSettingsDescriptor, IAllocationFilteringSettings> selector)
    {
      return this.Assign<Func<AllocationFilteringSettingsDescriptor, IAllocationFilteringSettings>>(selector, (Action<IClusterModuleSettings, Func<AllocationFilteringSettingsDescriptor, IAllocationFilteringSettings>>) ((a, v) => a.AllocationFiltering = v != null ? v(new AllocationFilteringSettingsDescriptor()) : (IAllocationFilteringSettings) null));
    }

    public ClusterModuleSettingsDescriptor DiskBasedShardAllocation(
      Func<DiskBasedShardAllocationSettingsDescriptor, IDiskBasedShardAllocationSettings> selector)
    {
      return this.Assign<Func<DiskBasedShardAllocationSettingsDescriptor, IDiskBasedShardAllocationSettings>>(selector, (Action<IClusterModuleSettings, Func<DiskBasedShardAllocationSettingsDescriptor, IDiskBasedShardAllocationSettings>>) ((a, v) => a.DiskBasedShardAllocation = v != null ? v(new DiskBasedShardAllocationSettingsDescriptor()) : (IDiskBasedShardAllocationSettings) null));
    }

    public ClusterModuleSettingsDescriptor ShardAllocation(
      Func<ShardAllocationSettingsDescriptor, IShardAllocationSettings> selector)
    {
      return this.Assign<Func<ShardAllocationSettingsDescriptor, IShardAllocationSettings>>(selector, (Action<IClusterModuleSettings, Func<ShardAllocationSettingsDescriptor, IShardAllocationSettings>>) ((a, v) => a.ShardAllocation = v != null ? v(new ShardAllocationSettingsDescriptor()) : (IShardAllocationSettings) null));
    }

    public ClusterModuleSettingsDescriptor ShardBalancingHeuristics(
      Func<ShardBalancingHeuristicsSettingsDescriptor, IShardBalancingHeuristicsSettings> selector)
    {
      return this.Assign<Func<ShardBalancingHeuristicsSettingsDescriptor, IShardBalancingHeuristicsSettings>>(selector, (Action<IClusterModuleSettings, Func<ShardBalancingHeuristicsSettingsDescriptor, IShardBalancingHeuristicsSettings>>) ((a, v) => a.ShardBalancingHeuristics = v != null ? v(new ShardBalancingHeuristicsSettingsDescriptor()) : (IShardBalancingHeuristicsSettings) null));
    }

    public ClusterModuleSettingsDescriptor ShardRebalancing(
      Func<ShardRebalancingSettingsDescriptor, IShardRebalancingSettings> selector)
    {
      return this.Assign<Func<ShardRebalancingSettingsDescriptor, IShardRebalancingSettings>>(selector, (Action<IClusterModuleSettings, Func<ShardRebalancingSettingsDescriptor, IShardRebalancingSettings>>) ((a, v) => a.ShardRebalancing = v != null ? v(new ShardRebalancingSettingsDescriptor()) : (IShardRebalancingSettings) null));
    }
  }
}

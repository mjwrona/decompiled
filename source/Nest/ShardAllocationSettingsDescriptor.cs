// Decompiled with JetBrains decompiler
// Type: Nest.ShardAllocationSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ShardAllocationSettingsDescriptor : 
    DescriptorBase<ShardAllocationSettingsDescriptor, IShardAllocationSettings>,
    IShardAllocationSettings
  {
    Nest.AllocationEnable? IShardAllocationSettings.AllocationEnable { get; set; }

    int? IShardAllocationSettings.NodeConcurrentRecoveries { get; set; }

    int? IShardAllocationSettings.NodeInitialPrimariesRecoveries { get; set; }

    bool? IShardAllocationSettings.SameShardHost { get; set; }

    public ShardAllocationSettingsDescriptor AllocationEnable(Nest.AllocationEnable? enable) => this.Assign<Nest.AllocationEnable?>(enable, (Action<IShardAllocationSettings, Nest.AllocationEnable?>) ((a, v) => a.AllocationEnable = v));

    public ShardAllocationSettingsDescriptor NodeConcurrentRecoveries(int? concurrent) => this.Assign<int?>(concurrent, (Action<IShardAllocationSettings, int?>) ((a, v) => a.NodeConcurrentRecoveries = v));

    public ShardAllocationSettingsDescriptor NodeInitialPrimariesRecoveries(int? initial) => this.Assign<int?>(initial, (Action<IShardAllocationSettings, int?>) ((a, v) => a.NodeInitialPrimariesRecoveries = v));

    public ShardAllocationSettingsDescriptor SameShardHost(bool? same = true) => this.Assign<bool?>(same, (Action<IShardAllocationSettings, bool?>) ((a, v) => a.SameShardHost = v));
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.DiskBasedShardAllocationSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DiskBasedShardAllocationSettingsDescriptor : 
    DescriptorBase<DiskBasedShardAllocationSettingsDescriptor, IDiskBasedShardAllocationSettings>,
    IDiskBasedShardAllocationSettings
  {
    string IDiskBasedShardAllocationSettings.HighWatermark { get; set; }

    bool? IDiskBasedShardAllocationSettings.IncludeRelocations { get; set; }

    string IDiskBasedShardAllocationSettings.LowWatermark { get; set; }

    bool? IDiskBasedShardAllocationSettings.ThresholdEnabled { get; set; }

    Time IDiskBasedShardAllocationSettings.UpdateInterval { get; set; }

    public DiskBasedShardAllocationSettingsDescriptor ThresholdEnabled(bool? enable = true) => this.Assign<bool?>(enable, (Action<IDiskBasedShardAllocationSettings, bool?>) ((a, v) => a.ThresholdEnabled = v));

    public DiskBasedShardAllocationSettingsDescriptor LowWatermark(string low) => this.Assign<string>(low, (Action<IDiskBasedShardAllocationSettings, string>) ((a, v) => a.LowWatermark = v));

    public DiskBasedShardAllocationSettingsDescriptor HighWatermark(string high) => this.Assign<string>(high, (Action<IDiskBasedShardAllocationSettings, string>) ((a, v) => a.HighWatermark = v));

    public DiskBasedShardAllocationSettingsDescriptor UpdateInterval(Time time) => this.Assign<Time>(time, (Action<IDiskBasedShardAllocationSettings, Time>) ((a, v) => a.UpdateInterval = v));

    public DiskBasedShardAllocationSettingsDescriptor IncludeRelocations(bool? include = true) => this.Assign<bool?>(include, (Action<IDiskBasedShardAllocationSettings, bool?>) ((a, v) => a.IncludeRelocations = v));
  }
}

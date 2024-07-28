// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotRetentionConfigurationDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SnapshotRetentionConfigurationDescriptor : 
    DescriptorBase<SnapshotRetentionConfigurationDescriptor, ISnapshotRetentionConfiguration>,
    ISnapshotRetentionConfiguration
  {
    Time ISnapshotRetentionConfiguration.ExpireAfter { get; set; }

    int? ISnapshotRetentionConfiguration.MinimumCount { get; set; }

    int? ISnapshotRetentionConfiguration.MaximumCount { get; set; }

    public SnapshotRetentionConfigurationDescriptor ExpireAfter(Time time) => this.Assign<Time>(time, (Action<ISnapshotRetentionConfiguration, Time>) ((a, v) => a.ExpireAfter = v));

    public SnapshotRetentionConfigurationDescriptor MinimumCount(int? count) => this.Assign<int?>(count, (Action<ISnapshotRetentionConfiguration, int?>) ((a, v) => a.MinimumCount = v));

    public SnapshotRetentionConfigurationDescriptor MaximumCount(int? count) => this.Assign<int?>(count, (Action<ISnapshotRetentionConfiguration, int?>) ((a, v) => a.MaximumCount = v));
  }
}

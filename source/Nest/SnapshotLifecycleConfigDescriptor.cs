// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotLifecycleConfigDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SnapshotLifecycleConfigDescriptor : 
    DescriptorBase<SnapshotLifecycleConfigDescriptor, ISnapshotLifecycleConfig>,
    ISnapshotLifecycleConfig
  {
    bool? ISnapshotLifecycleConfig.IgnoreUnavailable { get; set; }

    bool? ISnapshotLifecycleConfig.IncludeGlobalState { get; set; }

    Nest.Indices ISnapshotLifecycleConfig.Indices { get; set; }

    public SnapshotLifecycleConfigDescriptor Indices(IndexName index) => this.Indices((Nest.Indices) index);

    public SnapshotLifecycleConfigDescriptor Indices<T>() where T : class => this.Indices((IndexName) typeof (T));

    public SnapshotLifecycleConfigDescriptor Indices(Nest.Indices indices) => this.Assign<Nest.Indices>(indices, (Action<ISnapshotLifecycleConfig, Nest.Indices>) ((a, v) => a.Indices = v));

    public SnapshotLifecycleConfigDescriptor IgnoreUnavailable(bool? ignoreUnavailable = true) => this.Assign<bool?>(ignoreUnavailable, (Action<ISnapshotLifecycleConfig, bool?>) ((a, v) => a.IgnoreUnavailable = v));

    public SnapshotLifecycleConfigDescriptor IncludeGlobalState(bool? includeGlobalState = true) => this.Assign<bool?>(includeGlobalState, (Action<ISnapshotLifecycleConfig, bool?>) ((a, v) => a.IncludeGlobalState = v));
  }
}

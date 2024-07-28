// Decompiled with JetBrains decompiler
// Type: Nest.MergeSchedulerSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MergeSchedulerSettingsDescriptor : 
    DescriptorBase<MergeSchedulerSettingsDescriptor, IMergeSchedulerSettings>,
    IMergeSchedulerSettings
  {
    bool? IMergeSchedulerSettings.AutoThrottle { get; set; }

    int? IMergeSchedulerSettings.MaxThreadCount { get; set; }

    public MergeSchedulerSettingsDescriptor AutoThrottle(bool? throttle = true) => this.Assign<bool?>(throttle, (Action<IMergeSchedulerSettings, bool?>) ((a, v) => a.AutoThrottle = v));

    public MergeSchedulerSettingsDescriptor MaxThreadCount(int? maxThreadCount) => this.Assign<int?>(maxThreadCount, (Action<IMergeSchedulerSettings, int?>) ((a, v) => a.MaxThreadCount = v));
  }
}

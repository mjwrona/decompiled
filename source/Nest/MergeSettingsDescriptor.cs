// Decompiled with JetBrains decompiler
// Type: Nest.MergeSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MergeSettingsDescriptor : 
    DescriptorBase<MergeSettingsDescriptor, IMergeSettings>,
    IMergeSettings
  {
    IMergePolicySettings IMergeSettings.Policy { get; set; }

    IMergeSchedulerSettings IMergeSettings.Scheduler { get; set; }

    public MergeSettingsDescriptor Policy(
      Func<MergePolicySettingsDescriptor, IMergePolicySettings> selector)
    {
      return this.Assign<Func<MergePolicySettingsDescriptor, IMergePolicySettings>>(selector, (Action<IMergeSettings, Func<MergePolicySettingsDescriptor, IMergePolicySettings>>) ((a, v) => a.Policy = v != null ? v(new MergePolicySettingsDescriptor()) : (IMergePolicySettings) null));
    }

    public MergeSettingsDescriptor Scheduler(
      Func<MergeSchedulerSettingsDescriptor, IMergeSchedulerSettings> selector)
    {
      return this.Assign<Func<MergeSchedulerSettingsDescriptor, IMergeSchedulerSettings>>(selector, (Action<IMergeSettings, Func<MergeSchedulerSettingsDescriptor, IMergeSchedulerSettings>>) ((a, v) => a.Scheduler = v != null ? v(new MergeSchedulerSettingsDescriptor()) : (IMergeSchedulerSettings) null));
    }
  }
}

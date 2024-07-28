// Decompiled with JetBrains decompiler
// Type: Nest.AllocationAwarenessSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class AllocationAwarenessSettingsDescriptor : 
    DescriptorBase<AllocationAwarenessSettingsDescriptor, IAllocationAwarenessSettings>,
    IAllocationAwarenessSettings
  {
    IEnumerable<string> IAllocationAwarenessSettings.Attributes { get; set; }

    IAllocationAttributes IAllocationAwarenessSettings.Forced { get; set; }

    public AllocationAwarenessSettingsDescriptor Attributes(IEnumerable<string> attributes) => this.Assign<IEnumerable<string>>(attributes, (Action<IAllocationAwarenessSettings, IEnumerable<string>>) ((a, v) => a.Attributes = v));

    public AllocationAwarenessSettingsDescriptor Attributes(params string[] attributes) => this.Assign<string[]>(attributes, (Action<IAllocationAwarenessSettings, string[]>) ((a, v) => a.Attributes = (IEnumerable<string>) v));

    public AllocationAwarenessSettingsDescriptor Force(IAllocationAttributes forceValues) => this.Assign<IAllocationAttributes>(forceValues, (Action<IAllocationAwarenessSettings, IAllocationAttributes>) ((a, v) => a.Forced = v));

    public AllocationAwarenessSettingsDescriptor Force(
      Func<AllocationAttributesDescriptor, IAllocationAttributes> selector)
    {
      return this.Assign<Func<AllocationAttributesDescriptor, IAllocationAttributes>>(selector, (Action<IAllocationAwarenessSettings, Func<AllocationAttributesDescriptor, IAllocationAttributes>>) ((a, v) => a.Forced = v != null ? v(new AllocationAttributesDescriptor()) : (IAllocationAttributes) null));
    }
  }
}

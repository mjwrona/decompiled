// Decompiled with JetBrains decompiler
// Type: Nest.AllocationFilteringSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AllocationFilteringSettingsDescriptor : 
    DescriptorBase<AllocationFilteringSettingsDescriptor, IAllocationFilteringSettings>,
    IAllocationFilteringSettings
  {
    IAllocationAttributes IAllocationFilteringSettings.Exclude { get; set; }

    IAllocationAttributes IAllocationFilteringSettings.Include { get; set; }

    IAllocationAttributes IAllocationFilteringSettings.Require { get; set; }

    public AllocationFilteringSettingsDescriptor Include(IAllocationAttributes include) => this.Assign<IAllocationAttributes>(include, (Action<IAllocationFilteringSettings, IAllocationAttributes>) ((a, v) => a.Include = v));

    public AllocationFilteringSettingsDescriptor Include(
      Func<AllocationAttributesDescriptor, IAllocationAttributes> selector)
    {
      return this.Assign<Func<AllocationAttributesDescriptor, IAllocationAttributes>>(selector, (Action<IAllocationFilteringSettings, Func<AllocationAttributesDescriptor, IAllocationAttributes>>) ((a, v) => a.Include = v != null ? v(new AllocationAttributesDescriptor()) : (IAllocationAttributes) null));
    }

    public AllocationFilteringSettingsDescriptor Exlude(IAllocationAttributes include) => this.Assign<IAllocationAttributes>(include, (Action<IAllocationFilteringSettings, IAllocationAttributes>) ((a, v) => a.Exclude = v));

    public AllocationFilteringSettingsDescriptor Exclude(
      Func<AllocationAttributesDescriptor, IAllocationAttributes> selector)
    {
      return this.Assign<Func<AllocationAttributesDescriptor, IAllocationAttributes>>(selector, (Action<IAllocationFilteringSettings, Func<AllocationAttributesDescriptor, IAllocationAttributes>>) ((a, v) => a.Exclude = v != null ? v(new AllocationAttributesDescriptor()) : (IAllocationAttributes) null));
    }

    public AllocationFilteringSettingsDescriptor Require(IAllocationAttributes include) => this.Assign<IAllocationAttributes>(include, (Action<IAllocationFilteringSettings, IAllocationAttributes>) ((a, v) => a.Require = v));

    public AllocationFilteringSettingsDescriptor Require(
      Func<AllocationAttributesDescriptor, IAllocationAttributes> selector)
    {
      return this.Assign<Func<AllocationAttributesDescriptor, IAllocationAttributes>>(selector, (Action<IAllocationFilteringSettings, Func<AllocationAttributesDescriptor, IAllocationAttributes>>) ((a, v) => a.Require = v != null ? v(new AllocationAttributesDescriptor()) : (IAllocationAttributes) null));
    }
  }
}

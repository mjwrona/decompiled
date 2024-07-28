// Decompiled with JetBrains decompiler
// Type: Nest.FielddataFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class FielddataFilterDescriptor : 
    DescriptorBase<FielddataFilterDescriptor, IFielddataFilter>,
    IFielddataFilter
  {
    IFielddataFrequencyFilter IFielddataFilter.Frequency { get; set; }

    IFielddataRegexFilter IFielddataFilter.Regex { get; set; }

    public FielddataFilterDescriptor Frequency(
      Func<FielddataFrequencyFilterDescriptor, IFielddataFrequencyFilter> frequencyFilterSelector)
    {
      return this.Assign<IFielddataFrequencyFilter>(frequencyFilterSelector(new FielddataFrequencyFilterDescriptor()), (Action<IFielddataFilter, IFielddataFrequencyFilter>) ((a, v) => a.Frequency = v));
    }

    public FielddataFilterDescriptor Regex(
      Func<FielddataRegexFilterDescriptor, IFielddataRegexFilter> regexFilterSelector)
    {
      return this.Assign<IFielddataRegexFilter>(regexFilterSelector(new FielddataRegexFilterDescriptor()), (Action<IFielddataFilter, IFielddataRegexFilter>) ((a, v) => a.Regex = v));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.FielddataFrequencyFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class FielddataFrequencyFilterDescriptor : 
    DescriptorBase<FielddataFrequencyFilterDescriptor, IFielddataFrequencyFilter>,
    IFielddataFrequencyFilter
  {
    double? IFielddataFrequencyFilter.Max { get; set; }

    double? IFielddataFrequencyFilter.Min { get; set; }

    int? IFielddataFrequencyFilter.MinSegmentSize { get; set; }

    public FielddataFrequencyFilterDescriptor Min(double? min) => this.Assign<double?>(min, (Action<IFielddataFrequencyFilter, double?>) ((a, v) => a.Min = v));

    public FielddataFrequencyFilterDescriptor Max(double? max) => this.Assign<double?>(max, (Action<IFielddataFrequencyFilter, double?>) ((a, v) => a.Max = v));

    public FielddataFrequencyFilterDescriptor MinSegmentSize(int? minSegmentSize) => this.Assign<int?>(minSegmentSize, (Action<IFielddataFrequencyFilter, int?>) ((a, v) => a.MinSegmentSize = v));
  }
}

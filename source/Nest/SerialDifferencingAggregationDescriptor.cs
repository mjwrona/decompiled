// Decompiled with JetBrains decompiler
// Type: Nest.SerialDifferencingAggregationDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SerialDifferencingAggregationDescriptor : 
    PipelineAggregationDescriptorBase<SerialDifferencingAggregationDescriptor, ISerialDifferencingAggregation, SingleBucketsPath>,
    ISerialDifferencingAggregation,
    IPipelineAggregation,
    IAggregation
  {
    int? ISerialDifferencingAggregation.Lag { get; set; }

    public SerialDifferencingAggregationDescriptor Lag(int? lag) => this.Assign<int?>(lag, (Action<ISerialDifferencingAggregation, int?>) ((a, v) => a.Lag = v));
  }
}

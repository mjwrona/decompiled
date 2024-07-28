// Decompiled with JetBrains decompiler
// Type: Nest.MovingPercentilesAggregationDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MovingPercentilesAggregationDescriptor : 
    PipelineAggregationDescriptorBase<MovingPercentilesAggregationDescriptor, IMovingPercentilesAggregation, SingleBucketsPath>,
    IMovingPercentilesAggregation,
    IPipelineAggregation,
    IAggregation
  {
    int? IMovingPercentilesAggregation.Window { get; set; }

    int? IMovingPercentilesAggregation.Shift { get; set; }

    public MovingPercentilesAggregationDescriptor Window(int? windowSize) => this.Assign<int?>(windowSize, (Action<IMovingPercentilesAggregation, int?>) ((a, v) => a.Window = v));

    public MovingPercentilesAggregationDescriptor Shift(int? shift) => this.Assign<int?>(shift, (Action<IMovingPercentilesAggregation, int?>) ((a, v) => a.Shift = v));
  }
}

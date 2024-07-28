// Decompiled with JetBrains decompiler
// Type: Nest.MovingFunctionAggregationDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MovingFunctionAggregationDescriptor : 
    PipelineAggregationDescriptorBase<MovingFunctionAggregationDescriptor, IMovingFunctionAggregation, SingleBucketsPath>,
    IMovingFunctionAggregation,
    IPipelineAggregation,
    IAggregation
  {
    string IMovingFunctionAggregation.Script { get; set; }

    int? IMovingFunctionAggregation.Window { get; set; }

    int? IMovingFunctionAggregation.Shift { get; set; }

    public MovingFunctionAggregationDescriptor Window(int? windowSize) => this.Assign<int?>(windowSize, (Action<IMovingFunctionAggregation, int?>) ((a, v) => a.Window = v));

    public MovingFunctionAggregationDescriptor Script(string script) => this.Assign<string>(script, (Action<IMovingFunctionAggregation, string>) ((a, v) => a.Script = v));

    public MovingFunctionAggregationDescriptor Shift(int? shift) => this.Assign<int?>(shift, (Action<IMovingFunctionAggregation, int?>) ((a, v) => a.Shift = v));
  }
}

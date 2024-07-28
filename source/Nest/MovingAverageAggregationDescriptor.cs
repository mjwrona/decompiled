// Decompiled with JetBrains decompiler
// Type: Nest.MovingAverageAggregationDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MovingAverageAggregationDescriptor : 
    PipelineAggregationDescriptorBase<MovingAverageAggregationDescriptor, IMovingAverageAggregation, SingleBucketsPath>,
    IMovingAverageAggregation,
    IPipelineAggregation,
    IAggregation
  {
    bool? IMovingAverageAggregation.Minimize { get; set; }

    IMovingAverageModel IMovingAverageAggregation.Model { get; set; }

    int? IMovingAverageAggregation.Predict { get; set; }

    int? IMovingAverageAggregation.Window { get; set; }

    public MovingAverageAggregationDescriptor Minimize(bool? minimize = true) => this.Assign<bool?>(minimize, (Action<IMovingAverageAggregation, bool?>) ((a, v) => a.Minimize = v));

    public MovingAverageAggregationDescriptor Window(int? window) => this.Assign<int?>(window, (Action<IMovingAverageAggregation, int?>) ((a, v) => a.Window = v));

    public MovingAverageAggregationDescriptor Predict(int? predict) => this.Assign<int?>(predict, (Action<IMovingAverageAggregation, int?>) ((a, v) => a.Predict = v));

    public MovingAverageAggregationDescriptor Model(
      Func<MovingAverageModelDescriptor, IMovingAverageModel> modelSelector)
    {
      return this.Assign<Func<MovingAverageModelDescriptor, IMovingAverageModel>>(modelSelector, (Action<IMovingAverageAggregation, Func<MovingAverageModelDescriptor, IMovingAverageModel>>) ((a, v) => a.Model = v != null ? v(new MovingAverageModelDescriptor()) : (IMovingAverageModel) null));
    }
  }
}

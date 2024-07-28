// Decompiled with JetBrains decompiler
// Type: Nest.CircleProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class CircleProcessorDescriptor<T> : 
    ProcessorDescriptorBase<CircleProcessorDescriptor<T>, ICircleProcessor>,
    ICircleProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "circle";

    Nest.Field ICircleProcessor.Field { get; set; }

    Nest.Field ICircleProcessor.TargetField { get; set; }

    bool? ICircleProcessor.IgnoreMissing { get; set; }

    double? ICircleProcessor.ErrorDistance { get; set; }

    Nest.ShapeType? ICircleProcessor.ShapeType { get; set; }

    public CircleProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ICircleProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public CircleProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ICircleProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public CircleProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ICircleProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public CircleProcessorDescriptor<T> TargetField<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ICircleProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public CircleProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<ICircleProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));

    public CircleProcessorDescriptor<T> ErrorDistance(double? errorDistance) => this.Assign<double?>(errorDistance, (Action<ICircleProcessor, double?>) ((a, v) => a.ErrorDistance = v));

    public CircleProcessorDescriptor<T> ShapeType(Nest.ShapeType? shapeType) => this.Assign<Nest.ShapeType?>(shapeType, (Action<ICircleProcessor, Nest.ShapeType?>) ((a, v) => a.ShapeType = v));
  }
}

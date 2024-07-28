// Decompiled with JetBrains decompiler
// Type: Nest.SumDetectorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class SumDetectorDescriptor<T> : 
    DetectorDescriptorBase<SumDetectorDescriptor<T>, ISumDetector>,
    ISumDetector,
    IDetector,
    IFieldNameDetector,
    IByFieldNameDetector,
    IOverFieldNameDetector,
    IPartitionFieldNameDetector
    where T : class
  {
    public SumDetectorDescriptor(SumFunction function)
      : base(function.GetStringValue())
    {
    }

    Field IByFieldNameDetector.ByFieldName { get; set; }

    Field IFieldNameDetector.FieldName { get; set; }

    Field IOverFieldNameDetector.OverFieldName { get; set; }

    Field IPartitionFieldNameDetector.PartitionFieldName { get; set; }

    public SumDetectorDescriptor<T> FieldName(Field fieldName) => this.Assign<Field>(fieldName, (Action<ISumDetector, Field>) ((a, v) => a.FieldName = v));

    public SumDetectorDescriptor<T> FieldName<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ISumDetector, Expression<Func<T, TValue>>>) ((a, v) => a.FieldName = (Field) (Expression) v));

    public SumDetectorDescriptor<T> ByFieldName(Field byFieldName) => this.Assign<Field>(byFieldName, (Action<ISumDetector, Field>) ((a, v) => a.ByFieldName = v));

    public SumDetectorDescriptor<T> ByFieldName<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ISumDetector, Expression<Func<T, TValue>>>) ((a, v) => a.ByFieldName = (Field) (Expression) v));

    public SumDetectorDescriptor<T> OverFieldName(Field overFieldName) => this.Assign<Field>(overFieldName, (Action<ISumDetector, Field>) ((a, v) => a.OverFieldName = v));

    public SumDetectorDescriptor<T> OverFieldName<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ISumDetector, Expression<Func<T, TValue>>>) ((a, v) => a.OverFieldName = (Field) (Expression) v));

    public SumDetectorDescriptor<T> PartitionFieldName(Field partitionFieldName) => this.Assign<Field>(partitionFieldName, (Action<ISumDetector, Field>) ((a, v) => a.PartitionFieldName = v));

    public SumDetectorDescriptor<T> PartitionFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ISumDetector, Expression<Func<T, TValue>>>) ((a, v) => a.PartitionFieldName = (Field) (Expression) v));
    }
  }
}

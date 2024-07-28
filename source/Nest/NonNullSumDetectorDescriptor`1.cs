// Decompiled with JetBrains decompiler
// Type: Nest.NonNullSumDetectorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class NonNullSumDetectorDescriptor<T> : 
    DetectorDescriptorBase<NonNullSumDetectorDescriptor<T>, INonNullSumDetector>,
    INonNullSumDetector,
    IDetector,
    IFieldNameDetector,
    IByFieldNameDetector,
    IPartitionFieldNameDetector
    where T : class
  {
    public NonNullSumDetectorDescriptor(NonNullSumFunction function)
      : base(function.GetStringValue())
    {
    }

    Field IByFieldNameDetector.ByFieldName { get; set; }

    Field IFieldNameDetector.FieldName { get; set; }

    Field IPartitionFieldNameDetector.PartitionFieldName { get; set; }

    public NonNullSumDetectorDescriptor<T> FieldName(Field fieldName) => this.Assign<Field>(fieldName, (Action<INonNullSumDetector, Field>) ((a, v) => a.FieldName = v));

    public NonNullSumDetectorDescriptor<T> FieldName<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INonNullSumDetector, Expression<Func<T, TValue>>>) ((a, v) => a.FieldName = (Field) (Expression) v));

    public NonNullSumDetectorDescriptor<T> ByFieldName(Field byFieldName) => this.Assign<Field>(byFieldName, (Action<INonNullSumDetector, Field>) ((a, v) => a.ByFieldName = v));

    public NonNullSumDetectorDescriptor<T> ByFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INonNullSumDetector, Expression<Func<T, TValue>>>) ((a, v) => a.ByFieldName = (Field) (Expression) v));
    }

    public NonNullSumDetectorDescriptor<T> PartitionFieldName(Field partitionFieldName) => this.Assign<Field>(partitionFieldName, (Action<INonNullSumDetector, Field>) ((a, v) => a.PartitionFieldName = v));

    public NonNullSumDetectorDescriptor<T> PartitionFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INonNullSumDetector, Expression<Func<T, TValue>>>) ((a, v) => a.PartitionFieldName = (Field) (Expression) v));
    }
  }
}

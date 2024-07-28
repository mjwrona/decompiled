// Decompiled with JetBrains decompiler
// Type: Nest.DistinctCountDetectorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class DistinctCountDetectorDescriptor<T> : 
    DetectorDescriptorBase<DistinctCountDetectorDescriptor<T>, IDistinctCountDetector>,
    IDistinctCountDetector,
    IDetector,
    IByFieldNameDetector,
    IOverFieldNameDetector,
    IPartitionFieldNameDetector,
    IFieldNameDetector
    where T : class
  {
    public DistinctCountDetectorDescriptor(DistinctCountFunction function)
      : base(function.GetStringValue())
    {
    }

    Field IByFieldNameDetector.ByFieldName { get; set; }

    Field IFieldNameDetector.FieldName { get; set; }

    Field IOverFieldNameDetector.OverFieldName { get; set; }

    Field IPartitionFieldNameDetector.PartitionFieldName { get; set; }

    public DistinctCountDetectorDescriptor<T> FieldName(Field fieldName) => this.Assign<Field>(fieldName, (Action<IDistinctCountDetector, Field>) ((a, v) => a.FieldName = v));

    public DistinctCountDetectorDescriptor<T> FieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IDistinctCountDetector, Expression<Func<T, TValue>>>) ((a, v) => a.FieldName = (Field) (Expression) v));
    }

    public DistinctCountDetectorDescriptor<T> ByFieldName(Field byFieldName) => this.Assign<Field>(byFieldName, (Action<IDistinctCountDetector, Field>) ((a, v) => a.ByFieldName = v));

    public DistinctCountDetectorDescriptor<T> ByFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IDistinctCountDetector, Expression<Func<T, TValue>>>) ((a, v) => a.ByFieldName = (Field) (Expression) v));
    }

    public DistinctCountDetectorDescriptor<T> OverFieldName(Field overFieldName) => this.Assign<Field>(overFieldName, (Action<IDistinctCountDetector, Field>) ((a, v) => a.OverFieldName = v));

    public DistinctCountDetectorDescriptor<T> OverFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IDistinctCountDetector, Expression<Func<T, TValue>>>) ((a, v) => a.OverFieldName = (Field) (Expression) v));
    }

    public DistinctCountDetectorDescriptor<T> PartitionFieldName(Field partitionFieldName) => this.Assign<Field>(partitionFieldName, (Action<IDistinctCountDetector, Field>) ((a, v) => a.PartitionFieldName = v));

    public DistinctCountDetectorDescriptor<T> PartitionFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IDistinctCountDetector, Expression<Func<T, TValue>>>) ((a, v) => a.PartitionFieldName = (Field) (Expression) v));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.NonZeroCountDetectorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class NonZeroCountDetectorDescriptor<T> : 
    DetectorDescriptorBase<NonZeroCountDetectorDescriptor<T>, INonZeroCountDetector>,
    INonZeroCountDetector,
    IDetector,
    IByFieldNameDetector,
    IPartitionFieldNameDetector
    where T : class
  {
    public NonZeroCountDetectorDescriptor(NonZeroCountFunction function)
      : base(function.GetStringValue())
    {
    }

    Field IByFieldNameDetector.ByFieldName { get; set; }

    Field IPartitionFieldNameDetector.PartitionFieldName { get; set; }

    public NonZeroCountDetectorDescriptor<T> ByFieldName(Field byFieldName) => this.Assign<Field>(byFieldName, (Action<INonZeroCountDetector, Field>) ((a, v) => a.ByFieldName = v));

    public NonZeroCountDetectorDescriptor<T> ByFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INonZeroCountDetector, Expression<Func<T, TValue>>>) ((a, v) => a.ByFieldName = (Field) (Expression) v));
    }

    public NonZeroCountDetectorDescriptor<T> PartitionFieldName(Field partitionFieldName) => this.Assign<Field>(partitionFieldName, (Action<INonZeroCountDetector, Field>) ((a, v) => a.PartitionFieldName = v));

    public NonZeroCountDetectorDescriptor<T> PartitionFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INonZeroCountDetector, Expression<Func<T, TValue>>>) ((a, v) => a.PartitionFieldName = (Field) (Expression) v));
    }
  }
}

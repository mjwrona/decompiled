// Decompiled with JetBrains decompiler
// Type: Nest.RareDetectorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class RareDetectorDescriptor<T> : 
    DetectorDescriptorBase<RareDetectorDescriptor<T>, IRareDetector>,
    IRareDetector,
    IDetector,
    IByFieldNameDetector,
    IOverFieldNameDetector,
    IPartitionFieldNameDetector
    where T : class
  {
    public RareDetectorDescriptor(RareFunction function)
      : base(function.GetStringValue())
    {
    }

    Field IByFieldNameDetector.ByFieldName { get; set; }

    Field IOverFieldNameDetector.OverFieldName { get; set; }

    Field IPartitionFieldNameDetector.PartitionFieldName { get; set; }

    public RareDetectorDescriptor<T> ByFieldName(Field byFieldName) => this.Assign<Field>(byFieldName, (Action<IRareDetector, Field>) ((a, v) => a.ByFieldName = v));

    public RareDetectorDescriptor<T> ByFieldName<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IRareDetector, Expression<Func<T, TValue>>>) ((a, v) => a.ByFieldName = (Field) (Expression) v));

    public RareDetectorDescriptor<T> OverFieldName(Field overFieldName) => this.Assign<Field>(overFieldName, (Action<IRareDetector, Field>) ((a, v) => a.OverFieldName = v));

    public RareDetectorDescriptor<T> OverFieldName<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IRareDetector, Expression<Func<T, TValue>>>) ((a, v) => a.OverFieldName = (Field) (Expression) v));

    public RareDetectorDescriptor<T> PartitionFieldName(Field partitionFieldName) => this.Assign<Field>(partitionFieldName, (Action<IRareDetector, Field>) ((a, v) => a.PartitionFieldName = v));

    public RareDetectorDescriptor<T> PartitionFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IRareDetector, Expression<Func<T, TValue>>>) ((a, v) => a.PartitionFieldName = (Field) (Expression) v));
    }
  }
}

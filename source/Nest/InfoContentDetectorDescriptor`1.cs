// Decompiled with JetBrains decompiler
// Type: Nest.InfoContentDetectorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class InfoContentDetectorDescriptor<T> : 
    DetectorDescriptorBase<InfoContentDetectorDescriptor<T>, IInfoContentDetector>,
    IInfoContentDetector,
    IDetector,
    IByFieldNameDetector,
    IOverFieldNameDetector,
    IPartitionFieldNameDetector,
    IFieldNameDetector
    where T : class
  {
    public InfoContentDetectorDescriptor(InfoContentFunction function)
      : base(function.GetStringValue())
    {
    }

    Field IByFieldNameDetector.ByFieldName { get; set; }

    Field IFieldNameDetector.FieldName { get; set; }

    Field IOverFieldNameDetector.OverFieldName { get; set; }

    Field IPartitionFieldNameDetector.PartitionFieldName { get; set; }

    public InfoContentDetectorDescriptor<T> FieldName(Field fieldName) => this.Assign<Field>(fieldName, (Action<IInfoContentDetector, Field>) ((a, v) => a.FieldName = v));

    public InfoContentDetectorDescriptor<T> FieldName<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IInfoContentDetector, Expression<Func<T, TValue>>>) ((a, v) => a.FieldName = (Field) (Expression) v));

    public InfoContentDetectorDescriptor<T> ByFieldName(Field byFieldName) => this.Assign<Field>(byFieldName, (Action<IInfoContentDetector, Field>) ((a, v) => a.ByFieldName = v));

    public InfoContentDetectorDescriptor<T> ByFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IInfoContentDetector, Expression<Func<T, TValue>>>) ((a, v) => a.ByFieldName = (Field) (Expression) v));
    }

    public InfoContentDetectorDescriptor<T> OverFieldName(Field overFieldName) => this.Assign<Field>(overFieldName, (Action<IInfoContentDetector, Field>) ((a, v) => a.OverFieldName = v));

    public InfoContentDetectorDescriptor<T> OverFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IInfoContentDetector, Expression<Func<T, TValue>>>) ((a, v) => a.OverFieldName = (Field) (Expression) v));
    }

    public InfoContentDetectorDescriptor<T> PartitionFieldName(Field partitionFieldName) => this.Assign<Field>(partitionFieldName, (Action<IInfoContentDetector, Field>) ((a, v) => a.PartitionFieldName = v));

    public InfoContentDetectorDescriptor<T> PartitionFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IInfoContentDetector, Expression<Func<T, TValue>>>) ((a, v) => a.PartitionFieldName = (Field) (Expression) v));
    }
  }
}

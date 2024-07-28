// Decompiled with JetBrains decompiler
// Type: Nest.TransformTimeSyncDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class TransformTimeSyncDescriptor<T> : 
    DescriptorBase<TransformTimeSyncDescriptor<T>, ITransformTimeSync>,
    ITransformTimeSync,
    ITransformSync
  {
    Nest.Field ITransformTimeSync.Field { get; set; }

    Time ITransformTimeSync.Delay { get; set; }

    public TransformTimeSyncDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ITransformTimeSync, Nest.Field>) ((a, v) => a.Field = v));

    public TransformTimeSyncDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ITransformTimeSync, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public TransformTimeSyncDescriptor<T> Delay(Time delay) => this.Assign<Time>(delay, (Action<ITransformTimeSync, Time>) ((a, v) => a.Delay = v));
  }
}

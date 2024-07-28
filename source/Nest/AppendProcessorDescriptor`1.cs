// Decompiled with JetBrains decompiler
// Type: Nest.AppendProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class AppendProcessorDescriptor<T> : 
    ProcessorDescriptorBase<AppendProcessorDescriptor<T>, IAppendProcessor>,
    IAppendProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "append";

    Nest.Field IAppendProcessor.Field { get; set; }

    IEnumerable<object> IAppendProcessor.Value { get; set; }

    bool? IAppendProcessor.AllowDuplicates { get; set; }

    public AppendProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IAppendProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public AppendProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IAppendProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public AppendProcessorDescriptor<T> Value<TValue>(IEnumerable<TValue> values) => this.Assign<IEnumerable<TValue>>(values, (Action<IAppendProcessor, IEnumerable<TValue>>) ((a, v) => a.Value = v != null ? v.Cast<object>() : (IEnumerable<object>) null));

    public AppendProcessorDescriptor<T> Value<TValue>(params TValue[] values) => this.Assign<TValue[]>(values, (Action<IAppendProcessor, TValue[]>) ((a, v) =>
    {
      if (v != null && v.Length == 1 && typeof (IEnumerable).IsAssignableFrom(typeof (TValue)) && typeof (TValue) != typeof (string))
        a.Value = ((IEnumerable<TValue>) v).First<TValue>() is IEnumerable source2 ? source2.Cast<object>() : (IEnumerable<object>) null;
      else
        a.Value = v != null ? v.Cast<object>() : (IEnumerable<object>) null;
    }));

    public AppendProcessorDescriptor<T> AllowDuplicates(bool? allowDuplicates = true) => this.Assign<bool?>(allowDuplicates, (Action<IAppendProcessor, bool?>) ((a, v) => a.AllowDuplicates = v));
  }
}

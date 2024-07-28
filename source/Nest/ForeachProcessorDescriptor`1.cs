// Decompiled with JetBrains decompiler
// Type: Nest.ForeachProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class ForeachProcessorDescriptor<T> : 
    ProcessorDescriptorBase<ForeachProcessorDescriptor<T>, IForeachProcessor>,
    IForeachProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "foreach";

    Nest.Field IForeachProcessor.Field { get; set; }

    IProcessor IForeachProcessor.Processor { get; set; }

    bool? IForeachProcessor.IgnoreMissing { get; set; }

    public ForeachProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IForeachProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public ForeachProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IForeachProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public ForeachProcessorDescriptor<T> Processor(
      Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>> selector)
    {
      return this.Assign<Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>>>(selector, (Action<IForeachProcessor, Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>>>) ((a, v) =>
      {
        IForeachProcessor foreachProcessor = a;
        IProcessor processor;
        if (v == null)
        {
          processor = (IProcessor) null;
        }
        else
        {
          IPromise<IList<IProcessor>> promise = v(new ProcessorsDescriptor());
          if (promise == null)
          {
            processor = (IProcessor) null;
          }
          else
          {
            IList<IProcessor> source = promise.Value;
            processor = source != null ? source.FirstOrDefault<IProcessor>() : (IProcessor) null;
          }
        }
        foreachProcessor.Processor = processor;
      }));
    }

    public ForeachProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IForeachProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));
  }
}

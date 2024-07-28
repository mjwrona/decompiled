// Decompiled with JetBrains decompiler
// Type: Nest.GrokProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class GrokProcessorDescriptor<T> : 
    ProcessorDescriptorBase<GrokProcessorDescriptor<T>, IGrokProcessor>,
    IGrokProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "grok";

    Nest.Field IGrokProcessor.Field { get; set; }

    IDictionary<string, string> IGrokProcessor.PatternDefinitions { get; set; }

    IEnumerable<string> IGrokProcessor.Patterns { get; set; }

    bool? IGrokProcessor.TraceMatch { get; set; }

    bool? IGrokProcessor.IgnoreMissing { get; set; }

    public GrokProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IGrokProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public GrokProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IGrokProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public GrokProcessorDescriptor<T> Patterns(IEnumerable<string> patterns) => this.Assign<IEnumerable<string>>(patterns, (Action<IGrokProcessor, IEnumerable<string>>) ((a, v) => a.Patterns = v));

    public GrokProcessorDescriptor<T> Patterns(params string[] patterns) => this.Assign<string[]>(patterns, (Action<IGrokProcessor, string[]>) ((a, v) => a.Patterns = (IEnumerable<string>) v));

    public GrokProcessorDescriptor<T> PatternDefinitions(
      Func<FluentDictionary<string, string>, FluentDictionary<string, string>> patternDefinitions)
    {
      return this.Assign<Func<FluentDictionary<string, string>, FluentDictionary<string, string>>>(patternDefinitions, (Action<IGrokProcessor, Func<FluentDictionary<string, string>, FluentDictionary<string, string>>>) ((a, v) => a.PatternDefinitions = v != null ? (IDictionary<string, string>) v(new FluentDictionary<string, string>()) : (IDictionary<string, string>) null));
    }

    public GrokProcessorDescriptor<T> TraceMatch(bool? traceMatch = true) => this.Assign<bool?>(traceMatch, (Action<IGrokProcessor, bool?>) ((a, v) => a.TraceMatch = v));

    public GrokProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IGrokProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));
  }
}

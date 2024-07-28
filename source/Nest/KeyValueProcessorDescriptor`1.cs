// Decompiled with JetBrains decompiler
// Type: Nest.KeyValueProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class KeyValueProcessorDescriptor<T> : 
    ProcessorDescriptorBase<KeyValueProcessorDescriptor<T>, IKeyValueProcessor>,
    IKeyValueProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "kv";

    IEnumerable<string> IKeyValueProcessor.ExcludeKeys { get; set; }

    Nest.Field IKeyValueProcessor.Field { get; set; }

    string IKeyValueProcessor.FieldSplit { get; set; }

    bool? IKeyValueProcessor.IgnoreMissing { get; set; }

    IEnumerable<string> IKeyValueProcessor.IncludeKeys { get; set; }

    string IKeyValueProcessor.Prefix { get; set; }

    bool? IKeyValueProcessor.StripBrackets { get; set; }

    Nest.Field IKeyValueProcessor.TargetField { get; set; }

    string IKeyValueProcessor.TrimKey { get; set; }

    string IKeyValueProcessor.TrimValue { get; set; }

    string IKeyValueProcessor.ValueSplit { get; set; }

    public KeyValueProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IKeyValueProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public KeyValueProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IKeyValueProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public KeyValueProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IKeyValueProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public KeyValueProcessorDescriptor<T> TargetField<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IKeyValueProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public KeyValueProcessorDescriptor<T> FieldSplit(string split) => this.Assign<string>(split, (Action<IKeyValueProcessor, string>) ((a, v) => a.FieldSplit = v));

    public KeyValueProcessorDescriptor<T> ValueSplit(string split) => this.Assign<string>(split, (Action<IKeyValueProcessor, string>) ((a, v) => a.ValueSplit = v));

    public KeyValueProcessorDescriptor<T> Prefix(string prefix) => this.Assign<string>(prefix, (Action<IKeyValueProcessor, string>) ((a, v) => a.Prefix = v));

    public KeyValueProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IKeyValueProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));

    public KeyValueProcessorDescriptor<T> IncludeKeys(IEnumerable<string> includeKeys) => this.Assign<IEnumerable<string>>(includeKeys, (Action<IKeyValueProcessor, IEnumerable<string>>) ((a, v) => a.IncludeKeys = v));

    public KeyValueProcessorDescriptor<T> IncludeKeys(params string[] includeKeys) => this.Assign<string[]>(includeKeys, (Action<IKeyValueProcessor, string[]>) ((a, v) => a.IncludeKeys = (IEnumerable<string>) v));

    public KeyValueProcessorDescriptor<T> ExcludeKeys(IEnumerable<string> excludeKeys) => this.Assign<IEnumerable<string>>(excludeKeys, (Action<IKeyValueProcessor, IEnumerable<string>>) ((a, v) => a.ExcludeKeys = v));

    public KeyValueProcessorDescriptor<T> ExcludeKeys(params string[] excludeKeys) => this.Assign<string[]>(excludeKeys, (Action<IKeyValueProcessor, string[]>) ((a, v) => a.ExcludeKeys = (IEnumerable<string>) v));

    public KeyValueProcessorDescriptor<T> TrimKey(string trimKeys) => this.Assign<string>(trimKeys, (Action<IKeyValueProcessor, string>) ((a, v) => a.TrimKey = v));

    public KeyValueProcessorDescriptor<T> TrimValue(string trimValues) => this.Assign<string>(trimValues, (Action<IKeyValueProcessor, string>) ((a, v) => a.TrimValue = v));

    public KeyValueProcessorDescriptor<T> StripBrackets(bool? skip = true) => this.Assign<bool?>(skip, (Action<IKeyValueProcessor, bool?>) ((a, v) => a.StripBrackets = v));
  }
}

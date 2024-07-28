// Decompiled with JetBrains decompiler
// Type: Nest.CsvProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class CsvProcessorDescriptor<T> : 
    ProcessorDescriptorBase<CsvProcessorDescriptor<T>, ICsvProcessor>,
    ICsvProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "csv";

    Nest.Field ICsvProcessor.Field { get; set; }

    Fields ICsvProcessor.TargetFields { get; set; }

    bool? ICsvProcessor.IgnoreMissing { get; set; }

    string ICsvProcessor.Quote { get; set; }

    string ICsvProcessor.Separator { get; set; }

    bool? ICsvProcessor.Trim { get; set; }

    object ICsvProcessor.EmptyValue { get; set; }

    public CsvProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ICsvProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public CsvProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ICsvProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public CsvProcessorDescriptor<T> TargetFields(
      Func<FieldsDescriptor<T>, IPromise<Fields>> targetFields)
    {
      return this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(targetFields, (Action<ICsvProcessor, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.TargetFields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));
    }

    public CsvProcessorDescriptor<T> TargetFields(Fields targetFields) => this.Assign<Fields>(targetFields, (Action<ICsvProcessor, Fields>) ((a, v) => a.TargetFields = v));

    public CsvProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<ICsvProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));

    public CsvProcessorDescriptor<T> Trim(bool? trim = true) => this.Assign<bool?>(trim, (Action<ICsvProcessor, bool?>) ((a, v) => a.Trim = v));

    public CsvProcessorDescriptor<T> Quote(string quote) => this.Assign<string>(quote, (Action<ICsvProcessor, string>) ((a, v) => a.Quote = v));

    public CsvProcessorDescriptor<T> Separator(string separator) => this.Assign<string>(separator, (Action<ICsvProcessor, string>) ((a, v) => a.Separator = v));

    public CsvProcessorDescriptor<T> EmptyValue(object value) => this.Assign<object>(value, (Action<ICsvProcessor, object>) ((a, v) => a.EmptyValue = v));
  }
}

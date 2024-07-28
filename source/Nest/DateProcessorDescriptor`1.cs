// Decompiled with JetBrains decompiler
// Type: Nest.DateProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class DateProcessorDescriptor<T> : 
    ProcessorDescriptorBase<DateProcessorDescriptor<T>, IDateProcessor>,
    IDateProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "date";

    Nest.Field IDateProcessor.Field { get; set; }

    IEnumerable<string> IDateProcessor.Formats { get; set; }

    string IDateProcessor.Locale { get; set; }

    Nest.Field IDateProcessor.TargetField { get; set; }

    string IDateProcessor.TimeZone { get; set; }

    public DateProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IDateProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public DateProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IDateProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public DateProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IDateProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public DateProcessorDescriptor<T> TargetField<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IDateProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public DateProcessorDescriptor<T> Formats(IEnumerable<string> matchFormats) => this.Assign<IEnumerable<string>>(matchFormats, (Action<IDateProcessor, IEnumerable<string>>) ((a, v) => a.Formats = v));

    public DateProcessorDescriptor<T> Formats(params string[] matchFormats) => this.Assign<string[]>(matchFormats, (Action<IDateProcessor, string[]>) ((a, v) => a.Formats = (IEnumerable<string>) v));

    public DateProcessorDescriptor<T> TimeZone(string timezone) => this.Assign<string>(timezone, (Action<IDateProcessor, string>) ((a, v) => a.TimeZone = v));

    public DateProcessorDescriptor<T> Locale(string locale) => this.Assign<string>(locale, (Action<IDateProcessor, string>) ((a, v) => a.Locale = v));
  }
}

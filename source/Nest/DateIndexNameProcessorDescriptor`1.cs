// Decompiled with JetBrains decompiler
// Type: Nest.DateIndexNameProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class DateIndexNameProcessorDescriptor<T> : 
    ProcessorDescriptorBase<DateIndexNameProcessorDescriptor<T>, IDateIndexNameProcessor>,
    IDateIndexNameProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "date_index_name";

    IEnumerable<string> IDateIndexNameProcessor.DateFormats { get; set; }

    Nest.DateRounding? IDateIndexNameProcessor.DateRounding { get; set; }

    Nest.Field IDateIndexNameProcessor.Field { get; set; }

    string IDateIndexNameProcessor.IndexNameFormat { get; set; }

    string IDateIndexNameProcessor.IndexNamePrefix { get; set; }

    string IDateIndexNameProcessor.Locale { get; set; }

    string IDateIndexNameProcessor.TimeZone { get; set; }

    public DateIndexNameProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IDateIndexNameProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public DateIndexNameProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IDateIndexNameProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public DateIndexNameProcessorDescriptor<T> IndexNamePrefix(string indexNamePrefix) => this.Assign<string>(indexNamePrefix, (Action<IDateIndexNameProcessor, string>) ((a, v) => a.IndexNamePrefix = v));

    public DateIndexNameProcessorDescriptor<T> DateRounding(Nest.DateRounding? dateRounding) => this.Assign<Nest.DateRounding?>(dateRounding, (Action<IDateIndexNameProcessor, Nest.DateRounding?>) ((a, v) => a.DateRounding = v));

    public DateIndexNameProcessorDescriptor<T> DateFormats(IEnumerable<string> dateFormats) => this.Assign<IEnumerable<string>>(dateFormats, (Action<IDateIndexNameProcessor, IEnumerable<string>>) ((a, v) => a.DateFormats = v));

    public DateIndexNameProcessorDescriptor<T> DateFormats(params string[] dateFormats) => this.Assign<string[]>(dateFormats, (Action<IDateIndexNameProcessor, string[]>) ((a, v) => a.DateFormats = (IEnumerable<string>) v));

    public DateIndexNameProcessorDescriptor<T> TimeZone(string timeZone) => this.Assign<string>(timeZone, (Action<IDateIndexNameProcessor, string>) ((a, v) => a.TimeZone = v));

    public DateIndexNameProcessorDescriptor<T> Locale(string locale) => this.Assign<string>(locale, (Action<IDateIndexNameProcessor, string>) ((a, v) => a.Locale = v));

    public DateIndexNameProcessorDescriptor<T> IndexNameFormat(string indexNameFormat) => this.Assign<string>(indexNameFormat, (Action<IDateIndexNameProcessor, string>) ((a, v) => a.IndexNameFormat = v));
  }
}

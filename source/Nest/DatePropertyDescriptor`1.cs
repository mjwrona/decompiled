// Decompiled with JetBrains decompiler
// Type: Nest.DatePropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class DatePropertyDescriptor<T> : 
    DocValuesPropertyDescriptorBase<DatePropertyDescriptor<T>, IDateProperty, T>,
    IDateProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public DatePropertyDescriptor()
      : base(FieldType.Date)
    {
    }

    double? IDateProperty.Boost { get; set; }

    INumericFielddata IDateProperty.Fielddata { get; set; }

    string IDateProperty.Format { get; set; }

    bool? IDateProperty.IgnoreMalformed { get; set; }

    bool? IDateProperty.Index { get; set; }

    DateTime? IDateProperty.NullValue { get; set; }

    public DatePropertyDescriptor<T> Index(bool? index = true) => this.Assign<bool?>(index, (Action<IDateProperty, bool?>) ((a, v) => a.Index = v));

    public DatePropertyDescriptor<T> Boost(double? boost) => this.Assign<double?>(boost, (Action<IDateProperty, double?>) ((a, v) => a.Boost = v));

    public DatePropertyDescriptor<T> NullValue(DateTime? nullValue) => this.Assign<DateTime?>(nullValue, (Action<IDateProperty, DateTime?>) ((a, v) => a.NullValue = v));

    public DatePropertyDescriptor<T> IgnoreMalformed(bool? ignoreMalformed = true) => this.Assign<bool?>(ignoreMalformed, (Action<IDateProperty, bool?>) ((a, v) => a.IgnoreMalformed = v));

    public DatePropertyDescriptor<T> Format(string format) => this.Assign<string>(format, (Action<IDateProperty, string>) ((a, v) => a.Format = v));

    public DatePropertyDescriptor<T> Fielddata(
      Func<NumericFielddataDescriptor, INumericFielddata> selector)
    {
      return this.Assign<INumericFielddata>(selector(new NumericFielddataDescriptor()), (Action<IDateProperty, INumericFielddata>) ((a, v) => a.Fielddata = v));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.DateNanosPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class DateNanosPropertyDescriptor<T> : 
    DocValuesPropertyDescriptorBase<DateNanosPropertyDescriptor<T>, IDateNanosProperty, T>,
    IDateNanosProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public DateNanosPropertyDescriptor()
      : base(FieldType.DateNanos)
    {
    }

    double? IDateNanosProperty.Boost { get; set; }

    string IDateNanosProperty.Format { get; set; }

    bool? IDateNanosProperty.IgnoreMalformed { get; set; }

    bool? IDateNanosProperty.Index { get; set; }

    DateTime? IDateNanosProperty.NullValue { get; set; }

    public DateNanosPropertyDescriptor<T> Index(bool? index = true) => this.Assign<bool?>(index, (Action<IDateNanosProperty, bool?>) ((a, v) => a.Index = v));

    public DateNanosPropertyDescriptor<T> Boost(double? boost) => this.Assign<double?>(boost, (Action<IDateNanosProperty, double?>) ((a, v) => a.Boost = v));

    public DateNanosPropertyDescriptor<T> NullValue(DateTime? nullValue) => this.Assign<DateTime?>(nullValue, (Action<IDateNanosProperty, DateTime?>) ((a, v) => a.NullValue = v));

    public DateNanosPropertyDescriptor<T> IgnoreMalformed(bool? ignoreMalformed = true) => this.Assign<bool?>(ignoreMalformed, (Action<IDateNanosProperty, bool?>) ((a, v) => a.IgnoreMalformed = v));

    public DateNanosPropertyDescriptor<T> Format(string format) => this.Assign<string>(format, (Action<IDateNanosProperty, string>) ((a, v) => a.Format = v));
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.DateRangePropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DateRangePropertyDescriptor<T> : 
    RangePropertyDescriptorBase<DateRangePropertyDescriptor<T>, IDateRangeProperty, T>,
    IDateRangeProperty,
    IRangeProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public DateRangePropertyDescriptor()
      : base(RangeType.DateRange)
    {
    }

    string IDateRangeProperty.Format { get; set; }

    public DateRangePropertyDescriptor<T> Format(string format) => this.Assign<string>(format, (Action<IDateRangeProperty, string>) ((a, v) => a.Format = v));
  }
}

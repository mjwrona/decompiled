// Decompiled with JetBrains decompiler
// Type: Nest.BooleanPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class BooleanPropertyDescriptor<T> : 
    DocValuesPropertyDescriptorBase<BooleanPropertyDescriptor<T>, IBooleanProperty, T>,
    IBooleanProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public BooleanPropertyDescriptor()
      : base(FieldType.Boolean)
    {
    }

    double? IBooleanProperty.Boost { get; set; }

    INumericFielddata IBooleanProperty.Fielddata { get; set; }

    bool? IBooleanProperty.Index { get; set; }

    bool? IBooleanProperty.NullValue { get; set; }

    public BooleanPropertyDescriptor<T> Boost(double? boost) => this.Assign<double?>(boost, (Action<IBooleanProperty, double?>) ((a, v) => a.Boost = v));

    public BooleanPropertyDescriptor<T> Index(bool? index = true) => this.Assign<bool?>(index, (Action<IBooleanProperty, bool?>) ((a, v) => a.Index = v));

    public BooleanPropertyDescriptor<T> NullValue(bool? nullValue) => this.Assign<bool?>(nullValue, (Action<IBooleanProperty, bool?>) ((a, v) => a.NullValue = v));

    public BooleanPropertyDescriptor<T> Fielddata(
      Func<NumericFielddataDescriptor, INumericFielddata> selector)
    {
      return this.Assign<INumericFielddata>(selector(new NumericFielddataDescriptor()), (Action<IBooleanProperty, INumericFielddata>) ((a, v) => a.Fielddata = v));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.NumberPropertyDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public abstract class NumberPropertyDescriptorBase<TDescriptor, TInterface, T> : 
    DocValuesPropertyDescriptorBase<TDescriptor, TInterface, T>,
    INumberProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where TDescriptor : NumberPropertyDescriptorBase<TDescriptor, TInterface, T>, TInterface
    where TInterface : class, INumberProperty
    where T : class
  {
    protected NumberPropertyDescriptorBase()
      : base(FieldType.Float)
    {
    }

    protected NumberPropertyDescriptorBase(FieldType type)
      : base(type)
    {
    }

    double? INumberProperty.Boost { get; set; }

    bool? INumberProperty.Coerce { get; set; }

    INumericFielddata INumberProperty.Fielddata { get; set; }

    bool? INumberProperty.IgnoreMalformed { get; set; }

    bool? INumberProperty.Index { get; set; }

    double? INumberProperty.NullValue { get; set; }

    double? INumberProperty.ScalingFactor { get; set; }

    IInlineScript INumberProperty.Script { get; set; }

    Nest.OnScriptError? INumberProperty.OnScriptError { get; set; }

    public TDescriptor Type(NumberType? type) => this.Assign<string>(type.HasValue ? type.GetValueOrDefault().GetStringValue() : (string) null, (Action<TInterface, string>) ((a, v) => a.Type = v));

    public TDescriptor Index(bool? index = true) => this.Assign<bool?>(index, (Action<TInterface, bool?>) ((a, v) => a.Index = v));

    [Obsolete("The server always treated this as a noop and has been removed in 7.10")]
    public TDescriptor Boost(double? boost) => this.Assign<double?>(boost, (Action<TInterface, double?>) ((a, v) => a.Boost = v));

    public TDescriptor NullValue(double? nullValue) => this.Assign<double?>(nullValue, (Action<TInterface, double?>) ((a, v) => a.NullValue = v));

    public TDescriptor IgnoreMalformed(bool? ignoreMalformed = true) => this.Assign<bool?>(ignoreMalformed, (Action<TInterface, bool?>) ((a, v) => a.IgnoreMalformed = v));

    public TDescriptor Coerce(bool? coerce = true) => this.Assign<bool?>(coerce, (Action<TInterface, bool?>) ((a, v) => a.Coerce = v));

    public TDescriptor Fielddata(
      Func<NumericFielddataDescriptor, INumericFielddata> selector)
    {
      return this.Assign<INumericFielddata>(selector(new NumericFielddataDescriptor()), (Action<TInterface, INumericFielddata>) ((a, v) => a.Fielddata = v));
    }

    public TDescriptor ScalingFactor(double? scalingFactor) => this.Assign<double?>(scalingFactor, (Action<TInterface, double?>) ((a, v) => a.ScalingFactor = v));

    public TDescriptor Script(IInlineScript inlineScript) => this.Assign<IInlineScript>(inlineScript, (Action<TInterface, IInlineScript>) ((a, v) => a.Script = v));

    public TDescriptor Script(string source) => this.Assign<string>(source, (Action<TInterface, string>) ((a, v) => a.Script = (IInlineScript) new InlineScript(source)));

    public TDescriptor Script(
      Func<InlineScriptDescriptor, IInlineScript> selector)
    {
      return this.Assign<Func<InlineScriptDescriptor, IInlineScript>>(selector, (Action<TInterface, Func<InlineScriptDescriptor, IInlineScript>>) ((a, v) => a.Script = v != null ? v(new InlineScriptDescriptor()) : (IInlineScript) null));
    }

    public TDescriptor OnScriptError(Nest.OnScriptError? onScriptError) => this.Assign<Nest.OnScriptError?>(onScriptError, (Action<TInterface, Nest.OnScriptError?>) ((a, v) => a.OnScriptError = v));
  }
}

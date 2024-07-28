// Decompiled with JetBrains decompiler
// Type: Nest.IpPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class IpPropertyDescriptor<T> : 
    DocValuesPropertyDescriptorBase<IpPropertyDescriptor<T>, IIpProperty, T>,
    IIpProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public IpPropertyDescriptor()
      : base(FieldType.Ip)
    {
    }

    double? IIpProperty.Boost { get; set; }

    bool? IIpProperty.Index { get; set; }

    string IIpProperty.NullValue { get; set; }

    bool? IIpProperty.IgnoreMalformed { get; set; }

    IInlineScript IIpProperty.Script { get; set; }

    Nest.OnScriptError? IIpProperty.OnScriptError { get; set; }

    public IpPropertyDescriptor<T> Index(bool? index = true) => this.Assign<bool?>(index, (Action<IIpProperty, bool?>) ((a, v) => a.Index = v));

    [Obsolete("The server always treated this as a noop and has been removed in 7.10")]
    public IpPropertyDescriptor<T> Boost(double? boost) => this.Assign<double?>(boost, (Action<IIpProperty, double?>) ((a, v) => a.Boost = v));

    public IpPropertyDescriptor<T> NullValue(string nullValue) => this.Assign<string>(nullValue, (Action<IIpProperty, string>) ((a, v) => a.NullValue = v));

    public IpPropertyDescriptor<T> IgnoreMalformed(bool? ignoreMalformed = true) => this.Assign<bool?>(ignoreMalformed, (Action<IIpProperty, bool?>) ((a, v) => a.IgnoreMalformed = v));

    public IpPropertyDescriptor<T> Script(IInlineScript inlineScript) => this.Assign<IInlineScript>(inlineScript, (Action<IIpProperty, IInlineScript>) ((a, v) => a.Script = v));

    public IpPropertyDescriptor<T> Script(string source) => this.Assign<string>(source, (Action<IIpProperty, string>) ((a, v) => a.Script = (IInlineScript) new InlineScript(source)));

    public IpPropertyDescriptor<T> Script(
      Func<InlineScriptDescriptor, IInlineScript> selector)
    {
      return this.Assign<Func<InlineScriptDescriptor, IInlineScript>>(selector, (Action<IIpProperty, Func<InlineScriptDescriptor, IInlineScript>>) ((a, v) => a.Script = v != null ? v(new InlineScriptDescriptor()) : (IInlineScript) null));
    }

    public IpPropertyDescriptor<T> OnScriptError(Nest.OnScriptError? onScriptError) => this.Assign<Nest.OnScriptError?>(onScriptError, (Action<IIpProperty, Nest.OnScriptError?>) ((a, v) => a.OnScriptError = v));
  }
}

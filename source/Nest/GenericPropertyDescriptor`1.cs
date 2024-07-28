// Decompiled with JetBrains decompiler
// Type: Nest.GenericPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class GenericPropertyDescriptor<T> : 
    DocValuesPropertyDescriptorBase<GenericPropertyDescriptor<T>, IGenericProperty, T>,
    IGenericProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public GenericPropertyDescriptor()
      : base(FieldType.Object)
    {
      this.TypeOverride = (string) null;
    }

    string IGenericProperty.Analyzer { get; set; }

    double? IGenericProperty.Boost { get; set; }

    IStringFielddata IGenericProperty.Fielddata { get; set; }

    int? IGenericProperty.IgnoreAbove { get; set; }

    bool? IGenericProperty.Index { get; set; }

    Nest.IndexOptions? IGenericProperty.IndexOptions { get; set; }

    bool? IGenericProperty.Norms { get; set; }

    string IGenericProperty.NullValue { get; set; }

    int? IGenericProperty.PositionIncrementGap { get; set; }

    string IGenericProperty.SearchAnalyzer { get; set; }

    TermVectorOption? IGenericProperty.TermVector { get; set; }

    public GenericPropertyDescriptor<T> Type(string type) => this.Assign<string>(type, (Action<IGenericProperty, string>) ((a, v) => this.TypeOverride = v));

    public GenericPropertyDescriptor<T> Index(bool? index = true) => this.Assign<bool?>(index, (Action<IGenericProperty, bool?>) ((a, v) => a.Index = v));

    public GenericPropertyDescriptor<T> Boost(double? boost) => this.Assign<double?>(boost, (Action<IGenericProperty, double?>) ((a, v) => a.Boost = v));

    public GenericPropertyDescriptor<T> NullValue(string nullValue) => this.Assign<string>(nullValue, (Action<IGenericProperty, string>) ((a, v) => a.NullValue = v));

    public GenericPropertyDescriptor<T> TermVector(TermVectorOption? termVector) => this.Assign<TermVectorOption?>(termVector, (Action<IGenericProperty, TermVectorOption?>) ((a, v) => a.TermVector = v));

    public GenericPropertyDescriptor<T> IndexOptions(Nest.IndexOptions? indexOptions) => this.Assign<Nest.IndexOptions?>(indexOptions, (Action<IGenericProperty, Nest.IndexOptions?>) ((a, v) => a.IndexOptions = v));

    public GenericPropertyDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IGenericProperty, string>) ((a, v) => a.Analyzer = v));

    public GenericPropertyDescriptor<T> SearchAnalyzer(string searchAnalyzer) => this.Assign<string>(searchAnalyzer, (Action<IGenericProperty, string>) ((a, v) => a.SearchAnalyzer = v));

    public GenericPropertyDescriptor<T> Norms(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<IGenericProperty, bool?>) ((a, v) => a.Norms = v));

    public GenericPropertyDescriptor<T> IgnoreAbove(int? ignoreAbove) => this.Assign<int?>(ignoreAbove, (Action<IGenericProperty, int?>) ((a, v) => a.IgnoreAbove = v));

    public GenericPropertyDescriptor<T> PositionIncrementGap(int? positionIncrementGap) => this.Assign<int?>(positionIncrementGap, (Action<IGenericProperty, int?>) ((a, v) => a.PositionIncrementGap = v));

    public GenericPropertyDescriptor<T> Fielddata(
      Func<StringFielddataDescriptor, IStringFielddata> selector)
    {
      return this.Assign<Func<StringFielddataDescriptor, IStringFielddata>>(selector, (Action<IGenericProperty, Func<StringFielddataDescriptor, IStringFielddata>>) ((a, v) => a.Fielddata = v != null ? v(new StringFielddataDescriptor()) : (IStringFielddata) null));
    }
  }
}

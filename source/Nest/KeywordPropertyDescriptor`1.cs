// Decompiled with JetBrains decompiler
// Type: Nest.KeywordPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class KeywordPropertyDescriptor<T> : 
    DocValuesPropertyDescriptorBase<KeywordPropertyDescriptor<T>, IKeywordProperty, T>,
    IKeywordProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public KeywordPropertyDescriptor()
      : base(FieldType.Keyword)
    {
    }

    double? IKeywordProperty.Boost { get; set; }

    bool? IKeywordProperty.EagerGlobalOrdinals { get; set; }

    int? IKeywordProperty.IgnoreAbove { get; set; }

    bool? IKeywordProperty.Index { get; set; }

    Nest.IndexOptions? IKeywordProperty.IndexOptions { get; set; }

    string IKeywordProperty.Normalizer { get; set; }

    bool? IKeywordProperty.Norms { get; set; }

    string IKeywordProperty.NullValue { get; set; }

    bool? IKeywordProperty.SplitQueriesOnWhitespace { get; set; }

    public KeywordPropertyDescriptor<T> Boost(double? boost) => this.Assign<double?>(boost, (Action<IKeywordProperty, double?>) ((a, v) => a.Boost = v));

    public KeywordPropertyDescriptor<T> EagerGlobalOrdinals(bool? eagerGlobalOrdinals = true) => this.Assign<bool?>(eagerGlobalOrdinals, (Action<IKeywordProperty, bool?>) ((a, v) => a.EagerGlobalOrdinals = v));

    public KeywordPropertyDescriptor<T> IgnoreAbove(int? ignoreAbove) => this.Assign<int?>(ignoreAbove, (Action<IKeywordProperty, int?>) ((a, v) => a.IgnoreAbove = v));

    public KeywordPropertyDescriptor<T> Index(bool? index = true) => this.Assign<bool?>(index, (Action<IKeywordProperty, bool?>) ((a, v) => a.Index = v));

    public KeywordPropertyDescriptor<T> IndexOptions(Nest.IndexOptions? indexOptions) => this.Assign<Nest.IndexOptions?>(indexOptions, (Action<IKeywordProperty, Nest.IndexOptions?>) ((a, v) => a.IndexOptions = v));

    public KeywordPropertyDescriptor<T> Norms(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<IKeywordProperty, bool?>) ((a, v) => a.Norms = v));

    public KeywordPropertyDescriptor<T> SplitQueriesOnWhitespace(bool? split = true) => this.Assign<bool?>(split, (Action<IKeywordProperty, bool?>) ((a, v) => a.SplitQueriesOnWhitespace = v));

    public KeywordPropertyDescriptor<T> NullValue(string nullValue) => this.Assign<string>(nullValue, (Action<IKeywordProperty, string>) ((a, v) => a.NullValue = v));

    public KeywordPropertyDescriptor<T> Normalizer(string normalizer) => this.Assign<string>(normalizer, (Action<IKeywordProperty, string>) ((a, v) => a.Normalizer = v));
  }
}

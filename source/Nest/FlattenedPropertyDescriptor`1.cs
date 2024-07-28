// Decompiled with JetBrains decompiler
// Type: Nest.FlattenedPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class FlattenedPropertyDescriptor<T> : 
    PropertyDescriptorBase<FlattenedPropertyDescriptor<T>, IFlattenedProperty, T>,
    IFlattenedProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public FlattenedPropertyDescriptor()
      : base(FieldType.Flattened)
    {
    }

    double? IFlattenedProperty.Boost { get; set; }

    int? IFlattenedProperty.DepthLimit { get; set; }

    bool? IFlattenedProperty.DocValues { get; set; }

    bool? IFlattenedProperty.EagerGlobalOrdinals { get; set; }

    int? IFlattenedProperty.IgnoreAbove { get; set; }

    bool? IFlattenedProperty.Index { get; set; }

    Nest.IndexOptions? IFlattenedProperty.IndexOptions { get; set; }

    string IFlattenedProperty.NullValue { get; set; }

    string IFlattenedProperty.Similarity { get; set; }

    bool? IFlattenedProperty.SplitQueriesOnWhitespace { get; set; }

    public FlattenedPropertyDescriptor<T> Boost(double? boost) => this.Assign<double?>(boost, (Action<IFlattenedProperty, double?>) ((a, v) => a.Boost = v));

    public FlattenedPropertyDescriptor<T> DepthLimit(int? depthLimit) => this.Assign<int?>(depthLimit, (Action<IFlattenedProperty, int?>) ((a, v) => a.DepthLimit = v));

    public FlattenedPropertyDescriptor<T> DocValues(bool? docValues = true) => this.Assign<bool?>(docValues, (Action<IFlattenedProperty, bool?>) ((a, v) => a.DocValues = v));

    public FlattenedPropertyDescriptor<T> EagerGlobalOrdinals(bool? eagerGlobalOrdinals = true) => this.Assign<bool?>(eagerGlobalOrdinals, (Action<IFlattenedProperty, bool?>) ((a, v) => a.EagerGlobalOrdinals = v));

    public FlattenedPropertyDescriptor<T> IgnoreAbove(int? ignoreAbove) => this.Assign<int?>(ignoreAbove, (Action<IFlattenedProperty, int?>) ((a, v) => a.IgnoreAbove = v));

    public FlattenedPropertyDescriptor<T> Index(bool? index = true) => this.Assign<bool?>(index, (Action<IFlattenedProperty, bool?>) ((a, v) => a.Index = v));

    public FlattenedPropertyDescriptor<T> IndexOptions(Nest.IndexOptions? indexOptions) => this.Assign<Nest.IndexOptions?>(indexOptions, (Action<IFlattenedProperty, Nest.IndexOptions?>) ((a, v) => a.IndexOptions = v));

    public FlattenedPropertyDescriptor<T> NullValue(string nullValue) => this.Assign<string>(nullValue, (Action<IFlattenedProperty, string>) ((a, v) => a.NullValue = v));

    public FlattenedPropertyDescriptor<T> SplitQueriesOnWhitespace(bool? splitQueriesOnWhitespace = true) => this.Assign<bool?>(splitQueriesOnWhitespace, (Action<IFlattenedProperty, bool?>) ((a, v) => a.SplitQueriesOnWhitespace = v));

    public FlattenedPropertyDescriptor<T> Similarity(string similarity) => this.Assign<string>(similarity, (Action<IFlattenedProperty, string>) ((a, v) => a.Similarity = v));
  }
}

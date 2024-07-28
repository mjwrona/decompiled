// Decompiled with JetBrains decompiler
// Type: Nest.FlattenedAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class FlattenedAttribute : 
    ElasticsearchPropertyAttributeBase,
    IFlattenedProperty,
    IProperty,
    IFieldMapping
  {
    public FlattenedAttribute()
      : base(FieldType.Flattened)
    {
    }

    private IFlattenedProperty Self => (IFlattenedProperty) this;

    double? IFlattenedProperty.Boost { get; set; }

    int? IFlattenedProperty.DepthLimit { get; set; }

    bool? IFlattenedProperty.DocValues { get; set; }

    bool? IFlattenedProperty.EagerGlobalOrdinals { get; set; }

    int? IFlattenedProperty.IgnoreAbove { get; set; }

    bool? IFlattenedProperty.Index { get; set; }

    IndexOptions? IFlattenedProperty.IndexOptions { get; set; }

    bool? IFlattenedProperty.SplitQueriesOnWhitespace { get; set; }

    public double Boost
    {
      get => this.Self.Boost.GetValueOrDefault(1.0);
      set => this.Self.Boost = new double?(value);
    }

    public int DepthLimit
    {
      get => this.Self.DepthLimit.GetValueOrDefault(20);
      set => this.Self.DepthLimit = new int?(value);
    }

    public bool DocValues
    {
      get => this.Self.DocValues.GetValueOrDefault(true);
      set => this.Self.DocValues = new bool?(value);
    }

    public bool EagerGlobalOrdinals
    {
      get => this.Self.EagerGlobalOrdinals.GetValueOrDefault(false);
      set => this.Self.EagerGlobalOrdinals = new bool?(value);
    }

    public int IgnoreAbove
    {
      get => this.Self.IgnoreAbove.GetValueOrDefault(-1);
      set => this.Self.IgnoreAbove = new int?(value);
    }

    public bool Index
    {
      get => this.Self.Index.GetValueOrDefault(true);
      set => this.Self.Index = new bool?(value);
    }

    public IndexOptions IndexOptions
    {
      get => this.Self.IndexOptions.GetValueOrDefault(IndexOptions.Docs);
      set => this.Self.IndexOptions = new IndexOptions?(value);
    }

    public bool SplitQueriesOnWhitespace
    {
      get => this.Self.SplitQueriesOnWhitespace.GetValueOrDefault(false);
      set => this.Self.SplitQueriesOnWhitespace = new bool?(value);
    }

    public string NullValue { get; set; }

    public string Similarity { get; set; }
  }
}

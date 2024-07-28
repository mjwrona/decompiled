// Decompiled with JetBrains decompiler
// Type: Nest.RangePropertyAttributeBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public abstract class RangePropertyAttributeBase : 
    ElasticsearchDocValuesPropertyAttributeBase,
    IRangeProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    protected RangePropertyAttributeBase(RangeType type)
      : base(type.ToFieldType())
    {
    }

    public double Boost
    {
      get => this.Self.Boost.GetValueOrDefault();
      set => this.Self.Boost = new double?(value);
    }

    public bool Coerce
    {
      get => this.Self.Coerce.GetValueOrDefault();
      set => this.Self.Coerce = new bool?(value);
    }

    public bool Index
    {
      get => this.Self.Index.GetValueOrDefault();
      set => this.Self.Index = new bool?(value);
    }

    double? IRangeProperty.Boost { get; set; }

    bool? IRangeProperty.Coerce { get; set; }

    bool? IRangeProperty.Index { get; set; }

    private IRangeProperty Self => (IRangeProperty) this;
  }
}

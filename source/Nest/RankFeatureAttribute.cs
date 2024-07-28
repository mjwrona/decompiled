// Decompiled with JetBrains decompiler
// Type: Nest.RankFeatureAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class RankFeatureAttribute : 
    ElasticsearchPropertyAttributeBase,
    IRankFeatureProperty,
    IProperty,
    IFieldMapping
  {
    public RankFeatureAttribute()
      : base(FieldType.RankFeature)
    {
    }

    private IRankFeatureProperty Self => (IRankFeatureProperty) this;

    public bool PositiveScoreImpact
    {
      get => this.Self.PositiveScoreImpact.GetValueOrDefault(true);
      set => this.Self.PositiveScoreImpact = new bool?(value);
    }

    bool? IRankFeatureProperty.PositiveScoreImpact { get; set; }
  }
}

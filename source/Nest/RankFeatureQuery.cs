// Decompiled with JetBrains decompiler
// Type: Nest.RankFeatureQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class RankFeatureQuery : FieldNameQueryBase, IRankFeatureQuery, IFieldNameQuery, IQuery
  {
    protected override bool Conditionless => RankFeatureQuery.IsConditionless((IRankFeatureQuery) this);

    internal static bool IsConditionless(IRankFeatureQuery q) => q.Field.IsConditionless();

    internal override void InternalWrapInContainer(IQueryContainer container) => container.RankFeature = (IRankFeatureQuery) this;

    public IRankFeatureFunction Function { get; set; }
  }
}

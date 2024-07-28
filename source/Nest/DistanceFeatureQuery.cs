// Decompiled with JetBrains decompiler
// Type: Nest.DistanceFeatureQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class DistanceFeatureQuery : 
    FieldNameQueryBase,
    IDistanceFeatureQuery,
    IFieldNameQuery,
    IQuery
  {
    protected override bool Conditionless => DistanceFeatureQuery.IsConditionless((IDistanceFeatureQuery) this);

    internal static bool IsConditionless(IDistanceFeatureQuery q)
    {
      if (q.Field.IsConditionless())
        return true;
      return q.Origin == null && q.Pivot == null;
    }

    internal override void InternalWrapInContainer(IQueryContainer container) => container.DistanceFeature = (IDistanceFeatureQuery) this;

    public Union<GeoCoordinate, DateMath> Origin { get; set; }

    public Union<Distance, Time> Pivot { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.GeoDistanceQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class GeoDistanceQuery : FieldNameQueryBase, IGeoDistanceQuery, IFieldNameQuery, IQuery
  {
    public Distance Distance { get; set; }

    public GeoDistanceType? DistanceType { get; set; }

    public GeoLocation Location { get; set; }

    public GeoValidationMethod? ValidationMethod { get; set; }

    public bool? IgnoreUnmapped { get; set; }

    protected override bool Conditionless => GeoDistanceQuery.IsConditionless((IGeoDistanceQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.GeoDistance = (IGeoDistanceQuery) this;

    internal static bool IsConditionless(IGeoDistanceQuery q) => q.Location == null || q.Distance == null || q.Field.IsConditionless();
  }
}

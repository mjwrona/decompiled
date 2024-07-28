// Decompiled with JetBrains decompiler
// Type: Nest.GeoPolygonQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class GeoPolygonQuery : FieldNameQueryBase, IGeoPolygonQuery, IFieldNameQuery, IQuery
  {
    public IEnumerable<GeoLocation> Points { get; set; }

    public GeoValidationMethod? ValidationMethod { get; set; }

    public bool? IgnoreUnmapped { get; set; }

    protected override bool Conditionless => GeoPolygonQuery.IsConditionless((IGeoPolygonQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.GeoPolygon = (IGeoPolygonQuery) this;

    internal static bool IsConditionless(IGeoPolygonQuery q) => q.Field == (Field) null || !q.Points.HasAny<GeoLocation>();
  }
}

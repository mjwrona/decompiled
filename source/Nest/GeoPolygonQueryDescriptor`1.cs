// Decompiled with JetBrains decompiler
// Type: Nest.GeoPolygonQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class GeoPolygonQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<GeoPolygonQueryDescriptor<T>, IGeoPolygonQuery, T>,
    IGeoPolygonQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => GeoPolygonQuery.IsConditionless((IGeoPolygonQuery) this);

    IEnumerable<GeoLocation> IGeoPolygonQuery.Points { get; set; }

    GeoValidationMethod? IGeoPolygonQuery.ValidationMethod { get; set; }

    bool? IGeoPolygonQuery.IgnoreUnmapped { get; set; }

    public GeoPolygonQueryDescriptor<T> Points(IEnumerable<GeoLocation> points) => this.Assign<IEnumerable<GeoLocation>>(points, (Action<IGeoPolygonQuery, IEnumerable<GeoLocation>>) ((a, v) => a.Points = v));

    public GeoPolygonQueryDescriptor<T> Points(params GeoLocation[] points) => this.Assign<GeoLocation[]>(points, (Action<IGeoPolygonQuery, GeoLocation[]>) ((a, v) => a.Points = (IEnumerable<GeoLocation>) v));

    public GeoPolygonQueryDescriptor<T> ValidationMethod(GeoValidationMethod? validation) => this.Assign<GeoValidationMethod?>(validation, (Action<IGeoPolygonQuery, GeoValidationMethod?>) ((a, v) => a.ValidationMethod = v));

    public GeoPolygonQueryDescriptor<T> IgnoreUnmapped(bool? ignoreUnmapped = true) => this.Assign<bool?>(ignoreUnmapped, (Action<IGeoPolygonQuery, bool?>) ((a, v) => a.IgnoreUnmapped = v));
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.GeoShapeQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class GeoShapeQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<GeoShapeQueryDescriptor<T>, IGeoShapeQuery, T>,
    IGeoShapeQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => GeoShapeQuery.IsConditionless((IGeoShapeQuery) this);

    bool? IGeoShapeQuery.IgnoreUnmapped { get; set; }

    IFieldLookup IGeoShapeQuery.IndexedShape { get; set; }

    GeoShapeRelation? IGeoShapeQuery.Relation { get; set; }

    IGeoShape IGeoShapeQuery.Shape { get; set; }

    public GeoShapeQueryDescriptor<T> Relation(GeoShapeRelation? relation) => this.Assign<GeoShapeRelation?>(relation, (Action<IGeoShapeQuery, GeoShapeRelation?>) ((a, v) => a.Relation = v));

    public GeoShapeQueryDescriptor<T> IgnoreUnmapped(bool? ignoreUnmapped = true) => this.Assign<bool?>(ignoreUnmapped, (Action<IGeoShapeQuery, bool?>) ((a, v) => a.IgnoreUnmapped = v));

    public GeoShapeQueryDescriptor<T> Shape(Func<GeoShapeDescriptor, IGeoShape> selector) => this.Assign<Func<GeoShapeDescriptor, IGeoShape>>(selector, (Action<IGeoShapeQuery, Func<GeoShapeDescriptor, IGeoShape>>) ((a, v) => a.Shape = v != null ? v(new GeoShapeDescriptor()) : (IGeoShape) null));

    public GeoShapeQueryDescriptor<T> IndexedShape(
      Func<FieldLookupDescriptor<T>, IFieldLookup> selector)
    {
      return this.Assign<Func<FieldLookupDescriptor<T>, IFieldLookup>>(selector, (Action<IGeoShapeQuery, Func<FieldLookupDescriptor<T>, IFieldLookup>>) ((a, v) => a.IndexedShape = v != null ? v(new FieldLookupDescriptor<T>()) : (IFieldLookup) null));
    }
  }
}

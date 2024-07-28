// Decompiled with JetBrains decompiler
// Type: Nest.ShapeQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ShapeQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<ShapeQueryDescriptor<T>, IShapeQuery, T>,
    IShapeQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => ShapeQuery.IsConditionless((IShapeQuery) this);

    bool? IShapeQuery.IgnoreUnmapped { get; set; }

    IFieldLookup IShapeQuery.IndexedShape { get; set; }

    ShapeRelation? IShapeQuery.Relation { get; set; }

    IGeoShape IShapeQuery.Shape { get; set; }

    public ShapeQueryDescriptor<T> Relation(ShapeRelation? relation) => this.Assign<ShapeRelation?>(relation, (Action<IShapeQuery, ShapeRelation?>) ((a, v) => a.Relation = v));

    public ShapeQueryDescriptor<T> IgnoreUnmapped(bool? ignoreUnmapped = true) => this.Assign<bool?>(ignoreUnmapped, (Action<IShapeQuery, bool?>) ((a, v) => a.IgnoreUnmapped = v));

    public ShapeQueryDescriptor<T> Shape(Func<GeoShapeDescriptor, IGeoShape> selector) => this.Assign<Func<GeoShapeDescriptor, IGeoShape>>(selector, (Action<IShapeQuery, Func<GeoShapeDescriptor, IGeoShape>>) ((a, v) => a.Shape = v != null ? v(new GeoShapeDescriptor()) : (IGeoShape) null));

    public ShapeQueryDescriptor<T> IndexedShape(
      Func<FieldLookupDescriptor<T>, IFieldLookup> selector)
    {
      return this.Assign<Func<FieldLookupDescriptor<T>, IFieldLookup>>(selector, (Action<IShapeQuery, Func<FieldLookupDescriptor<T>, IFieldLookup>>) ((a, v) => a.IndexedShape = v != null ? v(new FieldLookupDescriptor<T>()) : (IFieldLookup) null));
    }
  }
}

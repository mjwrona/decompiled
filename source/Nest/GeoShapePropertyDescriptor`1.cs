// Decompiled with JetBrains decompiler
// Type: Nest.GeoShapePropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class GeoShapePropertyDescriptor<T> : 
    DocValuesPropertyDescriptorBase<GeoShapePropertyDescriptor<T>, IGeoShapeProperty, T>,
    IGeoShapeProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public GeoShapePropertyDescriptor()
      : base(FieldType.GeoShape)
    {
    }

    bool? IGeoShapeProperty.IgnoreMalformed { get; set; }

    bool? IGeoShapeProperty.IgnoreZValue { get; set; }

    GeoOrientation? IGeoShapeProperty.Orientation { get; set; }

    GeoStrategy? IGeoShapeProperty.Strategy { get; set; }

    bool? IGeoShapeProperty.Coerce { get; set; }

    public GeoShapePropertyDescriptor<T> Strategy(GeoStrategy? strategy) => this.Assign<GeoStrategy?>(strategy, (Action<IGeoShapeProperty, GeoStrategy?>) ((a, v) => a.Strategy = v));

    public GeoShapePropertyDescriptor<T> Orientation(GeoOrientation? orientation) => this.Assign<GeoOrientation?>(orientation, (Action<IGeoShapeProperty, GeoOrientation?>) ((a, v) => a.Orientation = v));

    public GeoShapePropertyDescriptor<T> IgnoreMalformed(bool? ignoreMalformed = true) => this.Assign<bool?>(ignoreMalformed, (Action<IGeoShapeProperty, bool?>) ((a, v) => a.IgnoreMalformed = v));

    public GeoShapePropertyDescriptor<T> IgnoreZValue(bool? ignoreZValue = true) => this.Assign<bool?>(ignoreZValue, (Action<IGeoShapeProperty, bool?>) ((a, v) => a.IgnoreZValue = v));

    public GeoShapePropertyDescriptor<T> Coerce(bool? coerce = true) => this.Assign<bool?>(coerce, (Action<IGeoShapeProperty, bool?>) ((a, v) => a.Coerce = v));
  }
}

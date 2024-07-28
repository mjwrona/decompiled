// Decompiled with JetBrains decompiler
// Type: Nest.ShapePropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class ShapePropertyDescriptor<T> : 
    DocValuesPropertyDescriptorBase<ShapePropertyDescriptor<T>, IShapeProperty, T>,
    IShapeProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public ShapePropertyDescriptor()
      : base(FieldType.Shape)
    {
    }

    bool? IShapeProperty.IgnoreMalformed { get; set; }

    bool? IShapeProperty.IgnoreZValue { get; set; }

    ShapeOrientation? IShapeProperty.Orientation { get; set; }

    bool? IShapeProperty.Coerce { get; set; }

    public ShapePropertyDescriptor<T> Orientation(ShapeOrientation? orientation) => this.Assign<ShapeOrientation?>(orientation, (Action<IShapeProperty, ShapeOrientation?>) ((a, v) => a.Orientation = v));

    public ShapePropertyDescriptor<T> IgnoreMalformed(bool? ignoreMalformed = true) => this.Assign<bool?>(ignoreMalformed, (Action<IShapeProperty, bool?>) ((a, v) => a.IgnoreMalformed = v));

    public ShapePropertyDescriptor<T> IgnoreZValue(bool? ignoreZValue = true) => this.Assign<bool?>(ignoreZValue, (Action<IShapeProperty, bool?>) ((a, v) => a.IgnoreZValue = v));

    public ShapePropertyDescriptor<T> Coerce(bool? coerce = true) => this.Assign<bool?>(coerce, (Action<IShapeProperty, bool?>) ((a, v) => a.Coerce = v));
  }
}

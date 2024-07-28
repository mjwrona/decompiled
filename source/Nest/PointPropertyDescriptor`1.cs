// Decompiled with JetBrains decompiler
// Type: Nest.PointPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class PointPropertyDescriptor<T> : 
    PropertyDescriptorBase<PointPropertyDescriptor<T>, IPointProperty, T>,
    IPointProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public PointPropertyDescriptor()
      : base(FieldType.Point)
    {
    }

    bool? IPointProperty.IgnoreMalformed { get; set; }

    bool? IPointProperty.IgnoreZValue { get; set; }

    CartesianPoint IPointProperty.NullValue { get; set; }

    public PointPropertyDescriptor<T> IgnoreMalformed(bool? ignoreMalformed = true) => this.Assign<bool?>(ignoreMalformed, (Action<IPointProperty, bool?>) ((a, v) => a.IgnoreMalformed = v));

    public PointPropertyDescriptor<T> IgnoreZValue(bool? ignoreZValue = true) => this.Assign<bool?>(ignoreZValue, (Action<IPointProperty, bool?>) ((a, v) => a.IgnoreZValue = v));

    public PointPropertyDescriptor<T> NullValue(CartesianPoint nullValue) => this.Assign<CartesianPoint>(nullValue, (Action<IPointProperty, CartesianPoint>) ((a, v) => a.NullValue = v));
  }
}

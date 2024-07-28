// Decompiled with JetBrains decompiler
// Type: Nest.GeoPointPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class GeoPointPropertyDescriptor<T> : 
    DocValuesPropertyDescriptorBase<GeoPointPropertyDescriptor<T>, IGeoPointProperty, T>,
    IGeoPointProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public GeoPointPropertyDescriptor()
      : base(FieldType.GeoPoint)
    {
    }

    bool? IGeoPointProperty.IgnoreMalformed { get; set; }

    bool? IGeoPointProperty.IgnoreZValue { get; set; }

    GeoLocation IGeoPointProperty.NullValue { get; set; }

    public GeoPointPropertyDescriptor<T> IgnoreMalformed(bool? ignoreMalformed = true) => this.Assign<bool?>(ignoreMalformed, (Action<IGeoPointProperty, bool?>) ((a, v) => a.IgnoreMalformed = v));

    public GeoPointPropertyDescriptor<T> IgnoreZValue(bool? ignoreZValue = true) => this.Assign<bool?>(ignoreZValue, (Action<IGeoPointProperty, bool?>) ((a, v) => a.IgnoreZValue = v));

    public GeoPointPropertyDescriptor<T> NullValue(GeoLocation defaultValue) => this.Assign<GeoLocation>(defaultValue, (Action<IGeoPointProperty, GeoLocation>) ((a, v) => a.NullValue = v));
  }
}

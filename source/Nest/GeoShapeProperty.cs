// Decompiled with JetBrains decompiler
// Type: Nest.GeoShapeProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class GeoShapeProperty : 
    DocValuesPropertyBase,
    IGeoShapeProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public GeoShapeProperty()
      : base(FieldType.GeoShape)
    {
    }

    public bool? IgnoreMalformed { get; set; }

    public bool? IgnoreZValue { get; set; }

    public GeoOrientation? Orientation { get; set; }

    public GeoStrategy? Strategy { get; set; }

    public bool? Coerce { get; set; }
  }
}

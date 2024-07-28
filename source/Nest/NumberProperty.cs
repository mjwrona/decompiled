// Decompiled with JetBrains decompiler
// Type: Nest.NumberProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class NumberProperty : 
    DocValuesPropertyBase,
    INumberProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public NumberProperty()
      : base(FieldType.Float)
    {
    }

    public NumberProperty(NumberType type)
      : base(type.ToFieldType())
    {
    }

    public double? Boost { get; set; }

    public bool? Coerce { get; set; }

    public INumericFielddata Fielddata { get; set; }

    public bool? IgnoreMalformed { get; set; }

    public bool? Index { get; set; }

    public double? NullValue { get; set; }

    public double? ScalingFactor { get; set; }

    public IInlineScript Script { get; set; }

    public Nest.OnScriptError? OnScriptError { get; set; }
  }
}

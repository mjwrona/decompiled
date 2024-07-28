// Decompiled with JetBrains decompiler
// Type: Nest.DateProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class DateProperty : 
    DocValuesPropertyBase,
    IDateProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public DateProperty()
      : base(FieldType.Date)
    {
    }

    public double? Boost { get; set; }

    public INumericFielddata Fielddata { get; set; }

    public string Format { get; set; }

    public bool? IgnoreMalformed { get; set; }

    public bool? Index { get; set; }

    public DateTime? NullValue { get; set; }

    public int? PrecisionStep { get; set; }
  }
}

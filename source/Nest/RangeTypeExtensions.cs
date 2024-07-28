// Decompiled with JetBrains decompiler
// Type: Nest.RangeTypeExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  internal static class RangeTypeExtensions
  {
    public static FieldType ToFieldType(this RangeType rangeType)
    {
      switch (rangeType)
      {
        case RangeType.IntegerRange:
          return FieldType.IntegerRange;
        case RangeType.FloatRange:
          return FieldType.FloatRange;
        case RangeType.LongRange:
          return FieldType.LongRange;
        case RangeType.DoubleRange:
          return FieldType.DoubleRange;
        case RangeType.DateRange:
          return FieldType.DateRange;
        case RangeType.IpRange:
          return FieldType.IpRange;
        default:
          throw new ArgumentOutOfRangeException(nameof (rangeType), (object) rangeType, (string) null);
      }
    }
  }
}

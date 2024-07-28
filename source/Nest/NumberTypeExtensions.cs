// Decompiled with JetBrains decompiler
// Type: Nest.NumberTypeExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  internal static class NumberTypeExtensions
  {
    public static FieldType ToFieldType(this NumberType numberType)
    {
      switch (numberType)
      {
        case NumberType.Float:
          return FieldType.Float;
        case NumberType.HalfFloat:
          return FieldType.HalfFloat;
        case NumberType.ScaledFloat:
          return FieldType.ScaledFloat;
        case NumberType.Double:
          return FieldType.Double;
        case NumberType.Integer:
          return FieldType.Integer;
        case NumberType.Long:
          return FieldType.Long;
        case NumberType.Short:
          return FieldType.Short;
        case NumberType.Byte:
          return FieldType.Byte;
        case NumberType.UnsignedLong:
          return FieldType.UnsignedLong;
        default:
          throw new ArgumentOutOfRangeException(nameof (numberType), (object) numberType, (string) null);
      }
    }
  }
}

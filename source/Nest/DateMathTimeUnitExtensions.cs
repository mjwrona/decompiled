// Decompiled with JetBrains decompiler
// Type: Nest.DateMathTimeUnitExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public static class DateMathTimeUnitExtensions
  {
    public static string GetStringValue(this DateMathTimeUnit value)
    {
      switch (value)
      {
        case DateMathTimeUnit.Second:
          return "s";
        case DateMathTimeUnit.Minute:
          return "m";
        case DateMathTimeUnit.Hour:
          return "h";
        case DateMathTimeUnit.Day:
          return "d";
        case DateMathTimeUnit.Week:
          return "w";
        case DateMathTimeUnit.Month:
          return "M";
        case DateMathTimeUnit.Year:
          return "y";
        default:
          throw new ArgumentOutOfRangeException(nameof (value), (object) value, (string) null);
      }
    }
  }
}

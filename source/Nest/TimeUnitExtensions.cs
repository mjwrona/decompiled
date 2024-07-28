// Decompiled with JetBrains decompiler
// Type: Nest.TimeUnitExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public static class TimeUnitExtensions
  {
    public static string GetStringValue(this TimeUnit value)
    {
      switch (value)
      {
        case TimeUnit.Nanoseconds:
          return "nanos";
        case TimeUnit.Microseconds:
          return "micros";
        case TimeUnit.Millisecond:
          return "ms";
        case TimeUnit.Second:
          return "s";
        case TimeUnit.Minute:
          return "m";
        case TimeUnit.Hour:
          return "h";
        case TimeUnit.Day:
          return "d";
        default:
          throw new ArgumentOutOfRangeException(nameof (value), (object) value, (string) null);
      }
    }
  }
}

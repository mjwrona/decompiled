// Decompiled with JetBrains decompiler
// Type: Nest.CountFunctionExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public static class CountFunctionExtensions
  {
    public static string GetStringValue(this CountFunction countFunction)
    {
      switch (countFunction)
      {
        case CountFunction.Count:
          return "count";
        case CountFunction.HighCount:
          return "high_count";
        case CountFunction.LowCount:
          return "low_count";
        default:
          throw new ArgumentOutOfRangeException(nameof (countFunction), (object) countFunction, (string) null);
      }
    }

    public static string GetStringValue(this NonZeroCountFunction nonZeroCountFunction)
    {
      switch (nonZeroCountFunction)
      {
        case NonZeroCountFunction.NonZeroCount:
          return "non_zero_count";
        case NonZeroCountFunction.LowNonZeroCount:
          return "low_non_zero_count";
        case NonZeroCountFunction.HighNonZeroCount:
          return "high_non_zero_count";
        default:
          throw new ArgumentOutOfRangeException(nameof (nonZeroCountFunction), (object) nonZeroCountFunction, (string) null);
      }
    }

    public static string GetStringValue(this DistinctCountFunction distinctCountFunction)
    {
      switch (distinctCountFunction)
      {
        case DistinctCountFunction.DistinctCount:
          return "distinct_count";
        case DistinctCountFunction.LowDistinctCount:
          return "low_distinct_count";
        case DistinctCountFunction.HighDistinctCount:
          return "high_distinct_count";
        default:
          throw new ArgumentOutOfRangeException(nameof (distinctCountFunction), (object) distinctCountFunction, (string) null);
      }
    }
  }
}

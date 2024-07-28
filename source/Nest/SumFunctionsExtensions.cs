// Decompiled with JetBrains decompiler
// Type: Nest.SumFunctionsExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public static class SumFunctionsExtensions
  {
    public static string GetStringValue(this SumFunction sumFunction)
    {
      switch (sumFunction)
      {
        case SumFunction.Sum:
          return "sum";
        case SumFunction.HighSum:
          return "high_sum";
        case SumFunction.LowSum:
          return "low_sum";
        default:
          throw new ArgumentOutOfRangeException(nameof (sumFunction), (object) sumFunction, (string) null);
      }
    }

    public static string GetStringValue(this NonNullSumFunction nonNullSumFunction)
    {
      switch (nonNullSumFunction)
      {
        case NonNullSumFunction.NonNullSum:
          return "non_null_sum";
        case NonNullSumFunction.HighNonNullSum:
          return "high_non_null_sum";
        case NonNullSumFunction.LowNonNullSum:
          return "low_non_null_sum";
        default:
          throw new ArgumentOutOfRangeException(nameof (nonNullSumFunction), (object) nonNullSumFunction, (string) null);
      }
    }
  }
}

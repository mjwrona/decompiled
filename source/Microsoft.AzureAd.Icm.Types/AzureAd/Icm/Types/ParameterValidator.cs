// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.ParameterValidator
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.AzureAd.Icm.Types
{
  public static class ParameterValidator
  {
    public static bool IsNotInRangeExclusive<T>(T value, T minValue, T maxValue) where T : IComparable<T> => value.CompareTo(maxValue) >= 0 || value.CompareTo(minValue) <= 0;

    public static bool IsNotInRangeExclusive<T>(T? value, T minValue, T maxValue) where T : struct, IComparable<T>
    {
      if (!value.HasValue)
        return false;
      return value.Value.CompareTo(maxValue) >= 0 || value.Value.CompareTo(minValue) <= 0;
    }

    public static bool IsNotInRangeInclusive<T>(T value, T minValue, T maxValue) where T : IComparable<T> => value.CompareTo(maxValue) > 0 || value.CompareTo(minValue) < 0;

    public static bool IsNotInRangeInclusive<T>(T? value, T minValue, T maxValue) where T : struct, IComparable<T>
    {
      if (!value.HasValue)
        return false;
      return value.Value.CompareTo(maxValue) > 0 || value.Value.CompareTo(minValue) < 0;
    }

    public static bool IsWithinValidDateRange(DateTime date) => !ParameterValidator.IsNotInRangeInclusive<DateTime>(date, IcmConstants.Dates.MinDate, IcmConstants.Dates.MaxDate);

    public static bool IsLessThanOrEqualTo<T>(T value, T minValue) where T : IComparable<T> => value.CompareTo(minValue) <= 0;

    public static bool IsGreaterThan<T>(T value, T maxValue) where T : IComparable<T> => value.CompareTo(maxValue) > 0;

    public static bool IsLessThan<T>(T value, T minValue) where T : IComparable<T> => value.CompareTo(minValue) < 0;
  }
}

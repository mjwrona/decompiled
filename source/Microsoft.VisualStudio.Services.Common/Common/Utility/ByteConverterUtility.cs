// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Utility.ByteConverterUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Common.Utility
{
  public static class ByteConverterUtility
  {
    private const int s_conversionFactor = 1024;

    public static double ConvertUnits(
      long amount,
      ByteConverterUtilityUnit fromUnit,
      ByteConverterUtilityUnit toUnit)
    {
      return ByteConverterUtility.ConvertUnits((double) amount, fromUnit, toUnit);
    }

    public static double ConvertUnits(
      int amount,
      ByteConverterUtilityUnit fromUnit,
      ByteConverterUtilityUnit toUnit)
    {
      return ByteConverterUtility.ConvertUnits((double) amount, fromUnit, toUnit);
    }

    public static double ConvertUnits(
      double amount,
      ByteConverterUtilityUnit fromUnit,
      ByteConverterUtilityUnit toUnit)
    {
      double num = Math.Pow(1024.0, (double) (toUnit - fromUnit));
      return amount / num;
    }

    public static string ConvertBytesToString(
      long amount,
      ByteConverterUtilityUnit fromUnit,
      int decimalPlaces = 3)
    {
      return ByteConverterUtility.ConvertBytesToString((double) amount, fromUnit, decimalPlaces);
    }

    public static string ConvertBytesToString(
      int amount,
      ByteConverterUtilityUnit fromUnit,
      int decimalPlaces = 3)
    {
      return ByteConverterUtility.ConvertBytesToString((double) amount, fromUnit, decimalPlaces);
    }

    public static string ConvertBytesToString(
      double amount,
      ByteConverterUtilityUnit fromUnit,
      int decimalPlaces = 3)
    {
      double num = ByteConverterUtility.ConvertUnits(amount, fromUnit, ByteConverterUtilityUnit.B);
      string[] strArray1 = new string[5]
      {
        "B",
        "KB",
        "MB",
        "GB",
        "TB"
      };
      int index;
      for (index = 0; num >= 1024.0 && index < strArray1.Length - 1; ++index)
        num /= 1024.0;
      string str1 = Math.Round(num, decimalPlaces).ToString();
      string[] strArray2 = str1.Split('.');
      string str2 = str1;
      if (strArray2.Length == 1 && decimalPlaces > 0)
        str2 = str1 + ".".PadRight(decimalPlaces + 1, '0');
      else if (strArray2.Length >= 2)
        str2 = strArray2[0] + "." + strArray2[1].PadRight(decimalPlaces, '0');
      return str2 + " " + strArray1[index];
    }
  }
}

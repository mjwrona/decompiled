// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.YamlFormatter
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Globalization;

namespace YamlDotNet.Serialization
{
  internal static class YamlFormatter
  {
    public static readonly NumberFormatInfo NumberFormat = new NumberFormatInfo()
    {
      CurrencyDecimalSeparator = ".",
      CurrencyGroupSeparator = "_",
      CurrencyGroupSizes = new int[1]{ 3 },
      CurrencySymbol = string.Empty,
      CurrencyDecimalDigits = 99,
      NumberDecimalSeparator = ".",
      NumberGroupSeparator = "_",
      NumberGroupSizes = new int[1]{ 3 },
      NumberDecimalDigits = 99,
      NaNSymbol = ".nan",
      PositiveInfinitySymbol = ".inf",
      NegativeInfinitySymbol = "-.inf"
    };

    public static string FormatNumber(object number) => Convert.ToString(number, (IFormatProvider) YamlFormatter.NumberFormat);

    public static string FormatNumber(double number) => number.ToString("G17", (IFormatProvider) YamlFormatter.NumberFormat);

    public static string FormatNumber(float number) => number.ToString("G17", (IFormatProvider) YamlFormatter.NumberFormat);

    public static string FormatBoolean(object boolean) => !boolean.Equals((object) true) ? "false" : "true";

    public static string FormatDateTime(object dateTime) => ((DateTime) dateTime).ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);

    public static string FormatTimeSpan(object timeSpan) => ((TimeSpan) timeSpan).ToString();
  }
}

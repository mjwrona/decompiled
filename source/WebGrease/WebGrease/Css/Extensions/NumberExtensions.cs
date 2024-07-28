// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Extensions.NumberExtensions
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WebGrease.Css.Extensions
{
  internal static class NumberExtensions
  {
    private static readonly Regex NumberWithUnitsRegex = new Regex("([+-]?[0-9]*\\.?[0-9]+)[a-z]*", RegexOptions.IgnoreCase);

    internal static string UnaryOperator(this float? number)
    {
      float? nullable = number;
      return ((double) nullable.GetValueOrDefault() >= 0.0 ? 0 : (nullable.HasValue ? 1 : 0)) == 0 ? (string) null : "-";
    }

    internal static string CssUnitValue(this float? number, string unit = "px")
    {
      if (!number.HasValue)
        return (string) null;
      float num = Math.Abs(number.Value);
      string format;
      switch (unit)
      {
        case "em":
          format = "{0}em";
          break;
        case "rem":
          format = "{0}rem";
          break;
        default:
          format = "{0}px";
          break;
      }
      if ((double) num == 0.0)
        return "0";
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, new object[1]
      {
        (object) num
      });
    }

    internal static float ParseFloat(this string text)
    {
      float result;
      return !string.IsNullOrWhiteSpace(text) && float.TryParse(text, out result) ? result : 0.0f;
    }

    internal static int SignInt(this string unaryOperator) => unaryOperator == "-" ? -1 : 1;

    internal static bool TryParseZeroBasedNumberValue(this string numberBasedValue)
    {
      if (string.IsNullOrWhiteSpace(numberBasedValue))
        return true;
      Match match = NumberExtensions.NumberWithUnitsRegex.Match(numberBasedValue);
      float result;
      return match.Success && float.TryParse(match.Result("$1"), out result) && (double) Math.Abs(result) == 0.0;
    }
  }
}

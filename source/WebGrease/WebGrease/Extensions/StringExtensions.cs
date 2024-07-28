// Decompiled with JetBrains decompiler
// Type: WebGrease.Extensions.StringExtensions
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WebGrease.Extensions
{
  internal static class StringExtensions
  {
    public static string AsNullIfWhiteSpace(this string value) => !string.IsNullOrWhiteSpace(value) ? value : (string) null;

    public static string InvariantFormat(this string format, params object[] args)
    {
      if (format == null)
        throw new ArgumentNullException(nameof (format));
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args);
    }

    public static TEnum? TryParseToEnum<TEnum>(this string value, TEnum? defaultValue = null) where TEnum : struct
    {
      TEnum result;
      return !Enum.TryParse<TEnum>(value, true, out result) || !Enum.IsDefined(typeof (TEnum), (object) result) ? defaultValue : new TEnum?(result);
    }

    internal static bool IsNullOrWhitespace(this string text) => string.IsNullOrWhiteSpace(text);

    public static bool TryParseBool(this string textToParse)
    {
      bool result;
      return !bool.TryParse(textToParse, out result) || result;
    }

    internal static int TryParseInt32(this string textToParse)
    {
      int result;
      return !int.TryParse(textToParse, out result) ? 0 : result;
    }

    internal static float? TryParseFloat(this string textToParse)
    {
      float result;
      return !float.TryParse(textToParse, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? new float?() : new float?(result);
    }

    internal static IEnumerable<string> SafeSplitSemiColonSeperatedValue(
      this string semicolonSeperatedValue)
    {
      return !string.IsNullOrWhiteSpace(semicolonSeperatedValue) ? ((IEnumerable<string>) semicolonSeperatedValue.Split(Strings.SemicolonSeparator, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (t => t.Trim())) : (IEnumerable<string>) new string[0];
    }
  }
}

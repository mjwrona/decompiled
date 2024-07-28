// Decompiled with JetBrains decompiler
// Type: Tomlyn.Helpers.TomlFormatHelper
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Globalization;
using Tomlyn.Model;
using Tomlyn.Text;


#nullable enable
namespace Tomlyn.Helpers
{
  public class TomlFormatHelper
  {
    public static string ToString(bool b) => !b ? "false" : "true";

    public static string ToString(string s, TomlPropertyDisplayKind displayKind)
    {
      switch (displayKind)
      {
        case TomlPropertyDisplayKind.StringMulti:
          return "\"\"\"" + s.EscapeForToml(true) + "\"\"\"";
        case TomlPropertyDisplayKind.StringLiteral:
          if (TomlFormatHelper.IsSafeStringLiteral(s))
            return "'" + s + "'";
          break;
        case TomlPropertyDisplayKind.StringLiteralMulti:
          if (TomlFormatHelper.IsSafeStringLiteralMulti(s))
            return "'''" + s + "'''";
          break;
      }
      return "\"" + s.EscapeForToml() + "\"";
    }

    public static string ToString(int i32, TomlPropertyDisplayKind displayKind)
    {
      string str;
      switch (displayKind)
      {
        case TomlPropertyDisplayKind.IntegerHexadecimal:
          str = string.Format("0x{0:x8}", (object) i32);
          break;
        case TomlPropertyDisplayKind.IntegerOctal:
          str = "0o" + Convert.ToString(i32, 8);
          break;
        case TomlPropertyDisplayKind.IntegerBinary:
          str = "0b" + Convert.ToString(i32, 2);
          break;
        default:
          str = i32.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          break;
      }
      return str;
    }

    public static string ToString(long i64, TomlPropertyDisplayKind displayKind)
    {
      string str;
      switch (displayKind)
      {
        case TomlPropertyDisplayKind.IntegerHexadecimal:
          str = string.Format("0x{0:x16}", (object) i64);
          break;
        case TomlPropertyDisplayKind.IntegerOctal:
          str = "0o" + Convert.ToString(i64, 8);
          break;
        case TomlPropertyDisplayKind.IntegerBinary:
          str = "0b" + Convert.ToString(i64, 2);
          break;
        default:
          str = i64.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          break;
      }
      return str;
    }

    public static string ToString(uint u32, TomlPropertyDisplayKind displayKind)
    {
      string str;
      switch (displayKind)
      {
        case TomlPropertyDisplayKind.IntegerHexadecimal:
          str = string.Format("0x{0:x8}", (object) u32);
          break;
        case TomlPropertyDisplayKind.IntegerOctal:
          str = "0o" + Convert.ToString((long) u32, 8);
          break;
        case TomlPropertyDisplayKind.IntegerBinary:
          str = "0b" + Convert.ToString((long) u32, 2);
          break;
        default:
          str = u32.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          break;
      }
      return str;
    }

    public static string ToString(ulong u64, TomlPropertyDisplayKind displayKind) => TomlFormatHelper.ToString((long) u64, displayKind);

    public static string ToString(sbyte i8, TomlPropertyDisplayKind displayKind)
    {
      string str;
      switch (displayKind)
      {
        case TomlPropertyDisplayKind.IntegerHexadecimal:
          str = string.Format("0x{0:x2}", (object) i8);
          break;
        case TomlPropertyDisplayKind.IntegerOctal:
          str = "0o" + Convert.ToString((short) i8, 8);
          break;
        case TomlPropertyDisplayKind.IntegerBinary:
          str = "0b" + Convert.ToString((short) i8, 2);
          break;
        default:
          str = i8.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          break;
      }
      return str;
    }

    public static string ToString(byte u8, TomlPropertyDisplayKind displayKind)
    {
      string str;
      switch (displayKind)
      {
        case TomlPropertyDisplayKind.IntegerHexadecimal:
          str = string.Format("0x{0:x2}", (object) u8);
          break;
        case TomlPropertyDisplayKind.IntegerOctal:
          str = "0o" + Convert.ToString(u8, 8);
          break;
        case TomlPropertyDisplayKind.IntegerBinary:
          str = "0b" + Convert.ToString(u8, 2);
          break;
        default:
          str = u8.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          break;
      }
      return str;
    }

    public static string ToString(short i16, TomlPropertyDisplayKind displayKind)
    {
      string str;
      switch (displayKind)
      {
        case TomlPropertyDisplayKind.IntegerHexadecimal:
          str = string.Format("0x{0:x4}", (object) i16);
          break;
        case TomlPropertyDisplayKind.IntegerOctal:
          str = "0o" + Convert.ToString(i16, 8);
          break;
        case TomlPropertyDisplayKind.IntegerBinary:
          str = "0b" + Convert.ToString(i16, 2);
          break;
        default:
          str = i16.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          break;
      }
      return str;
    }

    public static string ToString(ushort u16, TomlPropertyDisplayKind displayKind)
    {
      string str;
      switch (displayKind)
      {
        case TomlPropertyDisplayKind.IntegerHexadecimal:
          str = string.Format("0x{0:x4}", (object) u16);
          break;
        case TomlPropertyDisplayKind.IntegerOctal:
          str = "0o" + Convert.ToString((int) u16, 8);
          break;
        case TomlPropertyDisplayKind.IntegerBinary:
          str = "0b" + Convert.ToString((int) u16, 2);
          break;
        default:
          str = u16.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          break;
      }
      return str;
    }

    public static string ToString(float value)
    {
      if (float.IsNaN(value))
        return "nan";
      if (float.IsPositiveInfinity(value))
        return "+inf";
      return float.IsNegativeInfinity(value) ? "-inf" : TomlFormatHelper.AppendDecimalPoint(value.ToString("g9", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static string ToString(double value)
    {
      if (double.IsNaN(value))
        return "nan";
      if (double.IsPositiveInfinity(value))
        return "+inf";
      return double.IsNegativeInfinity(value) ? "-inf" : TomlFormatHelper.AppendDecimalPoint(value.ToString("g16", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static string ToString(TomlDateTime tomlDateTime) => tomlDateTime.ToString();

    public static string ToString(DateTime dateTime, TomlPropertyDisplayKind displayKind) => new TomlDateTime((DateTimeOffset) dateTime, 0, TomlFormatHelper.GetDateTimeDisplayKind(displayKind == TomlPropertyDisplayKind.Default ? TomlPropertyDisplayKind.LocalDateTime : displayKind)).ToString();

    public static string ToString(
      DateTimeOffset dateTimeOffset,
      TomlPropertyDisplayKind displayKind)
    {
      return new TomlDateTime(dateTimeOffset, 0, TomlFormatHelper.GetDateTimeDisplayKind(displayKind == TomlPropertyDisplayKind.Default ? TomlPropertyDisplayKind.OffsetDateTimeByZ : displayKind)).ToString();
    }

    private static string AppendDecimalPoint(string text)
    {
      if (text == "0")
        return "0.0";
      for (int index = 0; index < text.Length; ++index)
      {
        switch (text[index])
        {
          case '.':
          case 'E':
          case 'e':
            return text;
          default:
            continue;
        }
      }
      return text + ".0";
    }

    private static bool IsSafeStringLiteral(string text)
    {
      foreach (char c in text)
      {
        if (char.IsControl(c) || c == '\'')
          return false;
      }
      return true;
    }

    private static bool IsSafeStringLiteralMulti(string text)
    {
      for (int index = 0; index < text.Length; ++index)
      {
        char c = text[index];
        if ((c != '\r' || index + 1 >= text.Length || text[index + 1] != '\n') && c != '\n' && (char.IsControl(c) || c == '\'' && index + 1 < text.Length && text[index + 1] == '\'' && index + 2 < text.Length && text[index + 2] == '\''))
          return false;
      }
      return true;
    }

    private static TomlDateTimeKind GetDateTimeDisplayKind(TomlPropertyDisplayKind kind)
    {
      switch (kind)
      {
        case TomlPropertyDisplayKind.OffsetDateTimeByNumber:
          return TomlDateTimeKind.OffsetDateTimeByNumber;
        case TomlPropertyDisplayKind.LocalDateTime:
          return TomlDateTimeKind.LocalDateTime;
        case TomlPropertyDisplayKind.LocalDate:
          return TomlDateTimeKind.LocalDate;
        case TomlPropertyDisplayKind.LocalTime:
          return TomlDateTimeKind.LocalTime;
        default:
          return TomlDateTimeKind.OffsetDateTimeByZ;
      }
    }
  }
}

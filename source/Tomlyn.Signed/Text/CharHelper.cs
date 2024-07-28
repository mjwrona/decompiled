// Decompiled with JetBrains decompiler
// Type: Tomlyn.Text.CharHelper
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Runtime.CompilerServices;
using System.Text;


#nullable enable
namespace Tomlyn.Text
{
  internal static class CharHelper
  {
    public static readonly Func<char32, bool> IsHexFunc = new Func<char32, bool>(CharHelper.IsHex);
    public static readonly Func<char32, bool> IsOctalFunc = new Func<char32, bool>(CharHelper.IsOctal);
    public static readonly Func<char32, bool> IsBinaryFunc = new Func<char32, bool>(CharHelper.IsBinary);
    public static readonly Func<char32, int> HexToDecFunc = new Func<char32, int>(CharHelper.HexToDecimal);
    public static readonly Func<char32, int> OctalToDecFunc = new Func<char32, int>(CharHelper.OctalToDecimal);
    public static readonly Func<char32, int> BinaryToDecFunc = new Func<char32, int>(CharHelper.BinaryToDecimal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsControlCharacter(char32 c) => (int) c <= 31 || (int) c == (int) sbyte.MaxValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKeyStart(char32 c)
    {
      if ((int) c >= 97 && (int) c <= 122 || (int) c >= 65 && (int) c <= 90 || (int) c == 95 || (int) c == 45)
        return true;
      return (int) c >= 48 && (int) c <= 57;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKeyContinue(char32 c)
    {
      if ((int) c >= 97 && (int) c <= 122 || (int) c >= 65 && (int) c <= 90 || (int) c == 95 || (int) c == 45)
        return true;
      return (int) c >= 48 && (int) c <= 57;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIdentifierStart(char32 c)
    {
      if ((int) c >= 97 && (int) c <= 122)
        return true;
      return (int) c >= 65 && (int) c <= 90;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIdentifierContinue(char32 c)
    {
      if ((int) c >= 97 && (int) c <= 122)
        return true;
      return (int) c >= 65 && (int) c <= 90;
    }

    public static bool IsValidUnicodeScalarValue(char32 c)
    {
      if ((int) c >= 0 && (int) c <= 55295)
        return true;
      return (int) c >= 57344 && (int) c < 1114111;
    }

    public static bool IsDigit(char32 c) => (int) c >= 48 && (int) c <= 57;

    public static bool IsDateTime(char32 c) => CharHelper.IsDigit(c) || (int) c == 58 || (int) c == 45 || (int) c == 90 || (int) c == 84 || (int) c == 122 || (int) c == 116 || (int) c == 43 || (int) c == 46;

    public static string EscapeForToml(this string text, bool allowNewLinesAndSpace = false)
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      for (int index = 0; index < text.Length; ++index)
      {
        char ch = text[index];
        string str = (string) null;
        if (allowNewLinesAndSpace)
        {
          if (ch == '\r' && index + 1 < text.Length && text[index + 1] == '\n')
          {
            str = "\r\n";
            ++index;
            ch = '\n';
          }
          else
          {
            switch (ch)
            {
              case '\t':
                str = "\t";
                break;
              case '\n':
                str = "\n";
                break;
            }
          }
        }
        if (str == null)
          str = CharHelper.EscapeChar(text[index]);
        if (str != null)
        {
          if (stringBuilder == null)
          {
            stringBuilder = new StringBuilder(text.Length * 2);
            stringBuilder.Append(text.Substring(0, index));
          }
          stringBuilder.Append(str);
        }
        else
          stringBuilder?.Append(ch);
      }
      return stringBuilder?.ToString() ?? text;
    }

    private static string? EscapeChar(char c)
    {
      if (c >= ' ' && c != '"' && c != '\\' && !char.IsControl(c))
        return (string) null;
      switch (c)
      {
        case '\b':
          return "\\b";
        case '\t':
          return "\\t";
        case '\n':
          return "\\n";
        case '\f':
          return "\\f";
        case '\r':
          return "\\r";
        case '"':
          return "\\\"";
        case '\\':
          return "\\\\";
        default:
          return string.Format("\\u{0:X4}", (object) (ushort) c);
      }
    }

    public static string? ToPrintableString(this string? text)
    {
      if (text == null)
        return (string) null;
      StringBuilder stringBuilder = (StringBuilder) null;
      for (int index = 0; index < text.Length; ++index)
      {
        char ch = text[index];
        string printableString = text[index].ToPrintableString();
        if (printableString != null)
        {
          if (stringBuilder == null)
          {
            stringBuilder = new StringBuilder(text.Length * 2);
            stringBuilder.Append(text.Substring(0, index));
          }
          stringBuilder.Append(printableString);
        }
        else
          stringBuilder?.Append(ch);
      }
      return stringBuilder?.ToString() ?? text;
    }

    public static string? ToPrintableString(this char c)
    {
      if (c >= ' ' && !CharHelper.IsWhiteSpace((char32) (int) c))
        return (string) null;
      switch (c)
      {
        case '\a':
          return "\\a";
        case '\b':
          return "\\b";
        case '\t':
          return "\\t";
        case '\n':
          return "␤";
        case '\v':
          return "\\v";
        case '\f':
          return "\\f";
        case '\r':
          return "␍";
        case ' ':
          return " ";
        default:
          return string.Format("\\u{0:X};", (object) (int) c);
      }
    }

    public static bool IsWhiteSpace(char32 c) => (int) c == 32 || (int) c == 9;

    public static bool IsWhiteSpaceOrNewLine(char32 c) => (int) c == 32 || (int) c == 9 || (int) c == 13 || (int) c == 10;

    public static bool IsNewLine(char32 c) => (int) c == 13 || (int) c == 10;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char32? ToUtf8(byte[] buffer, ref int position)
    {
      if (position < buffer.Length)
      {
        sbyte c1 = (sbyte) buffer[position++];
        return new char32?(c1 >= (sbyte) 0 ? (char32) (int) c1 : CharHelper.DecodeUTF8_24(buffer, ref position, c1));
      }
      position = buffer.Length;
      return new char32?();
    }

    private static char32 DecodeUTF8_24(byte[] buffer, ref int position, sbyte c1)
    {
      int num1 = 0;
      while (c1 < (sbyte) 0)
      {
        c1 <<= 1;
        ++num1;
      }
      if (num1 > 4 || position + num1 - 1 > buffer.Length)
        throw new CharReaderException(string.Format("Invalid UTF8 character at position {0}", (object) position));
      int num2 = (int) c1 << 6 - num1 | (int) buffer[position++] & 63;
      if (num1 == 2)
        return (char32) num2;
      if (num1 >= 3)
        num2 = num2 << 6 | (int) buffer[position++] & 63;
      if (num1 == 4)
        num2 = num2 << 6 | (int) buffer[position++] & 63;
      return (char32) num2;
    }

    public static int HexToDecimal(char32 c)
    {
      if ((int) c >= 48 && (int) c <= 57)
        return (int) c - 48;
      return (int) c < 97 || (int) c > 102 ? (int) c - 65 + 10 : (int) c - 97 + 10;
    }

    public static int OctalToDecimal(char32 c) => (int) c - 48;

    public static int BinaryToDecimal(char32 c) => (int) c - 48;

    private static bool IsHex(char32 c)
    {
      if ((int) c >= 48 && (int) c <= 57 || (int) c >= 97 && (int) c <= 102)
        return true;
      return (int) c >= 65 && (int) c <= 70;
    }

    private static bool IsOctal(char32 c) => (int) c >= 48 && (int) c <= 55;

    private static bool IsBinary(char32 c) => (int) c == 48 || (int) c == 49;

    public static void AppendUtf32(this StringBuilder builder, char32 utf32)
    {
      if ((int) utf32 < 65536)
      {
        builder.Append((char) (int) utf32);
      }
      else
      {
        utf32 = (char32) ((int) utf32 - 65536);
        builder.Append((char) ((int) utf32 / 1024 + 55296));
        builder.Append((char) ((int) utf32 % 1024 + 56320));
      }
    }
  }
}

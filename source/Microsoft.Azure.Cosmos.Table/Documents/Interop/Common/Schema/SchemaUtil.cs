// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Interop.Common.Schema.SchemaUtil
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Documents.Interop.Common.Schema
{
  internal static class SchemaUtil
  {
    public static object ConvertEdmType(object value)
    {
      if (value == null)
        return (object) "null";
      Type type = value.GetType();
      if (type.IsValueType)
      {
        if (type == typeof (bool))
          return !(bool) value ? (object) "false" : (object) "true";
        if (type == typeof (byte) || type == typeof (sbyte) || type == typeof (Decimal) || type == typeof (double) || type == typeof (float) || type == typeof (int) || type == typeof (uint) || type == typeof (short) || type == typeof (ushort))
          return (object) Convert.ToString(value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
        if (type == typeof (long))
          return (object) SchemaUtil.GetQuotedString(((long) value).ToString("D20", (IFormatProvider) CultureInfo.InvariantCulture));
        if (type == typeof (ulong))
          return (object) SchemaUtil.GetQuotedString(((ulong) value).ToString("D20", (IFormatProvider) CultureInfo.InvariantCulture));
        if (type == typeof (Guid))
          return (object) SchemaUtil.GetQuotedString(((Guid) value).ToString());
        if (type == typeof (DateTime))
          return (object) SchemaUtil.GetQuotedString(SchemaUtil.GetUtcTicksString((DateTime) value));
        if (type == typeof (DateTimeOffset))
          return (object) SchemaUtil.GetQuotedString(SchemaUtil.GetUtcTicksString(((DateTimeOffset) value).UtcDateTime));
      }
      if (type == typeof (string))
        return (object) SchemaUtil.GetQuotedString((string) value);
      if (type == typeof (byte[]))
        return (object) SchemaUtil.GetQuotedString(SchemaUtil.BytesToString((byte[]) value));
      throw new InvalidCastException();
    }

    public static object ConvertEdmType<T>(T? value) where T : struct => !value.HasValue ? (object) "null" : SchemaUtil.ConvertEdmType((object) value.Value);

    public static bool IsNullable(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);

    public static string GetTimestampEpochSecondString(object dateTime)
    {
      Type type = dateTime != null ? dateTime.GetType() : throw new ArgumentNullException(nameof (dateTime), "Timestamp value shouldn't be null");
      if (type.IsValueType && type == typeof (DateTimeOffset))
        return (((DateTimeOffset) dateTime).UtcDateTime.Ticks / 10000000L - 62135596800L).ToString();
      throw new InvalidCastException("Timestamp value should be DateTimeOffset type");
    }

    public static string GetUtcTicksString(DateTime dateTime) => dateTime.ToUniversalTime().Ticks.ToString("D20", (IFormatProvider) CultureInfo.InvariantCulture);

    public static DateTime GetDateTimeFromUtcTicks(long utcTicks) => new DateTime(utcTicks, DateTimeKind.Utc);

    public static string BytesToString(byte[] bytes)
    {
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < bytes.Length; ++index)
        stringBuilder.Append((char) bytes[index]);
      return stringBuilder.ToString();
    }

    public static byte[] StringToBytes(string bytesString)
    {
      byte[] bytes = bytesString != null ? new byte[bytesString.Length] : throw new ArgumentNullException(nameof (bytesString));
      for (int index = 0; index < bytesString.Length; ++index)
        bytes[index] = (byte) bytesString[index];
      return bytes;
    }

    public static bool IsIdentifierCharacter(char c) => c == '_' || char.IsLetterOrDigit(c);

    public static int GetHexValue(char c)
    {
      if (c >= '0' && c <= '9')
        return (int) c - 48;
      if (c >= 'a' && c <= 'f')
        return (int) c - 97 + 10;
      return c >= 'A' && c <= 'F' ? (int) c - 65 + 10 : -1;
    }

    public static bool IsHexCharacter(char c) => c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F';

    public static string GetQuotedString(string value, bool escapeValue = true, char quotationCharacter = '"')
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      string str = escapeValue ? SchemaUtil.GetEscapedString(value) : value;
      return quotationCharacter.ToString() + str + quotationCharacter.ToString();
    }

    public static string GetEscapedString(string value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (value.All<char>((Func<char, bool>) (c => !SchemaUtil.IsEscapedCharacter(c))))
        return value;
      StringBuilder stringBuilder = new StringBuilder(value.Length);
      foreach (char c in value)
      {
        switch (c)
        {
          case '\b':
            stringBuilder.Append("\\b");
            break;
          case '\t':
            stringBuilder.Append("\\t");
            break;
          case '\n':
            stringBuilder.Append("\\n");
            break;
          case '\f':
            stringBuilder.Append("\\f");
            break;
          case '\r':
            stringBuilder.Append("\\r");
            break;
          case '"':
            stringBuilder.Append("\\\"");
            break;
          case '\\':
            stringBuilder.Append("\\\\");
            break;
          default:
            switch (char.GetUnicodeCategory(c))
            {
              case UnicodeCategory.UppercaseLetter:
              case UnicodeCategory.LowercaseLetter:
              case UnicodeCategory.TitlecaseLetter:
              case UnicodeCategory.OtherLetter:
              case UnicodeCategory.DecimalDigitNumber:
              case UnicodeCategory.LetterNumber:
              case UnicodeCategory.OtherNumber:
              case UnicodeCategory.SpaceSeparator:
              case UnicodeCategory.ConnectorPunctuation:
              case UnicodeCategory.DashPunctuation:
              case UnicodeCategory.OpenPunctuation:
              case UnicodeCategory.ClosePunctuation:
              case UnicodeCategory.InitialQuotePunctuation:
              case UnicodeCategory.FinalQuotePunctuation:
              case UnicodeCategory.OtherPunctuation:
              case UnicodeCategory.MathSymbol:
              case UnicodeCategory.CurrencySymbol:
              case UnicodeCategory.ModifierSymbol:
              case UnicodeCategory.OtherSymbol:
                stringBuilder.Append(c);
                continue;
              default:
                stringBuilder.AppendFormat("\\u{0:x4}", (object) (int) c);
                continue;
            }
        }
      }
      return stringBuilder.ToString();
    }

    private static bool IsEscapedCharacter(char c)
    {
      switch (c)
      {
        case '\b':
        case '\t':
        case '\n':
        case '\f':
        case '\r':
        case '"':
        case '\\':
          return true;
        default:
          switch (char.GetUnicodeCategory(c))
          {
            case UnicodeCategory.UppercaseLetter:
            case UnicodeCategory.LowercaseLetter:
            case UnicodeCategory.TitlecaseLetter:
            case UnicodeCategory.OtherLetter:
            case UnicodeCategory.DecimalDigitNumber:
            case UnicodeCategory.LetterNumber:
            case UnicodeCategory.OtherNumber:
            case UnicodeCategory.SpaceSeparator:
            case UnicodeCategory.ConnectorPunctuation:
            case UnicodeCategory.DashPunctuation:
            case UnicodeCategory.OpenPunctuation:
            case UnicodeCategory.ClosePunctuation:
            case UnicodeCategory.InitialQuotePunctuation:
            case UnicodeCategory.FinalQuotePunctuation:
            case UnicodeCategory.OtherPunctuation:
            case UnicodeCategory.MathSymbol:
            case UnicodeCategory.CurrencySymbol:
            case UnicodeCategory.ModifierSymbol:
            case UnicodeCategory.OtherSymbol:
              return false;
            default:
              return true;
          }
      }
    }
  }
}

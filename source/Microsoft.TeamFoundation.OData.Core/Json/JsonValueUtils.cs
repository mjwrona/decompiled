// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.JsonValueUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Buffers;
using Microsoft.OData.Edm;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.OData.Json
{
  internal static class JsonValueUtils
  {
    internal static readonly string ODataJsonPositiveInfinitySymbol = "INF";
    internal static readonly string ODataJsonNegativeInfinitySymbol = "-INF";
    internal static readonly NumberFormatInfo ODataNumberFormatInfo = JsonValueUtils.InitializeODataNumberFormatInfo();
    private static readonly long JsonDateTimeMinTimeTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
    private static readonly char[] DoubleIndicatingCharacters = new char[3]
    {
      '.',
      'e',
      'E'
    };
    private static readonly string[] SpecialCharToEscapedStringMap = JsonValueUtils.CreateSpecialCharToEscapedStringMap();

    internal static void WriteValue(
      TextWriter writer,
      char value,
      ODataStringEscapeOption stringEscapeOption)
    {
      if (stringEscapeOption == ODataStringEscapeOption.EscapeNonAscii || value <= '\u007F')
      {
        string charToEscapedString = JsonValueUtils.SpecialCharToEscapedStringMap[(int) value];
        if (charToEscapedString != null)
        {
          writer.Write(charToEscapedString);
          return;
        }
      }
      writer.Write(value);
    }

    internal static void WriteValue(TextWriter writer, bool value) => writer.Write(value ? "true" : "false");

    internal static void WriteValue(TextWriter writer, int value) => writer.Write(value.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    internal static void WriteValue(TextWriter writer, float value)
    {
      if (float.IsInfinity(value) || float.IsNaN(value))
        JsonValueUtils.WriteQuoted(writer, value.ToString((IFormatProvider) JsonValueUtils.ODataNumberFormatInfo));
      else
        writer.Write(XmlConvert.ToString(value));
    }

    internal static void WriteValue(TextWriter writer, short value) => writer.Write(value.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    internal static void WriteValue(TextWriter writer, long value) => writer.Write(value.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    internal static void WriteValue(TextWriter writer, double value)
    {
      if (JsonSharedUtils.IsDoubleValueSerializedAsString(value))
      {
        JsonValueUtils.WriteQuoted(writer, value.ToString((IFormatProvider) JsonValueUtils.ODataNumberFormatInfo));
      }
      else
      {
        string str = XmlConvert.ToString(value);
        writer.Write(str);
        if (str.IndexOfAny(JsonValueUtils.DoubleIndicatingCharacters) >= 0)
          return;
        writer.Write(".0");
      }
    }

    internal static void WriteValue(TextWriter writer, Guid value) => JsonValueUtils.WriteQuoted(writer, value.ToString());

    internal static void WriteValue(TextWriter writer, Decimal value) => writer.Write(value.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.OData.Json.JsonValueUtils.WriteQuoted(System.IO.TextWriter,System.String)", Justification = "Constant defined by the JSON spec.")]
    internal static void WriteValue(
      TextWriter writer,
      DateTimeOffset value,
      ODataJsonDateTimeFormat dateTimeFormat)
    {
      int totalMinutes = (int) value.Offset.TotalMinutes;
      switch (dateTimeFormat)
      {
        case ODataJsonDateTimeFormat.ODataDateTime:
          string text1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\\/Date({0}{1}{2:D4})\\/", new object[3]
          {
            (object) JsonValueUtils.DateTimeTicksToJsonTicks(value.Ticks),
            totalMinutes >= 0 ? (object) "+" : (object) string.Empty,
            (object) totalMinutes
          });
          JsonValueUtils.WriteQuoted(writer, text1);
          break;
        case ODataJsonDateTimeFormat.ISO8601DateTime:
          string text2 = XmlConvert.ToString(value);
          JsonValueUtils.WriteQuoted(writer, text2);
          break;
      }
    }

    internal static void WriteValue(TextWriter writer, TimeSpan value) => JsonValueUtils.WriteQuoted(writer, EdmValueWriter.DurationAsXml(value));

    internal static void WriteValue(TextWriter writer, TimeOfDay value) => JsonValueUtils.WriteQuoted(writer, value.ToString());

    internal static void WriteValue(TextWriter writer, Date value) => JsonValueUtils.WriteQuoted(writer, value.ToString());

    internal static void WriteValue(TextWriter writer, byte value) => writer.Write(value.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    internal static void WriteValue(TextWriter writer, sbyte value) => writer.Write(value.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    internal static void WriteValue(
      TextWriter writer,
      string value,
      ODataStringEscapeOption stringEscapeOption,
      ref char[] buffer,
      ICharArrayPool arrayPool = null)
    {
      if (value == null)
        writer.Write("null");
      else
        JsonValueUtils.WriteEscapedJsonString(writer, value, stringEscapeOption, ref buffer, arrayPool);
    }

    internal static void WriteValue(
      TextWriter writer,
      byte[] value,
      ref char[] buffer,
      ICharArrayPool arrayPool = null)
    {
      if (value == null)
      {
        writer.Write("null");
      }
      else
      {
        writer.Write('"');
        JsonValueUtils.WriteBinaryString(writer, value, ref buffer, arrayPool);
        writer.Write('"');
      }
    }

    internal static void WriteBinaryString(
      TextWriter writer,
      byte[] value,
      ref char[] buffer,
      ICharArrayPool arrayPool)
    {
      buffer = BufferUtils.InitializeBufferIfRequired(arrayPool, buffer);
      int num = buffer.Length * 3 / 4;
      for (int offsetIn = 0; offsetIn < value.Length; offsetIn += num)
      {
        int length = num;
        if (offsetIn + length > value.Length)
          length = value.Length - offsetIn;
        int base64CharArray = Convert.ToBase64CharArray(value, offsetIn, length, buffer, 0);
        writer.Write(buffer, 0, base64CharArray);
      }
    }

    internal static void WriteEscapedJsonString(
      TextWriter writer,
      string inputString,
      ODataStringEscapeOption stringEscapeOption,
      ref char[] buffer,
      ICharArrayPool bufferPool = null)
    {
      writer.Write('"');
      JsonValueUtils.WriteEscapedJsonStringValue(writer, inputString, stringEscapeOption, ref buffer, bufferPool);
      writer.Write('"');
    }

    internal static void WriteEscapedJsonStringValue(
      TextWriter writer,
      string inputString,
      ODataStringEscapeOption stringEscapeOption,
      ref char[] buffer,
      ICharArrayPool bufferPool)
    {
      int firstIndex;
      if (!JsonValueUtils.CheckIfStringHasSpecialChars(inputString, stringEscapeOption, out firstIndex))
      {
        writer.Write(inputString);
      }
      else
      {
        int length1 = inputString.Length;
        buffer = BufferUtils.InitializeBufferIfRequired(bufferPool, buffer);
        int length2 = buffer.Length;
        int num1 = 0;
        int num2 = 0;
        while (num2 < firstIndex)
        {
          int count = firstIndex - num2;
          if (count >= length2)
          {
            inputString.CopyTo(num2, buffer, 0, length2);
            writer.Write(buffer, 0, length2);
            num2 += length2;
          }
          else
          {
            inputString.CopyTo(num2, buffer, 0, count);
            num1 = count;
            num2 += count;
          }
        }
        for (; num2 < length1; ++num2)
          num1 = JsonValueUtils.EscapeAndWriteCharToBuffer(writer, inputString[num2], buffer, num1, stringEscapeOption);
        if (num1 <= 0)
          return;
        writer.Write(buffer, 0, num1);
      }
    }

    internal static void WriteEscapedCharArray(
      TextWriter writer,
      char[] inputArray,
      int inputArrayOffset,
      int inputArrayCount,
      ODataStringEscapeOption stringEscapeOption,
      ref char[] buffer,
      ICharArrayPool bufferPool)
    {
      int num = 0;
      buffer = BufferUtils.InitializeBufferIfRequired(bufferPool, buffer);
      for (; inputArrayOffset < inputArrayCount; ++inputArrayOffset)
        num = JsonValueUtils.EscapeAndWriteCharToBuffer(writer, inputArray[inputArrayOffset], buffer, num, stringEscapeOption);
      if (num <= 0)
        return;
      writer.Write(buffer, 0, num);
    }

    internal static long JsonTicksToDateTimeTicks(long ticks) => ticks * 10000L + JsonValueUtils.JsonDateTimeMinTimeTicks;

    internal static string GetEscapedJsonString(string inputString)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int startIndex = 0;
      int length1 = inputString.Length;
      for (int index1 = 0; index1 < length1; ++index1)
      {
        char index2 = inputString[index1];
        if (JsonValueUtils.SpecialCharToEscapedStringMap[(int) index2] != null)
        {
          int length2 = index1 - startIndex;
          if (length2 > 0)
            stringBuilder.Append(inputString.Substring(startIndex, length2));
          stringBuilder.Append(JsonValueUtils.SpecialCharToEscapedStringMap[(int) index2]);
          startIndex = index1 + 1;
        }
      }
      int length3 = length1 - startIndex;
      if (length3 > 0)
        stringBuilder.Append(inputString.Substring(startIndex, length3));
      return stringBuilder.ToString();
    }

    private static int EscapeAndWriteCharToBuffer(
      TextWriter writer,
      char character,
      char[] buffer,
      int bufferIndex,
      ODataStringEscapeOption stringEscapeOption)
    {
      int length1 = buffer.Length;
      string str = (string) null;
      if (stringEscapeOption == ODataStringEscapeOption.EscapeNonAscii || character <= '\u007F')
        str = JsonValueUtils.SpecialCharToEscapedStringMap[(int) character];
      if (str == null)
      {
        buffer[bufferIndex] = character;
        ++bufferIndex;
      }
      else
      {
        int length2 = str.Length;
        if (bufferIndex + length2 > length1)
        {
          writer.Write(buffer, 0, bufferIndex);
          bufferIndex = 0;
        }
        str.CopyTo(0, buffer, bufferIndex, length2);
        bufferIndex += length2;
      }
      if (bufferIndex >= length1)
      {
        writer.Write(buffer, 0, bufferIndex);
        bufferIndex = 0;
      }
      return bufferIndex;
    }

    private static bool CheckIfStringHasSpecialChars(
      string inputString,
      ODataStringEscapeOption stringEscapeOption,
      out int firstIndex)
    {
      firstIndex = -1;
      int length = inputString.Length;
      for (int index1 = 0; index1 < length; ++index1)
      {
        char index2 = inputString[index1];
        if ((stringEscapeOption != ODataStringEscapeOption.EscapeOnlyControls || index2 < '\u007F') && JsonValueUtils.SpecialCharToEscapedStringMap[(int) index2] != null)
        {
          firstIndex = index1;
          return true;
        }
      }
      return false;
    }

    private static NumberFormatInfo InitializeODataNumberFormatInfo()
    {
      NumberFormatInfo numberFormatInfo = (NumberFormatInfo) CultureInfo.InvariantCulture.NumberFormat.Clone();
      numberFormatInfo.PositiveInfinitySymbol = JsonValueUtils.ODataJsonPositiveInfinitySymbol;
      numberFormatInfo.NegativeInfinitySymbol = JsonValueUtils.ODataJsonNegativeInfinitySymbol;
      return numberFormatInfo;
    }

    private static void WriteQuoted(TextWriter writer, string text)
    {
      writer.Write('"');
      writer.Write(text);
      writer.Write('"');
    }

    private static long DateTimeTicksToJsonTicks(long ticks) => (ticks - JsonValueUtils.JsonDateTimeMinTimeTicks) / 10000L;

    private static string[] CreateSpecialCharToEscapedStringMap()
    {
      string[] escapedStringMap = new string[65536];
      for (int index = 0; index <= (int) ushort.MaxValue; ++index)
      {
        if (index < 32 || index > (int) sbyte.MaxValue)
          escapedStringMap[index] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\\u{0:x4}", new object[1]
          {
            (object) index
          });
        else
          escapedStringMap[index] = (string) null;
      }
      escapedStringMap[13] = "\\r";
      escapedStringMap[9] = "\\t";
      escapedStringMap[34] = "\\\"";
      escapedStringMap[92] = "\\\\";
      escapedStringMap[10] = "\\n";
      escapedStringMap[8] = "\\b";
      escapedStringMap[12] = "\\f";
      return escapedStringMap;
    }
  }
}

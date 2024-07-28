// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.JsonTextParser
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using System;
using System.Buffers.Text;
using System.Text;

namespace Microsoft.Azure.Cosmos.Json
{
  internal static class JsonTextParser
  {
    private static readonly ReadOnlyMemory<byte> ReverseSolidusBytes = (ReadOnlyMemory<byte>) new byte[1]
    {
      (byte) 92
    };

    public static Number64 GetNumberValue(ReadOnlySpan<byte> token)
    {
      long num1;
      int bytesConsumed1;
      Number64 numberValue;
      if (Utf8Parser.TryParse(token, out num1, out bytesConsumed1) && bytesConsumed1 == token.Length)
      {
        numberValue = (Number64) num1;
      }
      else
      {
        double num2;
        int bytesConsumed2;
        if (!Utf8Parser.TryParse(token, out num2, out bytesConsumed2) && bytesConsumed2 == token.Length)
          throw new JsonNotNumberTokenException();
        numberValue = (Number64) num2;
      }
      return numberValue;
    }

    public static Utf8String GetStringValue(Utf8Memory token) => JsonTextParser.UnescapeJson(token.Slice(1, token.Length - 2));

    public static sbyte GetInt8Value(ReadOnlySpan<byte> intToken)
    {
      long integerValue = JsonTextParser.GetIntegerValue(intToken.Slice("B".Length, intToken.Length - "B".Length));
      return integerValue <= (long) sbyte.MaxValue && integerValue >= (long) sbyte.MinValue ? (sbyte) integerValue : throw new ArgumentOutOfRangeException(string.Format("Tried to read {0} as an {1}", (object) integerValue, (object) typeof (sbyte).FullName));
    }

    public static short GetInt16Value(ReadOnlySpan<byte> intToken)
    {
      long integerValue = JsonTextParser.GetIntegerValue(intToken.Slice("H".Length, intToken.Length - "H".Length));
      return integerValue <= (long) short.MaxValue && integerValue >= (long) short.MinValue ? (short) integerValue : throw new ArgumentOutOfRangeException(string.Format("Tried to read {0} as an {1}", (object) integerValue, (object) typeof (short).FullName));
    }

    public static int GetInt32Value(ReadOnlySpan<byte> intToken)
    {
      long integerValue = JsonTextParser.GetIntegerValue(intToken.Slice("L".Length, intToken.Length - "L".Length));
      return integerValue <= (long) int.MaxValue && integerValue >= (long) int.MinValue ? (int) integerValue : throw new ArgumentOutOfRangeException(string.Format("Tried to read {0} as an {1}", (object) integerValue, (object) typeof (int).FullName));
    }

    public static long GetInt64Value(ReadOnlySpan<byte> intToken)
    {
      long integerValue = JsonTextParser.GetIntegerValue(intToken.Slice("LL".Length, intToken.Length - "LL".Length));
      return integerValue <= long.MaxValue && integerValue >= long.MinValue ? integerValue : throw new ArgumentOutOfRangeException(string.Format("Tried to read {0} as an {1}", (object) integerValue, (object) typeof (long).FullName));
    }

    public static uint GetUInt32Value(ReadOnlySpan<byte> intToken)
    {
      long integerValue = JsonTextParser.GetIntegerValue(intToken.Slice("UL".Length, intToken.Length - "UL".Length));
      return integerValue <= (long) uint.MaxValue && integerValue >= 0L ? (uint) integerValue : throw new ArgumentOutOfRangeException(string.Format("Tried to read {0} as an {1}", (object) integerValue, (object) typeof (uint).FullName));
    }

    public static float GetFloat32Value(ReadOnlySpan<byte> floatToken)
    {
      float floatValue = JsonTextParser.GetFloatValue(floatToken.Slice("F".Length, floatToken.Length - "F".Length));
      return (double) floatValue <= 3.4028234663852886E+38 && (double) floatValue >= -3.4028234663852886E+38 ? floatValue : throw new ArgumentOutOfRangeException(string.Format("Tried to read {0} as an {1}", (object) floatValue, (object) typeof (float).FullName));
    }

    public static double GetFloat64Value(ReadOnlySpan<byte> floatToken)
    {
      double doubleValue = JsonTextParser.GetDoubleValue(floatToken.Slice("D".Length, floatToken.Length - "D".Length));
      return doubleValue <= double.MaxValue && doubleValue >= double.MinValue ? doubleValue : throw new ArgumentOutOfRangeException(string.Format("Tried to read {0} as an {1}", (object) doubleValue, (object) typeof (double).FullName));
    }

    public static Guid GetGuidValue(ReadOnlySpan<byte> guidToken)
    {
      Guid guidValue;
      if (!Utf8Parser.TryParse(guidToken.Slice("G".Length, guidToken.Length - "G".Length), out guidValue, out int _))
        throw new JsonInvalidTokenException();
      return guidValue;
    }

    public static ReadOnlyMemory<byte> GetBinaryValue(ReadOnlySpan<byte> binaryToken) => (ReadOnlyMemory<byte>) Convert.FromBase64String(Encoding.UTF8.GetString(binaryToken.Slice("B".Length, binaryToken.Length - "B".Length).ToArray()));

    private static double GetDoubleValue(ReadOnlySpan<byte> token)
    {
      double doubleValue;
      if (!Utf8Parser.TryParse(token, out doubleValue, out int _))
        throw new JsonNotNumberTokenException();
      return doubleValue;
    }

    private static float GetFloatValue(ReadOnlySpan<byte> token)
    {
      float floatValue;
      if (!Utf8Parser.TryParse(token, out floatValue, out int _))
        throw new JsonNotNumberTokenException();
      return floatValue;
    }

    private static long GetIntegerValue(ReadOnlySpan<byte> token)
    {
      long integerValue;
      if (!Utf8Parser.TryParse(token, out integerValue, out int _))
        throw new JsonNotNumberTokenException();
      return integerValue;
    }

    private static Utf8String UnescapeJson(Utf8Memory escapedString, bool checkIfNeedsEscaping = true)
    {
      if (escapedString.IsEmpty)
        return Utf8String.Empty;
      if (checkIfNeedsEscaping)
      {
        Utf8Span span = escapedString.Span;
        if (((Utf8Span) ref span).Span.IndexOf<byte>(JsonTextParser.ReverseSolidusBytes.Span) < 0)
          return Utf8String.UnsafeFromUtf8BytesNoValidation(escapedString.Memory);
      }
      Memory<byte> destination = (Memory<byte>) new byte[escapedString.Length];
      escapedString.Memory.CopyTo(destination);
      Span<byte> span1 = destination.Span;
      int index1 = 0;
      int num1 = 0;
      while (index1 != destination.Length)
      {
        if (span1[index1] == (byte) 92)
        {
          int num2 = index1 + 1;
          ref Span<byte> local1 = ref span1;
          int index2 = num2;
          index1 = index2 + 1;
          switch (local1[index2])
          {
            case 34:
              span1[num1++] = (byte) 34;
              continue;
            case 47:
              span1[num1++] = (byte) 47;
              continue;
            case 92:
              span1[num1++] = (byte) 92;
              continue;
            case 98:
              span1[num1++] = (byte) 8;
              continue;
            case 102:
              span1[num1++] = (byte) 12;
              continue;
            case 110:
              span1[num1++] = (byte) 10;
              continue;
            case 114:
              span1[num1++] = (byte) 13;
              continue;
            case 116:
              span1[num1++] = (byte) 9;
              continue;
            case 117:
              char ch1 = char.MinValue;
              for (int index3 = 0; index3 < 4; ++index3)
              {
                char ch2 = (char) ((uint) ch1 << 4);
                byte num3 = span1[index1++];
                if (num3 >= (byte) 48 && num3 <= (byte) 57)
                  ch1 = (char) ((uint) ch2 + (uint) (ushort) ((uint) num3 - 48U));
                else if (num3 >= (byte) 65 && num3 <= (byte) 70)
                {
                  ch1 = (char) ((uint) ch2 + (uint) (ushort) (10 + (int) num3 - 65));
                }
                else
                {
                  if (num3 < (byte) 97 || num3 > (byte) 102)
                    throw new JsonInvalidEscapedCharacterException();
                  ch1 = (char) ((uint) ch2 + (uint) (ushort) (10 + (int) num3 - 97));
                }
              }
              if (ch1 >= '\uD800' && ch1 <= '\uDBFF')
              {
                ref Span<byte> local2 = ref span1;
                int index4 = index1;
                int num4 = index4 + 1;
                if (local2[index4] != (byte) 92)
                  throw new JsonInvalidEscapedCharacterException();
                ref Span<byte> local3 = ref span1;
                int index5 = num4;
                index1 = index5 + 1;
                if (local3[index5] != (byte) 117)
                  throw new JsonInvalidEscapedCharacterException();
                char highSurrogate = ch1;
                char lowSurrogate = char.MinValue;
                for (int index6 = 0; index6 < 4; ++index6)
                {
                  char ch3 = (char) ((uint) lowSurrogate << 4);
                  byte num5 = span1[index1++];
                  if (num5 >= (byte) 48 && num5 <= (byte) 57)
                    lowSurrogate = (char) ((uint) ch3 + (uint) (ushort) ((uint) num5 - 48U));
                  else if (num5 >= (byte) 65 && num5 <= (byte) 70)
                  {
                    lowSurrogate = (char) ((uint) ch3 + (uint) (ushort) (10 + (int) num5 - 65));
                  }
                  else
                  {
                    if (num5 < (byte) 97 || num5 > (byte) 102)
                      throw new JsonInvalidEscapedCharacterException();
                    lowSurrogate = (char) ((uint) ch3 + (uint) (ushort) (10 + (int) num5 - 97));
                  }
                }
                num1 += JsonTextParser.WideCharToMultiByte(highSurrogate, lowSurrogate, span1.Slice(num1));
                continue;
              }
              num1 += JsonTextParser.WideCharToMultiByte(ch1, span1.Slice(num1));
              continue;
            default:
              continue;
          }
        }
        else
          span1[num1++] = span1[index1++];
      }
      return Utf8String.UnsafeFromUtf8BytesNoValidation((ReadOnlyMemory<byte>) destination.Slice(0, num1));
    }

    private static unsafe int WideCharToMultiByte(char value, Span<byte> multiByteBuffer)
    {
      // ISSUE: untyped stack allocation
      Span<char> src = new Span<char>((void*) __untypedstackalloc(new IntPtr(2)), 1);
      src[0] = value;
      return Encoding.UTF8.GetBytes((ReadOnlySpan<char>) src, multiByteBuffer);
    }

    private static unsafe int WideCharToMultiByte(
      char highSurrogate,
      char lowSurrogate,
      Span<byte> multiByteBuffer)
    {
      // ISSUE: untyped stack allocation
      Span<char> src = new Span<char>((void*) __untypedstackalloc(new IntPtr(4)), 2);
      src[0] = highSurrogate;
      src[1] = lowSurrogate;
      return Encoding.UTF8.GetBytes((ReadOnlySpan<char>) src, multiByteBuffer);
    }

    private static class Utf16Surrogate
    {
      public static class High
      {
        public const char Min = '\uD800';
        public const char Max = '\uDBFF';
      }

      public static class Low
      {
        public const char Min = '\uDC00';
        public const char Max = '\uDFFF';
      }
    }
  }
}

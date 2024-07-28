// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.JsonBinaryEncoding
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Azure.Cosmos.Json
{
  internal static class JsonBinaryEncoding
  {
    public const int TypeMarkerLength = 1;
    public const int OneByteLength = 1;
    public const int OneByteCount = 1;
    public const int TwoByteLength = 2;
    public const int TwoByteCount = 2;
    public const int FourByteLength = 4;
    public const int FourByteCount = 4;
    public const int OneByteBaseChar = 1;
    public const int OneByteOffset = 1;
    public const int TwoByteOffset = 2;
    public const int ThreeByteOffset = 3;
    public const int FourByteOffset = 4;
    public const int GuidLength = 36;
    public const int GuidWithQuotesLength = 38;
    public const int EncodedGuidLength = 17;
    private const int MaxStackAlloc = 4096;
    private const int Min4BitCharSetStringLength = 16;
    private const int MinCompressedStringLength4 = 24;
    private const int MinCompressedStringLength5 = 32;
    private const int MinCompressedStringLength6 = 40;
    private const int MinCompressedStringLength7 = 88;
    private const int MinCompressedStringLength = 16;
    private static readonly ImmutableArray<bool> IsBufferedStringCandidate = ((IEnumerable<bool>) new bool[256]
    {
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false
    }).ToImmutableArray<bool>();

    public static Guid GetGuidValue(ReadOnlySpan<byte> guidToken)
    {
      Guid guidValue;
      if (!JsonBinaryEncoding.TryGetGuidValue(guidToken, out guidValue))
        throw new JsonInvalidNumberException();
      return guidValue;
    }

    public static bool TryGetGuidValue(ReadOnlySpan<byte> guidToken, out Guid guidValue) => JsonBinaryEncoding.TryGetFixedWidthValue<Guid>(guidToken, 211, out guidValue);

    public static ReadOnlyMemory<byte> GetBinaryValue(ReadOnlyMemory<byte> binaryToken)
    {
      ReadOnlyMemory<byte> binaryValue;
      if (!JsonBinaryEncoding.TryGetBinaryValue(binaryToken, out binaryValue))
        throw new JsonInvalidTokenException();
      return binaryValue;
    }

    public static bool TryGetBinaryValue(
      ReadOnlyMemory<byte> binaryToken,
      out ReadOnlyMemory<byte> binaryValue)
    {
      binaryValue = new ReadOnlyMemory<byte>();
      if (binaryToken.Length < 1)
        return false;
      byte num = binaryToken.Span[0];
      binaryToken = binaryToken.Slice(1);
      uint length;
      switch (num)
      {
        case 221:
          if (binaryToken.Length < 1)
            return false;
          length = (uint) MemoryMarshal.Read<byte>(binaryToken.Span);
          binaryToken = binaryToken.Slice(1);
          break;
        case 222:
          if (binaryToken.Length < 2)
            return false;
          length = (uint) MemoryMarshal.Read<ushort>(binaryToken.Span);
          binaryToken = binaryToken.Slice(2);
          break;
        case 223:
          if (binaryToken.Length < 4)
            return false;
          length = MemoryMarshal.Read<uint>(binaryToken.Span);
          binaryToken = binaryToken.Slice(4);
          break;
        default:
          return false;
      }
      if (length > (uint) int.MaxValue || (long) binaryToken.Length < (long) length)
        return false;
      binaryValue = binaryToken.Slice(0, (int) length);
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetValueLength(ReadOnlySpan<byte> buffer)
    {
      long valueLength = JsonBinaryEncoding.ValueLengths.GetValueLength(buffer);
      return valueLength <= (long) int.MaxValue ? (int) valueLength : throw new InvalidOperationException("valueLength is greater than int.MaxValue");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetStringLengths(byte typeMarker) => JsonBinaryEncoding.StringLengths.Lengths[(int) typeMarker];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetFirstValueOffset(byte typeMarker) => JsonBinaryEncoding.FirstValueOffsets.Offsets[(int) typeMarker];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValueLength(ReadOnlySpan<byte> buffer, out int length)
    {
      length = JsonBinaryEncoding.GetValueLength(buffer);
      return true;
    }

    private static bool TryGetFixedWidthValue<T>(
      ReadOnlySpan<byte> token,
      int expectedTypeMarker,
      out T fixedWidthValue)
      where T : struct
    {
      fixedWidthValue = default (T);
      int num = Marshal.SizeOf<T>(fixedWidthValue);
      if (token.Length < 1 + num || (int) token[0] != expectedTypeMarker)
        return false;
      fixedWidthValue = MemoryMarshal.Read<T>(token.Slice(1));
      return true;
    }

    public static void SetFixedSizedValue<TFixedType>(Span<byte> buffer, TFixedType value) where TFixedType : struct => MemoryMarshal.Cast<byte, TFixedType>(buffer)[0] = value;

    public static TFixedType GetFixedSizedValue<TFixedType>(ReadOnlySpan<byte> buffer) where TFixedType : struct => MemoryMarshal.Cast<byte, TFixedType>(buffer)[0];

    public static string HexDump(byte[] bytes, int bytesPerLine = 16)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int count = 0; count < bytes.Length; count += bytesPerLine)
      {
        byte[] array = ((IEnumerable<byte>) bytes).Skip<byte>(count).Take<byte>(bytesPerLine).ToArray<byte>();
        stringBuilder.AppendFormat("{0:x8} ", (object) count);
        stringBuilder.Append(string.Join(" ", ((IEnumerable<byte>) array).Select<byte, string>((Func<byte, string>) (b => b.ToString("x2"))).ToArray<string>()).PadRight(bytesPerLine * 3));
        stringBuilder.Append(" ");
        stringBuilder.Append(new string(((IEnumerable<byte>) array).Select<byte, char>((Func<byte, char>) (b => b >= (byte) 32 ? (char) b : '.')).ToArray<char>()));
        stringBuilder.AppendLine();
      }
      return stringBuilder.ToString();
    }

    public static Number64 GetNumberValue(ReadOnlySpan<byte> numberToken)
    {
      Number64 number64;
      if (!JsonBinaryEncoding.TryGetNumberValue(numberToken, out number64, out int _))
        throw new JsonNotNumberTokenException();
      return number64;
    }

    public static bool TryGetNumberValue(
      ReadOnlySpan<byte> numberToken,
      out Number64 number64,
      out int bytesConsumed)
    {
      number64 = (Number64) 0L;
      bytesConsumed = 0;
      if (numberToken.IsEmpty)
        return false;
      byte num = numberToken[0];
      if (JsonBinaryEncoding.TypeMarker.IsEncodedNumberLiteral((long) num))
      {
        number64 = (Number64) (long) num;
        bytesConsumed = 1;
      }
      else
      {
        switch (num)
        {
          case 200:
            if (numberToken.Length < 2)
              return false;
            number64 = (Number64) (long) MemoryMarshal.Read<byte>(numberToken.Slice(1));
            bytesConsumed = 2;
            break;
          case 201:
            if (numberToken.Length < 3)
              return false;
            number64 = (Number64) (long) MemoryMarshal.Read<short>(numberToken.Slice(1));
            bytesConsumed = 3;
            break;
          case 202:
            if (numberToken.Length < 5)
              return false;
            number64 = (Number64) (long) MemoryMarshal.Read<int>(numberToken.Slice(1));
            bytesConsumed = 5;
            break;
          case 203:
            if (numberToken.Length < 9)
              return false;
            number64 = (Number64) MemoryMarshal.Read<long>(numberToken.Slice(1));
            bytesConsumed = 9;
            break;
          case 204:
            if (numberToken.Length < 9)
              return false;
            number64 = (Number64) MemoryMarshal.Read<double>(numberToken.Slice(1));
            bytesConsumed = 9;
            break;
          default:
            throw new JsonInvalidNumberException();
        }
      }
      return true;
    }

    public static sbyte GetInt8Value(ReadOnlySpan<byte> int8Token)
    {
      sbyte int8Value;
      if (!JsonBinaryEncoding.TryGetInt8Value(int8Token, out int8Value))
        throw new JsonInvalidNumberException();
      return int8Value;
    }

    public static bool TryGetInt8Value(ReadOnlySpan<byte> int8Token, out sbyte int8Value) => JsonBinaryEncoding.TryGetFixedWidthValue<sbyte>(int8Token, 216, out int8Value);

    public static short GetInt16Value(ReadOnlySpan<byte> int16Token)
    {
      short int16Value;
      if (!JsonBinaryEncoding.TryGetInt16Value(int16Token, out int16Value))
        throw new JsonInvalidNumberException();
      return int16Value;
    }

    public static bool TryGetInt16Value(ReadOnlySpan<byte> int16Token, out short int16Value) => JsonBinaryEncoding.TryGetFixedWidthValue<short>(int16Token, 217, out int16Value);

    public static int GetInt32Value(ReadOnlySpan<byte> int32Token)
    {
      int int32Value;
      if (!JsonBinaryEncoding.TryGetInt32Value(int32Token, out int32Value))
        throw new JsonInvalidNumberException();
      return int32Value;
    }

    public static bool TryGetInt32Value(ReadOnlySpan<byte> int32Token, out int int32Value) => JsonBinaryEncoding.TryGetFixedWidthValue<int>(int32Token, 218, out int32Value);

    public static long GetInt64Value(ReadOnlySpan<byte> int64Token)
    {
      long int64Value;
      if (!JsonBinaryEncoding.TryGetInt64Value(int64Token, out int64Value))
        throw new JsonInvalidNumberException();
      return int64Value;
    }

    public static bool TryGetInt64Value(ReadOnlySpan<byte> int64Token, out long int64Value) => JsonBinaryEncoding.TryGetFixedWidthValue<long>(int64Token, 219, out int64Value);

    public static uint GetUInt32Value(ReadOnlySpan<byte> uInt32Token)
    {
      uint uInt32Value;
      if (!JsonBinaryEncoding.TryGetUInt32Value(uInt32Token, out uInt32Value))
        throw new JsonInvalidNumberException();
      return uInt32Value;
    }

    public static bool TryGetUInt32Value(ReadOnlySpan<byte> uInt32Token, out uint uInt32Value) => JsonBinaryEncoding.TryGetFixedWidthValue<uint>(uInt32Token, 220, out uInt32Value);

    public static float GetFloat32Value(ReadOnlySpan<byte> float32Token)
    {
      float float32Value;
      if (!JsonBinaryEncoding.TryGetFloat32Value(float32Token, out float32Value))
        throw new JsonInvalidNumberException();
      return float32Value;
    }

    public static bool TryGetFloat32Value(ReadOnlySpan<byte> float32Token, out float float32Value) => JsonBinaryEncoding.TryGetFixedWidthValue<float>(float32Token, 205, out float32Value);

    public static double GetFloat64Value(ReadOnlySpan<byte> float64Token)
    {
      double float64Value;
      if (!JsonBinaryEncoding.TryGetFloat64Value(float64Token, out float64Value))
        throw new JsonInvalidNumberException();
      return float64Value;
    }

    public static bool TryGetFloat64Value(ReadOnlySpan<byte> float64Token, out double float64Value) => JsonBinaryEncoding.TryGetFixedWidthValue<double>(float64Token, 206, out float64Value);

    public static unsafe string GetStringValue(
      ReadOnlyMemory<byte> buffer,
      ReadOnlyMemory<byte> stringToken)
    {
      int valueLength;
      JsonBinaryEncoding.GetStringValue(buffer, stringToken, Span<byte>.Empty, out valueLength);
      Span<byte> span;
      if (valueLength < 4096)
      {
        int length = valueLength;
        // ISSUE: untyped stack allocation
        span = new Span<byte>((void*) __untypedstackalloc((IntPtr) (uint) length), length);
      }
      else
        span = (Span<byte>) new byte[valueLength];
      Span<byte> destinationBuffer = span;
      JsonBinaryEncoding.GetStringValue(buffer, stringToken, destinationBuffer, out valueLength);
      return Utf8Span.UnsafeFromUtf8BytesNoValidation((ReadOnlySpan<byte>) destinationBuffer).ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8Span GetUtf8SpanValue(
      ReadOnlyMemory<byte> buffer,
      ReadOnlyMemory<byte> stringToken)
    {
      return Utf8Span.UnsafeFromUtf8BytesNoValidation(JsonBinaryEncoding.GetUtf8MemoryValue(buffer, stringToken).Span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8String GetUtf8StringValue(
      ReadOnlyMemory<byte> buffer,
      ReadOnlyMemory<byte> stringToken)
    {
      return Utf8String.UnsafeFromUtf8BytesNoValidation(JsonBinaryEncoding.GetUtf8MemoryValue(buffer, stringToken));
    }

    public static bool TryGetBufferedStringValue(
      ReadOnlyMemory<byte> buffer,
      ReadOnlyMemory<byte> stringToken,
      out Utf8Memory value)
    {
      if (stringToken.IsEmpty)
      {
        value = new Utf8Memory();
        return false;
      }
      if (JsonBinaryEncoding.TryGetBufferedLengthPrefixedString(buffer, stringToken, out value))
        return true;
      UtfAllString utfAllString;
      if (JsonBinaryEncoding.TryGetEncodedStringValue(stringToken.Span, out utfAllString))
      {
        value = utfAllString.Utf8EscapedString;
        return true;
      }
      value = new Utf8Memory();
      return false;
    }

    public static bool TryGetDictionaryEncodedStringValue(
      ReadOnlySpan<byte> stringToken,
      out UtfAllString value)
    {
      return JsonBinaryEncoding.TryGetEncodedStringValue(stringToken, out value);
    }

    private static ReadOnlyMemory<byte> GetUtf8MemoryValue(
      ReadOnlyMemory<byte> buffer,
      ReadOnlyMemory<byte> stringToken)
    {
      byte num = stringToken.Span[0];
      if (JsonBinaryEncoding.IsBufferedStringCandidate[(int) num])
      {
        Utf8Memory utf8Memory;
        if (!JsonBinaryEncoding.TryGetBufferedStringValue(buffer, stringToken, out utf8Memory))
          throw new JsonInvalidTokenException();
        return utf8Memory.Memory;
      }
      if (!JsonBinaryEncoding.TypeMarker.IsCompressedString(num) && !JsonBinaryEncoding.TypeMarker.IsGuidString(num))
        throw new JsonInvalidTokenException();
      int bytesWritten;
      JsonBinaryEncoding.DecodeString(stringToken.Span, Span<byte>.Empty, out bytesWritten);
      Memory<byte> utf8MemoryValue = (Memory<byte>) new byte[bytesWritten];
      JsonBinaryEncoding.DecodeString(stringToken.Span, utf8MemoryValue.Span, out bytesWritten);
      return (ReadOnlyMemory<byte>) utf8MemoryValue;
    }

    private static void GetStringValue(
      ReadOnlyMemory<byte> buffer,
      ReadOnlyMemory<byte> stringToken,
      Span<byte> destinationBuffer,
      out int valueLength)
    {
      if (stringToken.IsEmpty)
        throw new JsonInvalidTokenException();
      byte num = stringToken.Span[0];
      if (JsonBinaryEncoding.IsBufferedStringCandidate[(int) num])
      {
        Utf8Memory utf8Memory;
        if (!JsonBinaryEncoding.TryGetBufferedStringValue(buffer, stringToken, out utf8Memory))
          throw new JsonInvalidTokenException();
        if (!destinationBuffer.IsEmpty)
        {
          if (utf8Memory.Length > destinationBuffer.Length)
            throw new InvalidOperationException("buffer is too small.");
          utf8Memory.Memory.Span.CopyTo(destinationBuffer);
        }
        valueLength = utf8Memory.Length;
      }
      else
      {
        if (!JsonBinaryEncoding.TypeMarker.IsCompressedString(num) && !JsonBinaryEncoding.TypeMarker.IsGuidString(num))
          throw new JsonInvalidTokenException();
        JsonBinaryEncoding.DecodeString(stringToken.Span, destinationBuffer, out valueLength);
      }
    }

    private static bool TryGetEncodedStringValue(
      ReadOnlySpan<byte> stringToken,
      out UtfAllString value)
    {
      if (JsonBinaryEncoding.TryGetEncodedSystemStringValue(stringToken, out value) || JsonBinaryEncoding.TryGetEncodedUserStringValue(stringToken, out value))
        return true;
      value = (UtfAllString) null;
      return false;
    }

    private static bool TryGetEncodedSystemStringValue(
      ReadOnlySpan<byte> stringToken,
      out UtfAllString value)
    {
      if (!JsonBinaryEncoding.TypeMarker.IsSystemString(stringToken[0]))
      {
        value = (UtfAllString) null;
        return false;
      }
      if (stringToken.Length >= 1)
        return JsonBinaryEncoding.SystemStrings.TryGetSystemStringById((int) stringToken[0] - 32, out value);
      value = (UtfAllString) null;
      return false;
    }

    private static bool TryGetEncodedUserStringValue(
      ReadOnlySpan<byte> stringToken,
      out UtfAllString encodedUserStringValue)
    {
      encodedUserStringValue = (UtfAllString) null;
      return false;
    }

    private static bool TryGetUserStringId(ReadOnlySpan<byte> stringToken, out int userStringId)
    {
      if (!JsonBinaryEncoding.TypeMarker.IsUserString(stringToken[0]))
      {
        userStringId = 0;
        return false;
      }
      if (stringToken.Length < 2)
      {
        userStringId = 0;
        return false;
      }
      userStringId = (int) stringToken[1] + ((int) stringToken[0] - 96) * (int) byte.MaxValue;
      return true;
    }

    private static bool TryGetBufferedLengthPrefixedString(
      ReadOnlyMemory<byte> buffer,
      ReadOnlyMemory<byte> stringToken,
      out Utf8Memory value)
    {
      ReadOnlySpan<byte> source = stringToken.Span;
      byte typeMarker = source[0];
      source = source.Slice(1);
      int start;
      long length;
      if (JsonBinaryEncoding.TypeMarker.IsEncodedLengthString(typeMarker))
      {
        start = 1;
        length = (long) JsonBinaryEncoding.GetStringLengths(typeMarker);
      }
      else
      {
        switch (typeMarker)
        {
          case 192:
            if (source.Length < 1)
            {
              value = new Utf8Memory();
              return false;
            }
            start = 2;
            length = (long) source[0];
            break;
          case 193:
            if (source.Length < 2)
            {
              value = new Utf8Memory();
              return false;
            }
            start = 3;
            length = (long) MemoryMarshal.Read<ushort>(source);
            break;
          case 194:
            if (source.Length < 4)
            {
              value = new Utf8Memory();
              return false;
            }
            start = 5;
            length = (long) MemoryMarshal.Read<uint>(source);
            break;
          case 195:
            if (source.Length >= 1)
              return JsonBinaryEncoding.TryGetBufferedStringValue(buffer, buffer.Slice((int) source[0]), out value);
            value = new Utf8Memory();
            return false;
          case 196:
            if (source.Length >= 2)
              return JsonBinaryEncoding.TryGetBufferedStringValue(buffer, buffer.Slice((int) MemoryMarshal.Read<ushort>(source)), out value);
            value = new Utf8Memory();
            return false;
          case 197:
            if (source.Length >= 3)
              return JsonBinaryEncoding.TryGetBufferedStringValue(buffer, buffer.Slice((int) MemoryMarshal.Read<JsonBinaryEncoding.UInt24>(source)), out value);
            value = new Utf8Memory();
            return false;
          case 198:
            if (source.Length >= 4)
              return JsonBinaryEncoding.TryGetBufferedStringValue(buffer, buffer.Slice((int) MemoryMarshal.Read<uint>(source)), out value);
            value = new Utf8Memory();
            return false;
          default:
            value = new Utf8Memory();
            return false;
        }
        if ((long) start + length > (long) stringToken.Length)
        {
          value = new Utf8Memory();
          return false;
        }
      }
      value = Utf8Memory.UnsafeCreateNoValidation(stringToken.Slice(start, (int) length));
      return true;
    }

    public static bool TryGetEncodedStringTypeMarker(
      Utf8Span utf8Span,
      out JsonBinaryEncoding.MultiByteTypeMarker multiByteTypeMarker)
    {
      if (JsonBinaryEncoding.TryGetEncodedSystemStringTypeMarker(utf8Span, out multiByteTypeMarker) || JsonBinaryEncoding.TryGetEncodedUserStringTypeMarker(utf8Span, out multiByteTypeMarker))
        return true;
      multiByteTypeMarker = new JsonBinaryEncoding.MultiByteTypeMarker();
      return false;
    }

    private static bool TryGetEncodedSystemStringTypeMarker(
      Utf8Span utf8Span,
      out JsonBinaryEncoding.MultiByteTypeMarker multiByteTypeMarker)
    {
      int systemStringId;
      if (JsonBinaryEncoding.SystemStrings.TryGetSystemStringId(utf8Span, out systemStringId))
      {
        multiByteTypeMarker = new JsonBinaryEncoding.MultiByteTypeMarker((byte) 1, (byte) (32 + systemStringId));
        return true;
      }
      multiByteTypeMarker = new JsonBinaryEncoding.MultiByteTypeMarker();
      return false;
    }

    private static bool TryGetEncodedUserStringTypeMarker(
      Utf8Span utf8Span,
      out JsonBinaryEncoding.MultiByteTypeMarker multiByteTypeMarker)
    {
      multiByteTypeMarker = new JsonBinaryEncoding.MultiByteTypeMarker();
      return false;
    }

    public static bool TryEncodeGuidString(
      ReadOnlySpan<byte> guidString,
      Span<byte> destinationBuffer)
    {
      if (guidString.Length < 36 || destinationBuffer.Length < 17)
        return false;
      JsonBinaryEncoding.EncodeGuidParseFlags encodeGuidParseFlags = JsonBinaryEncoding.EncodeGuidParseFlags.None;
      Span<byte> span = destinationBuffer.Slice(1);
      int num1 = 8;
      int num2 = 0;
      for (int index = 0; index < 36; ++index)
      {
        char ch = (char) guidString[index];
        if (index == num1 && index <= 23)
        {
          if (ch != '-')
          {
            encodeGuidParseFlags = JsonBinaryEncoding.EncodeGuidParseFlags.Invalid;
            break;
          }
          num1 += 5;
          num2 = num2 == 0 ? 1 : 0;
        }
        else
        {
          byte num3;
          if (ch >= '0' && ch <= '9')
            num3 = (byte) ((uint) ch - 48U);
          else if (ch >= 'a' && ch <= 'f')
          {
            num3 = (byte) (10 + (int) ch - 97);
            encodeGuidParseFlags |= JsonBinaryEncoding.EncodeGuidParseFlags.LowerCase;
          }
          else if (ch >= 'A' && ch <= 'F')
          {
            num3 = (byte) (10 + (int) ch - 65);
            encodeGuidParseFlags |= JsonBinaryEncoding.EncodeGuidParseFlags.UpperCase;
          }
          else
          {
            encodeGuidParseFlags = JsonBinaryEncoding.EncodeGuidParseFlags.Invalid;
            break;
          }
          if (index % 2 == num2)
          {
            span[0] = num3;
          }
          else
          {
            span[0] = (byte) ((uint) span[0] | (uint) num3 << 4);
            span = span.Slice(1);
          }
        }
      }
      switch (encodeGuidParseFlags)
      {
        case JsonBinaryEncoding.EncodeGuidParseFlags.None:
        case JsonBinaryEncoding.EncodeGuidParseFlags.LowerCase:
          destinationBuffer[0] = (byte) 117;
          break;
        case JsonBinaryEncoding.EncodeGuidParseFlags.UpperCase:
          destinationBuffer[0] = (byte) 118;
          break;
        default:
          return false;
      }
      return true;
    }

    public static bool TryEncodeCompressedString(
      ReadOnlySpan<byte> stringValue,
      Span<byte> destinationBuffer,
      out int bytesWritten)
    {
      if (destinationBuffer.Length < 16)
      {
        bytesWritten = 0;
        return false;
      }
      int num1 = 128;
      int val2 = 0;
      int num2 = 0;
      BitArray first = new BitArray(128);
      for (int index = 0; index < stringValue.Length; ++index)
      {
        byte num3 = stringValue[index];
        if (num3 >= (byte) 128)
        {
          bytesWritten = 0;
          return false;
        }
        if (!first[(int) num3])
        {
          ++num2;
          num1 = Math.Min((int) num3, num1);
          val2 = Math.Max((int) num3, val2);
        }
        first.Set((int) num3, true);
      }
      int num4 = val2 - num1 + 1;
      if (stringValue.Length <= (int) byte.MaxValue && num2 <= 16 && stringValue.Length >= 16)
      {
        if (first.IsSubset(JsonBinaryEncoding.StringCompressionLookupTables.DateTime.Bitmap))
          return JsonBinaryEncoding.TryEncodeString((byte) 122, stringValue, (byte) 0, destinationBuffer, out bytesWritten);
        if (first.IsSubset(JsonBinaryEncoding.StringCompressionLookupTables.LowercaseHex.Bitmap))
          return JsonBinaryEncoding.TryEncodeString((byte) 120, stringValue, (byte) 0, destinationBuffer, out bytesWritten);
        if (first.IsSubset(JsonBinaryEncoding.StringCompressionLookupTables.UppercaseHex.Bitmap))
          return JsonBinaryEncoding.TryEncodeString((byte) 121, stringValue, (byte) 0, destinationBuffer, out bytesWritten);
      }
      if (stringValue.Length <= (int) byte.MaxValue)
      {
        if (num4 <= 16 && stringValue.Length >= 24)
          return JsonBinaryEncoding.TryEncodeString((byte) 123, stringValue, (byte) num1, destinationBuffer, out bytesWritten);
        if (num4 <= 32 && stringValue.Length >= 32)
          return JsonBinaryEncoding.TryEncodeString((byte) 124, stringValue, (byte) num1, destinationBuffer, out bytesWritten);
        if (num4 <= 64 && stringValue.Length >= 40)
          return JsonBinaryEncoding.TryEncodeString((byte) 125, stringValue, (byte) num1, destinationBuffer, out bytesWritten);
      }
      if (stringValue.Length >= 88)
      {
        if (stringValue.Length <= (int) byte.MaxValue)
          return JsonBinaryEncoding.TryEncodeString((byte) 126, stringValue, (byte) 0, destinationBuffer, out bytesWritten);
        if (stringValue.Length <= (int) ushort.MaxValue)
          return JsonBinaryEncoding.TryEncodeString((byte) 127, stringValue, (byte) 0, destinationBuffer, out bytesWritten);
      }
      bytesWritten = 0;
      return false;
    }

    private static bool TryEncodeString(
      byte typeMarker,
      ReadOnlySpan<byte> stringValue,
      byte baseChar,
      Span<byte> destinationBuffer,
      out int bytesWritten)
    {
      bool flag1 = JsonBinaryEncoding.TypeMarker.IsHexadecimalString(typeMarker);
      bool flag2 = JsonBinaryEncoding.TypeMarker.IsDateTimeString(typeMarker);
      bool flag3 = JsonBinaryEncoding.TypeMarker.IsCompressedString(typeMarker);
      if (!(flag1 | flag2 | flag3))
        throw new ArgumentException("typeMarker must be a hexadecimal, datetime, or compressed string");
      int num1 = flag1 | flag2 ? 1 : (flag3 ? (typeMarker == (byte) 127 ? 2 : 1) : 0);
      int num2 = JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 123L, 126L) ? 1 : 0;
      int num3 = 1 + num1 + num2;
      int numberOfBits = flag1 | flag2 ? 4 : (typeMarker >= (byte) 126 ? 7 : 4 + (int) typeMarker - 123);
      int compressedStringLength = JsonBinaryEncoding.ValueLengths.GetCompressedStringLength(stringValue.Length, numberOfBits);
      int num4 = compressedStringLength;
      int num5 = num3 + num4;
      if (!destinationBuffer.IsEmpty)
      {
        if (destinationBuffer.Length < num5)
          throw new ArgumentException("destinationBuffer is too small.");
        destinationBuffer[0] = typeMarker;
        destinationBuffer = destinationBuffer.Slice(1);
        if (num1 == 1)
        {
          destinationBuffer[0] = (byte) stringValue.Length;
          destinationBuffer = destinationBuffer.Slice(1);
        }
        else
        {
          JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer, (ushort) stringValue.Length);
          destinationBuffer = destinationBuffer.Slice(2);
        }
        if (num2 == 1)
        {
          destinationBuffer[0] = baseChar;
          destinationBuffer = destinationBuffer.Slice(1);
        }
        destinationBuffer = destinationBuffer.Slice(0, compressedStringLength);
        JsonBinaryEncoding.EncodeStringValue(typeMarker, stringValue, baseChar, destinationBuffer);
      }
      bytesWritten = num5;
      return true;
    }

    private static void EncodeStringValue(
      byte typeMarker,
      ReadOnlySpan<byte> stringValue,
      byte baseChar,
      Span<byte> destinationBuffer)
    {
      switch (typeMarker)
      {
        case 120:
          JsonBinaryEncoding.Encode4BitCharacterStringValue(JsonBinaryEncoding.StringCompressionLookupTables.LowercaseHex, stringValue, destinationBuffer);
          break;
        case 121:
          JsonBinaryEncoding.Encode4BitCharacterStringValue(JsonBinaryEncoding.StringCompressionLookupTables.UppercaseHex, stringValue, destinationBuffer);
          break;
        case 122:
          JsonBinaryEncoding.Encode4BitCharacterStringValue(JsonBinaryEncoding.StringCompressionLookupTables.DateTime, stringValue, destinationBuffer);
          break;
        case 123:
          JsonBinaryEncoding.EncodeCompressedStringValue(4, stringValue, baseChar, destinationBuffer);
          break;
        case 124:
          JsonBinaryEncoding.EncodeCompressedStringValue(5, stringValue, baseChar, destinationBuffer);
          break;
        case 125:
          JsonBinaryEncoding.EncodeCompressedStringValue(6, stringValue, baseChar, destinationBuffer);
          break;
        case 126:
        case 127:
          JsonBinaryEncoding.EncodeCompressedStringValue(7, stringValue, baseChar, destinationBuffer);
          break;
        default:
          throw new ArgumentOutOfRangeException(string.Format("Unknown {0} {1}.", (object) nameof (typeMarker), (object) typeMarker));
      }
    }

    private static void Encode4BitCharacterStringValue(
      JsonBinaryEncoding.StringCompressionLookupTables chars,
      ReadOnlySpan<byte> stringValue,
      Span<byte> destinationBuffer)
    {
      for (int index1 = 0; index1 < stringValue.Length; ++index1)
      {
        byte index2 = stringValue[index1];
        byte num = chars.CharToByte[(int) index2];
        if (index1 % 2 == 0)
        {
          destinationBuffer[0] = num;
        }
        else
        {
          destinationBuffer[0] = (byte) ((uint) destinationBuffer[0] | (uint) num << 4);
          destinationBuffer = destinationBuffer.Slice(1);
        }
      }
    }

    private static unsafe void EncodeCompressedStringValue(
      int numberOfBits,
      ReadOnlySpan<byte> stringValue,
      byte baseChar,
      Span<byte> destinationBuffer)
    {
      // ISSUE: untyped stack allocation
      Span<ulong> span1 = new Span<ulong>((void*) __untypedstackalloc(new IntPtr(8)), 1);
      int num;
      Span<byte> span2;
      for (num = 0; num < stringValue.Length / 8 * 8; num += 8)
      {
        span1[0] = (ulong) stringValue[num] - (ulong) baseChar;
        span1[0] |= (ulong) ((long) stringValue[num + 1] - (long) baseChar << numberOfBits);
        span1[0] |= (ulong) ((long) stringValue[num + 2] - (long) baseChar << 2 * numberOfBits);
        span1[0] |= (ulong) ((long) stringValue[num + 3] - (long) baseChar << 3 * numberOfBits);
        span1[0] |= (ulong) ((long) stringValue[num + 4] - (long) baseChar << 4 * numberOfBits);
        span1[0] |= (ulong) ((long) stringValue[num + 5] - (long) baseChar << 5 * numberOfBits);
        span1[0] |= (ulong) ((long) stringValue[num + 6] - (long) baseChar << 6 * numberOfBits);
        span1[0] |= (ulong) ((long) stringValue[num + 7] - (long) baseChar << 7 * numberOfBits);
        span2 = MemoryMarshal.AsBytes<ulong>(span1).Slice(0, numberOfBits);
        span2.CopyTo(destinationBuffer);
        destinationBuffer = destinationBuffer.Slice(numberOfBits);
      }
      if (num >= stringValue.Length)
        return;
      // ISSUE: untyped stack allocation
      Span<byte> span3 = new Span<byte>((void*) __untypedstackalloc(new IntPtr(8)), 8);
      // ISSUE: untyped stack allocation
      Span<byte> destinationBuffer1 = new Span<byte>((void*) __untypedstackalloc(new IntPtr(8)), 8);
      stringValue.Slice(num).CopyTo(span3);
      JsonBinaryEncoding.EncodeCompressedStringValue(numberOfBits, (ReadOnlySpan<byte>) span3, baseChar, destinationBuffer1);
      span2 = destinationBuffer1.Slice(0, destinationBuffer.Length);
      span2.CopyTo(destinationBuffer);
    }

    private static unsafe bool EncodedStringEqualsTo(
      byte typeMarker,
      ReadOnlySpan<byte> encodedStringValue,
      ReadOnlySpan<byte> stringValue)
    {
      if (encodedStringValue.Length != JsonBinaryEncoding.GetEncodedStringValueLength(stringValue))
        return false;
      switch (typeMarker)
      {
        case 117:
        case 118:
          // ISSUE: untyped stack allocation
          Span<byte> span1 = new Span<byte>((void*) __untypedstackalloc(new IntPtr(36)), 36);
          JsonBinaryEncoding.DecodeGuidStringValue(encodedStringValue, typeMarker == (byte) 118, span1);
          return span1.SequenceEqual<byte>(stringValue);
        case 119:
          if (stringValue[0] != (byte) 34 || stringValue[37] != (byte) 34)
            return false;
          // ISSUE: untyped stack allocation
          Span<byte> span2 = new Span<byte>((void*) __untypedstackalloc(new IntPtr(36)), 36);
          JsonBinaryEncoding.DecodeGuidStringValue(encodedStringValue, false, span2);
          return span2.SequenceEqual<byte>(stringValue.Slice(1, stringValue.Length - 2));
        case 120:
        case 121:
        case 122:
          encodedStringValue = encodedStringValue.Slice(2);
          // ISSUE: untyped stack allocation
          Span<byte> span3 = new Span<byte>((void*) __untypedstackalloc(new IntPtr(8)), 8);
          int start;
          for (start = 0; start < stringValue.Length / 8 * 8; ++start)
          {
            switch (typeMarker)
            {
              case 120:
                JsonBinaryEncoding.Decode4BitCharacterStringValue(JsonBinaryEncoding.StringCompressionLookupTables.LowercaseHex, encodedStringValue, span3);
                break;
              case 121:
                JsonBinaryEncoding.Decode4BitCharacterStringValue(JsonBinaryEncoding.StringCompressionLookupTables.UppercaseHex, encodedStringValue, span3);
                break;
              default:
                JsonBinaryEncoding.Decode4BitCharacterStringValue(JsonBinaryEncoding.StringCompressionLookupTables.DateTime, encodedStringValue, span3);
                break;
            }
            if (!span3.SequenceEqual<byte>(stringValue.Slice(0, 8)))
              return false;
            stringValue = stringValue.Slice(8);
            encodedStringValue = encodedStringValue.Slice(4);
          }
          if (start >= stringValue.Length)
            return true;
          // ISSUE: untyped stack allocation
          Span<byte> span4 = new Span<byte>((void*) __untypedstackalloc(new IntPtr(8)), 8);
          encodedStringValue.CopyTo(span4);
          switch (typeMarker)
          {
            case 120:
              JsonBinaryEncoding.Decode4BitCharacterStringValue(JsonBinaryEncoding.StringCompressionLookupTables.LowercaseHex, (ReadOnlySpan<byte>) span4, span3);
              break;
            case 121:
              JsonBinaryEncoding.Decode4BitCharacterStringValue(JsonBinaryEncoding.StringCompressionLookupTables.UppercaseHex, (ReadOnlySpan<byte>) span4, span3);
              break;
            default:
              JsonBinaryEncoding.Decode4BitCharacterStringValue(JsonBinaryEncoding.StringCompressionLookupTables.DateTime, (ReadOnlySpan<byte>) span4, span3);
              break;
          }
          return span4.SequenceEqual<byte>(stringValue.Slice(start));
        case 123:
        case 124:
        case 125:
        case 126:
        case 127:
          byte num1;
          switch (typeMarker)
          {
            case 123:
              num1 = (byte) 4;
              break;
            case 124:
              num1 = (byte) 5;
              break;
            case 125:
              num1 = (byte) 6;
              break;
            default:
              num1 = (byte) 7;
              break;
          }
          byte num2 = num1;
          byte encodedStringBaseChar = JsonBinaryEncoding.GetEncodedStringBaseChar(stringValue);
          encodedStringValue = encodedStringValue.Slice(typeMarker == (byte) 126 ? 2 : 3);
          // ISSUE: untyped stack allocation
          Span<byte> span5 = new Span<byte>((void*) __untypedstackalloc(new IntPtr(8)), 8);
          int num3;
          for (num3 = 0; num3 < stringValue.Length / 8 * 8; ++num3)
          {
            JsonBinaryEncoding.DecodeCompressedStringValue((int) num2, encodedStringValue, encodedStringBaseChar, span5);
            if (!span5.SequenceEqual<byte>(stringValue.Slice(0, 8)))
              return false;
            stringValue = stringValue.Slice(8);
            encodedStringValue = encodedStringValue.Slice((int) num2);
          }
          if (num3 >= stringValue.Length)
            return true;
          // ISSUE: untyped stack allocation
          Span<byte> span6 = new Span<byte>((void*) __untypedstackalloc(new IntPtr(8)), 8);
          encodedStringValue.CopyTo(span6);
          JsonBinaryEncoding.DecodeCompressedStringValue((int) num2, (ReadOnlySpan<byte>) span6, encodedStringBaseChar, span5);
          return span5.SequenceEqual<byte>(stringValue.Slice(0, 8));
        default:
          throw new ArgumentOutOfRangeException(string.Format("Unrecognized {0}: {1}.", (object) nameof (typeMarker), (object) typeMarker));
      }
    }

    private static void DecodeString(
      ReadOnlySpan<byte> stringToken,
      Span<byte> destinationBuffer,
      out int bytesWritten)
    {
      byte typeMarker = stringToken[0];
      bool flag1 = JsonBinaryEncoding.TypeMarker.IsHexadecimalString(typeMarker);
      bool flag2 = JsonBinaryEncoding.TypeMarker.IsDateTimeString(typeMarker);
      bool flag3 = JsonBinaryEncoding.TypeMarker.IsCompressedString(typeMarker);
      bool flag4 = JsonBinaryEncoding.TypeMarker.IsGuidString(typeMarker);
      if (!(flag1 | flag2 | flag3 | flag4))
        throw new ArgumentException("token must be a hex, datetime, compressed, or guid string.");
      int start = 1 + (flag1 | flag2 ? 1 : (flag3 ? (typeMarker == (byte) 127 ? 2 : 1) : 0)) + (JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 123L, 126L) ? 1 : 0);
      if (stringToken.Length < start)
        throw new JsonInvalidTokenException();
      bytesWritten = JsonBinaryEncoding.GetEncodedStringValueLength(stringToken);
      int stringBufferLength = JsonBinaryEncoding.GetEncodedStringBufferLength(stringToken);
      byte encodedStringBaseChar = JsonBinaryEncoding.GetEncodedStringBaseChar(stringToken);
      if (stringToken.Length < start + stringBufferLength)
        throw new JsonInvalidTokenException();
      if (destinationBuffer.IsEmpty)
        return;
      if (bytesWritten > destinationBuffer.Length)
        throw new InvalidOperationException("buffer is too small");
      ReadOnlySpan<byte> encodedString = stringToken.Slice(start, stringBufferLength);
      JsonBinaryEncoding.DecodeStringValue(typeMarker, encodedString, encodedStringBaseChar, destinationBuffer.Slice(0, bytesWritten));
    }

    private static int GetEncodedStringValueLength(ReadOnlySpan<byte> stringToken)
    {
      byte num = stringToken[0];
      switch (num)
      {
        case 117:
        case 118:
          return 36;
        case 119:
          return 38;
        case 120:
        case 121:
        case 122:
        case 123:
        case 124:
        case 125:
        case 126:
          return (int) stringToken[1];
        case 127:
          return (int) JsonBinaryEncoding.GetFixedSizedValue<ushort>(stringToken.Slice(1));
        default:
          throw new ArgumentOutOfRangeException(string.Format("Unexpected type marker: {0}.", (object) num));
      }
    }

    private static int GetEncodedStringBufferLength(ReadOnlySpan<byte> stringToken)
    {
      byte num = stringToken[0];
      switch (num)
      {
        case 117:
        case 118:
        case 119:
          return 16;
        case 120:
        case 121:
        case 122:
          return JsonBinaryEncoding.ValueLengths.GetCompressedStringLength((int) stringToken[1], 4);
        case 123:
          return JsonBinaryEncoding.ValueLengths.GetCompressedStringLength((int) stringToken[1], 4);
        case 124:
          return JsonBinaryEncoding.ValueLengths.GetCompressedStringLength((int) stringToken[1], 5);
        case 125:
          return JsonBinaryEncoding.ValueLengths.GetCompressedStringLength((int) stringToken[1], 6);
        case 126:
          return JsonBinaryEncoding.ValueLengths.GetCompressedStringLength((int) stringToken[1], 7);
        case 127:
          return JsonBinaryEncoding.ValueLengths.GetCompressedStringLength((int) JsonBinaryEncoding.GetFixedSizedValue<ushort>(stringToken.Slice(1)), 7);
        default:
          throw new ArgumentException(string.Format("Invalid type marker: {0}", (object) num));
      }
    }

    private static byte GetEncodedStringBaseChar(ReadOnlySpan<byte> stringToken)
    {
      byte num = stringToken[0];
      switch (num)
      {
        case 117:
        case 118:
        case 119:
          return 0;
        case 120:
        case 121:
        case 122:
          return 0;
        case 123:
        case 124:
        case 125:
          return stringToken[2];
        case 126:
        case 127:
          return 0;
        default:
          throw new ArgumentException(string.Format("Invalid type marker: {0}", (object) num));
      }
    }

    private static void DecodeStringValue(
      byte typeMarker,
      ReadOnlySpan<byte> encodedString,
      byte baseChar,
      Span<byte> destinationBuffer)
    {
      switch (typeMarker)
      {
        case 117:
        case 118:
          JsonBinaryEncoding.DecodeGuidStringValue(encodedString, typeMarker == (byte) 118, destinationBuffer);
          break;
        case 119:
          destinationBuffer[0] = (byte) 34;
          JsonBinaryEncoding.DecodeGuidStringValue(encodedString, false, destinationBuffer.Slice(1));
          destinationBuffer[37] = (byte) 34;
          break;
        case 120:
          if (baseChar != (byte) 0)
            throw new InvalidOperationException("base char needs to be 0.");
          JsonBinaryEncoding.Decode4BitCharacterStringValue(JsonBinaryEncoding.StringCompressionLookupTables.LowercaseHex, encodedString, destinationBuffer);
          break;
        case 121:
          if (baseChar != (byte) 0)
            throw new InvalidOperationException("base char needs to be 0.");
          JsonBinaryEncoding.Decode4BitCharacterStringValue(JsonBinaryEncoding.StringCompressionLookupTables.UppercaseHex, encodedString, destinationBuffer);
          break;
        case 122:
          if (baseChar != (byte) 0)
            throw new InvalidOperationException("base char needs to be 0.");
          JsonBinaryEncoding.Decode4BitCharacterStringValue(JsonBinaryEncoding.StringCompressionLookupTables.DateTime, encodedString, destinationBuffer);
          break;
        case 123:
          JsonBinaryEncoding.DecodeCompressedStringValue(4, encodedString, baseChar, destinationBuffer);
          break;
        case 124:
          JsonBinaryEncoding.DecodeCompressedStringValue(5, encodedString, baseChar, destinationBuffer);
          break;
        case 125:
          JsonBinaryEncoding.DecodeCompressedStringValue(6, encodedString, baseChar, destinationBuffer);
          break;
        case 126:
        case 127:
          if (baseChar != (byte) 0)
            throw new InvalidOperationException("base char needs to be 0.");
          JsonBinaryEncoding.DecodeCompressedStringValue(7, encodedString, baseChar, destinationBuffer);
          break;
        default:
          throw new JsonInvalidTokenException();
      }
    }

    private static void Decode4BitCharacterStringValue(
      JsonBinaryEncoding.StringCompressionLookupTables chars,
      ReadOnlySpan<byte> encodedString,
      Span<byte> destinationBuffer)
    {
      if (encodedString.Length != JsonBinaryEncoding.ValueLengths.GetCompressedStringLength(destinationBuffer.Length, 4))
        throw new ArgumentException("destination buffer is too small.");
      ReadOnlySpan<byte> readOnlySpan = encodedString;
      Span<ushort> span = MemoryMarshal.Cast<byte, ushort>(destinationBuffer);
      for (int index = 0; index < destinationBuffer.Length / 2; ++index)
      {
        ushort byteToTwoChar = chars.ByteToTwoChars[(int) readOnlySpan[index]];
        span[index] = byteToTwoChar;
      }
      if (destinationBuffer.Length % 2 != 1)
        return;
      if (readOnlySpan[readOnlySpan.Length - 1] > (byte) 15)
        throw new InvalidOperationException();
      destinationBuffer[destinationBuffer.Length - 1] = (byte) chars.List[(int) encodedString[encodedString.Length - 1]];
    }

    private static unsafe void DecodeCompressedStringValue(
      int numberOfBits,
      ReadOnlySpan<byte> encodedString,
      byte baseChar,
      Span<byte> destinationBuffer)
    {
      if (numberOfBits > 8 || numberOfBits < 0)
        throw new ArgumentException("Invalid number of bits.");
      long num1 = (long) ((int) byte.MaxValue >> 8 - numberOfBits);
      int num2 = 0;
      // ISSUE: untyped stack allocation
      Span<byte> span1 = new Span<byte>((void*) __untypedstackalloc(new IntPtr(8)), 8);
      for (int index = destinationBuffer.Length / 8 * 8; num2 < index; num2 += 8)
      {
        encodedString.Slice(0, numberOfBits).CopyTo(span1);
        long num3 = MemoryMarshal.Cast<byte, long>(span1)[0];
        destinationBuffer[0] = (byte) ((uint) (byte) (num3 & num1) + (uint) baseChar);
        long num4 = num3 >> numberOfBits;
        destinationBuffer[1] = (byte) ((uint) (byte) (num4 & num1) + (uint) baseChar);
        long num5 = num4 >> numberOfBits;
        destinationBuffer[2] = (byte) ((uint) (byte) (num5 & num1) + (uint) baseChar);
        long num6 = num5 >> numberOfBits;
        destinationBuffer[3] = (byte) ((uint) (byte) (num6 & num1) + (uint) baseChar);
        long num7 = num6 >> numberOfBits;
        destinationBuffer[4] = (byte) ((uint) (byte) (num7 & num1) + (uint) baseChar);
        long num8 = num7 >> numberOfBits;
        destinationBuffer[5] = (byte) ((uint) (byte) (num8 & num1) + (uint) baseChar);
        long num9 = num8 >> numberOfBits;
        destinationBuffer[6] = (byte) ((uint) (byte) (num9 & num1) + (uint) baseChar);
        long num10 = num9 >> numberOfBits;
        destinationBuffer[7] = (byte) ((uint) (byte) (num10 & num1) + (uint) baseChar);
        encodedString = num10 >> numberOfBits == 0L ? encodedString.Slice(numberOfBits) : throw new InvalidOperationException();
        destinationBuffer = destinationBuffer.Slice(8);
      }
      if (destinationBuffer.IsEmpty)
        return;
      // ISSUE: untyped stack allocation
      Span<byte> span2 = new Span<byte>((void*) __untypedstackalloc(new IntPtr(8)), 8);
      // ISSUE: untyped stack allocation
      Span<byte> destinationBuffer1 = new Span<byte>((void*) __untypedstackalloc(new IntPtr(8)), 8);
      encodedString.CopyTo(span2);
      JsonBinaryEncoding.DecodeCompressedStringValue(numberOfBits, (ReadOnlySpan<byte>) span2, baseChar, destinationBuffer1);
      destinationBuffer1.Slice(0, destinationBuffer.Length).CopyTo(destinationBuffer);
    }

    private static void DecodeGuidStringValue(
      ReadOnlySpan<byte> encodedString,
      bool isUpperCaseGuid,
      Span<byte> destinationBuffer)
    {
      ImmutableArray<ushort> immutableArray = isUpperCaseGuid ? JsonBinaryEncoding.StringCompressionLookupTables.UppercaseHex.ByteToTwoChars : JsonBinaryEncoding.StringCompressionLookupTables.LowercaseHex.ByteToTwoChars;
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(0), immutableArray[(int) encodedString[0]]);
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(2), immutableArray[(int) encodedString[1]]);
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(4), immutableArray[(int) encodedString[2]]);
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(6), immutableArray[(int) encodedString[3]]);
      destinationBuffer[8] = (byte) 45;
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(9), immutableArray[(int) encodedString[4]]);
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(11), immutableArray[(int) encodedString[5]]);
      destinationBuffer[13] = (byte) 45;
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(14), immutableArray[(int) encodedString[6]]);
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(16), immutableArray[(int) encodedString[7]]);
      destinationBuffer[18] = (byte) 45;
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(19), immutableArray[(int) encodedString[8]]);
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(21), immutableArray[(int) encodedString[9]]);
      destinationBuffer[23] = (byte) 45;
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(24), immutableArray[(int) encodedString[10]]);
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(26), immutableArray[(int) encodedString[11]]);
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(28), immutableArray[(int) encodedString[12]]);
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(30), immutableArray[(int) encodedString[13]]);
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(32), immutableArray[(int) encodedString[14]]);
      JsonBinaryEncoding.SetFixedSizedValue<ushort>(destinationBuffer.Slice(34), immutableArray[(int) encodedString[15]]);
    }

    public readonly struct StringCompressionLookupTables
    {
      public static readonly JsonBinaryEncoding.StringCompressionLookupTables DateTime = JsonBinaryEncoding.StringCompressionLookupTables.Create(new char[16]
      {
        ' ',
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9',
        ':',
        '-',
        '.',
        'T',
        'Z'
      }, new byte[16]
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 96,
        byte.MaxValue,
        (byte) 7,
        (byte) 0,
        (byte) 0,
        (byte) 16,
        (byte) 4,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      }, new byte[256]
      {
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        (byte) 0,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        (byte) 12,
        (byte) 13,
        byte.MaxValue,
        (byte) 1,
        (byte) 2,
        (byte) 3,
        (byte) 4,
        (byte) 5,
        (byte) 6,
        (byte) 7,
        (byte) 8,
        (byte) 9,
        (byte) 10,
        (byte) 11,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        (byte) 14,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        (byte) 15,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue
      }, new ushort[256]
      {
        (ushort) 8224,
        (ushort) 8240,
        (ushort) 8241,
        (ushort) 8242,
        (ushort) 8243,
        (ushort) 8244,
        (ushort) 8245,
        (ushort) 8246,
        (ushort) 8247,
        (ushort) 8248,
        (ushort) 8249,
        (ushort) 8250,
        (ushort) 8237,
        (ushort) 8238,
        (ushort) 8276,
        (ushort) 8282,
        (ushort) 12320,
        (ushort) 12336,
        (ushort) 12337,
        (ushort) 12338,
        (ushort) 12339,
        (ushort) 12340,
        (ushort) 12341,
        (ushort) 12342,
        (ushort) 12343,
        (ushort) 12344,
        (ushort) 12345,
        (ushort) 12346,
        (ushort) 12333,
        (ushort) 12334,
        (ushort) 12372,
        (ushort) 12378,
        (ushort) 12576,
        (ushort) 12592,
        (ushort) 12593,
        (ushort) 12594,
        (ushort) 12595,
        (ushort) 12596,
        (ushort) 12597,
        (ushort) 12598,
        (ushort) 12599,
        (ushort) 12600,
        (ushort) 12601,
        (ushort) 12602,
        (ushort) 12589,
        (ushort) 12590,
        (ushort) 12628,
        (ushort) 12634,
        (ushort) 12832,
        (ushort) 12848,
        (ushort) 12849,
        (ushort) 12850,
        (ushort) 12851,
        (ushort) 12852,
        (ushort) 12853,
        (ushort) 12854,
        (ushort) 12855,
        (ushort) 12856,
        (ushort) 12857,
        (ushort) 12858,
        (ushort) 12845,
        (ushort) 12846,
        (ushort) 12884,
        (ushort) 12890,
        (ushort) 13088,
        (ushort) 13104,
        (ushort) 13105,
        (ushort) 13106,
        (ushort) 13107,
        (ushort) 13108,
        (ushort) 13109,
        (ushort) 13110,
        (ushort) 13111,
        (ushort) 13112,
        (ushort) 13113,
        (ushort) 13114,
        (ushort) 13101,
        (ushort) 13102,
        (ushort) 13140,
        (ushort) 13146,
        (ushort) 13344,
        (ushort) 13360,
        (ushort) 13361,
        (ushort) 13362,
        (ushort) 13363,
        (ushort) 13364,
        (ushort) 13365,
        (ushort) 13366,
        (ushort) 13367,
        (ushort) 13368,
        (ushort) 13369,
        (ushort) 13370,
        (ushort) 13357,
        (ushort) 13358,
        (ushort) 13396,
        (ushort) 13402,
        (ushort) 13600,
        (ushort) 13616,
        (ushort) 13617,
        (ushort) 13618,
        (ushort) 13619,
        (ushort) 13620,
        (ushort) 13621,
        (ushort) 13622,
        (ushort) 13623,
        (ushort) 13624,
        (ushort) 13625,
        (ushort) 13626,
        (ushort) 13613,
        (ushort) 13614,
        (ushort) 13652,
        (ushort) 13658,
        (ushort) 13856,
        (ushort) 13872,
        (ushort) 13873,
        (ushort) 13874,
        (ushort) 13875,
        (ushort) 13876,
        (ushort) 13877,
        (ushort) 13878,
        (ushort) 13879,
        (ushort) 13880,
        (ushort) 13881,
        (ushort) 13882,
        (ushort) 13869,
        (ushort) 13870,
        (ushort) 13908,
        (ushort) 13914,
        (ushort) 14112,
        (ushort) 14128,
        (ushort) 14129,
        (ushort) 14130,
        (ushort) 14131,
        (ushort) 14132,
        (ushort) 14133,
        (ushort) 14134,
        (ushort) 14135,
        (ushort) 14136,
        (ushort) 14137,
        (ushort) 14138,
        (ushort) 14125,
        (ushort) 14126,
        (ushort) 14164,
        (ushort) 14170,
        (ushort) 14368,
        (ushort) 14384,
        (ushort) 14385,
        (ushort) 14386,
        (ushort) 14387,
        (ushort) 14388,
        (ushort) 14389,
        (ushort) 14390,
        (ushort) 14391,
        (ushort) 14392,
        (ushort) 14393,
        (ushort) 14394,
        (ushort) 14381,
        (ushort) 14382,
        (ushort) 14420,
        (ushort) 14426,
        (ushort) 14624,
        (ushort) 14640,
        (ushort) 14641,
        (ushort) 14642,
        (ushort) 14643,
        (ushort) 14644,
        (ushort) 14645,
        (ushort) 14646,
        (ushort) 14647,
        (ushort) 14648,
        (ushort) 14649,
        (ushort) 14650,
        (ushort) 14637,
        (ushort) 14638,
        (ushort) 14676,
        (ushort) 14682,
        (ushort) 14880,
        (ushort) 14896,
        (ushort) 14897,
        (ushort) 14898,
        (ushort) 14899,
        (ushort) 14900,
        (ushort) 14901,
        (ushort) 14902,
        (ushort) 14903,
        (ushort) 14904,
        (ushort) 14905,
        (ushort) 14906,
        (ushort) 14893,
        (ushort) 14894,
        (ushort) 14932,
        (ushort) 14938,
        (ushort) 11552,
        (ushort) 11568,
        (ushort) 11569,
        (ushort) 11570,
        (ushort) 11571,
        (ushort) 11572,
        (ushort) 11573,
        (ushort) 11574,
        (ushort) 11575,
        (ushort) 11576,
        (ushort) 11577,
        (ushort) 11578,
        (ushort) 11565,
        (ushort) 11566,
        (ushort) 11604,
        (ushort) 11610,
        (ushort) 11808,
        (ushort) 11824,
        (ushort) 11825,
        (ushort) 11826,
        (ushort) 11827,
        (ushort) 11828,
        (ushort) 11829,
        (ushort) 11830,
        (ushort) 11831,
        (ushort) 11832,
        (ushort) 11833,
        (ushort) 11834,
        (ushort) 11821,
        (ushort) 11822,
        (ushort) 11860,
        (ushort) 11866,
        (ushort) 21536,
        (ushort) 21552,
        (ushort) 21553,
        (ushort) 21554,
        (ushort) 21555,
        (ushort) 21556,
        (ushort) 21557,
        (ushort) 21558,
        (ushort) 21559,
        (ushort) 21560,
        (ushort) 21561,
        (ushort) 21562,
        (ushort) 21549,
        (ushort) 21550,
        (ushort) 21588,
        (ushort) 21594,
        (ushort) 23072,
        (ushort) 23088,
        (ushort) 23089,
        (ushort) 23090,
        (ushort) 23091,
        (ushort) 23092,
        (ushort) 23093,
        (ushort) 23094,
        (ushort) 23095,
        (ushort) 23096,
        (ushort) 23097,
        (ushort) 23098,
        (ushort) 23085,
        (ushort) 23086,
        (ushort) 23124,
        (ushort) 23130
      });
      public static readonly JsonBinaryEncoding.StringCompressionLookupTables LowercaseHex = JsonBinaryEncoding.StringCompressionLookupTables.Create(new char[16]
      {
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9',
        'a',
        'b',
        'c',
        'd',
        'e',
        'f'
      }, new byte[16]
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        byte.MaxValue,
        (byte) 3,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 126,
        (byte) 0,
        (byte) 0,
        (byte) 0
      }, new byte[256]
      {
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        (byte) 0,
        (byte) 1,
        (byte) 2,
        (byte) 3,
        (byte) 4,
        (byte) 5,
        (byte) 6,
        (byte) 7,
        (byte) 8,
        (byte) 9,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        (byte) 10,
        (byte) 11,
        (byte) 12,
        (byte) 13,
        (byte) 14,
        (byte) 15,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue
      }, new ushort[256]
      {
        (ushort) 12336,
        (ushort) 12337,
        (ushort) 12338,
        (ushort) 12339,
        (ushort) 12340,
        (ushort) 12341,
        (ushort) 12342,
        (ushort) 12343,
        (ushort) 12344,
        (ushort) 12345,
        (ushort) 12385,
        (ushort) 12386,
        (ushort) 12387,
        (ushort) 12388,
        (ushort) 12389,
        (ushort) 12390,
        (ushort) 12592,
        (ushort) 12593,
        (ushort) 12594,
        (ushort) 12595,
        (ushort) 12596,
        (ushort) 12597,
        (ushort) 12598,
        (ushort) 12599,
        (ushort) 12600,
        (ushort) 12601,
        (ushort) 12641,
        (ushort) 12642,
        (ushort) 12643,
        (ushort) 12644,
        (ushort) 12645,
        (ushort) 12646,
        (ushort) 12848,
        (ushort) 12849,
        (ushort) 12850,
        (ushort) 12851,
        (ushort) 12852,
        (ushort) 12853,
        (ushort) 12854,
        (ushort) 12855,
        (ushort) 12856,
        (ushort) 12857,
        (ushort) 12897,
        (ushort) 12898,
        (ushort) 12899,
        (ushort) 12900,
        (ushort) 12901,
        (ushort) 12902,
        (ushort) 13104,
        (ushort) 13105,
        (ushort) 13106,
        (ushort) 13107,
        (ushort) 13108,
        (ushort) 13109,
        (ushort) 13110,
        (ushort) 13111,
        (ushort) 13112,
        (ushort) 13113,
        (ushort) 13153,
        (ushort) 13154,
        (ushort) 13155,
        (ushort) 13156,
        (ushort) 13157,
        (ushort) 13158,
        (ushort) 13360,
        (ushort) 13361,
        (ushort) 13362,
        (ushort) 13363,
        (ushort) 13364,
        (ushort) 13365,
        (ushort) 13366,
        (ushort) 13367,
        (ushort) 13368,
        (ushort) 13369,
        (ushort) 13409,
        (ushort) 13410,
        (ushort) 13411,
        (ushort) 13412,
        (ushort) 13413,
        (ushort) 13414,
        (ushort) 13616,
        (ushort) 13617,
        (ushort) 13618,
        (ushort) 13619,
        (ushort) 13620,
        (ushort) 13621,
        (ushort) 13622,
        (ushort) 13623,
        (ushort) 13624,
        (ushort) 13625,
        (ushort) 13665,
        (ushort) 13666,
        (ushort) 13667,
        (ushort) 13668,
        (ushort) 13669,
        (ushort) 13670,
        (ushort) 13872,
        (ushort) 13873,
        (ushort) 13874,
        (ushort) 13875,
        (ushort) 13876,
        (ushort) 13877,
        (ushort) 13878,
        (ushort) 13879,
        (ushort) 13880,
        (ushort) 13881,
        (ushort) 13921,
        (ushort) 13922,
        (ushort) 13923,
        (ushort) 13924,
        (ushort) 13925,
        (ushort) 13926,
        (ushort) 14128,
        (ushort) 14129,
        (ushort) 14130,
        (ushort) 14131,
        (ushort) 14132,
        (ushort) 14133,
        (ushort) 14134,
        (ushort) 14135,
        (ushort) 14136,
        (ushort) 14137,
        (ushort) 14177,
        (ushort) 14178,
        (ushort) 14179,
        (ushort) 14180,
        (ushort) 14181,
        (ushort) 14182,
        (ushort) 14384,
        (ushort) 14385,
        (ushort) 14386,
        (ushort) 14387,
        (ushort) 14388,
        (ushort) 14389,
        (ushort) 14390,
        (ushort) 14391,
        (ushort) 14392,
        (ushort) 14393,
        (ushort) 14433,
        (ushort) 14434,
        (ushort) 14435,
        (ushort) 14436,
        (ushort) 14437,
        (ushort) 14438,
        (ushort) 14640,
        (ushort) 14641,
        (ushort) 14642,
        (ushort) 14643,
        (ushort) 14644,
        (ushort) 14645,
        (ushort) 14646,
        (ushort) 14647,
        (ushort) 14648,
        (ushort) 14649,
        (ushort) 14689,
        (ushort) 14690,
        (ushort) 14691,
        (ushort) 14692,
        (ushort) 14693,
        (ushort) 14694,
        (ushort) 24880,
        (ushort) 24881,
        (ushort) 24882,
        (ushort) 24883,
        (ushort) 24884,
        (ushort) 24885,
        (ushort) 24886,
        (ushort) 24887,
        (ushort) 24888,
        (ushort) 24889,
        (ushort) 24929,
        (ushort) 24930,
        (ushort) 24931,
        (ushort) 24932,
        (ushort) 24933,
        (ushort) 24934,
        (ushort) 25136,
        (ushort) 25137,
        (ushort) 25138,
        (ushort) 25139,
        (ushort) 25140,
        (ushort) 25141,
        (ushort) 25142,
        (ushort) 25143,
        (ushort) 25144,
        (ushort) 25145,
        (ushort) 25185,
        (ushort) 25186,
        (ushort) 25187,
        (ushort) 25188,
        (ushort) 25189,
        (ushort) 25190,
        (ushort) 25392,
        (ushort) 25393,
        (ushort) 25394,
        (ushort) 25395,
        (ushort) 25396,
        (ushort) 25397,
        (ushort) 25398,
        (ushort) 25399,
        (ushort) 25400,
        (ushort) 25401,
        (ushort) 25441,
        (ushort) 25442,
        (ushort) 25443,
        (ushort) 25444,
        (ushort) 25445,
        (ushort) 25446,
        (ushort) 25648,
        (ushort) 25649,
        (ushort) 25650,
        (ushort) 25651,
        (ushort) 25652,
        (ushort) 25653,
        (ushort) 25654,
        (ushort) 25655,
        (ushort) 25656,
        (ushort) 25657,
        (ushort) 25697,
        (ushort) 25698,
        (ushort) 25699,
        (ushort) 25700,
        (ushort) 25701,
        (ushort) 25702,
        (ushort) 25904,
        (ushort) 25905,
        (ushort) 25906,
        (ushort) 25907,
        (ushort) 25908,
        (ushort) 25909,
        (ushort) 25910,
        (ushort) 25911,
        (ushort) 25912,
        (ushort) 25913,
        (ushort) 25953,
        (ushort) 25954,
        (ushort) 25955,
        (ushort) 25956,
        (ushort) 25957,
        (ushort) 25958,
        (ushort) 26160,
        (ushort) 26161,
        (ushort) 26162,
        (ushort) 26163,
        (ushort) 26164,
        (ushort) 26165,
        (ushort) 26166,
        (ushort) 26167,
        (ushort) 26168,
        (ushort) 26169,
        (ushort) 26209,
        (ushort) 26210,
        (ushort) 26211,
        (ushort) 26212,
        (ushort) 26213,
        (ushort) 26214
      });
      public static readonly JsonBinaryEncoding.StringCompressionLookupTables UppercaseHex = JsonBinaryEncoding.StringCompressionLookupTables.Create(new char[16]
      {
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9',
        'A',
        'B',
        'C',
        'D',
        'E',
        'F'
      }, new byte[16]
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        byte.MaxValue,
        (byte) 3,
        (byte) 126,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      }, new byte[256]
      {
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        (byte) 0,
        (byte) 1,
        (byte) 2,
        (byte) 3,
        (byte) 4,
        (byte) 5,
        (byte) 6,
        (byte) 7,
        (byte) 8,
        (byte) 9,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        (byte) 10,
        (byte) 11,
        (byte) 12,
        (byte) 13,
        (byte) 14,
        (byte) 15,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue
      }, new ushort[256]
      {
        (ushort) 12336,
        (ushort) 12337,
        (ushort) 12338,
        (ushort) 12339,
        (ushort) 12340,
        (ushort) 12341,
        (ushort) 12342,
        (ushort) 12343,
        (ushort) 12344,
        (ushort) 12345,
        (ushort) 12353,
        (ushort) 12354,
        (ushort) 12355,
        (ushort) 12356,
        (ushort) 12357,
        (ushort) 12358,
        (ushort) 12592,
        (ushort) 12593,
        (ushort) 12594,
        (ushort) 12595,
        (ushort) 12596,
        (ushort) 12597,
        (ushort) 12598,
        (ushort) 12599,
        (ushort) 12600,
        (ushort) 12601,
        (ushort) 12609,
        (ushort) 12610,
        (ushort) 12611,
        (ushort) 12612,
        (ushort) 12613,
        (ushort) 12614,
        (ushort) 12848,
        (ushort) 12849,
        (ushort) 12850,
        (ushort) 12851,
        (ushort) 12852,
        (ushort) 12853,
        (ushort) 12854,
        (ushort) 12855,
        (ushort) 12856,
        (ushort) 12857,
        (ushort) 12865,
        (ushort) 12866,
        (ushort) 12867,
        (ushort) 12868,
        (ushort) 12869,
        (ushort) 12870,
        (ushort) 13104,
        (ushort) 13105,
        (ushort) 13106,
        (ushort) 13107,
        (ushort) 13108,
        (ushort) 13109,
        (ushort) 13110,
        (ushort) 13111,
        (ushort) 13112,
        (ushort) 13113,
        (ushort) 13121,
        (ushort) 13122,
        (ushort) 13123,
        (ushort) 13124,
        (ushort) 13125,
        (ushort) 13126,
        (ushort) 13360,
        (ushort) 13361,
        (ushort) 13362,
        (ushort) 13363,
        (ushort) 13364,
        (ushort) 13365,
        (ushort) 13366,
        (ushort) 13367,
        (ushort) 13368,
        (ushort) 13369,
        (ushort) 13377,
        (ushort) 13378,
        (ushort) 13379,
        (ushort) 13380,
        (ushort) 13381,
        (ushort) 13382,
        (ushort) 13616,
        (ushort) 13617,
        (ushort) 13618,
        (ushort) 13619,
        (ushort) 13620,
        (ushort) 13621,
        (ushort) 13622,
        (ushort) 13623,
        (ushort) 13624,
        (ushort) 13625,
        (ushort) 13633,
        (ushort) 13634,
        (ushort) 13635,
        (ushort) 13636,
        (ushort) 13637,
        (ushort) 13638,
        (ushort) 13872,
        (ushort) 13873,
        (ushort) 13874,
        (ushort) 13875,
        (ushort) 13876,
        (ushort) 13877,
        (ushort) 13878,
        (ushort) 13879,
        (ushort) 13880,
        (ushort) 13881,
        (ushort) 13889,
        (ushort) 13890,
        (ushort) 13891,
        (ushort) 13892,
        (ushort) 13893,
        (ushort) 13894,
        (ushort) 14128,
        (ushort) 14129,
        (ushort) 14130,
        (ushort) 14131,
        (ushort) 14132,
        (ushort) 14133,
        (ushort) 14134,
        (ushort) 14135,
        (ushort) 14136,
        (ushort) 14137,
        (ushort) 14145,
        (ushort) 14146,
        (ushort) 14147,
        (ushort) 14148,
        (ushort) 14149,
        (ushort) 14150,
        (ushort) 14384,
        (ushort) 14385,
        (ushort) 14386,
        (ushort) 14387,
        (ushort) 14388,
        (ushort) 14389,
        (ushort) 14390,
        (ushort) 14391,
        (ushort) 14392,
        (ushort) 14393,
        (ushort) 14401,
        (ushort) 14402,
        (ushort) 14403,
        (ushort) 14404,
        (ushort) 14405,
        (ushort) 14406,
        (ushort) 14640,
        (ushort) 14641,
        (ushort) 14642,
        (ushort) 14643,
        (ushort) 14644,
        (ushort) 14645,
        (ushort) 14646,
        (ushort) 14647,
        (ushort) 14648,
        (ushort) 14649,
        (ushort) 14657,
        (ushort) 14658,
        (ushort) 14659,
        (ushort) 14660,
        (ushort) 14661,
        (ushort) 14662,
        (ushort) 16688,
        (ushort) 16689,
        (ushort) 16690,
        (ushort) 16691,
        (ushort) 16692,
        (ushort) 16693,
        (ushort) 16694,
        (ushort) 16695,
        (ushort) 16696,
        (ushort) 16697,
        (ushort) 16705,
        (ushort) 16706,
        (ushort) 16707,
        (ushort) 16708,
        (ushort) 16709,
        (ushort) 16710,
        (ushort) 16944,
        (ushort) 16945,
        (ushort) 16946,
        (ushort) 16947,
        (ushort) 16948,
        (ushort) 16949,
        (ushort) 16950,
        (ushort) 16951,
        (ushort) 16952,
        (ushort) 16953,
        (ushort) 16961,
        (ushort) 16962,
        (ushort) 16963,
        (ushort) 16964,
        (ushort) 16965,
        (ushort) 16966,
        (ushort) 17200,
        (ushort) 17201,
        (ushort) 17202,
        (ushort) 17203,
        (ushort) 17204,
        (ushort) 17205,
        (ushort) 17206,
        (ushort) 17207,
        (ushort) 17208,
        (ushort) 17209,
        (ushort) 17217,
        (ushort) 17218,
        (ushort) 17219,
        (ushort) 17220,
        (ushort) 17221,
        (ushort) 17222,
        (ushort) 17456,
        (ushort) 17457,
        (ushort) 17458,
        (ushort) 17459,
        (ushort) 17460,
        (ushort) 17461,
        (ushort) 17462,
        (ushort) 17463,
        (ushort) 17464,
        (ushort) 17465,
        (ushort) 17473,
        (ushort) 17474,
        (ushort) 17475,
        (ushort) 17476,
        (ushort) 17477,
        (ushort) 17478,
        (ushort) 17712,
        (ushort) 17713,
        (ushort) 17714,
        (ushort) 17715,
        (ushort) 17716,
        (ushort) 17717,
        (ushort) 17718,
        (ushort) 17719,
        (ushort) 17720,
        (ushort) 17721,
        (ushort) 17729,
        (ushort) 17730,
        (ushort) 17731,
        (ushort) 17732,
        (ushort) 17733,
        (ushort) 17734,
        (ushort) 17968,
        (ushort) 17969,
        (ushort) 17970,
        (ushort) 17971,
        (ushort) 17972,
        (ushort) 17973,
        (ushort) 17974,
        (ushort) 17975,
        (ushort) 17976,
        (ushort) 17977,
        (ushort) 17985,
        (ushort) 17986,
        (ushort) 17987,
        (ushort) 17988,
        (ushort) 17989,
        (ushort) 17990
      });

      private StringCompressionLookupTables(
        ImmutableArray<char> list,
        BitArray bitmap,
        ImmutableArray<byte> charToByte,
        ImmutableArray<ushort> byteToTwoChars)
      {
        if (list.Length != 16)
          throw new ArgumentException("list must be length 16.");
        if (bitmap == null)
          throw new ArgumentNullException(nameof (bitmap));
        if (bitmap.Length != 128)
          throw new ArgumentException("bitmap must be length 128.");
        if (charToByte.Length != 256)
          throw new ArgumentException("charToByte must be length 256.");
        if (byteToTwoChars.Length != 256)
          throw new ArgumentException("byteToTwoChars must be length 256.");
        this.List = list;
        this.Bitmap = bitmap;
        this.CharToByte = charToByte;
        this.ByteToTwoChars = byteToTwoChars;
      }

      public ImmutableArray<char> List { get; }

      public BitArray Bitmap { get; }

      public ImmutableArray<byte> CharToByte { get; }

      public ImmutableArray<ushort> ByteToTwoChars { get; }

      private static JsonBinaryEncoding.StringCompressionLookupTables Create(
        char[] list,
        byte[] charSet,
        byte[] charToByte,
        ushort[] byteToTwoChars)
      {
        if (list == null)
          throw new ArgumentNullException(nameof (list));
        if (charSet == null)
          throw new ArgumentNullException(nameof (charSet));
        if (charToByte == null)
          throw new ArgumentNullException(nameof (charToByte));
        if (byteToTwoChars == null)
          throw new ArgumentNullException(nameof (byteToTwoChars));
        return new JsonBinaryEncoding.StringCompressionLookupTables(((IEnumerable<char>) list).ToImmutableArray<char>(), new BitArray(charSet), ((IEnumerable<byte>) charToByte).ToImmutableArray<byte>(), ((IEnumerable<ushort>) byteToTwoChars).ToImmutableArray<ushort>());
      }
    }

    [StructLayout(LayoutKind.Sequential, Size = 3)]
    public readonly struct UInt24
    {
      public static readonly JsonBinaryEncoding.UInt24 MinValue = new JsonBinaryEncoding.UInt24((byte) 0, (byte) 0, (byte) 0);
      public static readonly JsonBinaryEncoding.UInt24 MaxValue = new JsonBinaryEncoding.UInt24(byte.MaxValue, byte.MaxValue, byte.MaxValue);

      public UInt24(byte byte1, byte byte2, byte byte3)
      {
        this.Byte1 = byte1;
        this.Byte2 = byte2;
        this.Byte3 = byte3;
      }

      public byte Byte1 { get; }

      public byte Byte2 { get; }

      public byte Byte3 { get; }

      public static implicit operator int(JsonBinaryEncoding.UInt24 value) => (int) value.Byte3 << 16 | (int) value.Byte2 << 8 | (int) value.Byte1;

      public static explicit operator JsonBinaryEncoding.UInt24(int value) => ((long) value & 4278190080L) == 0L ? new JsonBinaryEncoding.UInt24((byte) (value & (int) byte.MaxValue), (byte) ((value & 65280) >> 8), (byte) ((value & 16711680) >> 16)) : throw new ArgumentOutOfRangeException("value must not have any of the top 8 bits set.");
    }

    public static class Enumerator
    {
      public static IEnumerable<ReadOnlyMemory<byte>> GetArrayItems(ReadOnlyMemory<byte> buffer)
      {
        int typeMarker = (int) buffer.Span[0];
        int start = JsonBinaryEncoding.TypeMarker.IsArray((byte) typeMarker) ? JsonBinaryEncoding.GetFirstValueOffset((byte) typeMarker) : throw new JsonInvalidTokenException();
        buffer = buffer.Slice(0, JsonBinaryEncoding.GetValueLength(buffer.Span));
        int arrayItemLength;
        for (buffer = buffer.Slice(start); buffer.Length != 0; buffer = buffer.Slice(arrayItemLength))
        {
          arrayItemLength = JsonBinaryEncoding.GetValueLength(buffer.Span);
          if (arrayItemLength > buffer.Length)
            throw new JsonInvalidTokenException();
          yield return buffer.Slice(0, arrayItemLength);
        }
      }

      public static IEnumerable<Memory<byte>> GetMutableArrayItems(Memory<byte> buffer)
      {
        foreach (ReadOnlyMemory<byte> arrayItem in JsonBinaryEncoding.Enumerator.GetArrayItems((ReadOnlyMemory<byte>) buffer))
        {
          ArraySegment<byte> segment;
          if (!MemoryMarshal.TryGetArray<byte>(arrayItem, out segment))
            throw new InvalidOperationException("failed to get array segment.");
          yield return (Memory<byte>) segment;
        }
      }

      public static IEnumerable<JsonBinaryEncoding.Enumerator.ObjectProperty> GetObjectProperties(
        ReadOnlyMemory<byte> buffer)
      {
        int typeMarker = (int) buffer.Span[0];
        int start = JsonBinaryEncoding.TypeMarker.IsObject((byte) typeMarker) ? JsonBinaryEncoding.GetFirstValueOffset((byte) typeMarker) : throw new JsonInvalidTokenException();
        buffer = buffer.Slice(0, JsonBinaryEncoding.GetValueLength(buffer.Span));
        buffer = buffer.Slice(start);
        while (buffer.Length != 0)
        {
          int valueLength1 = JsonBinaryEncoding.GetValueLength(buffer.Span);
          if (valueLength1 > buffer.Length)
            throw new JsonInvalidTokenException();
          ReadOnlyMemory<byte> name = buffer.Slice(0, valueLength1);
          buffer = buffer.Slice(valueLength1);
          int valueLength2 = JsonBinaryEncoding.GetValueLength(buffer.Span);
          if (valueLength2 > buffer.Length)
            throw new JsonInvalidTokenException();
          ReadOnlyMemory<byte> readOnlyMemory = buffer.Slice(0, valueLength2);
          buffer = buffer.Slice(valueLength2);
          yield return new JsonBinaryEncoding.Enumerator.ObjectProperty(name, readOnlyMemory);
        }
      }

      public static IEnumerable<JsonBinaryEncoding.Enumerator.MutableObjectProperty> GetMutableObjectProperties(
        Memory<byte> buffer)
      {
        foreach (JsonBinaryEncoding.Enumerator.ObjectProperty objectProperty in JsonBinaryEncoding.Enumerator.GetObjectProperties((ReadOnlyMemory<byte>) buffer))
        {
          ArraySegment<byte> segment1;
          if (!MemoryMarshal.TryGetArray<byte>(objectProperty.Name, out segment1))
            throw new InvalidOperationException("failed to get array segment.");
          ArraySegment<byte> segment2;
          if (!MemoryMarshal.TryGetArray<byte>(objectProperty.Value, out segment2))
            throw new InvalidOperationException("failed to get array segment.");
          yield return new JsonBinaryEncoding.Enumerator.MutableObjectProperty((Memory<byte>) segment1, (Memory<byte>) segment2);
        }
      }

      public readonly struct ObjectProperty
      {
        public ObjectProperty(ReadOnlyMemory<byte> name, ReadOnlyMemory<byte> value)
        {
          this.Name = name;
          this.Value = value;
        }

        public ReadOnlyMemory<byte> Name { get; }

        public ReadOnlyMemory<byte> Value { get; }
      }

      public readonly struct MutableObjectProperty
      {
        public MutableObjectProperty(Memory<byte> name, Memory<byte> value)
        {
          this.Name = name;
          this.Value = value;
        }

        public Memory<byte> Name { get; }

        public Memory<byte> Value { get; }
      }
    }

    private static class FirstValueOffsets
    {
      public static readonly ImmutableArray<int> Offsets = ((IEnumerable<int>) new int[256]
      {
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        1,
        1,
        2,
        3,
        5,
        3,
        5,
        9,
        1,
        1,
        2,
        3,
        5,
        3,
        5,
        9,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
      }).ToImmutableArray<int>();
    }

    public readonly struct MultiByteTypeMarker
    {
      public MultiByteTypeMarker(
        byte length,
        byte one = 0,
        byte two = 0,
        byte three = 0,
        byte four = 0,
        byte five = 0,
        byte six = 0,
        byte seven = 0)
      {
        this.Length = length;
        this.One = one;
        this.Two = two;
        this.Three = three;
        this.Four = four;
        this.Five = five;
        this.Six = six;
        this.Seven = seven;
      }

      public byte Length { get; }

      public byte One { get; }

      public byte Two { get; }

      public byte Three { get; }

      public byte Four { get; }

      public byte Five { get; }

      public byte Six { get; }

      public byte Seven { get; }
    }

    public static class NodeTypes
    {
      private const JsonNodeType Array = JsonNodeType.Array;
      private const JsonNodeType Binary = JsonNodeType.Binary;
      private const JsonNodeType False = JsonNodeType.False;
      private const JsonNodeType Float32 = JsonNodeType.Float32;
      private const JsonNodeType Float64 = JsonNodeType.Float64;
      private const JsonNodeType Guid = JsonNodeType.Guid;
      private const JsonNodeType Int16 = JsonNodeType.Int16;
      private const JsonNodeType Int32 = JsonNodeType.Int32;
      private const JsonNodeType Int64 = JsonNodeType.Int64;
      private const JsonNodeType Int8 = JsonNodeType.Int8;
      private const JsonNodeType Null = JsonNodeType.Null;
      private const JsonNodeType Number = JsonNodeType.Number64;
      private const JsonNodeType Object = JsonNodeType.Object;
      private const JsonNodeType String = JsonNodeType.String;
      private const JsonNodeType True = JsonNodeType.True;
      private const JsonNodeType UInt32 = JsonNodeType.UInt32;
      private const JsonNodeType Unknown = JsonNodeType.Unknown;
      public static readonly ImmutableArray<JsonNodeType> Lookup = ((IEnumerable<JsonNodeType>) new JsonNodeType[256]
      {
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.String,
        JsonNodeType.Unknown,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Number64,
        JsonNodeType.Float32,
        JsonNodeType.Float64,
        JsonNodeType.Unknown,
        JsonNodeType.Null,
        JsonNodeType.False,
        JsonNodeType.True,
        JsonNodeType.Guid,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Int8,
        JsonNodeType.Int16,
        JsonNodeType.Int32,
        JsonNodeType.Int64,
        JsonNodeType.UInt32,
        JsonNodeType.Binary,
        JsonNodeType.Binary,
        JsonNodeType.Binary,
        JsonNodeType.Array,
        JsonNodeType.Array,
        JsonNodeType.Array,
        JsonNodeType.Array,
        JsonNodeType.Array,
        JsonNodeType.Array,
        JsonNodeType.Array,
        JsonNodeType.Array,
        JsonNodeType.Object,
        JsonNodeType.Object,
        JsonNodeType.Object,
        JsonNodeType.Object,
        JsonNodeType.Object,
        JsonNodeType.Object,
        JsonNodeType.Object,
        JsonNodeType.Object,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown,
        JsonNodeType.Unknown
      }).ToImmutableArray<JsonNodeType>();
    }

    private static class StringLengths
    {
      private const int UsrStr1 = -1;
      private const int UsrStr2 = -2;
      private const int StrL1 = -3;
      private const int StrL2 = -4;
      private const int StrL4 = -5;
      private const int StrR1 = -6;
      private const int StrR2 = -7;
      private const int StrR3 = -8;
      private const int StrR4 = -9;
      private const int StrComp = -10;
      private const int NotStr = -11;
      public static readonly ImmutableArray<int> Lengths = ((IEnumerable<int>) new int[256]
      {
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        JsonBinaryEncoding.SystemStrings.Strings[0].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[1].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[2].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[3].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[4].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[5].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[6].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[7].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[8].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[9].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[10].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[11].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[12].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[13].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[14].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[15].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[16].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[17].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[18].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[19].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[20].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[21].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[22].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[23].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[24].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[25].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[26].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[27].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[28].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[29].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[30].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[31].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[32].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[33].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[34].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[35].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[36].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[37].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[38].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[39].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[40].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[41].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[42].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[43].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[44].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[45].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[46].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[47].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[48].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[49].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[50].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[51].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[52].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[53].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[54].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[55].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[56].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[57].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[58].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[59].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[60].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[61].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[62].Utf8String.Length,
        JsonBinaryEncoding.SystemStrings.Strings[63].Utf8String.Length,
        -2,
        -2,
        -2,
        -2,
        -2,
        -2,
        -2,
        -2,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        36,
        36,
        38,
        -10,
        -10,
        -10,
        -10,
        -10,
        -10,
        -10,
        -10,
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        14,
        15,
        16,
        17,
        18,
        19,
        20,
        21,
        22,
        23,
        24,
        25,
        26,
        27,
        28,
        29,
        30,
        31,
        32,
        33,
        34,
        35,
        36,
        37,
        38,
        39,
        40,
        41,
        42,
        43,
        44,
        45,
        46,
        47,
        48,
        49,
        50,
        51,
        52,
        53,
        54,
        55,
        56,
        57,
        58,
        59,
        60,
        61,
        62,
        63,
        -3,
        -4,
        -5,
        -6,
        -7,
        -8,
        -9,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11,
        -11
      }).ToImmutableArray<int>();
    }

    [Flags]
    private enum EncodeGuidParseFlags
    {
      None = 0,
      LowerCase = 1,
      UpperCase = 2,
      Invalid = 255, // 0x000000FF
    }

    public static class SystemStrings
    {
      public static readonly ImmutableArray<UtfAllString> Strings = ((IEnumerable<UtfAllString>) new UtfAllString[64]
      {
        UtfAllString.Create("$s"),
        UtfAllString.Create("$t"),
        UtfAllString.Create("$v"),
        UtfAllString.Create("_attachments"),
        UtfAllString.Create("_etag"),
        UtfAllString.Create("_rid"),
        UtfAllString.Create("_self"),
        UtfAllString.Create("_ts"),
        UtfAllString.Create("attachments/"),
        UtfAllString.Create("coordinates"),
        UtfAllString.Create("geometry"),
        UtfAllString.Create("GeometryCollection"),
        UtfAllString.Create("id"),
        UtfAllString.Create("url"),
        UtfAllString.Create("Value"),
        UtfAllString.Create("label"),
        UtfAllString.Create("LineString"),
        UtfAllString.Create("link"),
        UtfAllString.Create("MultiLineString"),
        UtfAllString.Create("MultiPoint"),
        UtfAllString.Create("MultiPolygon"),
        UtfAllString.Create("name"),
        UtfAllString.Create("Name"),
        UtfAllString.Create("Type"),
        UtfAllString.Create("Point"),
        UtfAllString.Create("Polygon"),
        UtfAllString.Create("properties"),
        UtfAllString.Create("type"),
        UtfAllString.Create("value"),
        UtfAllString.Create("Feature"),
        UtfAllString.Create("FeatureCollection"),
        UtfAllString.Create("_id"),
        UtfAllString.Create("$id"),
        UtfAllString.Create("$pk"),
        UtfAllString.Create("_isEdge"),
        UtfAllString.Create("_partitionKey"),
        UtfAllString.Create("_type"),
        UtfAllString.Create("_value"),
        UtfAllString.Create("data"),
        UtfAllString.Create("Data"),
        UtfAllString.Create("entity"),
        UtfAllString.Create("isDeleted"),
        UtfAllString.Create("IsDeleted"),
        UtfAllString.Create("key"),
        UtfAllString.Create("Key"),
        UtfAllString.Create("Location"),
        UtfAllString.Create("partition"),
        UtfAllString.Create("partition_id"),
        UtfAllString.Create("partitionKey"),
        UtfAllString.Create("PartitionKey"),
        UtfAllString.Create("pk"),
        UtfAllString.Create("state"),
        UtfAllString.Create("State"),
        UtfAllString.Create("status"),
        UtfAllString.Create("Status"),
        UtfAllString.Create("subscriptionId"),
        UtfAllString.Create("SubscriptionId"),
        UtfAllString.Create("tenantId"),
        UtfAllString.Create("TenantId"),
        UtfAllString.Create("timestamp"),
        UtfAllString.Create("Timestamp"),
        UtfAllString.Create("ttl"),
        UtfAllString.Create("userId"),
        UtfAllString.Create("UserId")
      }).ToImmutableArray<UtfAllString>();

      public static int? GetSystemStringId(Utf8Span buffer)
      {
        int? systemStringId;
        switch (((Utf8Span) ref buffer).Length)
        {
          case 2:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength2(((Utf8Span) ref buffer).Span);
            break;
          case 3:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength3(((Utf8Span) ref buffer).Span);
            break;
          case 4:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength4(((Utf8Span) ref buffer).Span);
            break;
          case 5:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength5(((Utf8Span) ref buffer).Span);
            break;
          case 6:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength6(((Utf8Span) ref buffer).Span);
            break;
          case 7:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength7(((Utf8Span) ref buffer).Span);
            break;
          case 8:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength8(((Utf8Span) ref buffer).Span);
            break;
          case 9:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength9(((Utf8Span) ref buffer).Span);
            break;
          case 10:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength10(((Utf8Span) ref buffer).Span);
            break;
          case 11:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength11(((Utf8Span) ref buffer).Span);
            break;
          case 12:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength12(((Utf8Span) ref buffer).Span);
            break;
          case 13:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength13(((Utf8Span) ref buffer).Span);
            break;
          case 14:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength14(((Utf8Span) ref buffer).Span);
            break;
          case 15:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength15(((Utf8Span) ref buffer).Span);
            break;
          case 17:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength17(((Utf8Span) ref buffer).Span);
            break;
          case 18:
            systemStringId = JsonBinaryEncoding.SystemStrings.GetSystemStringIdLength18(((Utf8Span) ref buffer).Span);
            break;
          default:
            systemStringId = new int?();
            break;
        }
        return systemStringId;
      }

      private static int? GetSystemStringIdLength2(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[0].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        if (span1.SequenceEqual<byte>(span3))
          return new int?(0);
        ReadOnlySpan<byte> span4 = buffer;
        Utf8Span span5 = JsonBinaryEncoding.SystemStrings.Strings[1].Utf8String.Span;
        ReadOnlySpan<byte> span6 = ((Utf8Span) ref span5).Span;
        if (span4.SequenceEqual<byte>(span6))
          return new int?(1);
        ReadOnlySpan<byte> span7 = buffer;
        Utf8Span span8 = JsonBinaryEncoding.SystemStrings.Strings[2].Utf8String.Span;
        ReadOnlySpan<byte> span9 = ((Utf8Span) ref span8).Span;
        if (span7.SequenceEqual<byte>(span9))
          return new int?(2);
        ReadOnlySpan<byte> span10 = buffer;
        Utf8Span span11 = JsonBinaryEncoding.SystemStrings.Strings[12].Utf8String.Span;
        ReadOnlySpan<byte> span12 = ((Utf8Span) ref span11).Span;
        if (span10.SequenceEqual<byte>(span12))
          return new int?(12);
        ReadOnlySpan<byte> span13 = buffer;
        Utf8Span span14 = JsonBinaryEncoding.SystemStrings.Strings[50].Utf8String.Span;
        ReadOnlySpan<byte> span15 = ((Utf8Span) ref span14).Span;
        return span13.SequenceEqual<byte>(span15) ? new int?(50) : new int?();
      }

      private static int? GetSystemStringIdLength3(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[7].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        if (span1.SequenceEqual<byte>(span3))
          return new int?(7);
        ReadOnlySpan<byte> span4 = buffer;
        Utf8Span span5 = JsonBinaryEncoding.SystemStrings.Strings[13].Utf8String.Span;
        ReadOnlySpan<byte> span6 = ((Utf8Span) ref span5).Span;
        if (span4.SequenceEqual<byte>(span6))
          return new int?(13);
        ReadOnlySpan<byte> span7 = buffer;
        Utf8Span span8 = JsonBinaryEncoding.SystemStrings.Strings[31].Utf8String.Span;
        ReadOnlySpan<byte> span9 = ((Utf8Span) ref span8).Span;
        if (span7.SequenceEqual<byte>(span9))
          return new int?(31);
        ReadOnlySpan<byte> span10 = buffer;
        Utf8Span span11 = JsonBinaryEncoding.SystemStrings.Strings[32].Utf8String.Span;
        ReadOnlySpan<byte> span12 = ((Utf8Span) ref span11).Span;
        if (span10.SequenceEqual<byte>(span12))
          return new int?(32);
        ReadOnlySpan<byte> span13 = buffer;
        Utf8Span span14 = JsonBinaryEncoding.SystemStrings.Strings[33].Utf8String.Span;
        ReadOnlySpan<byte> span15 = ((Utf8Span) ref span14).Span;
        if (span13.SequenceEqual<byte>(span15))
          return new int?(33);
        ReadOnlySpan<byte> span16 = buffer;
        Utf8Span span17 = JsonBinaryEncoding.SystemStrings.Strings[43].Utf8String.Span;
        ReadOnlySpan<byte> span18 = ((Utf8Span) ref span17).Span;
        if (span16.SequenceEqual<byte>(span18))
          return new int?(43);
        ReadOnlySpan<byte> span19 = buffer;
        Utf8Span span20 = JsonBinaryEncoding.SystemStrings.Strings[44].Utf8String.Span;
        ReadOnlySpan<byte> span21 = ((Utf8Span) ref span20).Span;
        if (span19.SequenceEqual<byte>(span21))
          return new int?(44);
        ReadOnlySpan<byte> span22 = buffer;
        Utf8Span span23 = JsonBinaryEncoding.SystemStrings.Strings[61].Utf8String.Span;
        ReadOnlySpan<byte> span24 = ((Utf8Span) ref span23).Span;
        return span22.SequenceEqual<byte>(span24) ? new int?(61) : new int?();
      }

      private static int? GetSystemStringIdLength4(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[5].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        if (span1.SequenceEqual<byte>(span3))
          return new int?(5);
        ReadOnlySpan<byte> span4 = buffer;
        Utf8Span span5 = JsonBinaryEncoding.SystemStrings.Strings[17].Utf8String.Span;
        ReadOnlySpan<byte> span6 = ((Utf8Span) ref span5).Span;
        if (span4.SequenceEqual<byte>(span6))
          return new int?(17);
        ReadOnlySpan<byte> span7 = buffer;
        Utf8Span span8 = JsonBinaryEncoding.SystemStrings.Strings[21].Utf8String.Span;
        ReadOnlySpan<byte> span9 = ((Utf8Span) ref span8).Span;
        if (span7.SequenceEqual<byte>(span9))
          return new int?(21);
        ReadOnlySpan<byte> span10 = buffer;
        Utf8Span span11 = JsonBinaryEncoding.SystemStrings.Strings[22].Utf8String.Span;
        ReadOnlySpan<byte> span12 = ((Utf8Span) ref span11).Span;
        if (span10.SequenceEqual<byte>(span12))
          return new int?(22);
        ReadOnlySpan<byte> span13 = buffer;
        Utf8Span span14 = JsonBinaryEncoding.SystemStrings.Strings[23].Utf8String.Span;
        ReadOnlySpan<byte> span15 = ((Utf8Span) ref span14).Span;
        if (span13.SequenceEqual<byte>(span15))
          return new int?(23);
        ReadOnlySpan<byte> span16 = buffer;
        Utf8Span span17 = JsonBinaryEncoding.SystemStrings.Strings[27].Utf8String.Span;
        ReadOnlySpan<byte> span18 = ((Utf8Span) ref span17).Span;
        if (span16.SequenceEqual<byte>(span18))
          return new int?(27);
        ReadOnlySpan<byte> span19 = buffer;
        Utf8Span span20 = JsonBinaryEncoding.SystemStrings.Strings[38].Utf8String.Span;
        ReadOnlySpan<byte> span21 = ((Utf8Span) ref span20).Span;
        if (span19.SequenceEqual<byte>(span21))
          return new int?(38);
        ReadOnlySpan<byte> span22 = buffer;
        Utf8Span span23 = JsonBinaryEncoding.SystemStrings.Strings[39].Utf8String.Span;
        ReadOnlySpan<byte> span24 = ((Utf8Span) ref span23).Span;
        return span22.SequenceEqual<byte>(span24) ? new int?(39) : new int?();
      }

      private static int? GetSystemStringIdLength5(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[4].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        if (span1.SequenceEqual<byte>(span3))
          return new int?(4);
        ReadOnlySpan<byte> span4 = buffer;
        Utf8Span span5 = JsonBinaryEncoding.SystemStrings.Strings[6].Utf8String.Span;
        ReadOnlySpan<byte> span6 = ((Utf8Span) ref span5).Span;
        if (span4.SequenceEqual<byte>(span6))
          return new int?(6);
        ReadOnlySpan<byte> span7 = buffer;
        Utf8Span span8 = JsonBinaryEncoding.SystemStrings.Strings[14].Utf8String.Span;
        ReadOnlySpan<byte> span9 = ((Utf8Span) ref span8).Span;
        if (span7.SequenceEqual<byte>(span9))
          return new int?(14);
        ReadOnlySpan<byte> span10 = buffer;
        Utf8Span span11 = JsonBinaryEncoding.SystemStrings.Strings[15].Utf8String.Span;
        ReadOnlySpan<byte> span12 = ((Utf8Span) ref span11).Span;
        if (span10.SequenceEqual<byte>(span12))
          return new int?(15);
        ReadOnlySpan<byte> span13 = buffer;
        Utf8Span span14 = JsonBinaryEncoding.SystemStrings.Strings[24].Utf8String.Span;
        ReadOnlySpan<byte> span15 = ((Utf8Span) ref span14).Span;
        if (span13.SequenceEqual<byte>(span15))
          return new int?(24);
        ReadOnlySpan<byte> span16 = buffer;
        Utf8Span span17 = JsonBinaryEncoding.SystemStrings.Strings[28].Utf8String.Span;
        ReadOnlySpan<byte> span18 = ((Utf8Span) ref span17).Span;
        if (span16.SequenceEqual<byte>(span18))
          return new int?(28);
        ReadOnlySpan<byte> span19 = buffer;
        Utf8Span span20 = JsonBinaryEncoding.SystemStrings.Strings[36].Utf8String.Span;
        ReadOnlySpan<byte> span21 = ((Utf8Span) ref span20).Span;
        if (span19.SequenceEqual<byte>(span21))
          return new int?(36);
        ReadOnlySpan<byte> span22 = buffer;
        Utf8Span span23 = JsonBinaryEncoding.SystemStrings.Strings[51].Utf8String.Span;
        ReadOnlySpan<byte> span24 = ((Utf8Span) ref span23).Span;
        if (span22.SequenceEqual<byte>(span24))
          return new int?(51);
        ReadOnlySpan<byte> span25 = buffer;
        Utf8Span span26 = JsonBinaryEncoding.SystemStrings.Strings[52].Utf8String.Span;
        ReadOnlySpan<byte> span27 = ((Utf8Span) ref span26).Span;
        return span25.SequenceEqual<byte>(span27) ? new int?(52) : new int?();
      }

      private static int? GetSystemStringIdLength6(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[37].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        if (span1.SequenceEqual<byte>(span3))
          return new int?(37);
        ReadOnlySpan<byte> span4 = buffer;
        Utf8Span span5 = JsonBinaryEncoding.SystemStrings.Strings[40].Utf8String.Span;
        ReadOnlySpan<byte> span6 = ((Utf8Span) ref span5).Span;
        if (span4.SequenceEqual<byte>(span6))
          return new int?(40);
        ReadOnlySpan<byte> span7 = buffer;
        Utf8Span span8 = JsonBinaryEncoding.SystemStrings.Strings[53].Utf8String.Span;
        ReadOnlySpan<byte> span9 = ((Utf8Span) ref span8).Span;
        if (span7.SequenceEqual<byte>(span9))
          return new int?(53);
        ReadOnlySpan<byte> span10 = buffer;
        Utf8Span span11 = JsonBinaryEncoding.SystemStrings.Strings[54].Utf8String.Span;
        ReadOnlySpan<byte> span12 = ((Utf8Span) ref span11).Span;
        if (span10.SequenceEqual<byte>(span12))
          return new int?(54);
        ReadOnlySpan<byte> span13 = buffer;
        Utf8Span span14 = JsonBinaryEncoding.SystemStrings.Strings[62].Utf8String.Span;
        ReadOnlySpan<byte> span15 = ((Utf8Span) ref span14).Span;
        if (span13.SequenceEqual<byte>(span15))
          return new int?(62);
        ReadOnlySpan<byte> span16 = buffer;
        Utf8Span span17 = JsonBinaryEncoding.SystemStrings.Strings[63].Utf8String.Span;
        ReadOnlySpan<byte> span18 = ((Utf8Span) ref span17).Span;
        return span16.SequenceEqual<byte>(span18) ? new int?(63) : new int?();
      }

      private static int? GetSystemStringIdLength7(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[25].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        if (span1.SequenceEqual<byte>(span3))
          return new int?(25);
        ReadOnlySpan<byte> span4 = buffer;
        Utf8Span span5 = JsonBinaryEncoding.SystemStrings.Strings[29].Utf8String.Span;
        ReadOnlySpan<byte> span6 = ((Utf8Span) ref span5).Span;
        if (span4.SequenceEqual<byte>(span6))
          return new int?(29);
        ReadOnlySpan<byte> span7 = buffer;
        Utf8Span span8 = JsonBinaryEncoding.SystemStrings.Strings[34].Utf8String.Span;
        ReadOnlySpan<byte> span9 = ((Utf8Span) ref span8).Span;
        return span7.SequenceEqual<byte>(span9) ? new int?(34) : new int?();
      }

      private static int? GetSystemStringIdLength8(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[10].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        if (span1.SequenceEqual<byte>(span3))
          return new int?(10);
        ReadOnlySpan<byte> span4 = buffer;
        Utf8Span span5 = JsonBinaryEncoding.SystemStrings.Strings[45].Utf8String.Span;
        ReadOnlySpan<byte> span6 = ((Utf8Span) ref span5).Span;
        if (span4.SequenceEqual<byte>(span6))
          return new int?(45);
        ReadOnlySpan<byte> span7 = buffer;
        Utf8Span span8 = JsonBinaryEncoding.SystemStrings.Strings[57].Utf8String.Span;
        ReadOnlySpan<byte> span9 = ((Utf8Span) ref span8).Span;
        if (span7.SequenceEqual<byte>(span9))
          return new int?(57);
        ReadOnlySpan<byte> span10 = buffer;
        Utf8Span span11 = JsonBinaryEncoding.SystemStrings.Strings[58].Utf8String.Span;
        ReadOnlySpan<byte> span12 = ((Utf8Span) ref span11).Span;
        return span10.SequenceEqual<byte>(span12) ? new int?(58) : new int?();
      }

      private static int? GetSystemStringIdLength9(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[41].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        if (span1.SequenceEqual<byte>(span3))
          return new int?(41);
        ReadOnlySpan<byte> span4 = buffer;
        Utf8Span span5 = JsonBinaryEncoding.SystemStrings.Strings[42].Utf8String.Span;
        ReadOnlySpan<byte> span6 = ((Utf8Span) ref span5).Span;
        if (span4.SequenceEqual<byte>(span6))
          return new int?(42);
        ReadOnlySpan<byte> span7 = buffer;
        Utf8Span span8 = JsonBinaryEncoding.SystemStrings.Strings[46].Utf8String.Span;
        ReadOnlySpan<byte> span9 = ((Utf8Span) ref span8).Span;
        if (span7.SequenceEqual<byte>(span9))
          return new int?(46);
        ReadOnlySpan<byte> span10 = buffer;
        Utf8Span span11 = JsonBinaryEncoding.SystemStrings.Strings[59].Utf8String.Span;
        ReadOnlySpan<byte> span12 = ((Utf8Span) ref span11).Span;
        if (span10.SequenceEqual<byte>(span12))
          return new int?(59);
        ReadOnlySpan<byte> span13 = buffer;
        Utf8Span span14 = JsonBinaryEncoding.SystemStrings.Strings[60].Utf8String.Span;
        ReadOnlySpan<byte> span15 = ((Utf8Span) ref span14).Span;
        return span13.SequenceEqual<byte>(span15) ? new int?(60) : new int?();
      }

      private static int? GetSystemStringIdLength10(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[16].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        if (span1.SequenceEqual<byte>(span3))
          return new int?(16);
        ReadOnlySpan<byte> span4 = buffer;
        Utf8Span span5 = JsonBinaryEncoding.SystemStrings.Strings[19].Utf8String.Span;
        ReadOnlySpan<byte> span6 = ((Utf8Span) ref span5).Span;
        if (span4.SequenceEqual<byte>(span6))
          return new int?(19);
        ReadOnlySpan<byte> span7 = buffer;
        Utf8Span span8 = JsonBinaryEncoding.SystemStrings.Strings[26].Utf8String.Span;
        ReadOnlySpan<byte> span9 = ((Utf8Span) ref span8).Span;
        return span7.SequenceEqual<byte>(span9) ? new int?(26) : new int?();
      }

      private static int? GetSystemStringIdLength11(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[9].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        return span1.SequenceEqual<byte>(span3) ? new int?(9) : new int?();
      }

      private static int? GetSystemStringIdLength12(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[3].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        if (span1.SequenceEqual<byte>(span3))
          return new int?(3);
        ReadOnlySpan<byte> span4 = buffer;
        Utf8Span span5 = JsonBinaryEncoding.SystemStrings.Strings[8].Utf8String.Span;
        ReadOnlySpan<byte> span6 = ((Utf8Span) ref span5).Span;
        if (span4.SequenceEqual<byte>(span6))
          return new int?(8);
        ReadOnlySpan<byte> span7 = buffer;
        Utf8Span span8 = JsonBinaryEncoding.SystemStrings.Strings[20].Utf8String.Span;
        ReadOnlySpan<byte> span9 = ((Utf8Span) ref span8).Span;
        if (span7.SequenceEqual<byte>(span9))
          return new int?(20);
        ReadOnlySpan<byte> span10 = buffer;
        Utf8Span span11 = JsonBinaryEncoding.SystemStrings.Strings[47].Utf8String.Span;
        ReadOnlySpan<byte> span12 = ((Utf8Span) ref span11).Span;
        if (span10.SequenceEqual<byte>(span12))
          return new int?(47);
        ReadOnlySpan<byte> span13 = buffer;
        Utf8Span span14 = JsonBinaryEncoding.SystemStrings.Strings[48].Utf8String.Span;
        ReadOnlySpan<byte> span15 = ((Utf8Span) ref span14).Span;
        if (span13.SequenceEqual<byte>(span15))
          return new int?(48);
        ReadOnlySpan<byte> span16 = buffer;
        Utf8Span span17 = JsonBinaryEncoding.SystemStrings.Strings[49].Utf8String.Span;
        ReadOnlySpan<byte> span18 = ((Utf8Span) ref span17).Span;
        return span16.SequenceEqual<byte>(span18) ? new int?(49) : new int?();
      }

      private static int? GetSystemStringIdLength13(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[35].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        return span1.SequenceEqual<byte>(span3) ? new int?(35) : new int?();
      }

      private static int? GetSystemStringIdLength14(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[55].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        if (span1.SequenceEqual<byte>(span3))
          return new int?(55);
        ReadOnlySpan<byte> span4 = buffer;
        Utf8Span span5 = JsonBinaryEncoding.SystemStrings.Strings[56].Utf8String.Span;
        ReadOnlySpan<byte> span6 = ((Utf8Span) ref span5).Span;
        return span4.SequenceEqual<byte>(span6) ? new int?(56) : new int?();
      }

      private static int? GetSystemStringIdLength15(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[18].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        return span1.SequenceEqual<byte>(span3) ? new int?(18) : new int?();
      }

      private static int? GetSystemStringIdLength17(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[30].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        return span1.SequenceEqual<byte>(span3) ? new int?(30) : new int?();
      }

      private static int? GetSystemStringIdLength18(ReadOnlySpan<byte> buffer)
      {
        ReadOnlySpan<byte> span1 = buffer;
        Utf8Span span2 = JsonBinaryEncoding.SystemStrings.Strings[11].Utf8String.Span;
        ReadOnlySpan<byte> span3 = ((Utf8Span) ref span2).Span;
        return span1.SequenceEqual<byte>(span3) ? new int?(11) : new int?();
      }

      public static bool TryGetSystemStringId(Utf8Span utf8Span, out int systemStringId)
      {
        int? systemStringId1 = JsonBinaryEncoding.SystemStrings.GetSystemStringId(utf8Span);
        if (!systemStringId1.HasValue)
        {
          systemStringId = 0;
          return false;
        }
        systemStringId = systemStringId1.Value;
        return true;
      }

      public static bool TryGetSystemStringById(int id, out UtfAllString systemString)
      {
        if (id >= JsonBinaryEncoding.SystemStrings.Strings.Length)
        {
          systemString = (UtfAllString) null;
          return false;
        }
        systemString = JsonBinaryEncoding.SystemStrings.Strings[id];
        return true;
      }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public readonly struct TypeMarker
    {
      public const byte LiteralIntMin = 0;
      public const byte LiteralIntMax = 32;
      public const byte SystemString1ByteLengthMin = 32;
      public const byte SystemString1ByteLengthMax = 96;
      public const byte UserString2ByteLengthMin = 96;
      public const byte UserString2ByteLengthMax = 104;
      public const byte LowercaseGuidString = 117;
      public const byte UppercaseGuidString = 118;
      public const byte DoubleQuotedLowercaseGuidString = 119;
      public const byte CompressedLowercaseHexString = 120;
      public const byte CompressedUppercaseHexString = 121;
      public const byte CompressedDateTimeString = 122;
      public const byte Packed4BitString = 123;
      public const byte Packed5BitString = 124;
      public const byte Packed6BitString = 125;
      public const byte Packed7BitStringLength1 = 126;
      public const byte Packed7BitStringLength2 = 127;
      public const byte EncodedStringLengthMin = 128;
      public const byte EncodedStringLengthMax = 192;
      public const byte String1ByteLength = 192;
      public const byte String2ByteLength = 193;
      public const byte String4ByteLength = 194;
      public const byte ReferenceString1ByteOffset = 195;
      public const byte ReferenceString2ByteOffset = 196;
      public const byte ReferenceString3ByteOffset = 197;
      public const byte ReferenceString4ByteOffset = 198;
      public const byte NumberUInt8 = 200;
      public const byte NumberInt16 = 201;
      public const byte NumberInt32 = 202;
      public const byte NumberInt64 = 203;
      public const byte NumberDouble = 204;
      public const byte Float32 = 205;
      public const byte Float64 = 206;
      public const byte Null = 208;
      public const byte False = 209;
      public const byte True = 210;
      public const byte Guid = 211;
      public const byte Int8 = 216;
      public const byte Int16 = 217;
      public const byte Int32 = 218;
      public const byte Int64 = 219;
      public const byte UInt32 = 220;
      public const byte Binary1ByteLength = 221;
      public const byte Binary2ByteLength = 222;
      public const byte Binary4ByteLength = 223;
      public const byte EmptyArray = 224;
      public const byte SingleItemArray = 225;
      public const byte Array1ByteLength = 226;
      public const byte Array2ByteLength = 227;
      public const byte Array4ByteLength = 228;
      public const byte Array1ByteLengthAndCount = 229;
      public const byte Array2ByteLengthAndCount = 230;
      public const byte Array4ByteLengthAndCount = 231;
      public const byte EmptyObject = 232;
      public const byte SinglePropertyObject = 233;
      public const byte Object1ByteLength = 234;
      public const byte Object2ByteLength = 235;
      public const byte Object4ByteLength = 236;
      public const byte Object1ByteLengthAndCount = 237;
      public const byte Object2ByteLengthAndCount = 238;
      public const byte Object4ByteLengthAndCount = 239;
      public const byte Invalid = 255;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsEncodedNumberLiteral(long value) => JsonBinaryEncoding.TypeMarker.InRange(value, 0L, 32L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsFixedLengthNumber(long value) => JsonBinaryEncoding.TypeMarker.InRange(value, 200L, 205L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsNumber(long value) => JsonBinaryEncoding.TypeMarker.IsEncodedNumberLiteral(value) || JsonBinaryEncoding.TypeMarker.IsFixedLengthNumber(value);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static byte EncodeIntegerLiteral(long value) => !JsonBinaryEncoding.TypeMarker.IsEncodedNumberLiteral(value) ? byte.MaxValue : (byte) value;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsSystemString(byte typeMarker) => JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 32L, 96L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsTwoByteEncodedUserString(byte typeMarker) => JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 96L, 104L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsUserString(byte typeMarker) => JsonBinaryEncoding.TypeMarker.IsTwoByteEncodedUserString(typeMarker);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsOneByteEncodedString(byte typeMarker) => JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 32L, 96L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsTwoByteEncodedString(byte typeMarker) => JsonBinaryEncoding.TypeMarker.IsTwoByteEncodedUserString(typeMarker);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsEncodedString(byte typeMarker) => JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 32L, 104L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsEncodedLengthString(byte typeMarker) => JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 128L, 192L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsCompressedString(byte typeMarker) => JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 120L, 128L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsVariableLengthString(byte typeMarker) => JsonBinaryEncoding.TypeMarker.IsEncodedLengthString(typeMarker) || JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 192L, 195L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsReferenceString(byte typeMarker) => JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 195L, 199L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsGuidString(byte typeMarker) => JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 117L, 120L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsHexadecimalString(byte typeMarker) => JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 120L, 122L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsDateTimeString(byte typeMarker) => typeMarker == (byte) 122;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsString(byte typeMarker) => JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 32L, 104L) || JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 117L, 199L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static long GetEncodedStringLength(byte typeMarker) => (long) ((int) typeMarker & (int) sbyte.MaxValue);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool TryGetEncodedStringLengthTypeMarker(long length, out byte typeMarker)
      {
        if (length >= 64L)
        {
          typeMarker = (byte) 0;
          return false;
        }
        typeMarker = (byte) ((ulong) length | 128UL);
        return true;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsNull(byte typeMarker) => typeMarker == (byte) 208;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsFalse(byte typeMarker) => typeMarker == (byte) 209;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsTrue(byte typeMarker) => typeMarker == (byte) 210;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsBoolean(byte typeMarker) => typeMarker == (byte) 209 || typeMarker == (byte) 210;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsGuid(byte typeMarker) => typeMarker == (byte) 211;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsEmptyArray(byte typeMarker) => typeMarker == (byte) 224;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsArray(byte typeMarker) => JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 224L, 232L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsEmptyObject(byte typeMarker) => typeMarker == (byte) 232;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsObject(byte typeMarker) => JsonBinaryEncoding.TypeMarker.InRange((long) typeMarker, 232L, 240L);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsValid(byte typeMarker) => typeMarker != byte.MaxValue;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool InRange(long value, long minInclusive, long maxExclusive) => value >= minInclusive && value < maxExclusive;
    }

    private static class ValueLengths
    {
      private const int L1 = -1;
      private const int L2 = -2;
      private const int L4 = -3;
      private const int LC1 = -4;
      private const int LC2 = -5;
      private const int LC4 = -6;
      private const int CS4L1 = -7;
      private const int CS7L1 = -8;
      private const int CS7L2 = -9;
      private const int CS4BL1 = -10;
      private const int CS5BL1 = -11;
      private const int CS6BL1 = -12;
      private const int Arr1 = -13;
      private const int Obj1 = -14;
      public static readonly ImmutableArray<int> Lookup = ((IEnumerable<int>) new int[256]
      {
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        2,
        2,
        2,
        2,
        2,
        2,
        2,
        2,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        17,
        17,
        17,
        -7,
        -7,
        -7,
        -10,
        -11,
        -12,
        -8,
        -9,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        14,
        15,
        16,
        17,
        18,
        19,
        20,
        21,
        22,
        23,
        24,
        25,
        26,
        27,
        28,
        29,
        30,
        31,
        32,
        33,
        34,
        35,
        36,
        37,
        38,
        39,
        40,
        41,
        42,
        43,
        44,
        45,
        46,
        47,
        48,
        49,
        50,
        51,
        52,
        53,
        54,
        55,
        56,
        57,
        58,
        59,
        60,
        61,
        62,
        63,
        64,
        -1,
        -2,
        -3,
        2,
        3,
        4,
        5,
        0,
        2,
        3,
        5,
        9,
        9,
        5,
        9,
        0,
        1,
        1,
        1,
        17,
        0,
        0,
        0,
        0,
        2,
        3,
        5,
        9,
        5,
        -1,
        -2,
        -3,
        1,
        -13,
        -1,
        -2,
        -3,
        -4,
        -5,
        -6,
        1,
        -14,
        -1,
        -2,
        -3,
        -4,
        -5,
        -6,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
      }).ToImmutableArray<int>();

      public static long GetValueLength(ReadOnlySpan<byte> buffer)
      {
        long valueLength1 = (long) JsonBinaryEncoding.ValueLengths.Lookup[(int) buffer[0]];
        if (valueLength1 < 0L)
        {
          long num = valueLength1 - -14L;
          if ((ulong) num <= 13UL)
          {
            switch ((uint) num)
            {
              case 0:
                long valueLength2 = JsonBinaryEncoding.ValueLengths.GetValueLength(buffer.Slice(1));
                if (valueLength2 == 0L)
                {
                  valueLength1 = 0L;
                  goto label_20;
                }
                else
                {
                  long valueLength3 = JsonBinaryEncoding.ValueLengths.GetValueLength(buffer.Slice(1 + (int) valueLength2));
                  valueLength1 = 1L + valueLength2 + valueLength3;
                  goto label_20;
                }
              case 1:
                long valueLength4 = JsonBinaryEncoding.ValueLengths.GetValueLength(buffer.Slice(1));
                valueLength1 = valueLength4 == 0L ? 0L : 1L + valueLength4;
                goto label_20;
              case 2:
                valueLength1 = (long) (3 + JsonBinaryEncoding.ValueLengths.GetCompressedStringLength((int) buffer[1], 6));
                goto label_20;
              case 3:
                valueLength1 = (long) (3 + JsonBinaryEncoding.ValueLengths.GetCompressedStringLength((int) buffer[1], 5));
                goto label_20;
              case 4:
                valueLength1 = (long) (3 + JsonBinaryEncoding.ValueLengths.GetCompressedStringLength((int) buffer[1], 4));
                goto label_20;
              case 5:
                valueLength1 = (long) (3 + JsonBinaryEncoding.ValueLengths.GetCompressedStringLength((int) JsonBinaryEncoding.GetFixedSizedValue<ushort>(buffer.Slice(1)), 7));
                goto label_20;
              case 6:
                valueLength1 = (long) (2 + JsonBinaryEncoding.ValueLengths.GetCompressedStringLength((int) buffer[1], 7));
                goto label_20;
              case 7:
                valueLength1 = (long) (2 + JsonBinaryEncoding.ValueLengths.GetCompressedStringLength((int) buffer[1], 4));
                goto label_20;
              case 8:
                valueLength1 = (long) (9U + MemoryMarshal.Read<uint>(buffer.Slice(1)));
                goto label_20;
              case 9:
                valueLength1 = (long) (5 + (int) MemoryMarshal.Read<ushort>(buffer.Slice(1)));
                goto label_20;
              case 10:
                valueLength1 = (long) (3 + (int) buffer[1]);
                goto label_20;
              case 11:
                valueLength1 = (long) (5U + MemoryMarshal.Read<uint>(buffer.Slice(1)));
                goto label_20;
              case 12:
                valueLength1 = (long) (3 + (int) MemoryMarshal.Read<ushort>(buffer.Slice(1)));
                goto label_20;
              case 13:
                valueLength1 = (long) (2 + (int) buffer[1]);
                goto label_20;
            }
          }
          throw new ArgumentException(string.Format("Invalid variable length type marker length: {0}", (object) valueLength1));
        }
label_20:
        return valueLength1;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static int GetCompressedStringLength(int length, int numberOfBits) => (length * numberOfBits + 7) / 8;
    }
  }
}

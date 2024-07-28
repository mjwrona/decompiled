// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ResourceIdBase64Decoder
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Documents
{
  internal static class ResourceIdBase64Decoder
  {
    private const byte EncodingPad = 61;
    private const byte Space = 32;
    private static readonly sbyte[] DecodingMap = new sbyte[256]
    {
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 62,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 63,
      (sbyte) 52,
      (sbyte) 53,
      (sbyte) 54,
      (sbyte) 55,
      (sbyte) 56,
      (sbyte) 57,
      (sbyte) 58,
      (sbyte) 59,
      (sbyte) 60,
      (sbyte) 61,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 0,
      (sbyte) 1,
      (sbyte) 2,
      (sbyte) 3,
      (sbyte) 4,
      (sbyte) 5,
      (sbyte) 6,
      (sbyte) 7,
      (sbyte) 8,
      (sbyte) 9,
      (sbyte) 10,
      (sbyte) 11,
      (sbyte) 12,
      (sbyte) 13,
      (sbyte) 14,
      (sbyte) 15,
      (sbyte) 16,
      (sbyte) 17,
      (sbyte) 18,
      (sbyte) 19,
      (sbyte) 20,
      (sbyte) 21,
      (sbyte) 22,
      (sbyte) 23,
      (sbyte) 24,
      (sbyte) 25,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 26,
      (sbyte) 27,
      (sbyte) 28,
      (sbyte) 29,
      (sbyte) 30,
      (sbyte) 31,
      (sbyte) 32,
      (sbyte) 33,
      (sbyte) 34,
      (sbyte) 35,
      (sbyte) 36,
      (sbyte) 37,
      (sbyte) 38,
      (sbyte) 39,
      (sbyte) 40,
      (sbyte) 41,
      (sbyte) 42,
      (sbyte) 43,
      (sbyte) 44,
      (sbyte) 45,
      (sbyte) 46,
      (sbyte) 47,
      (sbyte) 48,
      (sbyte) 49,
      (sbyte) 50,
      (sbyte) 51,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1
    };

    public static unsafe bool TryDecode(string base64string, out byte[] bytes)
    {
      bytes = (byte[]) null;
      if (string.IsNullOrEmpty(base64string))
        return false;
      string str = base64string;
      char* chPtr = (char*) str;
      if ((IntPtr) chPtr != IntPtr.Zero)
        chPtr += RuntimeHelpers.OffsetToStringData;
      int length = base64string.Length;
      while (length > 0 && chPtr[length - 1] == ' ')
        --length;
      int resultLength;
      if (!ResourceIdBase64Decoder.TryComputeResultLength(chPtr, length, out resultLength))
        return false;
      bytes = new byte[resultLength];
      int sourceIndex = 0;
      int destIndex = 0;
      for (int index = length - 4; sourceIndex < index; sourceIndex += 4)
      {
        int num = ResourceIdBase64Decoder.Decode(chPtr, sourceIndex);
        if (num < 0)
        {
          bytes = (byte[]) null;
          return false;
        }
        ResourceIdBase64Decoder.WriteThreeLowOrderBytes(bytes, destIndex, num);
        destIndex += 3;
      }
      int index1 = (int) chPtr[length - 4];
      int index2 = (int) chPtr[length - 3];
      int index3 = (int) chPtr[length - 2];
      int index4 = (int) chPtr[length - 1];
      if (((long) (index1 | index2 | index3 | index4) & 4294967040L) != 0L)
      {
        bytes = (byte[]) null;
        return false;
      }
      int num1 = (int) ResourceIdBase64Decoder.DecodingMap[index1] << 18 | (int) ResourceIdBase64Decoder.DecodingMap[index2] << 12;
      int num2;
      if (index4 != 61)
      {
        int decoding1 = (int) ResourceIdBase64Decoder.DecodingMap[index3];
        int decoding2 = (int) ResourceIdBase64Decoder.DecodingMap[index4];
        int num3 = decoding1 << 6;
        int num4 = num1 | decoding2 | num3;
        if (num4 < 0)
        {
          bytes = (byte[]) null;
          return false;
        }
        if (destIndex > resultLength - 3)
        {
          bytes = (byte[]) null;
          return false;
        }
        ResourceIdBase64Decoder.WriteThreeLowOrderBytes(bytes, destIndex, num4);
        num2 = destIndex + 3;
      }
      else if (index3 != 61)
      {
        int num5 = (int) ResourceIdBase64Decoder.DecodingMap[index3] << 6;
        int num6 = num1 | num5;
        if (num6 < 0)
        {
          bytes = (byte[]) null;
          return false;
        }
        if (destIndex > resultLength - 2)
        {
          bytes = (byte[]) null;
          return false;
        }
        bytes[destIndex] = (byte) (num6 >> 16);
        bytes[destIndex + 1] = (byte) (num6 >> 8);
        num2 = destIndex + 2;
      }
      else
      {
        if (num1 < 0)
        {
          bytes = (byte[]) null;
          return false;
        }
        if (destIndex > resultLength - 1)
        {
          bytes = (byte[]) null;
          return false;
        }
        bytes[destIndex] = (byte) (num1 >> 16);
        num2 = destIndex + 1;
      }
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe int Decode(char* encodedChars, int sourceIndex)
    {
      int index1 = (int) encodedChars[sourceIndex];
      int index2 = (int) encodedChars[sourceIndex + 1];
      int index3 = (int) encodedChars[sourceIndex + 2];
      int index4 = (int) encodedChars[sourceIndex + 3];
      if (((long) (index1 | index2 | index3 | index4) & 4294967040L) != 0L)
        return -1;
      int decoding1 = (int) ResourceIdBase64Decoder.DecodingMap[index1];
      int decoding2 = (int) ResourceIdBase64Decoder.DecodingMap[index2];
      int decoding3 = (int) ResourceIdBase64Decoder.DecodingMap[index3];
      int decoding4 = (int) ResourceIdBase64Decoder.DecodingMap[index4];
      int num1 = decoding1 << 18;
      int num2 = decoding2 << 12;
      int num3 = decoding3 << 6;
      return num1 | decoding4 | num2 | num3;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteThreeLowOrderBytes(byte[] destination, int destIndex, int value)
    {
      destination[destIndex] = (byte) (value >> 16);
      destination[destIndex + 1] = (byte) (value >> 8);
      destination[destIndex + 2] = (byte) value;
    }

    private static unsafe bool TryComputeResultLength(
      char* inputPtr,
      int inputLength,
      out int resultLength)
    {
      resultLength = 0;
      if (inputLength >= 3 && inputPtr[inputLength - 3] == '=')
        return false;
      resultLength = inputLength < 2 || inputPtr[inputLength - 2] != '=' ? (inputPtr[inputLength - 1] != '=' ? (inputLength >> 2) * 3 : (inputLength - 1 >> 2) * 3 + 2) : (inputLength - 2 >> 2) * 3 + 1;
      return true;
    }
  }
}

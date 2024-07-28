// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.HexConverter
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class HexConverter
  {
    public static string ToString(byte[] data) => HexConverter.ToString(data ?? throw new ArgumentNullException(nameof (data)), 0, data.Length, HexConverter.Casing.Upper);

    public static string ToString(byte[] data, int startIndex, int length) => HexConverter.ToString(data ?? throw new ArgumentNullException(nameof (data)), startIndex, length, HexConverter.Casing.Upper);

    public static string ToStringLowerCase(byte[] data) => HexConverter.ToString(data ?? throw new ArgumentNullException(nameof (data)), 0, data.Length, HexConverter.Casing.Lower);

    public static string ToStringLowerCase(byte[] data, int startIndex, int length) => HexConverter.ToString(data ?? throw new ArgumentNullException(nameof (data)), startIndex, length, HexConverter.Casing.Lower);

    public static byte[] ToByteArray(string hexString) => hexString != null ? HexConverter.ToByteArray(hexString, 0, hexString.Length) : throw new ArgumentNullException(nameof (hexString));

    public static byte[] ToByteArray(string hexString, int startIndex, int length)
    {
      if (hexString == null)
        throw new ArgumentNullException(nameof (hexString));
      byte[] bytes;
      if (!HexConverter.TryToByteArray(hexString, startIndex, length, out bytes))
        throw new ArgumentException(hexString + " cannot be converted to byte array.");
      return bytes;
    }

    public static bool TryToByteArray(string hexString, out byte[] bytes)
    {
      if (hexString != null)
        return HexConverter.TryToByteArray(hexString, 0, hexString.Length, out bytes);
      bytes = (byte[]) null;
      return false;
    }

    public static bool TryToByteArray(
      string hexString,
      int startIndex,
      int length,
      out byte[] bytes)
    {
      if (hexString == null)
      {
        bytes = (byte[]) null;
        return false;
      }
      ArgumentUtility.CheckBoundsInclusive(startIndex, 0, hexString.Length, nameof (startIndex));
      ArgumentUtility.CheckBoundsInclusive(length, 0, hexString.Length - startIndex, nameof (length));
      bytes = new byte[(length + 1) / 2];
      bool flag = length % 2 == 0;
      int num1 = startIndex;
      int index = 0;
      while (num1 < startIndex + length)
      {
        char ch1 = hexString[num1++];
        int num2;
        if (ch1 >= 'A' && ch1 <= 'F')
          num2 = (int) ch1 - 65 + 10;
        else if (ch1 >= 'a' && ch1 <= 'f')
          num2 = (int) ch1 - 97 + 10;
        else if (ch1 >= '0' && ch1 <= '9')
        {
          num2 = (int) ch1 - 48;
        }
        else
        {
          bytes = (byte[]) null;
          return false;
        }
        if (flag)
        {
          char ch2 = hexString[num1++];
          int num3;
          if (ch2 >= 'A' && ch2 <= 'F')
            num3 = (int) ch2 - 65 + 10;
          else if (ch2 >= 'a' && ch2 <= 'f')
            num3 = (int) ch2 - 97 + 10;
          else if (ch2 >= '0' && ch2 <= '9')
          {
            num3 = (int) ch2 - 48;
          }
          else
          {
            bytes = (byte[]) null;
            return false;
          }
          bytes[index] = (byte) (num2 << 4 | num3);
        }
        else
        {
          bytes[index] = (byte) num2;
          flag = true;
        }
        ++index;
      }
      return true;
    }

    private static string ToString(
      byte[] data,
      int startIndex,
      int length,
      HexConverter.Casing casing)
    {
      ArgumentUtility.CheckBoundsInclusive(startIndex, 0, data.Length, nameof (startIndex));
      ArgumentUtility.CheckBoundsInclusive(length, 0, data.Length - startIndex, nameof (length));
      if (length == 0)
        return string.Empty;
      char[] buffer = new char[length * 2];
      int startingIndex = 0;
      for (int index = startIndex; index < startIndex + length; ++index)
      {
        HexConverter.ToCharsBuffer(data[index], buffer, startingIndex, casing);
        startingIndex += 2;
      }
      return new string(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ToCharsBuffer(
      byte value,
      char[] buffer,
      int startingIndex,
      HexConverter.Casing casing)
    {
      uint num1 = (uint) ((((int) value & 240) << 4) + ((int) value & 15) - 35209);
      uint num2 = (uint) ((HexConverter.Casing) (((-(int) num1 & 28784) >>> 4) + (int) num1 + 47545) | casing);
      buffer[startingIndex + 1] = (char) (num2 & (uint) byte.MaxValue);
      buffer[startingIndex] = (char) (num2 >> 8);
    }

    private enum Casing : uint
    {
      Upper = 0,
      Lower = 8224, // 0x00002020
    }
  }
}

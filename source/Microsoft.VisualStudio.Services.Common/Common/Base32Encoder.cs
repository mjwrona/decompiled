// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Base32Encoder
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class Base32Encoder
  {
    private const byte m_padNdx = 32;
    private const byte m_bitmask = 31;
    private static char[] m_rgEncodingChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567=".ToCharArray();

    public static string ToBase32String(this byte[] rgbData) => Base32Encoder.Encode(rgbData);

    public static byte[] FromBase32String(this string base32String) => Base32Encoder.Decode(base32String);

    public static string Encode(byte[] rgbData)
    {
      ArgumentUtility.CheckForNull<byte[]>(rgbData, nameof (rgbData));
      int tokenCount = Base32Encoder.GetTokenCount(rgbData);
      char[] chArray = new char[tokenCount];
      for (int index = 0; index < tokenCount; ++index)
        chArray[index] = Base32Encoder.GetToken(rgbData, index);
      return new string(chArray);
    }

    public static byte[] Decode(string base32String)
    {
      ArgumentUtility.CheckForNull<string>(base32String, nameof (base32String));
      if (base32String.Length % 8 != 0 || base32String.Where<char>((Func<char, bool>) (t => !((IEnumerable<char>) Base32Encoder.m_rgEncodingChars).Contains<char>(t))).Count<char>() != 0)
        throw new InvalidOperationException("base32string is not a valid base32 encoding");
      byte[] array = new byte[Base32Encoder.GetByteCount(base32String)];
      int num1 = 0;
      foreach (char ch in base32String.ToUpperInvariant())
      {
        int index = num1 / 8;
        int num2 = num1 % 8;
        byte num3 = (byte) Array.IndexOf<char>(Base32Encoder.m_rgEncodingChars, ch);
        if (num3 != (byte) 32)
        {
          int num4 = 3 - num2;
          if (num4 < 0)
            array[index] |= (byte) ((uint) num3 >> -num4);
          else
            array[index] |= (byte) ((uint) num3 << num4);
          if (num4 < 0 && index < array.Length - 1)
            array[index + 1] |= (byte) ((uint) num3 << 8 + num4);
          num1 += 5;
        }
        else
          break;
      }
      Array.Resize<byte>(ref array, num1 / 8);
      return array;
    }

    private static int GetTokenCount(byte[] rgbData) => (rgbData.Length * 8 + 39) / 40 * 8;

    private static int GetByteCount(string szEncoded) => szEncoded.Replace("=", "").Length * 5 / 8;

    private static char GetToken(byte[] rgbData, int index)
    {
      if (index < 0)
        throw new IndexOutOfRangeException();
      byte index1 = 32;
      int index2 = index * 5 / 8;
      if (index2 < rgbData.Length)
      {
        int num1 = index * 5 % 8 - 3;
        byte num2 = rgbData[index2];
        if (num1 < 0)
          num2 >>= -num1;
        else if (num1 > 0)
        {
          num2 <<= num1;
          if (index2 + 1 < rgbData.Length)
          {
            int num3 = 8 - num1;
            byte num4 = (byte) ((uint) rgbData[index2 + 1] >> num3);
            num2 |= num4;
          }
        }
        index1 = (byte) ((uint) num2 & 31U);
      }
      return Base32Encoder.m_rgEncodingChars[(int) index1];
    }
  }
}

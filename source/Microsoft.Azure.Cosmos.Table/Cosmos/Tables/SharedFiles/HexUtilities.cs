// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tables.SharedFiles.HexUtilities
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Tables.SharedFiles
{
  internal static class HexUtilities
  {
    public const string NullHex = "null";
    private static readonly char[] NybbleToHex = new char[16]
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
    };
    private static readonly char[] NybbleToHexUpper = new char[16]
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
    };
    private static readonly ushort[] HexToNybble = new ushort[55]
    {
      (ushort) 0,
      (ushort) 1,
      (ushort) 2,
      (ushort) 3,
      (ushort) 4,
      (ushort) 5,
      (ushort) 6,
      (ushort) 7,
      (ushort) 8,
      (ushort) 9,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 10,
      (ushort) 11,
      (ushort) 12,
      (ushort) 13,
      (ushort) 14,
      (ushort) 15,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 256,
      (ushort) 10,
      (ushort) 11,
      (ushort) 12,
      (ushort) 13,
      (ushort) 14,
      (ushort) 15
    };

    public static byte[] HexToBytes(string hex)
    {
      if (hex == null)
        return new byte[0];
      hex = hex.Trim();
      int index = hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? 2 : 0;
      byte[] bytes = new byte[(hex.Length - index) / 2];
      int num1 = 0;
      try
      {
        for (; index < hex.Length - 1; index += 2)
        {
          int num2 = (int) HexUtilities.HexToNybble[(int) hex[index] - 48] << 4 | (int) HexUtilities.HexToNybble[(int) hex[index + 1] - 48];
          bytes[num1++] = num2 < 256 ? (byte) num2 : throw new ArgumentException("Invalid hex string " + hex, nameof (hex));
        }
      }
      catch (IndexOutOfRangeException ex)
      {
        throw new ArgumentException("Invalid hex string " + hex, nameof (hex));
      }
      return bytes;
    }

    public static string BytesToHex(IList<byte> bytes)
    {
      if (bytes == null)
        return "null";
      char[] chArray = new char[bytes.Count * 2];
      for (int index = 0; index < bytes.Count; ++index)
      {
        byte num = bytes[index];
        chArray[index * 2] = HexUtilities.NybbleToHex[((int) num & 240) >> 4];
        chArray[index * 2 + 1] = HexUtilities.NybbleToHex[(int) num & 15];
      }
      return new string(chArray);
    }

    public static string BytesToHex(byte[] bytes, int start, int length, bool upperCase = false)
    {
      if (bytes == null)
        return "null";
      char[] chArray = new char[length * 2];
      for (int index = 0; index < length; ++index)
      {
        byte num = bytes[index];
        if (!upperCase)
        {
          chArray[index * 2] = HexUtilities.NybbleToHex[((int) num & 240) >> 4];
          chArray[index * 2 + 1] = HexUtilities.NybbleToHex[(int) num & 15];
        }
        else
        {
          chArray[index * 2] = HexUtilities.NybbleToHexUpper[((int) num & 240) >> 4];
          chArray[index * 2 + 1] = HexUtilities.NybbleToHexUpper[(int) num & 15];
        }
      }
      return new string(chArray);
    }
  }
}

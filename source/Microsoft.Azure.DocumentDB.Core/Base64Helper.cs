// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Base64Helper
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Azure.Documents
{
  internal static class Base64Helper
  {
    public static void SecureStringToNativeBytes(
      SecureString secureString,
      int secureStringLength,
      out IntPtr bytes,
      out uint bytesLength)
    {
      IntPtr num = IntPtr.Zero;
      try
      {
        num = Marshal.AllocCoTaskMem(secureStringLength);
        uint actualLength = 0;
        Base64Helper.ParseStringToIntPtr(secureString, num, secureStringLength, out actualLength);
        bytes = num;
        bytesLength = actualLength;
      }
      catch
      {
        if (num != IntPtr.Zero)
        {
          for (int ofs = 0; ofs < secureStringLength; ++ofs)
            Marshal.WriteByte(num, ofs, (byte) 0);
          Marshal.FreeCoTaskMem(num);
        }
        IntPtr zero = IntPtr.Zero;
        bytes = IntPtr.Zero;
        bytesLength = 0U;
        throw;
      }
    }

    private static void ParseStringToIntPtr(
      SecureString secureString,
      IntPtr bytes,
      int allocationSize,
      out uint actualLength)
    {
      IntPtr num1 = IntPtr.Zero;
      try
      {
        num1 = CustomTypeExtensions.SecureStringToCoTaskMemAnsi(secureString);
        int ofs1 = 0;
        int ofs2 = 0;
        byte num2 = 0;
        while (ofs1 < allocationSize && (num2 = Marshal.ReadByte(num1, ofs1)) != (byte) 0)
        {
          uint num3 = 0;
          int num4 = 0;
          for (int index = 0; index < 4 && ofs1 < allocationSize; ++index)
          {
            byte num5 = Marshal.ReadByte(num1, ofs1);
            int num6 = num5 < (byte) 65 || num5 > (byte) 90 ? (num5 < (byte) 97 || num5 > (byte) 122 ? (num5 < (byte) 48 || num5 > (byte) 57 ? (num5 != (byte) 43 ? (num5 != (byte) 47 ? -1 : 63) : 62) : (int) num5 - 48 + 52) : (int) num5 - 97 + 26) : (int) num5 - 65;
            ++ofs1;
            if (num6 == -1)
            {
              --index;
            }
            else
            {
              num3 = num3 << 6 | (uint) (byte) num6;
              num4 += 6;
            }
          }
          if (ofs2 + num4 / 8 > allocationSize)
            throw new ArgumentException(nameof (allocationSize));
          uint num7 = num3 << 24 - num4;
          for (int index = 0; index < num4 / 8; ++index)
          {
            Marshal.WriteByte(bytes, ofs2, (byte) ((num7 & 16711680U) >> 16));
            ++ofs2;
            num7 <<= 8;
          }
        }
        actualLength = (uint) ofs2;
      }
      finally
      {
        if (num1 != IntPtr.Zero)
        {
          Marshal.ZeroFreeCoTaskMemAnsi(num1);
          IntPtr zero = IntPtr.Zero;
        }
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.ByteOrder
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Json
{
  internal static class ByteOrder
  {
    public static byte Reverse(byte value) => value;

    public static bool Reverse(bool value) => value;

    public static char Reverse(char value) => (char) ((uint) ((int) (ushort) ((uint) value & (uint) byte.MaxValue) << 8) | (uint) (ushort) ((int) value >> 8 & (int) byte.MaxValue));

    public static short Reverse(short value) => (short) ((int) (ushort) ((uint) (ushort) value & (uint) byte.MaxValue) << 8 | (int) (ushort) ((int) (ushort) value >> 8 & (int) byte.MaxValue));

    public static ushort Reverse(ushort value) => (ushort) ((uint) ((int) (ushort) ((uint) value & (uint) byte.MaxValue) << 8) | (uint) (ushort) ((int) value >> 8 & (int) byte.MaxValue));

    public static int Reverse(int value) => (value & (int) byte.MaxValue) << 24 | (value >>> 8 & (int) byte.MaxValue) << 16 | (value >>> 16 & (int) byte.MaxValue) << 8 | value >>> 24 & (int) byte.MaxValue;

    public static uint Reverse(uint value) => (uint) (((int) value & (int) byte.MaxValue) << 24 | (int) (value >> 8 & (uint) byte.MaxValue) << 16 | (int) (value >> 16 & (uint) byte.MaxValue) << 8) | value >> 24 & (uint) byte.MaxValue;

    public static long Reverse(long value) => (value & (long) byte.MaxValue) << 56 | (long) ((ulong) (value >>> 8) & (ulong) byte.MaxValue) << 48 | (long) ((ulong) (value >>> 16) & (ulong) byte.MaxValue) << 40 | (long) ((ulong) (value >>> 24) & (ulong) byte.MaxValue) << 32 | (long) ((ulong) (value >>> 32) & (ulong) byte.MaxValue) << 24 | (long) ((ulong) (value >>> 40) & (ulong) byte.MaxValue) << 16 | (long) ((ulong) (value >>> 48) & (ulong) byte.MaxValue) << 8 | (long) ((ulong) (value >>> 56) & (ulong) byte.MaxValue);

    public static ulong Reverse(ulong value) => (ulong) (((long) value & (long) byte.MaxValue) << 56 | (long) (value >> 8 & (ulong) byte.MaxValue) << 48 | (long) (value >> 16 & (ulong) byte.MaxValue) << 40 | (long) (value >> 24 & (ulong) byte.MaxValue) << 32 | (long) (value >> 32 & (ulong) byte.MaxValue) << 24 | (long) (value >> 40 & (ulong) byte.MaxValue) << 16 | (long) (value >> 48 & (ulong) byte.MaxValue) << 8) | value >> 56 & (ulong) byte.MaxValue;

    public static float Reverse(float value)
    {
      byte[] bytes = BitConverter.GetBytes(value);
      Array.Reverse((Array) bytes);
      return BitConverter.ToSingle(bytes, 0);
    }

    public static double Reverse(double value)
    {
      byte[] bytes = BitConverter.GetBytes(value);
      Array.Reverse((Array) bytes);
      return BitConverter.ToDouble(bytes, 0);
    }
  }
}

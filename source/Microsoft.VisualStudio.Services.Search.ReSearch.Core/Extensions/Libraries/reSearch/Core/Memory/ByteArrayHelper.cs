// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Memory.ByteArrayHelper
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Memory
{
  public static class ByteArrayHelper
  {
    public static readonly uint MaxVUInt32Size = 5;
    public static readonly uint MaxVUInt64Size = 10;

    public static uint GetVUInt32Length(uint value)
    {
      if (value <= 16383U)
        return value > (uint) sbyte.MaxValue ? 2U : 1U;
      if (value <= 2097151U)
        return 3;
      return value <= 268435455U ? 4U : 5U;
    }

    public static uint ReadInt16(byte[] array, uint index, out short value)
    {
      value = (short) ((int) array[(int) index++] + ((int) array[(int) index++] << 8));
      return index;
    }

    public static uint WriteInt16(byte[] array, uint index, short value)
    {
      array[(int) index++] = (byte) value;
      array[(int) index++] = (byte) ((uint) value >> 8);
      return index;
    }

    public static uint ReadUInt16(byte[] array, uint index, out ushort value)
    {
      value = (ushort) ((int) array[(int) index++] + ((int) array[(int) index++] << 8));
      return index;
    }

    public static uint WriteUInt16(byte[] array, uint index, ushort value)
    {
      array[(int) index++] = (byte) value;
      array[(int) index++] = (byte) ((uint) value >> 8);
      return index;
    }

    public static uint ReadInt32(byte[] array, uint index, out int value)
    {
      value = (int) array[(int) index++] + ((int) array[(int) index++] << 8) + ((int) array[(int) index++] << 16) + ((int) array[(int) index++] << 24);
      return index;
    }

    public static uint WriteInt32(byte[] array, uint index, int value)
    {
      array[(int) index++] = (byte) value;
      array[(int) index++] = (byte) (value >> 8);
      array[(int) index++] = (byte) (value >> 16);
      array[(int) index++] = (byte) (value >> 24);
      return index;
    }

    public static uint ReadUInt32(byte[] array, uint index, out uint value)
    {
      value = (uint) ((int) array[(int) index++] + ((int) array[(int) index++] << 8) + ((int) array[(int) index++] << 16) + ((int) array[(int) index++] << 24));
      return index;
    }

    public static uint WriteUInt32(byte[] array, uint index, uint value)
    {
      array[(int) index++] = (byte) value;
      array[(int) index++] = (byte) (value >> 8);
      array[(int) index++] = (byte) (value >> 16);
      array[(int) index++] = (byte) (value >> 24);
      return index;
    }

    public static uint ReadInt64(byte[] array, uint index, out long value)
    {
      value = (long) array[(int) index++] + ((long) array[(int) index++] << 8) + ((long) array[(int) index++] << 16) + ((long) array[(int) index++] << 24) + ((long) array[(int) index++] << 32) + ((long) array[(int) index++] << 40) + ((long) array[(int) index++] << 48) + ((long) array[(int) index++] << 56);
      return index;
    }

    public static uint WriteInt64(byte[] array, uint index, long value)
    {
      array[(int) index++] = (byte) value;
      array[(int) index++] = (byte) (value >> 8);
      array[(int) index++] = (byte) (value >> 16);
      array[(int) index++] = (byte) (value >> 24);
      array[(int) index++] = (byte) (value >> 32);
      array[(int) index++] = (byte) (value >> 40);
      array[(int) index++] = (byte) (value >> 48);
      array[(int) index++] = (byte) (value >> 56);
      return index;
    }

    public static uint ReadUInt64(byte[] array, uint index, out ulong value)
    {
      value = (ulong) ((long) array[(int) index++] + ((long) array[(int) index++] << 8) + ((long) array[(int) index++] << 16) + ((long) array[(int) index++] << 24) + ((long) array[(int) index++] << 32) + ((long) array[(int) index++] << 40) + ((long) array[(int) index++] << 48) + ((long) array[(int) index++] << 56));
      return index;
    }

    public static uint WriteUInt64(byte[] array, uint index, ulong value)
    {
      array[(int) index++] = (byte) value;
      array[(int) index++] = (byte) (value >> 8);
      array[(int) index++] = (byte) (value >> 16);
      array[(int) index++] = (byte) (value >> 24);
      array[(int) index++] = (byte) (value >> 32);
      array[(int) index++] = (byte) (value >> 40);
      array[(int) index++] = (byte) (value >> 48);
      array[(int) index++] = (byte) (value >> 56);
      return index;
    }

    public static uint ReadVInt32(byte[] array, uint index, out int value)
    {
      uint num;
      index = ByteArrayHelper.ReadVUInt32(array, index, out num);
      value = ((int) num & 1) != 0 ? -(int) (num >> 1) - 1 : (int) (num >> 1);
      return index;
    }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "0-value", Justification = "value may not be lower than Int.MinValue")]
    public static uint WriteVInt32(byte[] array, uint index, int value)
    {
      uint num;
      if (((long) value & 2147483648L) != 0L)
      {
        if (value == int.MinValue)
          throw new ArgumentOutOfRangeException(nameof (value), "input must be greater than Int32.MinValue");
        num = (uint) ((-value << 1) - 1);
      }
      else
        num = (uint) (value << 1);
      return ByteArrayHelper.WriteVUInt32(array, index, num);
    }

    public static uint ReadVUInt32(byte[] array, uint index, out uint value)
    {
      value = 0U;
      for (int index1 = 0; (long) index1 < (long) ByteArrayHelper.MaxVUInt32Size; ++index1)
      {
        uint num = (uint) array[(int) index++];
        value += (uint) (((int) num & (int) sbyte.MaxValue) << index1 * 7);
        if (((int) num & 128) == 0)
          return index;
      }
      throw new ArithmeticException();
    }

    public static uint WriteVUInt32(byte[] array, uint index, uint value)
    {
      do
      {
        uint num = value & (uint) sbyte.MaxValue;
        value >>= 7;
        if (value != 0U)
          num += 128U;
        array[(int) index++] = (byte) num;
      }
      while (value != 0U);
      return index;
    }

    public static uint ReadVInt64(byte[] array, uint index, out long value)
    {
      ulong num;
      index = ByteArrayHelper.ReadVUInt64(array, index, out num);
      value = ((long) num & 1L) != 0L ? -(long) (num >> 1) - 1L : (long) (num >> 1);
      return index;
    }

    public static uint WriteVInt64(byte[] array, uint index, long value)
    {
      ulong num = (value & long.MinValue) == 0L ? (ulong) (value << 1) : (ulong) (-value << 1) - 1UL;
      return ByteArrayHelper.WriteVUInt64(array, index, num);
    }

    public static uint ReadVUInt64(byte[] array, uint index, out ulong value)
    {
      value = 0UL;
      for (int index1 = 0; (long) index1 < (long) ByteArrayHelper.MaxVUInt64Size; ++index1)
      {
        ulong num = (ulong) array[(int) index++];
        value += (ulong) (((long) num & (long) sbyte.MaxValue) << index1 * 7);
        if (((long) num & 128L) == 0L)
          return index;
      }
      throw new ArithmeticException();
    }

    public static uint WriteVUInt64(byte[] array, uint index, ulong value)
    {
      do
      {
        ulong num = value & (ulong) sbyte.MaxValue;
        value >>= 7;
        if (value != 0UL)
          num += 128UL;
        array[(int) index++] = (byte) num;
      }
      while (value != 0UL);
      return index;
    }

    public static unsafe uint WriteFloat(byte[] array, uint index, float value)
    {
      uint* numPtr = (uint*) &value;
      return ByteArrayHelper.WriteUInt32(array, index, *numPtr);
    }

    public static unsafe uint WriteDouble(byte[] array, uint index, double value)
    {
      ulong* numPtr = (ulong*) &value;
      return ByteArrayHelper.WriteUInt64(array, index, *numPtr);
    }
  }
}

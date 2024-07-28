// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Memory.ByteView
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.IO;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Memory
{
  public abstract class ByteView : IReader, IWriter, IDisposable
  {
    protected unsafe byte* ArrayStart { get; set; }

    protected unsafe byte* ArrayEnd { get; set; }

    protected unsafe byte* ArrayPosition { get; set; }

    protected long PositionDelta { get; set; }

    public unsafe byte* ArrayPtr => this.ArrayPosition;

    public ulong BytesWritten => this.PositionLong;

    public unsafe uint Position
    {
      get => (uint) ((ulong) (this.ArrayPosition - this.ArrayStart) + (ulong) this.PositionDelta);
      set
      {
        byte* numPtr = this.ArrayStart + value;
        if (numPtr < this.ArrayStart || numPtr > this.ArrayEnd)
          throw new ArgumentOutOfRangeException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Range of {0} should be in the range[{1}, {2}]", (object) (uint) numPtr, (object) (uint) this.ArrayStart, (object) (uint) this.ArrayEnd));
        this.ArrayPosition = numPtr;
      }
    }

    public unsafe ulong PositionLong
    {
      get => (ulong) (this.ArrayPosition - this.ArrayStart) + (ulong) this.PositionDelta;
      set
      {
        byte* numPtr = this.ArrayStart + value;
        if (numPtr < this.ArrayStart || numPtr > this.ArrayEnd)
          throw new ArgumentOutOfRangeException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Range of {0} should be in the range[{1}, {2}]", (object) (uint) numPtr, (object) (uint) this.ArrayStart, (object) (uint) this.ArrayEnd));
        this.ArrayPosition = numPtr;
      }
    }

    public virtual void Close()
    {
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void FillReadBuffer(uint size) => throw new ArgumentOutOfRangeException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Size is outside of the range."));

    private unsafe void CheckReadBuffer(uint size)
    {
      if (this.ArrayPosition + size <= this.ArrayEnd)
        return;
      this.FillReadBuffer(size);
    }

    public virtual unsafe bool IsEOS() => this.ArrayPosition == this.ArrayEnd;

    public unsafe bool ReadBool()
    {
      this.CheckReadBuffer(1U);
      int num = (int) *this.ArrayPosition;
      ++this.ArrayPosition;
      return num != 0;
    }

    public unsafe sbyte ReadInt8()
    {
      this.CheckReadBuffer(1U);
      int num = (int) (sbyte) *this.ArrayPosition;
      ++this.ArrayPosition;
      return (sbyte) num;
    }

    public unsafe short ReadInt16()
    {
      this.CheckReadBuffer(2U);
      int num = (int) *(short*) this.ArrayPosition;
      this.ArrayPosition += 2;
      return (short) num;
    }

    public unsafe int ReadInt32()
    {
      this.CheckReadBuffer(4U);
      int num = *(int*) this.ArrayPosition;
      this.ArrayPosition += 4;
      return num;
    }

    public unsafe long ReadInt64()
    {
      this.CheckReadBuffer(8U);
      long num = *(long*) this.ArrayPosition;
      this.ArrayPosition += 8;
      return num;
    }

    public int ReadVInt32()
    {
      uint num = this.ReadVUInt32();
      return ((int) num & 1) != 0 ? -(int) (num >> 1) - 1 : (int) (num >> 1);
    }

    public long ReadVInt64()
    {
      ulong num = this.ReadVUInt64();
      return ((long) num & 1L) != 0L ? -(long) (num >> 1) - 1L : (long) (num >> 1);
    }

    public unsafe byte ReadUInt8()
    {
      this.CheckReadBuffer(1U);
      int num = (int) *this.ArrayPosition;
      ++this.ArrayPosition;
      return (byte) num;
    }

    public unsafe ushort ReadUInt16()
    {
      this.CheckReadBuffer(2U);
      int num = (int) *(ushort*) this.ArrayPosition;
      this.ArrayPosition += 2;
      return (ushort) num;
    }

    public unsafe uint ReadUInt32()
    {
      this.CheckReadBuffer(4U);
      int num = (int) *(uint*) this.ArrayPosition;
      this.ArrayPosition += 4;
      return (uint) num;
    }

    public unsafe ulong ReadUInt64()
    {
      this.CheckReadBuffer(8U);
      long num = *(long*) this.ArrayPosition;
      this.ArrayPosition += 8;
      return (ulong) num;
    }

    public unsafe float ReadFloat()
    {
      this.CheckReadBuffer(4U);
      double num = (double) *(float*) this.ArrayPosition;
      this.ArrayPosition += 4;
      return (float) num;
    }

    public unsafe double ReadDouble()
    {
      this.CheckReadBuffer(8U);
      double num = *(double*) this.ArrayPosition;
      this.ArrayPosition += 8;
      return num;
    }

    public unsafe uint ReadVUInt32()
    {
      uint num1 = 0;
      for (int index = 0; (long) index < (long) ByteArrayHelper.MaxVUInt32Size; ++index)
      {
        if (this.ArrayPosition == this.ArrayEnd)
          this.CheckReadBuffer(1U);
        uint num2 = (uint) *this.ArrayPosition++;
        num1 += (uint) (((int) num2 & (int) sbyte.MaxValue) << index * 7);
        if (((int) num2 & 128) == 0)
          return num1;
      }
      throw new ArithmeticException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "A variable-length UInt32 cannot take more than {0} bytes.", (object) ByteArrayHelper.MaxVUInt32Size));
    }

    public unsafe uint ReadVUInt32Fast()
    {
      if (this.ArrayPosition + 5 > this.ArrayEnd)
        return this.ReadVUInt32();
      uint num = (uint) *this.ArrayPosition++;
      if (((int) num & 128) != 0)
      {
        num = (num & (uint) sbyte.MaxValue) + ((uint) *this.ArrayPosition++ << 7);
        if (((int) num & 16384) != 0)
        {
          num = (num & 16383U) + ((uint) *this.ArrayPosition++ << 14);
          if (((int) num & 2097152) != 0)
          {
            num = (num & 2097151U) + ((uint) *this.ArrayPosition++ << 21);
            if (((int) num & 268435456) != 0)
              num = (num & 268435455U) + (uint) ((int) *this.ArrayPosition++ << 28 & 15);
          }
        }
      }
      return num;
    }

    public unsafe uint ReadVUInt32Faster_()
    {
      if (this.ArrayPosition + 5 > this.ArrayEnd)
        return this.ReadVUInt32();
      uint num = *(uint*) this.ArrayPosition;
      if (((int) num & 128) == 0)
      {
        ++this.ArrayPosition;
        return num & (uint) sbyte.MaxValue;
      }
      if (((int) num & 16384) == 0)
      {
        this.ArrayPosition += 2;
        return (uint) (((int) num & (int) sbyte.MaxValue) + ((int) (num >> 1) & 16256));
      }
      if (((int) num & 2097152) == 0)
      {
        this.ArrayPosition += 3;
        return (uint) (((int) num & (int) sbyte.MaxValue) + ((int) (num >> 1) & 16256) + ((int) (num >> 2) & 1040384));
      }
      if (((int) num & 268435456) == 0)
      {
        this.ArrayPosition += 4;
        return (uint) (((int) num & (int) sbyte.MaxValue) + ((int) (num >> 1) & 16256) + ((int) (num >> 2) & 1040384) + ((int) (num >> 3) & 266338304));
      }
      this.ArrayPosition += 5;
      return (uint) (((int) num & (int) sbyte.MaxValue) + ((int) (num >> 1) & 16256) + ((int) (num >> 2) & 1040384) + ((int) (num >> 3) & 266338304) + (((int) *this.ArrayPosition++ & 15) << 28));
    }

    public unsafe uint ReadVUInt32Faster()
    {
      if (this.ArrayPosition + 5 > this.ArrayEnd)
        return this.ReadVUInt32();
      uint num = *(uint*) this.ArrayPosition;
      if (((int) num & 128) == 0)
      {
        ++this.ArrayPosition;
        return num & (uint) sbyte.MaxValue;
      }
      if (((int) num & 32768) == 0)
      {
        this.ArrayPosition += 2;
        return (uint) (((int) num & (int) sbyte.MaxValue) + ((int) (num >> 1) & 16256));
      }
      if (((int) num & 8388608) == 0)
      {
        this.ArrayPosition += 3;
        return (num & (uint) sbyte.MaxValue) + ((num & 32512U) >> 1) + ((num & 8323072U) >> 2);
      }
      if (((int) num & int.MinValue) != 0)
        return this.ReadVUInt32();
      this.ArrayPosition += 4;
      return (num & (uint) sbyte.MaxValue) + ((num & 32512U) >> 1) + ((num & 8323072U) >> 2) + ((num & 2130706432U) >> 3);
    }

    public unsafe void SkipVUInt32Fast()
    {
      if (this.ArrayPosition + 5 <= this.ArrayEnd)
      {
        for (int index = 0; index < 5 && ((int) *this.ArrayPosition & 128) != 0; ++this.ArrayPosition)
          ++index;
        ++this.ArrayPosition;
      }
      else
      {
        int num = (int) this.ReadVUInt32();
      }
    }

    public unsafe void SkipVUInt32()
    {
      if (this.ArrayPosition + 5 <= this.ArrayEnd)
      {
        switch (*(uint*) this.ArrayPosition & 2155905152U)
        {
          case 0:
          case 32768:
          case 8388608:
          case 8421376:
          case 2147483648:
          case 2147516416:
          case 2155872256:
          case 2155905024:
            ++this.ArrayPosition;
            break;
          case 128:
          case 8388736:
          case 2147483776:
          case 2155872384:
            this.ArrayPosition += 2;
            break;
          case 32896:
          case 2147516544:
            this.ArrayPosition += 3;
            break;
          case 8421504:
            this.ArrayPosition += 4;
            break;
          case 2155905152:
            this.ArrayPosition += 5;
            break;
        }
      }
      else
      {
        int num = (int) this.ReadVUInt32();
      }
    }

    public unsafe ulong ReadVUInt64()
    {
      ulong num1 = 0;
      for (int index = 0; (long) index < (long) ByteArrayHelper.MaxVUInt64Size; ++index)
      {
        if (this.ArrayPosition == this.ArrayEnd)
          this.CheckReadBuffer(1U);
        ulong num2 = (ulong) *this.ArrayPosition++;
        num1 += (ulong) (((long) num2 & (long) sbyte.MaxValue) << index * 7);
        if (((long) num2 & 128L) == 0L)
          return num1;
      }
      throw new ArithmeticException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "A variable-length UInt64 cannot take more than {0} bytes.", (object) ByteArrayHelper.MaxVUInt64Size));
    }

    public unsafe byte[] ReadBytes(uint length)
    {
      this.CheckReadBuffer(length);
      byte[] destination = new byte[(int) length];
      Marshal.Copy(new IntPtr((void*) this.ArrayPosition), destination, 0, (int) length);
      this.ArrayPosition += length;
      return destination;
    }

    public unsafe uint ReadBytes(byte[] data, uint offset, uint length)
    {
      this.CheckReadBuffer(length);
      Marshal.Copy(new IntPtr((void*) this.ArrayPosition), data, (int) offset, (int) length);
      this.ArrayPosition += length;
      return length;
    }

    public string ReadString()
    {
      int length = this.ReadVInt32();
      return length == -1 ? (string) null : Encoding.UTF8.GetString(this.ReadBytes((uint) length));
    }

    public void SkipBytes(int length) => throw new NotImplementedException();

    public void SkipString() => throw new NotImplementedException();

    public void SkipVInt() => throw new NotImplementedException();

    protected virtual void FillWriteBuffer(uint size) => throw new ArgumentOutOfRangeException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Size is outside of the range."));

    protected unsafe void CheckWriteBuffer(uint size)
    {
      if (this.ArrayPosition + size <= this.ArrayEnd)
        return;
      this.FillWriteBuffer(size);
    }

    public unsafe void WriteBool(bool value)
    {
      this.CheckWriteBuffer(1U);
      *this.ArrayPosition = (byte) value;
      ++this.ArrayPosition;
    }

    public unsafe void WriteInt8(sbyte value)
    {
      this.CheckWriteBuffer(1U);
      *this.ArrayPosition = (byte) value;
      ++this.ArrayPosition;
    }

    public unsafe void WriteInt16(short value)
    {
      this.CheckWriteBuffer(2U);
      *(short*) this.ArrayPosition = value;
      this.ArrayPosition += 2;
    }

    public unsafe void WriteInt32(int value)
    {
      this.CheckWriteBuffer(4U);
      *(int*) this.ArrayPosition = value;
      this.ArrayPosition += 4;
    }

    public unsafe void WriteInt64(long value)
    {
      this.CheckWriteBuffer(8U);
      *(long*) this.ArrayPosition = value;
      this.ArrayPosition += 8;
    }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "0-value", Justification = "value may not be lower than Int.MinValue")]
    public void WriteVInt32(int value)
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
      this.WriteVUInt32(num);
    }

    public void WriteVInt64(long value) => this.WriteVUInt64((value & long.MinValue) == 0L ? (ulong) (value << 1) : (ulong) (-value << 1) - 1UL);

    public unsafe void WriteUInt8(byte value)
    {
      this.CheckWriteBuffer(1U);
      *this.ArrayPosition = value;
      ++this.ArrayPosition;
    }

    public unsafe void WriteUInt16(ushort value)
    {
      this.CheckWriteBuffer(2U);
      *(short*) this.ArrayPosition = (short) value;
      this.ArrayPosition += 2;
    }

    public unsafe void WriteUInt32(uint value)
    {
      this.CheckWriteBuffer(4U);
      *(int*) this.ArrayPosition = (int) value;
      this.ArrayPosition += 4;
    }

    public unsafe void WriteUInt64(ulong value)
    {
      this.CheckWriteBuffer(8U);
      *(long*) this.ArrayPosition = (long) value;
      this.ArrayPosition += 8;
    }

    public unsafe void WriteFloat(float value)
    {
      this.CheckWriteBuffer(4U);
      *(float*) this.ArrayPosition = value;
      this.ArrayPosition += 4;
    }

    public unsafe void WriteDouble(double value)
    {
      this.CheckWriteBuffer(8U);
      *(double*) this.ArrayPosition = value;
      this.ArrayPosition += 8;
    }

    public unsafe void WriteVUInt32(uint value)
    {
      do
      {
        uint num = value & (uint) sbyte.MaxValue;
        value >>= 7;
        if (value != 0U)
          num += 128U;
        if (this.ArrayPosition == this.ArrayEnd)
          this.CheckWriteBuffer(1U);
        *this.ArrayPosition++ = (byte) num;
      }
      while (value != 0U);
    }

    public unsafe void WriteVUInt64(ulong value)
    {
      do
      {
        ulong num = value & (ulong) sbyte.MaxValue;
        value >>= 7;
        if (value != 0UL)
          num += 128UL;
        if (this.ArrayPosition == this.ArrayEnd)
          this.CheckWriteBuffer(1U);
        *this.ArrayPosition++ = (byte) num;
      }
      while (value != 0UL);
    }

    public void WriteBytes(byte[] bytes) => this.WriteBytes(bytes, 0U, (uint) bytes.Length);

    public void WriteBytes(byte[] bytes, uint length) => this.WriteBytes(bytes, 0U, length);

    public unsafe void WriteBytes(byte[] bytes, uint index, uint length)
    {
      this.CheckWriteBuffer(length);
      Marshal.Copy(bytes, (int) index, new IntPtr((void*) this.ArrayPosition), (int) length);
      this.ArrayPosition += length;
    }

    public void WriteString(string str)
    {
      if (str == null)
      {
        this.WriteVInt32(-1);
      }
      else
      {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        this.WriteVInt32(bytes.Length);
        this.WriteBytes(bytes);
      }
    }

    public void WriteString(string str, uint maxByteLength) => throw new NotImplementedException();

    private unsafe void CheckPointer(byte* ptr, uint size)
    {
      ptr += size;
      if (ptr < this.ArrayStart || ptr > this.ArrayEnd)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Tried accessing index {0} in an array of length {1}.", (object) (ptr - size - this.ArrayStart), (object) (this.ArrayEnd - this.ArrayStart)));
    }

    private unsafe void CheckReadPointer(byte* ptr, uint size) => this.CheckPointer(ptr, size);

    private unsafe void CheckWritePointer(byte* ptr, uint size) => this.CheckPointer(ptr, size);

    public unsafe bool ReadBool(uint index)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckReadPointer(ptr, 1U);
      return (bool) *ptr;
    }

    public unsafe sbyte ReadInt8(uint index)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckReadPointer(ptr, 1U);
      return (sbyte) *ptr;
    }

    public unsafe short ReadInt16(uint index)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckReadPointer(ptr, 2U);
      return *(short*) ptr;
    }

    public unsafe int ReadInt32(uint index)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckReadPointer(ptr, 4U);
      return *(int*) ptr;
    }

    public unsafe long ReadInt64(uint index)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckReadPointer(ptr, 8U);
      return *(long*) ptr;
    }

    public int ReadVInt32(uint index)
    {
      uint num = this.ReadVUInt32(index);
      return ((int) num & 1) != 0 ? -(int) (num >> 1) - 1 : (int) (num >> 1);
    }

    public long ReadVInt64(uint index)
    {
      ulong num = this.ReadVUInt64(index);
      return ((long) num & 1L) != 0L ? -(long) (num >> 1) - 1L : (long) (num >> 1);
    }

    public unsafe byte ReadUInt8(uint index)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckReadPointer(ptr, 1U);
      return *ptr;
    }

    public unsafe ushort ReadUInt16(uint index)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckReadPointer(ptr, 2U);
      return *(ushort*) ptr;
    }

    public unsafe uint ReadUInt32(uint index)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckReadPointer(ptr, 4U);
      return *(uint*) ptr;
    }

    public unsafe ulong ReadUInt64(uint index)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckReadPointer(ptr, 8U);
      return (ulong) *(long*) ptr;
    }

    public unsafe double ReadDouble(uint index)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckReadPointer(ptr, 8U);
      return *(double*) ptr;
    }

    public unsafe uint ReadVUInt32(uint index)
    {
      uint num1 = 0;
      byte* ptr = this.ArrayStart + index;
      this.CheckReadPointer(ptr, 1U);
      for (int index1 = 0; (long) index1 < (long) ByteArrayHelper.MaxVUInt32Size; ++index1)
      {
        if (ptr == this.ArrayEnd)
          this.CheckReadPointer(ptr, 1U);
        uint num2 = (uint) *ptr++;
        num1 += (uint) (((int) num2 & (int) sbyte.MaxValue) << index1 * 7);
        if (((int) num2 & 128) == 0)
          return num1;
      }
      throw new ArithmeticException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "A variable-length UInt32 cannot take more than {0} bytes.", (object) ByteArrayHelper.MaxVUInt32Size));
    }

    public unsafe ulong ReadVUInt64(uint index)
    {
      ulong num1 = 0;
      byte* ptr = this.ArrayStart + index;
      this.CheckReadPointer(ptr, 1U);
      for (int index1 = 0; (long) index1 < (long) ByteArrayHelper.MaxVUInt64Size; ++index1)
      {
        if (ptr == this.ArrayEnd)
          this.CheckReadPointer(ptr, 1U);
        ulong num2 = (ulong) *ptr++;
        num1 += (ulong) (((long) num2 & (long) sbyte.MaxValue) << index1 * 7);
        if (((long) num2 & 128L) == 0L)
          return num1;
      }
      throw new ArithmeticException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "A variable-length UInt64 cannot take more than {0} bytes.", (object) ByteArrayHelper.MaxVUInt64Size));
    }

    public unsafe uint ReadBytes(uint viewOffset, byte[] data, uint offset, uint length)
    {
      byte* ptr = this.ArrayStart + viewOffset;
      this.CheckReadPointer(ptr, length);
      for (int index = 0; (long) index < (long) length; ++index)
        data[(long) offset + (long) index] = ptr[index];
      return length;
    }

    public unsafe void WriteBool(uint index, bool value)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckWritePointer(ptr, 1U);
      *ptr = (byte) value;
    }

    public unsafe void WriteInt8(uint index, sbyte value)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckWritePointer(ptr, 1U);
      *ptr = (byte) value;
    }

    public unsafe void WriteInt16(uint index, short value)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckWritePointer(ptr, 2U);
      *(short*) ptr = value;
    }

    public unsafe void WriteInt32(uint index, int value)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckWritePointer(ptr, 4U);
      *(int*) ptr = value;
    }

    public unsafe void WriteInt64(uint index, long value)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckWritePointer(ptr, 8U);
      *(long*) ptr = value;
    }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "0-value", Justification = "value may not be lower than Int.MinValue")]
    public void WriteVInt32(uint index, int value)
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
      this.WriteVUInt32(index, num);
    }

    public void WriteVInt64(uint index, long value)
    {
      ulong num = (value & long.MinValue) == 0L ? (ulong) (value << 1) : (ulong) (-value << 1) - 1UL;
      this.WriteVUInt64(index, num);
    }

    public unsafe void WriteUInt8(uint index, byte value)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckWritePointer(ptr, 1U);
      *ptr = value;
    }

    public unsafe void WriteUInt16(uint index, ushort value)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckWritePointer(ptr, 2U);
      *(short*) ptr = (short) value;
    }

    public unsafe void WriteUInt32(uint index, uint value)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckWritePointer(ptr, 4U);
      *(int*) ptr = (int) value;
    }

    public unsafe void WriteUInt64(uint index, ulong value)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckWritePointer(ptr, 8U);
      *(long*) ptr = (long) value;
    }

    public unsafe void WriteFloat(uint index, float value)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckWritePointer(ptr, 4U);
      *(float*) ptr = value;
    }

    public unsafe void WriteDouble(uint index, double value)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckWritePointer(ptr, 8U);
      *(double*) ptr = value;
    }

    public unsafe void WriteVUInt32(uint index, uint value)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckWritePointer(ptr, 1U);
      do
      {
        uint num = value & (uint) sbyte.MaxValue;
        value >>= 7;
        if (value != 0U)
          num += 128U;
        if (ptr == this.ArrayEnd)
          this.CheckWritePointer(ptr, 1U);
        *ptr++ = (byte) num;
      }
      while (value != 0U);
    }

    public unsafe void WriteVUInt64(uint index, ulong value)
    {
      byte* ptr = this.ArrayStart + index;
      this.CheckWritePointer(ptr, 1U);
      do
      {
        ulong num = value & (ulong) sbyte.MaxValue;
        value >>= 7;
        if (value != 0UL)
          num += 128UL;
        if (ptr == this.ArrayEnd)
          this.CheckWritePointer(ptr, 1U);
        *ptr++ = (byte) num;
      }
      while (value != 0UL);
    }
  }
}

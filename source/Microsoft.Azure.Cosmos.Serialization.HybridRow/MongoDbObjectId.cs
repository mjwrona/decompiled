// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.MongoDbObjectId
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct MongoDbObjectId : IEquatable<MongoDbObjectId>
  {
    public const int Size = 12;
    private unsafe fixed byte data[12];

    public unsafe MongoDbObjectId(uint high, ulong low)
    {
      fixed (byte* numPtr = this.data)
      {
        IntPtr num;
        *(int*) (num = (IntPtr) numPtr) = (int) MongoDbObjectId.SwapByteOrder(high);
        *(long*) (num + 4) = (long) MongoDbObjectId.SwapByteOrder(low);
      }
    }

    public unsafe MongoDbObjectId(ReadOnlySpan<byte> src)
    {
      Contract.Requires(src.Length == 12);
      fixed (byte* numPtr1 = this.data)
        fixed (byte* numPtr2 = &src.GetPinnableReference())
        {
          *(long*) numPtr1 = *(long*) numPtr2;
          *(int*) (numPtr1 + 8) = (int) *(uint*) (numPtr2 + 8);
        }
    }

    public static bool operator ==(MongoDbObjectId left, MongoDbObjectId right) => left.Equals(right);

    public static bool operator !=(MongoDbObjectId left, MongoDbObjectId right) => !left.Equals(right);

    public unsafe bool Equals(MongoDbObjectId other)
    {
      fixed (byte* numPtr = this.data)
      {
        byte* data = other.data;
        if (*(long*) data != *(long*) numPtr || (int) *(uint*) (data + 8) != (int) *(uint*) (numPtr + 8))
          return false;
      }
      return true;
    }

    public override bool Equals(object obj) => obj != null && obj is MongoDbObjectId other && this.Equals(other);

    public override unsafe int GetHashCode()
    {
      int hashCode;
      fixed (byte* numPtr = this.data)
        hashCode = ((0 * 397 ^ *(int*) numPtr) * 397 ^ *(int*) (numPtr + 4)) * 397 ^ *(int*) (numPtr + 8);
      return hashCode;
    }

    public byte[] ToByteArray()
    {
      byte[] dest = new byte[12];
      this.CopyTo((Span<byte>) dest);
      return dest;
    }

    public unsafe void CopyTo(Span<byte> dest)
    {
      Contract.Requires(dest.Length == 12);
      fixed (byte* pointer = this.data)
        new Span<byte>((void*) pointer, 12).CopyTo(dest);
    }

    private static uint SwapByteOrder(uint value) => (uint) (((int) value & (int) byte.MaxValue) << 24 | ((int) value & 65280) << 8) | (value & 16711680U) >> 8 | (value & 4278190080U) >> 24;

    private static ulong SwapByteOrder(ulong value) => (ulong) (((long) value & (long) byte.MaxValue) << 56 | ((long) value & 65280L) << 40 | ((long) value & 16711680L) << 24 | ((long) value & 4278190080L) << 8) | (value & 1095216660480UL) >> 8 | (value & 280375465082880UL) >> 24 | (value & 71776119061217280UL) >> 40 | (value & 18374686479671623680UL) >> 56;
  }
}

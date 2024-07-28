// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Sharpen.BitSet
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Text;

namespace Antlr4.Runtime.Sharpen
{
  internal class BitSet
  {
    private static readonly ulong[] EmptyBits = new ulong[0];
    private const int BitsPerElement = 64;
    private ulong[] _data = BitSet.EmptyBits;
    private static readonly int[] index64 = new int[64]
    {
      0,
      47,
      1,
      56,
      48,
      27,
      2,
      60,
      57,
      49,
      41,
      37,
      28,
      16,
      3,
      61,
      54,
      58,
      35,
      52,
      50,
      42,
      21,
      44,
      38,
      32,
      29,
      23,
      17,
      11,
      4,
      62,
      46,
      55,
      26,
      59,
      40,
      36,
      15,
      53,
      34,
      51,
      20,
      43,
      31,
      22,
      10,
      45,
      25,
      39,
      14,
      33,
      19,
      30,
      9,
      24,
      13,
      18,
      8,
      12,
      7,
      6,
      5,
      63
    };

    public BitSet()
    {
    }

    public BitSet(int nbits)
    {
      if (nbits < 0)
        throw new ArgumentOutOfRangeException(nameof (nbits));
      if (nbits <= 0)
        return;
      this._data = new ulong[(nbits + 64 - 1) / 64];
    }

    private static int GetBitCount(ulong[] value)
    {
      int num1 = 0;
      uint length = (uint) value.Length;
      uint bitCount = 0;
      uint num2 = length - length % 30U;
      uint num3 = 0;
      while (num3 < num2)
      {
        ulong num4 = 0;
        for (uint index = 0; index < 30U; index += 3U)
        {
          ulong num5 = value[(long) num1 + (long) index];
          ulong num6 = value[(long) num1 + (long) index + 1L];
          ulong num7 = value[(long) num1 + (long) index + 2L];
          ulong num8 = num7;
          ulong num9 = num7 & 6148914691236517205UL;
          ulong num10 = num8 >> 1 & 6148914691236517205UL;
          ulong num11 = num5 - (num5 >> 1 & 6148914691236517205UL);
          ulong num12 = num6 - (num6 >> 1 & 6148914691236517205UL);
          ulong num13 = num11 + num9;
          ulong num14 = num12 + num10;
          ulong num15 = (ulong) (((long) num13 & 3689348814741910323L) + ((long) (num13 >> 2) & 3689348814741910323L)) + (ulong) (((long) num14 & 3689348814741910323L) + ((long) (num14 >> 2) & 3689348814741910323L));
          num4 += (ulong) (((long) num15 & 1085102592571150095L) + ((long) (num15 >> 4) & 1085102592571150095L));
        }
        ulong num16 = (ulong) (((long) num4 & 71777214294589695L) + ((long) (num4 >> 8) & 71777214294589695L));
        ulong num17 = (ulong) ((long) num16 + (long) (num16 >> 16) & 281470681808895L);
        ulong num18 = num17 + (num17 >> 32);
        bitCount += (uint) num18;
        num3 += 30U;
        num1 += 30;
      }
      for (uint index = 0; index < length - num2; ++index)
      {
        ulong num19 = value[(long) num1 + (long) index];
        ulong num20 = num19 - (num19 >> 1 & 6148914691236517205UL);
        ulong num21 = (ulong) (((long) num20 & 3689348814741910323L) + ((long) (num20 >> 2) & 3689348814741910323L));
        ulong num22 = (ulong) ((long) num21 + (long) (num21 >> 4) & 1085102592571150095L);
        bitCount += (uint) (num22 * 72340172838076673UL >> 56);
      }
      return (int) bitCount;
    }

    private static int BitScanForward(ulong value) => value == 0UL ? -1 : BitSet.index64[checked ((ulong) (unchecked ((long) value ^ (long) value - 1L * 285870213051386505L) >>> 58))];

    public BitSet Clone() => new BitSet()
    {
      _data = (ulong[]) this._data.Clone()
    };

    public void Clear(int index)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof (index));
      int index1 = index / 64;
      if (index1 >= this._data.Length)
        return;
      this._data[index1] &= (ulong) ~(1L << index % 64);
    }

    public bool this[int index]
    {
      get => this.Get(index);
      set => this.Set(index);
    }

    public bool Get(int index)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof (index));
      int index1 = index / 64;
      return index1 < this._data.Length && (this._data[index1] & (ulong) (1L << index % 64)) > 0UL;
    }

    public void Set(int index)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof (index));
      int index1 = index / 64;
      if (index1 >= this._data.Length)
        Array.Resize<ulong>(ref this._data, Math.Max(this._data.Length * 2, index1 + 1));
      this._data[index1] |= (ulong) (1L << index % 64);
    }

    public bool IsEmpty()
    {
      for (int index = 0; index < this._data.Length; ++index)
      {
        if (this._data[index] != 0UL)
          return false;
      }
      return true;
    }

    public int Cardinality() => BitSet.GetBitCount(this._data);

    public int NextSetBit(int fromIndex)
    {
      if (fromIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (fromIndex));
      if (this.IsEmpty())
        return -1;
      int index = fromIndex / 64;
      if (index >= this._data.Length)
        return -1;
      ulong num1 = this._data[index] & (ulong) ~((1L << fromIndex % 64) - 1L);
      int num2;
      while (true)
      {
        num2 = BitSet.BitScanForward(num1);
        if (num2 < 0)
        {
          ++index;
          if (index < this._data.Length)
            num1 = this._data[index];
          else
            goto label_11;
        }
        else
          break;
      }
      return num2 + index * 64;
label_11:
      return -1;
    }

    public void And(BitSet set)
    {
      if (set == null)
        throw new ArgumentNullException(nameof (set));
      int num = Math.Min(this._data.Length, set._data.Length);
      for (int index = 0; index < num; ++index)
        this._data[index] &= set._data[index];
      for (int index = num; index < this._data.Length; ++index)
        this._data[index] = 0UL;
    }

    public void Or(BitSet set)
    {
      if (set == null)
        throw new ArgumentNullException(nameof (set));
      if (set._data.Length > this._data.Length)
        Array.Resize<ulong>(ref this._data, set._data.Length);
      for (int index = 0; index < set._data.Length; ++index)
        this._data[index] |= set._data[index];
    }

    public override bool Equals(object obj)
    {
      if (!(obj is BitSet bitSet))
        return false;
      if (this.IsEmpty())
        return bitSet.IsEmpty();
      int num = Math.Min(this._data.Length, bitSet._data.Length);
      for (int index = 0; index < num; ++index)
      {
        if ((long) this._data[index] != (long) bitSet._data[index])
          return false;
      }
      for (int index = num; index < this._data.Length; ++index)
      {
        if (this._data[index] != 0UL)
          return false;
      }
      for (int index = num; index < bitSet._data.Length; ++index)
      {
        if (bitSet._data[index] != 0UL)
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      ulong num = 1;
      for (uint index = 0; (long) index < (long) this._data.Length; ++index)
      {
        if (this._data[(int) index] != 0UL)
          num = (num * 31UL ^ (ulong) index) * 31UL ^ this._data[(int) index];
      }
      return num.GetHashCode();
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('{');
      for (int index = this.NextSetBit(0); index >= 0; index = this.NextSetBit(index + 1))
      {
        if (stringBuilder.Length > 1)
          stringBuilder.Append(", ");
        stringBuilder.Append(index);
      }
      stringBuilder.Append('}');
      return stringBuilder.ToString();
    }
  }
}

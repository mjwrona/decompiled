// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SharedFiles.Routing.Int128
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Numerics;

namespace Microsoft.Azure.Documents.SharedFiles.Routing
{
  internal struct Int128
  {
    private readonly BigInteger value;
    private static readonly BigInteger MaxBigIntValue = new BigInteger(new byte[16]
    {
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 128
    });
    public static readonly Int128 MaxValue = new Int128(new BigInteger(new byte[16]
    {
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      (byte) 127
    }));

    private Int128(BigInteger value) => this.value = value % Int128.MaxBigIntValue;

    public static implicit operator Int128(int n) => new Int128(new BigInteger(n));

    public Int128(byte[] data)
    {
      this.value = data.Length == 16 ? new BigInteger(data) : throw new ArgumentException(nameof (data));
      if (this.value > Int128.MaxValue.value)
        throw new ArgumentException();
    }

    public static Int128 operator *(Int128 left, Int128 right) => new Int128(left.value * right.value);

    public static Int128 operator +(Int128 left, Int128 right) => new Int128(left.value + right.value);

    public static Int128 operator -(Int128 left, Int128 right) => new Int128(left.value - right.value);

    public static Int128 operator /(Int128 left, Int128 right) => new Int128(left.value / right.value);

    public static bool operator >(Int128 left, Int128 right) => left.value > right.value;

    public static bool operator <(Int128 left, Int128 right) => left.value < right.value;

    public byte[] Bytes
    {
      get
      {
        byte[] byteArray = this.value.ToByteArray();
        if (byteArray.Length >= 16)
          return byteArray;
        byte[] dst = new byte[16];
        Buffer.BlockCopy((Array) byteArray, 0, (Array) dst, 0, byteArray.Length);
        return dst;
      }
    }
  }
}

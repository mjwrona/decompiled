// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.PartitionKeyHash
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal readonly struct PartitionKeyHash : 
    IComparable<PartitionKeyHash>,
    IEquatable<PartitionKeyHash>
  {
    public PartitionKeyHash(UInt128 value) => this.Value = value;

    public UInt128 Value { get; }

    public int CompareTo(PartitionKeyHash other) => this.Value.CompareTo(other.Value);

    public override bool Equals(object obj)
    {
      if (!(obj is PartitionKeyHash other))
        return false;
      return (ValueType) this == obj || this.Equals(other);
    }

    public bool Equals(PartitionKeyHash other) => this.Value.Equals(other.Value);

    public override int GetHashCode() => this.Value.GetHashCode();

    public override string ToString() => this.Value.ToString();

    public static bool TryParse(string value, out PartitionKeyHash parsedValue)
    {
      UInt128 uInt128;
      if (!UInt128.TryParse(value, out uInt128))
      {
        parsedValue = new PartitionKeyHash();
        return false;
      }
      parsedValue = new PartitionKeyHash(uInt128);
      return true;
    }

    public static PartitionKeyHash Parse(string value)
    {
      PartitionKeyHash parsedValue;
      if (!PartitionKeyHash.TryParse(value, out parsedValue))
        throw new FormatException();
      return parsedValue;
    }

    public static bool operator ==(PartitionKeyHash left, PartitionKeyHash right) => left.Equals(right);

    public static bool operator !=(PartitionKeyHash left, PartitionKeyHash right) => !(left == right);

    public static bool operator <(PartitionKeyHash left, PartitionKeyHash right) => left.CompareTo(right) < 0;

    public static bool operator <=(PartitionKeyHash left, PartitionKeyHash right) => left.CompareTo(right) <= 0;

    public static bool operator >(PartitionKeyHash left, PartitionKeyHash right) => left.CompareTo(right) > 0;

    public static bool operator >=(PartitionKeyHash left, PartitionKeyHash right) => left.CompareTo(right) >= 0;

    public static class V1
    {
      private const int MaxStringLength = 100;
      private static readonly unsafe PartitionKeyHash True = PartitionKeyHash.V1.Hash(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.\u003084FED08B978AF4D7D196A7446A86B58009E636B611DB16211B65A9AADFF29C5, 1));
      private static readonly unsafe PartitionKeyHash False = PartitionKeyHash.V1.Hash(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.DBC1B4C900FFE48D575B5DA5C638040125F65DB0FE3E24494B76EA986457D986, 1));
      private static readonly unsafe PartitionKeyHash Null = PartitionKeyHash.V1.Hash(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.\u0034BF5122F344554C53BDE2EBB8CD2B7E3D1600AD631C385A5D7CCE23C7785459A, 1));
      private static readonly unsafe PartitionKeyHash Undefined = PartitionKeyHash.V1.Hash(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.\u0036E340B9CFFB37A989CA544E6BB780A2C78901D3FB33738768511A30617AFA01D, 1));
      private static readonly unsafe PartitionKeyHash EmptyString = PartitionKeyHash.V1.Hash(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.BEEAD77994CF573341EC17B58BBF7EB34D2711C993C1D976B128B3188DC1829A, 1));

      public static PartitionKeyHash Hash(bool value) => !value ? PartitionKeyHash.V1.False : PartitionKeyHash.V1.True;

      public static unsafe PartitionKeyHash Hash(double value)
      {
        // ISSUE: untyped stack allocation
        Span<byte> bytesForHashing = new Span<byte>((void*) __untypedstackalloc(new IntPtr(9)), 9);
        bytesForHashing[0] = (byte) 5;
        MemoryMarshal.Cast<byte, double>(bytesForHashing.Slice(1))[0] = value;
        return PartitionKeyHash.V1.Hash((ReadOnlySpan<byte>) bytesForHashing);
      }

      public static unsafe PartitionKeyHash Hash(string value)
      {
        switch (value)
        {
          case null:
            throw new ArgumentNullException(nameof (value));
          case "":
            return PartitionKeyHash.V1.EmptyString;
          default:
            ReadOnlySpan<char> src = MemoryExtensions.AsSpan(value, 0, Math.Min(value.Length, 100));
            int length = 1 + Encoding.UTF8.GetByteCount(src);
            // ISSUE: untyped stack allocation
            Span<byte> bytesForHashing = new Span<byte>((void*) __untypedstackalloc((IntPtr) (uint) length), length);
            bytesForHashing[0] = (byte) 8;
            Span<byte> dest = bytesForHashing.Slice(1);
            Encoding.UTF8.GetBytes(src, dest);
            return PartitionKeyHash.V1.Hash((ReadOnlySpan<byte>) bytesForHashing);
        }
      }

      public static PartitionKeyHash HashNull() => PartitionKeyHash.V1.Null;

      public static PartitionKeyHash HashUndefined() => PartitionKeyHash.V1.Undefined;

      private static PartitionKeyHash Hash(ReadOnlySpan<byte> bytesForHashing) => new PartitionKeyHash((UInt128) MurmurHash3.Hash32(bytesForHashing, 0U));
    }

    public static class V2
    {
      private const int MaxStringLength = 2048;
      private static readonly unsafe PartitionKeyHash True = PartitionKeyHash.V2.Hash(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.\u003084FED08B978AF4D7D196A7446A86B58009E636B611DB16211B65A9AADFF29C5, 1));
      private static readonly unsafe PartitionKeyHash False = PartitionKeyHash.V2.Hash(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.DBC1B4C900FFE48D575B5DA5C638040125F65DB0FE3E24494B76EA986457D986, 1));
      private static readonly unsafe PartitionKeyHash Null = PartitionKeyHash.V2.Hash(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.\u0034BF5122F344554C53BDE2EBB8CD2B7E3D1600AD631C385A5D7CCE23C7785459A, 1));
      private static readonly unsafe PartitionKeyHash Undefined = PartitionKeyHash.V2.Hash(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.\u0036E340B9CFFB37A989CA544E6BB780A2C78901D3FB33738768511A30617AFA01D, 1));
      private static readonly unsafe PartitionKeyHash EmptyString = PartitionKeyHash.V2.Hash(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.BEEAD77994CF573341EC17B58BBF7EB34D2711C993C1D976B128B3188DC1829A, 1));

      public static PartitionKeyHash Hash(bool value) => !value ? PartitionKeyHash.V2.False : PartitionKeyHash.V2.True;

      public static unsafe PartitionKeyHash Hash(double value)
      {
        // ISSUE: untyped stack allocation
        Span<byte> bytesForHashing = new Span<byte>((void*) __untypedstackalloc(new IntPtr(9)), 9);
        bytesForHashing[0] = (byte) 5;
        MemoryMarshal.Cast<byte, double>(bytesForHashing.Slice(1))[0] = value;
        return PartitionKeyHash.V2.Hash((ReadOnlySpan<byte>) bytesForHashing);
      }

      public static unsafe PartitionKeyHash Hash(string value)
      {
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        if (value.Length > 2048)
          throw new ArgumentOutOfRangeException("value is too long.");
        if (value.Length == 0)
          return PartitionKeyHash.V2.EmptyString;
        int length = 1 + Encoding.UTF8.GetByteCount(value);
        // ISSUE: untyped stack allocation
        Span<byte> bytesForHashing = new Span<byte>((void*) __untypedstackalloc((IntPtr) (uint) length), length);
        bytesForHashing[0] = (byte) 8;
        Span<byte> dest = bytesForHashing.Slice(1);
        Encoding.UTF8.GetBytes(value, dest);
        return PartitionKeyHash.V2.Hash((ReadOnlySpan<byte>) bytesForHashing);
      }

      public static PartitionKeyHash HashNull() => PartitionKeyHash.V2.Null;

      public static PartitionKeyHash HashUndefined() => PartitionKeyHash.V2.Undefined;

      private static PartitionKeyHash Hash(ReadOnlySpan<byte> bytesForHashing) => new PartitionKeyHash(MurmurHash3.Hash128(bytesForHashing, (UInt128) 0));
    }
  }
}

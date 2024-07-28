// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint.Hash
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint
{
  public class Hash : IComparable<Hash>
  {
    private string m_hexHash;
    private byte[] m_rawHash;

    public Hash(string hexHash)
    {
      if (hexHash == null)
        throw new ArgumentNullException(nameof (hexHash));
      this.m_hexHash = hexHash.Length % 2 == 0 ? hexHash.ToLowerInvariant() : throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("The hexHash argument must be mod 2 : {0}", (object) hexHash)));
    }

    public Hash(byte[] rawHash) => this.m_rawHash = rawHash;

    public byte[] RawHash => this.m_rawHash ?? (this.m_rawHash = HexConverter.ToByteArray(this.m_hexHash));

    public string HexHash => this.m_hexHash ?? (this.m_hexHash = HexConverter.ToStringLowerCase(this.m_rawHash));

    public int CompareTo(Hash other)
    {
      if (other == (Hash) null)
        throw new ArgumentNullException(nameof (other));
      int num1 = Math.Max(this.RawHash.Length, other.RawHash.Length);
      int index1 = this.m_rawHash.Length - num1;
      int index2 = other.m_rawHash.Length - num1;
      for (; num1 > 0; --num1)
      {
        int num2 = (index1 >= 0 ? (int) this.m_rawHash[index1] : 0) - (index2 >= 0 ? (int) other.m_rawHash[index2] : 0);
        if (num2 != 0)
          return num2;
        ++index1;
        ++index2;
      }
      return 0;
    }

    public override bool Equals(object obj)
    {
      Hash hash = obj as Hash;
      if ((object) hash == null)
        return false;
      if ((object) this == (object) hash)
        return true;
      byte[] rawHash1 = this.RawHash;
      byte[] rawHash2 = hash.RawHash;
      if (rawHash1.Length != rawHash2.Length)
        return false;
      for (int index = 0; index < rawHash1.Length; ++index)
      {
        if ((int) rawHash1[index] != (int) rawHash2[index])
          return false;
      }
      return true;
    }

    public static bool operator ==(Hash left, Hash right) => (object) left == null ? (object) right == null : left.Equals((object) right);

    public static bool operator !=(Hash left, Hash right) => !(left == right);

    public static bool operator <(Hash left, Hash right)
    {
      if (left == (Hash) null)
        throw new ArgumentNullException(nameof (left));
      return left.CompareTo(right) < 0;
    }

    public static bool operator >(Hash left, Hash right)
    {
      if (left == (Hash) null)
        throw new ArgumentNullException(nameof (left));
      return left.CompareTo(right) > 0;
    }

    public override unsafe int GetHashCode()
    {
      byte[] rawHash = this.RawHash;
      if (rawHash.Length >= 4)
      {
        fixed (byte* numPtr = &rawHash[0])
          return *(int*) numPtr;
      }
      else
        return rawHash.Length != 0 ? (int) rawHash[0] : 1;
    }

    public override string ToString() => this.HexHash;
  }
}

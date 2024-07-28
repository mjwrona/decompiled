// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Float128
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow
{
  [DebuggerDisplay("{Low,High}")]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public readonly struct Float128 : IEquatable<Float128>
  {
    public const int Size = 16;
    public readonly long Low;
    public readonly long High;

    public Float128(long high, long low)
    {
      this.High = high;
      this.Low = low;
    }

    public static bool operator ==(Float128 left, Float128 right) => left.Equals(right);

    public static bool operator !=(Float128 left, Float128 right) => !left.Equals(right);

    public bool Equals(Float128 other) => this.Low == other.Low && this.High == other.High;

    public override bool Equals(object obj) => obj is Float128 other && this.Equals(other);

    public override int GetHashCode() => HashCode.Combine<long, long>(this.Low, this.High);
  }
}

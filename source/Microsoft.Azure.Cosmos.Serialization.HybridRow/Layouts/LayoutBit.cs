// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutBit
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public readonly struct LayoutBit : IEquatable<LayoutBit>
  {
    public static readonly LayoutBit Invalid = new LayoutBit(-1);
    private readonly int index;

    internal LayoutBit(int index)
    {
      Contract.Requires(index >= -1);
      this.index = index;
    }

    public bool IsInvalid
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.index == -1;
    }

    public int Index
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(LayoutBit left, LayoutBit right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(LayoutBit left, LayoutBit right) => !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetOffset(int offset) => checked (offset + unchecked (this.index / 8));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetBit() => this.index % 8;

    public override bool Equals(object other) => other is LayoutBit other1 && this.Equals(other1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(LayoutBit other) => this.index == other.index;

    public override int GetHashCode() => HashCode.Combine<int>(this.index);

    internal static int DivCeiling(int numerator, int divisor) => checked (numerator + divisor - 1) / divisor;

    internal class Allocator
    {
      private int next;

      public Allocator() => this.next = 0;

      public int NumBytes => LayoutBit.DivCeiling(this.next, 8);

      public LayoutBit Allocate() => new LayoutBit(checked (this.next++));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.BinaryHybridRowSerializer
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct BinaryHybridRowSerializer : IHybridRowSerializer<byte[]>
  {
    public IEqualityComparer<byte[]> Comparer => (IEqualityComparer<byte[]>) BinaryHybridRowSerializer.BinaryComparer.Default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      byte[] value)
    {
      return LayoutType.Binary.WriteSparse(ref row, ref scope, (ReadOnlySpan<byte>) value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Read(ref RowBuffer row, ref RowCursor scope, bool isRoot, out byte[] value) => LayoutType.Binary.ReadSparse(ref row, ref scope, out value);

    public sealed class BinaryComparer : EqualityComparer<byte[]>
    {
      public static readonly BinaryHybridRowSerializer.BinaryComparer Default = new BinaryHybridRowSerializer.BinaryComparer();

      public override bool Equals(byte[] x, byte[] y) => x.AsSpan<byte>().SequenceEqual<byte>((ReadOnlySpan<byte>) y.AsSpan<byte>());

      public override int GetHashCode(byte[] obj)
      {
        HashCode hashCode = new HashCode();
        ReadOnlySpan<ulong> readOnlySpan1 = (ReadOnlySpan<ulong>) MemoryMarshal.Cast<byte, ulong>(obj.AsSpan<byte>());
        ReadOnlySpan<ulong> readOnlySpan2 = readOnlySpan1;
        for (int index = 0; index < readOnlySpan2.Length; ++index)
        {
          ulong num = readOnlySpan2[index];
          ((HashCode) ref hashCode).Add<ulong>(num);
        }
        ReadOnlySpan<byte> readOnlySpan3 = (ReadOnlySpan<byte>) obj.AsSpan<byte>().Slice(checked (readOnlySpan1.Length * 8));
        for (int index = 0; index < readOnlySpan3.Length; ++index)
        {
          byte num = readOnlySpan3[index];
          ((HashCode) ref hashCode).Add<byte>(num);
        }
        return ((HashCode) ref hashCode).ToHashCode();
      }
    }
  }
}

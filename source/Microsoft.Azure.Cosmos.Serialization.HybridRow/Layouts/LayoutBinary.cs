// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutBinary
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using System;
using System.Buffers;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class LayoutBinary : 
    LayoutType<byte[]>,
    ILayoutSpanWritable<byte>,
    ILayoutType,
    ILayoutSpanReadable<byte>,
    ILayoutSequenceWritable<byte>
  {
    internal LayoutBinary()
      : base(LayoutCode.Binary, 0)
    {
    }

    public override string Name => "binary";

    public override bool IsFixed => false;

    public override Result WriteFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      byte[] value)
    {
      Contract.Requires(value != null);
      return this.WriteFixed(ref b, ref scope, col, new ReadOnlySpan<byte>(value));
    }

    public Result WriteFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      ReadOnlySpan<byte> value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      Contract.Requires(col.Size >= 0);
      Contract.Requires(value.Length == col.Size);
      if (scope.immutable)
        return Result.InsufficientPermissions;
      b.WriteFixedBinary(checked (scope.start + col.Offset), value, col.Size);
      b.SetBit(scope.start, col.NullBit);
      return Result.Success;
    }

    public Result WriteFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      ReadOnlySequence<byte> value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      Contract.Requires(col.Size >= 0);
      Contract.Requires(value.Length == (long) col.Size);
      if (scope.immutable)
        return Result.InsufficientPermissions;
      b.WriteFixedBinary(checked (scope.start + col.Offset), value, col.Size);
      b.SetBit(scope.start, col.NullBit);
      return Result.Success;
    }

    public override Result ReadFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out byte[] value)
    {
      ReadOnlySpan<byte> readOnlySpan;
      Result result = this.ReadFixed(ref b, ref scope, col, out readOnlySpan);
      value = result == Result.Success ? readOnlySpan.ToArray() : (byte[]) null;
      return result;
    }

    public Result ReadFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out ReadOnlySpan<byte> value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      Contract.Requires(col.Size >= 0);
      if (!b.ReadBit(scope.start, col.NullBit))
      {
        value = (ReadOnlySpan<byte>) (byte[]) null;
        return Result.NotFound;
      }
      value = b.ReadFixedBinary(checked (scope.start + col.Offset), col.Size);
      return Result.Success;
    }

    public override Result WriteVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      byte[] value)
    {
      Contract.Requires(value != null);
      return this.WriteVariable(ref b, ref scope, col, new ReadOnlySpan<byte>(value));
    }

    public Result WriteVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      ReadOnlySpan<byte> value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      if (scope.immutable)
        return Result.InsufficientPermissions;
      int length = value.Length;
      if (col.Size > 0 && length > col.Size)
        return Result.TooBig;
      bool exists = b.ReadBit(scope.start, col.NullBit);
      int variableValueOffset = b.ComputeVariableValueOffset(scope.layout, scope.start, col.Offset);
      int shift;
      b.WriteVariableBinary(variableValueOffset, value, exists, out shift);
      b.SetBit(scope.start, col.NullBit);
      checked { scope.metaOffset += shift; }
      checked { scope.valueOffset += shift; }
      return Result.Success;
    }

    public Result WriteVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      ReadOnlySequence<byte> value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      if (scope.immutable)
        return Result.InsufficientPermissions;
      int length = checked ((int) value.Length);
      if (col.Size > 0 && length > col.Size)
        return Result.TooBig;
      bool exists = b.ReadBit(scope.start, col.NullBit);
      int variableValueOffset = b.ComputeVariableValueOffset(scope.layout, scope.start, col.Offset);
      int shift;
      b.WriteVariableBinary(variableValueOffset, value, exists, out shift);
      b.SetBit(scope.start, col.NullBit);
      checked { scope.metaOffset += shift; }
      checked { scope.valueOffset += shift; }
      return Result.Success;
    }

    public override Result ReadVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out byte[] value)
    {
      ReadOnlySpan<byte> readOnlySpan;
      Result result = this.ReadVariable(ref b, ref scope, col, out readOnlySpan);
      value = result == Result.Success ? readOnlySpan.ToArray() : (byte[]) null;
      return result;
    }

    public Result ReadVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out ReadOnlySpan<byte> value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      if (!b.ReadBit(scope.start, col.NullBit))
      {
        value = (ReadOnlySpan<byte>) (byte[]) null;
        return Result.NotFound;
      }
      int variableValueOffset = b.ComputeVariableValueOffset(scope.layout, scope.start, col.Offset);
      value = b.ReadVariableBinary(variableValueOffset);
      return Result.Success;
    }

    public override Result WriteSparse(
      ref RowBuffer b,
      ref RowCursor edit,
      byte[] value,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      Contract.Requires(value != null);
      return this.WriteSparse(ref b, ref edit, new ReadOnlySpan<byte>(value), options);
    }

    public Result WriteSparse(
      ref RowBuffer b,
      ref RowCursor edit,
      ReadOnlySpan<byte> value,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      Result result = LayoutType.PrepareSparseWrite(ref b, ref edit, this.TypeArg, options);
      if (result != Result.Success)
        return result;
      b.WriteSparseBinary(ref edit, value, options);
      return Result.Success;
    }

    public Result WriteSparse(
      ref RowBuffer b,
      ref RowCursor edit,
      ReadOnlySequence<byte> value,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      Result result = LayoutType.PrepareSparseWrite(ref b, ref edit, this.TypeArg, options);
      if (result != Result.Success)
        return result;
      b.WriteSparseBinary(ref edit, value, options);
      return Result.Success;
    }

    public override Result ReadSparse(ref RowBuffer b, ref RowCursor edit, out byte[] value)
    {
      ReadOnlySpan<byte> readOnlySpan;
      Result result = this.ReadSparse(ref b, ref edit, out readOnlySpan);
      value = result == Result.Success ? readOnlySpan.ToArray() : (byte[]) null;
      return result;
    }

    public Result ReadSparse(ref RowBuffer b, ref RowCursor edit, out ReadOnlySpan<byte> value)
    {
      Result result = LayoutType.PrepareSparseRead(ref b, ref edit, this.LayoutCode);
      if (result != Result.Success)
      {
        value = (ReadOnlySpan<byte>) (byte[]) null;
        return result;
      }
      value = b.ReadSparseBinary(ref edit);
      return Result.Success;
    }
  }
}

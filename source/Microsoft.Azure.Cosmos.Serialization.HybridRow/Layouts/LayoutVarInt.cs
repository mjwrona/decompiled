// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutVarInt
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class LayoutVarInt : LayoutType<long>
  {
    internal LayoutVarInt()
      : base(LayoutCode.VarInt, 0)
    {
    }

    public override string Name => "varint";

    public override bool IsFixed => false;

    public override bool IsVarint => true;

    public override Result WriteFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      long value)
    {
      Contract.Fail("Not Implemented");
      return Result.Failure;
    }

    public override Result ReadFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out long value)
    {
      Contract.Fail("Not Implemented");
      value = 0L;
      return Result.Failure;
    }

    public override Result WriteVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      long value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      if (scope.immutable)
        return Result.InsufficientPermissions;
      bool exists = b.ReadBit(scope.start, col.NullBit);
      int variableValueOffset = b.ComputeVariableValueOffset(scope.layout, scope.start, col.Offset);
      int shift;
      b.WriteVariableInt(variableValueOffset, value, exists, out shift);
      b.SetBit(scope.start, col.NullBit);
      checked { scope.metaOffset += shift; }
      checked { scope.valueOffset += shift; }
      return Result.Success;
    }

    public override Result ReadVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out long value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      if (!b.ReadBit(scope.start, col.NullBit))
      {
        value = 0L;
        return Result.NotFound;
      }
      int variableValueOffset = b.ComputeVariableValueOffset(scope.layout, scope.start, col.Offset);
      value = b.ReadVariableInt(variableValueOffset);
      return Result.Success;
    }

    public override Result WriteSparse(
      ref RowBuffer b,
      ref RowCursor edit,
      long value,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      Result result = LayoutType.PrepareSparseWrite(ref b, ref edit, this.TypeArg, options);
      if (result != Result.Success)
        return result;
      b.WriteSparseVarInt(ref edit, value, options);
      return Result.Success;
    }

    public override Result ReadSparse(ref RowBuffer b, ref RowCursor edit, out long value)
    {
      Result result = LayoutType.PrepareSparseRead(ref b, ref edit, this.LayoutCode);
      if (result != Result.Success)
      {
        value = 0L;
        return result;
      }
      value = b.ReadSparseVarInt(ref edit);
      return Result.Success;
    }
  }
}

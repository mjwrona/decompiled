// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutType`1
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public abstract class LayoutType<T> : LayoutType
  {
    private readonly TypeArgument typeArg;

    internal LayoutType(LayoutCode code, int size)
      : base(code, false, size)
    {
      this.typeArg = new TypeArgument((LayoutType) this);
    }

    internal TypeArgument TypeArg
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.typeArg;
    }

    public Result HasValue(ref RowBuffer b, ref RowCursor scope, LayoutColumn col) => !b.ReadBit(scope.start, col.NullBit) ? Result.NotFound : Result.Success;

    public abstract Result WriteFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      T value);

    public abstract Result ReadFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out T value);

    public Result DeleteFixed(ref RowBuffer b, ref RowCursor scope, LayoutColumn col)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      if (scope.immutable)
        return Result.InsufficientPermissions;
      if (col.NullBit.IsInvalid)
        return Result.TypeMismatch;
      b.UnsetBit(scope.start, col.NullBit);
      return Result.Success;
    }

    public Result DeleteVariable(ref RowBuffer b, ref RowCursor scope, LayoutColumn col)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      if (scope.immutable)
        return Result.InsufficientPermissions;
      if (b.ReadBit(scope.start, col.NullBit))
      {
        int variableValueOffset = b.ComputeVariableValueOffset(scope.layout, scope.start, col.Offset);
        b.DeleteVariable(variableValueOffset, this.IsVarint);
        b.UnsetBit(scope.start, col.NullBit);
      }
      return Result.Success;
    }

    public virtual Result WriteVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      T value)
    {
      return Result.Failure;
    }

    public virtual Result ReadVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out T value)
    {
      value = default (T);
      return Result.Failure;
    }

    public Result DeleteSparse(ref RowBuffer b, ref RowCursor edit)
    {
      Result result = LayoutType.PrepareSparseDelete(ref b, ref edit, this.LayoutCode);
      if (result != Result.Success)
        return result;
      b.DeleteSparse(ref edit);
      return Result.Success;
    }

    public abstract Result WriteSparse(
      ref RowBuffer b,
      ref RowCursor edit,
      T value,
      UpdateOptions options = UpdateOptions.Upsert);

    public abstract Result ReadSparse(ref RowBuffer b, ref RowCursor edit, out T value);
  }
}

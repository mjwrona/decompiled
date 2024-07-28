// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutScope
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public abstract class LayoutScope : LayoutType
  {
    internal readonly bool IsUniqueScope;
    internal readonly bool IsIndexedScope;
    internal readonly bool IsSizedScope;
    internal readonly bool IsFixedArity;
    internal readonly bool IsTypedScope;

    protected LayoutScope(
      LayoutCode code,
      bool immutable,
      bool isSizedScope,
      bool isIndexedScope,
      bool isFixedArity,
      bool isUniqueScope,
      bool isTypedScope)
      : base(code, immutable, 0)
    {
      this.IsSizedScope = isSizedScope;
      this.IsIndexedScope = isIndexedScope;
      this.IsFixedArity = isFixedArity;
      this.IsUniqueScope = isUniqueScope;
      this.IsTypedScope = isTypedScope;
    }

    public override sealed bool IsFixed => false;

    public Result ReadScope(ref RowBuffer b, ref RowCursor edit, out RowCursor value)
    {
      Result result = LayoutType.PrepareSparseRead(ref b, ref edit, this.LayoutCode);
      if (result != Result.Success)
      {
        value = new RowCursor();
        return result;
      }
      value = b.SparseIteratorReadScope(ref edit, this.Immutable || edit.immutable || edit.scopeType.IsUniqueScope);
      return Result.Success;
    }

    public abstract Result WriteScope(
      ref RowBuffer b,
      ref RowCursor scope,
      TypeArgumentList typeArgs,
      out RowCursor value,
      UpdateOptions options = UpdateOptions.Upsert);

    public virtual Result WriteScope<TContext>(
      ref RowBuffer b,
      ref RowCursor scope,
      TypeArgumentList typeArgs,
      TContext context,
      LayoutScope.WriterFunc<TContext> func,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      RowCursor rowCursor;
      Result result1 = this.WriteScope(ref b, ref scope, typeArgs, out rowCursor, options);
      if (result1 != Result.Success)
        return result1;
      Result result2 = func != null ? func(ref b, ref rowCursor, context) : Result.Success;
      if (result2 != Result.Success)
      {
        int num = (int) this.DeleteScope(ref b, ref scope);
        return result2;
      }
      scope.Skip(ref b, ref rowCursor);
      return Result.Success;
    }

    public Result DeleteScope(ref RowBuffer b, ref RowCursor edit)
    {
      Result result = LayoutType.PrepareSparseDelete(ref b, ref edit, this.LayoutCode);
      if (result != Result.Success)
        return result;
      b.DeleteSparse(ref edit);
      return Result.Success;
    }

    internal virtual bool HasImplicitTypeCode(ref RowCursor edit) => false;

    internal virtual void SetImplicitTypeCode(ref RowCursor edit) => Contract.Fail("No implicit type codes.");

    internal virtual void ReadSparsePath(ref RowBuffer row, ref RowCursor edit)
    {
      int pathLenInBytes;
      edit.pathToken = row.ReadSparsePathLen(edit.layout, edit.valueOffset, out pathLenInBytes, out edit.pathOffset);
      checked { edit.valueOffset += pathLenInBytes; }
    }

    public delegate Result WriterFunc<in TContext>(
      ref RowBuffer b,
      ref RowCursor scope,
      TContext context);
  }
}

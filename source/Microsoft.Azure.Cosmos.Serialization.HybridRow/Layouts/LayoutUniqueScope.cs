// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutUniqueScope
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public abstract class LayoutUniqueScope : LayoutIndexedScope
  {
    protected LayoutUniqueScope(
      LayoutCode code,
      bool immutable,
      bool isSizedScope,
      bool isTypedScope)
      : base(code, immutable, isSizedScope, false, true, isTypedScope)
    {
    }

    public abstract TypeArgument FieldType(ref RowCursor scope);

    public override Result WriteScope<TContext>(
      ref RowBuffer b,
      ref RowCursor scope,
      TypeArgumentList typeArgs,
      TContext context,
      LayoutScope.WriterFunc<TContext> func,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      RowCursor src;
      Result result1 = this.WriteScope(ref b, ref scope, typeArgs, out src, options);
      if (result1 != Result.Success)
        return result1;
      RowCursor dest;
      src.Clone(out dest);
      dest.deferUniqueIndex = true;
      Result result2 = func != null ? func(ref b, ref dest, context) : Result.Success;
      if (result2 != Result.Success)
      {
        int num = (int) this.DeleteScope(ref b, ref scope);
        return result2;
      }
      src.count = dest.count;
      Result result3 = b.TypedCollectionUniqueIndexRebuild(ref src);
      if (result3 != Result.Success)
      {
        int num = (int) this.DeleteScope(ref b, ref scope);
        return result3;
      }
      scope.Skip(ref b, ref dest);
      return Result.Success;
    }

    public Result MoveField(
      ref RowBuffer b,
      ref RowCursor destinationScope,
      ref RowCursor sourceEdit,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      RowCursor dstEdit;
      Result result = LayoutType.PrepareSparseMove(ref b, ref destinationScope, (LayoutScope) this, this.FieldType(ref destinationScope), ref sourceEdit, options, out dstEdit);
      if (result != Result.Success)
        return result;
      b.TypedCollectionMoveField(ref dstEdit, ref sourceEdit, (RowOptions) options);
      destinationScope.count = dstEdit.count;
      return Result.Success;
    }

    public Result Find(
      ref RowBuffer b,
      ref RowCursor scope,
      ref RowCursor patternScope,
      out RowCursor value)
    {
      Result result = LayoutType.PrepareSparseMove(ref b, ref scope, (LayoutScope) this, this.FieldType(ref scope), ref patternScope, UpdateOptions.Update, out value);
      if (result != Result.Success)
        return result;
      b.DeleteSparse(ref patternScope);
      return Result.Success;
    }
  }
}

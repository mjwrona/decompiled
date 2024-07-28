// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutTypedMap
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class LayoutTypedMap : LayoutUniqueScope
  {
    internal LayoutTypedMap(bool immutable)
      : base(immutable ? LayoutCode.ImmutableTypedMapScope : LayoutCode.TypedMapScope, immutable, true, true)
    {
    }

    public override string Name => !this.Immutable ? "map_t" : "im_map_t";

    public override TypeArgument FieldType(ref RowCursor scope) => new TypeArgument(scope.scopeType.Immutable ? (LayoutType) LayoutType.ImmutableTypedTuple : (LayoutType) LayoutType.TypedTuple, scope.scopeTypeArgs);

    public override Result WriteScope(
      ref RowBuffer b,
      ref RowCursor edit,
      TypeArgumentList typeArgs,
      out RowCursor value,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      Result result = LayoutType.PrepareSparseWrite(ref b, ref edit, new TypeArgument((LayoutType) this, typeArgs), options);
      if (result != Result.Success)
      {
        value = new RowCursor();
        return result;
      }
      b.WriteTypedMap(ref edit, (LayoutScope) this, typeArgs, options, out value);
      return Result.Success;
    }

    internal override bool HasImplicitTypeCode(ref RowCursor edit) => true;

    internal override void SetImplicitTypeCode(ref RowCursor edit)
    {
      edit.cellType = edit.scopeType.Immutable ? (LayoutType) LayoutType.ImmutableTypedTuple : (LayoutType) LayoutType.TypedTuple;
      edit.cellTypeArgs = edit.scopeTypeArgs;
    }

    internal override int CountTypeArgument(TypeArgumentList value)
    {
      // ISSUE: unable to decompile the method.
    }

    internal override int WriteTypeArgument(ref RowBuffer row, int offset, TypeArgumentList value)
    {
      // ISSUE: unable to decompile the method.
    }

    internal override TypeArgumentList ReadTypeArgumentList(
      ref RowBuffer row,
      int offset,
      out int lenInBytes)
    {
      lenInBytes = 0;
      TypeArgument[] args = new TypeArgument[2];
      int index = 0;
      while (index < 2)
      {
        int lenInBytes1;
        args[index] = LayoutType.ReadTypeArgument(ref row, checked (offset + lenInBytes), out lenInBytes1);
        checked { lenInBytes += lenInBytes1; }
        checked { ++index; }
      }
      return new TypeArgumentList(args);
    }
  }
}

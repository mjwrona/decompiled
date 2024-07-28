// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutTypedSet
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class LayoutTypedSet : LayoutUniqueScope
  {
    internal LayoutTypedSet(bool immutable)
      : base(immutable ? LayoutCode.ImmutableTypedSetScope : LayoutCode.TypedSetScope, immutable, true, true)
    {
    }

    public override string Name => !this.Immutable ? "set_t" : "im_set_t";

    public override TypeArgument FieldType(ref RowCursor scope) => scope.scopeTypeArgs[0];

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
      b.WriteTypedSet(ref edit, (LayoutScope) this, typeArgs, options, out value);
      return Result.Success;
    }

    internal override bool HasImplicitTypeCode(ref RowCursor edit) => !LayoutCodeTraits.AlwaysRequiresTypeCode(edit.scopeTypeArgs[0].Type.LayoutCode);

    internal override void SetImplicitTypeCode(ref RowCursor edit)
    {
      edit.cellType = edit.scopeTypeArgs[0].Type;
      edit.cellTypeArgs = edit.scopeTypeArgs[0].TypeArgs;
    }

    internal override int CountTypeArgument(TypeArgumentList value)
    {
      TypeArgument typeArgument = value[0];
      LayoutType type = typeArgument.Type;
      typeArgument = value[0];
      TypeArgumentList typeArgs = typeArgument.TypeArgs;
      return checked (1 + type.CountTypeArgument(typeArgs));
    }

    internal override int WriteTypeArgument(ref RowBuffer row, int offset, TypeArgumentList value)
    {
      row.WriteSparseTypeCode(offset, this.LayoutCode);
      int num1 = 1;
      int num2 = num1;
      TypeArgument typeArgument = value[0];
      LayoutType type = typeArgument.Type;
      ref RowBuffer local = ref row;
      int offset1 = checked (offset + num1);
      typeArgument = value[0];
      TypeArgumentList typeArgs = typeArgument.TypeArgs;
      int num3 = type.WriteTypeArgument(ref local, offset1, typeArgs);
      return checked (num2 + num3);
    }

    internal override TypeArgumentList ReadTypeArgumentList(
      ref RowBuffer row,
      int offset,
      out int lenInBytes)
    {
      return new TypeArgumentList(new TypeArgument[1]
      {
        LayoutType.ReadTypeArgument(ref row, offset, out lenInBytes)
      });
    }
  }
}

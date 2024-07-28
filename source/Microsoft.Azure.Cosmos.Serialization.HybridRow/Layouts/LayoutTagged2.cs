// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutTagged2
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class LayoutTagged2 : LayoutIndexedScope
  {
    internal LayoutTagged2(bool immutable)
      : base(immutable ? LayoutCode.ImmutableTagged2Scope : LayoutCode.Tagged2Scope, immutable, true, true, false, true)
    {
    }

    public override string Name => !this.Immutable ? "tagged2_t" : "im_tagged2_t";

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
      b.WriteTypedTuple(ref edit, (LayoutScope) this, typeArgs, options, out value);
      return Result.Success;
    }

    internal override bool HasImplicitTypeCode(ref RowCursor edit) => !LayoutCodeTraits.AlwaysRequiresTypeCode(edit.scopeTypeArgs[edit.index].Type.LayoutCode);

    internal override void SetImplicitTypeCode(ref RowCursor edit)
    {
      edit.cellType = edit.scopeTypeArgs[edit.index].Type;
      edit.cellTypeArgs = edit.scopeTypeArgs[edit.index].TypeArgs;
    }

    internal override int CountTypeArgument(TypeArgumentList value)
    {
      int num = 1;
      int i = 1;
      while (i < value.Count)
      {
        TypeArgument typeArgument = value[i];
        checked { num += typeArgument.Type.CountTypeArgument(typeArgument.TypeArgs); }
        checked { ++i; }
      }
      return num;
    }

    internal override int WriteTypeArgument(ref RowBuffer row, int offset, TypeArgumentList value)
    {
      row.WriteSparseTypeCode(offset, this.LayoutCode);
      int num = 1;
      int i = 1;
      while (i < value.Count)
      {
        TypeArgument typeArgument = value[i];
        checked { num += typeArgument.Type.WriteTypeArgument(ref row, offset + num, typeArgument.TypeArgs); }
        checked { ++i; }
      }
      return num;
    }

    internal override TypeArgumentList ReadTypeArgumentList(
      ref RowBuffer row,
      int offset,
      out int lenInBytes)
    {
      lenInBytes = 0;
      TypeArgument[] args = new TypeArgument[3];
      args[0] = new TypeArgument((LayoutType) LayoutType.UInt8, TypeArgumentList.Empty);
      int index = 1;
      while (index < 3)
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

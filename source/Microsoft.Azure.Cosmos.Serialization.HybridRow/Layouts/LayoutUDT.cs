// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutUDT
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class LayoutUDT : LayoutPropertyScope
  {
    internal LayoutUDT(bool immutable)
      : base(immutable ? LayoutCode.ImmutableSchema : LayoutCode.Schema, immutable)
    {
    }

    public override string Name => !this.Immutable ? "udt" : "im_udt";

    public override Result WriteScope(
      ref RowBuffer b,
      ref RowCursor edit,
      TypeArgumentList typeArgs,
      out RowCursor value,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      Layout udt = b.Resolver.Resolve(typeArgs.SchemaId);
      Result result = LayoutType.PrepareSparseWrite(ref b, ref edit, new TypeArgument((LayoutType) this, typeArgs), options);
      if (result != Result.Success)
      {
        value = new RowCursor();
        return result;
      }
      b.WriteSparseUDT(ref edit, (LayoutScope) this, udt, options, out value);
      return Result.Success;
    }

    internal override int CountTypeArgument(TypeArgumentList value) => 5;

    internal override int WriteTypeArgument(ref RowBuffer row, int offset, TypeArgumentList value)
    {
      row.WriteSparseTypeCode(offset, this.LayoutCode);
      row.WriteSchemaId(checked (offset + 1), value.SchemaId);
      return 5;
    }

    internal override TypeArgumentList ReadTypeArgumentList(
      ref RowBuffer row,
      int offset,
      out int lenInBytes)
    {
      SchemaId schemaId = row.ReadSchemaId(offset);
      lenInBytes = 4;
      return new TypeArgumentList(schemaId);
    }
  }
}

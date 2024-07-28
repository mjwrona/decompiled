// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutTuple
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class LayoutTuple : LayoutIndexedScope
  {
    internal LayoutTuple(bool immutable)
      : base(immutable ? LayoutCode.ImmutableTupleScope : LayoutCode.TupleScope, immutable, false, true, false, false)
    {
    }

    public override string Name => !this.Immutable ? "tuple" : "im_tuple";

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
      b.WriteSparseTuple(ref edit, (LayoutScope) this, typeArgs, options, out value);
      return Result.Success;
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
      int length = checked ((int) row.Read7BitEncodedUInt(offset, out lenInBytes));
      TypeArgument[] args = new TypeArgument[length];
      int index = 0;
      while (index < length)
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

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutArray
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class LayoutArray : LayoutIndexedScope
  {
    internal LayoutArray(bool immutable)
      : base(immutable ? LayoutCode.ImmutableArrayScope : LayoutCode.ArrayScope, immutable, false, false, false, false)
    {
      this.TypeArg = new TypeArgument((LayoutType) this);
    }

    public override string Name => !this.Immutable ? "array" : "im_array";

    internal TypeArgument TypeArg { get; }

    public override Result WriteScope(
      ref RowBuffer b,
      ref RowCursor edit,
      TypeArgumentList typeArgs,
      out RowCursor value,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      Result result = LayoutType.PrepareSparseWrite(ref b, ref edit, this.TypeArg, options);
      if (result != Result.Success)
      {
        value = new RowCursor();
        return result;
      }
      b.WriteSparseArray(ref edit, (LayoutScope) this, options, out value);
      return Result.Success;
    }
  }
}

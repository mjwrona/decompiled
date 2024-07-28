// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutEndScope
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class LayoutEndScope : LayoutScope
  {
    public LayoutEndScope()
      : base(LayoutCode.EndScope, false, false, false, false, false, false)
    {
    }

    public override string Name => "end";

    public override Result WriteScope(
      ref RowBuffer b,
      ref RowCursor scope,
      TypeArgumentList typeArgs,
      out RowCursor value,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      Contract.Fail("Cannot write an EndScope directly");
      value = new RowCursor();
      return Result.Failure;
    }
  }
}

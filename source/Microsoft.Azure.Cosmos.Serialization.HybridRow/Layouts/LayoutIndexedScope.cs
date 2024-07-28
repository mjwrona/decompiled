// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutIndexedScope
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public abstract class LayoutIndexedScope : LayoutScope
  {
    protected LayoutIndexedScope(
      LayoutCode code,
      bool immutable,
      bool isSizedScope,
      bool isFixedArity,
      bool isUniqueScope,
      bool isTypedScope)
      : base(code, immutable, isSizedScope, true, isFixedArity, isUniqueScope, isTypedScope)
    {
    }

    internal override void ReadSparsePath(ref RowBuffer row, ref RowCursor edit)
    {
      edit.pathToken = 0;
      edit.pathOffset = 0;
    }
  }
}

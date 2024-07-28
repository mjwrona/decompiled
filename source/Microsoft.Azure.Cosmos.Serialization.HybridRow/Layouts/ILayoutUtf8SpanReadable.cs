// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.ILayoutUtf8SpanReadable
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core.Utf8;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public interface ILayoutUtf8SpanReadable : ILayoutType
  {
    Result ReadFixed(ref RowBuffer b, ref RowCursor scope, LayoutColumn col, out Utf8Span value);

    Result ReadVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out Utf8Span value);

    Result ReadSparse(ref RowBuffer b, ref RowCursor scope, out Utf8Span value);
  }
}

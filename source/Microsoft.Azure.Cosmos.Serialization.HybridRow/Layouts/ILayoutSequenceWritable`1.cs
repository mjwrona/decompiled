// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.ILayoutSequenceWritable`1
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using System.Buffers;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public interface ILayoutSequenceWritable<TElement> : ILayoutType
  {
    Result WriteFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      ReadOnlySequence<TElement> value);

    Result WriteVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      ReadOnlySequence<TElement> value);

    Result WriteSparse(
      ref RowBuffer b,
      ref RowCursor edit,
      ReadOnlySequence<TElement> value,
      UpdateOptions options = UpdateOptions.Upsert);
  }
}

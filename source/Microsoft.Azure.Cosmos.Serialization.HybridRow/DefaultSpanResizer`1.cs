// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.DefaultSpanResizer`1
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow
{
  public class DefaultSpanResizer<T> : ISpanResizer<T>
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly DefaultSpanResizer<T> Default = new DefaultSpanResizer<T>();

    private DefaultSpanResizer()
    {
    }

    public Span<T> Resize(int minimumLength, Span<T> buffer = default (Span<T>))
    {
      Span<T> span = new Memory<T>(new T[Math.Max(minimumLength, buffer.Length)]).Span;
      if (!buffer.IsEmpty && span.Slice(0, buffer.Length) != buffer)
        buffer.CopyTo(span);
      return span;
    }
  }
}

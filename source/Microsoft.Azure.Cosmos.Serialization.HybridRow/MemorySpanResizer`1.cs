// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.MemorySpanResizer`1
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using System;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow
{
  public sealed class MemorySpanResizer<T> : ISpanResizer<T>
  {
    private System.Memory<T> memory;

    public MemorySpanResizer(int initialCapacity = 0)
    {
      Contract.Requires(initialCapacity >= 0);
      this.memory = initialCapacity == 0 ? new System.Memory<T>() : new System.Memory<T>(new T[initialCapacity]);
    }

    public System.Memory<T> Memory => this.memory;

    public Span<T> Resize(int minimumLength, Span<T> buffer = default (Span<T>))
    {
      if (this.memory.Length < minimumLength)
        this.memory = new System.Memory<T>(new T[Math.Max(minimumLength, buffer.Length)]);
      Span<T> span = this.memory.Span;
      if (!buffer.IsEmpty && span.Slice(0, buffer.Length) != buffer)
        buffer.CopyTo(span);
      return span;
    }
  }
}

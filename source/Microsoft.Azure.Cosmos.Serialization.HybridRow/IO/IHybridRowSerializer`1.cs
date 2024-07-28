// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.IO.IHybridRowSerializer`1
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.IO
{
  public interface IHybridRowSerializer<T>
  {
    IEqualityComparer<T> Comparer { get; }

    Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      T value);

    Result Read(ref RowBuffer row, ref RowCursor scope, bool isRoot, out T value);
  }
}

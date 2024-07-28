// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.DateTimeHybridRowSerializer
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct DateTimeHybridRowSerializer : IHybridRowSerializer<DateTime>
  {
    public IEqualityComparer<DateTime> Comparer => (IEqualityComparer<DateTime>) EqualityComparer<DateTime>.Default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      DateTime value)
    {
      return LayoutType.DateTime.WriteSparse(ref row, ref scope, value, UpdateOptions.Upsert);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Read(ref RowBuffer row, ref RowCursor scope, bool isRoot, out DateTime value) => LayoutType.DateTime.ReadSparse(ref row, ref scope, out value);
  }
}

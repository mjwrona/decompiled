// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.PrimarySortKeyHybridRowSerializer
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public readonly struct PrimarySortKeyHybridRowSerializer : IHybridRowSerializer<PrimarySortKey>
  {
    public const int SchemaId = 2147473655;
    public const int Size = 2;
    private static readonly Utf8String PathName = Utf8String.TranscodeUtf16("path");
    private static readonly Utf8String DirectionName = Utf8String.TranscodeUtf16("direction");
    private static readonly LayoutColumn PathColumn;
    private static readonly LayoutColumn DirectionColumn;

    public IEqualityComparer<PrimarySortKey> Comparer => (IEqualityComparer<PrimarySortKey>) PrimarySortKeyHybridRowSerializer.PrimarySortKeyComparer.Default;

    static PrimarySortKeyHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473655));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PrimarySortKeyHybridRowSerializer.PathName), out PrimarySortKeyHybridRowSerializer.PathColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PrimarySortKeyHybridRowSerializer.DirectionName), out PrimarySortKeyHybridRowSerializer.DirectionColumn));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      PrimarySortKey value)
    {
      if (isRoot)
        return PrimarySortKeyHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473655), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = PrimarySortKeyHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, PrimarySortKey value)
    {
      if (value.Direction != SortDirection.Ascending)
      {
        Result result = LayoutType.UInt8.WriteFixed(ref row, ref scope, PrimarySortKeyHybridRowSerializer.DirectionColumn, (byte) value.Direction);
        if (result != Result.Success)
          return result;
      }
      if (value.Path != null)
      {
        Result result = LayoutType.Utf8.WriteVariable(ref row, ref scope, PrimarySortKeyHybridRowSerializer.PathColumn, value.Path);
        if (result != Result.Success)
          return result;
      }
      return Result.Success;
    }

    public Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      out PrimarySortKey value)
    {
      if (isRoot)
      {
        value = new PrimarySortKey();
        return PrimarySortKeyHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (PrimarySortKey) null;
        return result1;
      }
      value = new PrimarySortKey();
      Result result2 = PrimarySortKeyHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (PrimarySortKey) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref PrimarySortKey value)
    {
      byte num;
      Result result1 = LayoutType.UInt8.ReadFixed(ref row, ref scope, PrimarySortKeyHybridRowSerializer.DirectionColumn, out num);
      switch (result1)
      {
        case Result.Success:
          value.Direction = (SortDirection) num;
          goto case Result.NotFound;
        case Result.NotFound:
          string str;
          Result result2 = LayoutType.Utf8.ReadVariable(ref row, ref scope, PrimarySortKeyHybridRowSerializer.PathColumn, out str);
          switch (result2)
          {
            case Result.Success:
              value.Path = str;
              goto case Result.NotFound;
            case Result.NotFound:
              return Result.Success;
            default:
              return result2;
          }
        default:
          return result1;
      }
    }

    public sealed class PrimarySortKeyComparer : EqualityComparer<PrimarySortKey>
    {
      public static readonly PrimarySortKeyHybridRowSerializer.PrimarySortKeyComparer Default = new PrimarySortKeyHybridRowSerializer.PrimarySortKeyComparer();

      public override bool Equals(PrimarySortKey x, PrimarySortKey y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<PrimarySortKey>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        return new Utf8HybridRowSerializer().Comparer.Equals(x.Path, y.Path) && new UInt8HybridRowSerializer().Comparer.Equals((byte) x.Direction, (byte) y.Direction);
      }

      public override int GetHashCode(PrimarySortKey obj) => HashCode.Combine<int, int>(new Utf8HybridRowSerializer().Comparer.GetHashCode(obj.Path), new UInt8HybridRowSerializer().Comparer.GetHashCode((byte) obj.Direction));
    }
  }
}

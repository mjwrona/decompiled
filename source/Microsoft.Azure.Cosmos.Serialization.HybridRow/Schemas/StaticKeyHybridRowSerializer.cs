// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.StaticKeyHybridRowSerializer
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public readonly struct StaticKeyHybridRowSerializer : IHybridRowSerializer<StaticKey>
  {
    public const int SchemaId = 2147473656;
    public const int Size = 1;
    private static readonly Utf8String PathName = Utf8String.TranscodeUtf16("path");
    private static readonly LayoutColumn PathColumn;

    public IEqualityComparer<StaticKey> Comparer => (IEqualityComparer<StaticKey>) StaticKeyHybridRowSerializer.StaticKeyComparer.Default;

    static StaticKeyHybridRowSerializer() => Contract.Invariant(SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473656)).TryFind(Utf8String.op_Implicit(StaticKeyHybridRowSerializer.PathName), out StaticKeyHybridRowSerializer.PathColumn));

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      StaticKey value)
    {
      if (isRoot)
        return StaticKeyHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473656), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = StaticKeyHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, StaticKey value)
    {
      if (value.Path != null)
      {
        Result result = LayoutType.Utf8.WriteVariable(ref row, ref scope, StaticKeyHybridRowSerializer.PathColumn, value.Path);
        if (result != Result.Success)
          return result;
      }
      return Result.Success;
    }

    public Result Read(ref RowBuffer row, ref RowCursor scope, bool isRoot, out StaticKey value)
    {
      if (isRoot)
      {
        value = new StaticKey();
        return StaticKeyHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (StaticKey) null;
        return result1;
      }
      value = new StaticKey();
      Result result2 = StaticKeyHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (StaticKey) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref StaticKey value)
    {
      string str;
      Result result = LayoutType.Utf8.ReadVariable(ref row, ref scope, StaticKeyHybridRowSerializer.PathColumn, out str);
      switch (result)
      {
        case Result.Success:
          value.Path = str;
          goto case Result.NotFound;
        case Result.NotFound:
          return Result.Success;
        default:
          return result;
      }
    }

    public sealed class StaticKeyComparer : EqualityComparer<StaticKey>
    {
      public static readonly StaticKeyHybridRowSerializer.StaticKeyComparer Default = new StaticKeyHybridRowSerializer.StaticKeyComparer();

      public override bool Equals(StaticKey x, StaticKey y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<StaticKey>(x, y);
        return equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown ? equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal : new Utf8HybridRowSerializer().Comparer.Equals(x.Path, y.Path);
      }

      public override int GetHashCode(StaticKey obj) => new Utf8HybridRowSerializer().Comparer.GetHashCode(obj.Path);
    }
  }
}

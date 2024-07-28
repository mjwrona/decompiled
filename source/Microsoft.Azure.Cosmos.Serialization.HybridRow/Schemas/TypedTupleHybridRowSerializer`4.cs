// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.TypedTupleHybridRowSerializer`4
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct TypedTupleHybridRowSerializer<T1, T1Serializer, T2, T2Serializer> : 
    IHybridRowSerializer<(T1 Item1, T2 Item2)>
    where T1Serializer : struct, IHybridRowSerializer<T1>
    where T2Serializer : struct, IHybridRowSerializer<T2>
  {
    public IEqualityComparer<(T1 Item1, T2 Item2)> Comparer => (IEqualityComparer<(T1, T2)>) TypedTupleHybridRowSerializer<T1, T1Serializer, T2, T2Serializer>.TypedTupleComparer.Default;

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      (T1 Item1, T2 Item2) value)
    {
      RowCursor rowCursor;
      Result result1 = LayoutType.TypedTuple.WriteScope(ref row, ref scope, typeArgs, out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = default (T1Serializer).Write(ref row, ref rowCursor, false, typeArgs[0].TypeArgs, value.Item1);
      if (result2 != Result.Success)
        return result2;
      rowCursor.MoveNext(ref row);
      Result result3 = default (T2Serializer).Write(ref row, ref rowCursor, false, typeArgs[1].TypeArgs, value.Item2);
      if (result3 != Result.Success)
        return result3;
      rowCursor.MoveNext(ref row);
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    public Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      out (T1 Item1, T2 Item2) value)
    {
      RowCursor rowCursor;
      Result result1 = LayoutType.TypedTuple.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = ();
        return result1;
      }
      T1 obj1 = default (T1);
      T2 obj2 = default (T2);
      if (rowCursor.MoveNext(ref row))
      {
        Result result2 = default (T1Serializer).Read(ref row, ref rowCursor, isRoot, out obj1);
        if (result2 != Result.Success)
        {
          value = ();
          return result2;
        }
        if (rowCursor.MoveNext(ref row))
        {
          Result result3 = default (T2Serializer).Read(ref row, ref rowCursor, isRoot, out obj2);
          if (result3 != Result.Success)
          {
            value = ();
            return result3;
          }
        }
      }
      value = (obj1, obj2);
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    public sealed class TypedTupleComparer : EqualityComparer<(T1 Item1, T2 Item2)>
    {
      public static readonly TypedTupleHybridRowSerializer<T1, T1Serializer, T2, T2Serializer>.TypedTupleComparer Default = new TypedTupleHybridRowSerializer<T1, T1Serializer, T2, T2Serializer>.TypedTupleComparer();

      public override bool Equals((T1 Item1, T2 Item2) x, (T1 Item1, T2 Item2) y) => default (T1Serializer).Comparer.Equals(x.Item1, y.Item1) && default (T2Serializer).Comparer.Equals(x.Item2, y.Item2);

      public override int GetHashCode((T1 Item1, T2 Item2) obj)
      {
        HashCode hashCode = new HashCode();
        ((HashCode) ref hashCode).Add<T1>(obj.Item1, default (T1Serializer).Comparer);
        ((HashCode) ref hashCode).Add<T2>(obj.Item2, default (T2Serializer).Comparer);
        return ((HashCode) ref hashCode).ToHashCode();
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.TypedTupleHybridRowSerializer`8
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
  public struct TypedTupleHybridRowSerializer<T1, T1Serializer, T2, T2Serializer, T3, T3Serializer, T4, T4Serializer> : 
    IHybridRowSerializer<(T1 Item1, T2 Item2, T3 Item3, T4 Item4)>
    where T1Serializer : struct, IHybridRowSerializer<T1>
    where T2Serializer : struct, IHybridRowSerializer<T2>
    where T3Serializer : struct, IHybridRowSerializer<T3>
    where T4Serializer : struct, IHybridRowSerializer<T4>
  {
    public IEqualityComparer<(T1 Item1, T2 Item2, T3 Item3, T4 Item4)> Comparer => (IEqualityComparer<(T1, T2, T3, T4)>) TypedTupleHybridRowSerializer<T1, T1Serializer, T2, T2Serializer, T3, T3Serializer, T4, T4Serializer>.TypedTupleComparer.Default;

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      (T1 Item1, T2 Item2, T3 Item3, T4 Item4) value)
    {
      RowCursor rowCursor;
      Result result1 = LayoutType.TypedTuple.WriteScope(ref row, ref scope, typeArgs, out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = default (T1Serializer).Write(ref row, ref rowCursor, false, typeArgs[0].TypeArgs, value.Item1);
      if (result2 != Result.Success)
        return result2;
      rowCursor.MoveNext(ref row);
      T2Serializer obj1 = default (T2Serializer);
      ref T2Serializer local1 = ref obj1;
      ref RowBuffer local2 = ref row;
      ref RowCursor local3 = ref rowCursor;
      TypeArgument typeArg = typeArgs[1];
      TypeArgumentList typeArgs1 = typeArg.TypeArgs;
      T2 obj2 = value.Item2;
      Result result3 = local1.Write(ref local2, ref local3, false, typeArgs1, obj2);
      if (result3 != Result.Success)
        return result3;
      rowCursor.MoveNext(ref row);
      T3Serializer obj3 = default (T3Serializer);
      ref T3Serializer local4 = ref obj3;
      ref RowBuffer local5 = ref row;
      ref RowCursor local6 = ref rowCursor;
      typeArg = typeArgs[2];
      TypeArgumentList typeArgs2 = typeArg.TypeArgs;
      T3 obj4 = value.Item3;
      Result result4 = local4.Write(ref local5, ref local6, false, typeArgs2, obj4);
      if (result4 != Result.Success)
        return result4;
      rowCursor.MoveNext(ref row);
      T4Serializer obj5 = default (T4Serializer);
      ref T4Serializer local7 = ref obj5;
      ref RowBuffer local8 = ref row;
      ref RowCursor local9 = ref rowCursor;
      typeArg = typeArgs[3];
      TypeArgumentList typeArgs3 = typeArg.TypeArgs;
      T4 obj6 = value.Item4;
      Result result5 = local7.Write(ref local8, ref local9, false, typeArgs3, obj6);
      if (result5 != Result.Success)
        return result5;
      rowCursor.MoveNext(ref row);
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    public Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      out (T1 Item1, T2 Item2, T3 Item3, T4 Item4) value)
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
      T3 obj3 = default (T3);
      T4 obj4 = default (T4);
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
          if (rowCursor.MoveNext(ref row))
          {
            Result result4 = default (T3Serializer).Read(ref row, ref rowCursor, isRoot, out obj3);
            if (result4 != Result.Success)
            {
              value = ();
              return result4;
            }
            if (rowCursor.MoveNext(ref row))
            {
              Result result5 = default (T4Serializer).Read(ref row, ref rowCursor, isRoot, out obj4);
              if (result5 != Result.Success)
              {
                value = ();
                return result5;
              }
            }
          }
        }
      }
      value = (obj1, obj2, obj3, obj4);
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    public sealed class TypedTupleComparer : 
      EqualityComparer<(T1 Item1, T2 Item2, T3 Item3, T4 Item4)>
    {
      public static readonly TypedTupleHybridRowSerializer<T1, T1Serializer, T2, T2Serializer, T3, T3Serializer, T4, T4Serializer>.TypedTupleComparer Default = new TypedTupleHybridRowSerializer<T1, T1Serializer, T2, T2Serializer, T3, T3Serializer, T4, T4Serializer>.TypedTupleComparer();

      public override bool Equals(
        (T1 Item1, T2 Item2, T3 Item3, T4 Item4) x,
        (T1 Item1, T2 Item2, T3 Item3, T4 Item4) y)
      {
        return default (T1Serializer).Comparer.Equals(x.Item1, y.Item1) && default (T2Serializer).Comparer.Equals(x.Item2, y.Item2) && default (T3Serializer).Comparer.Equals(x.Item3, y.Item3) && default (T4Serializer).Comparer.Equals(x.Item4, y.Item4);
      }

      public override int GetHashCode((T1 Item1, T2 Item2, T3 Item3, T4 Item4) obj)
      {
        HashCode hashCode = new HashCode();
        ((HashCode) ref hashCode).Add<T1>(obj.Item1, default (T1Serializer).Comparer);
        ((HashCode) ref hashCode).Add<T2>(obj.Item2, default (T2Serializer).Comparer);
        ((HashCode) ref hashCode).Add<T3>(obj.Item3, default (T3Serializer).Comparer);
        ((HashCode) ref hashCode).Add<T4>(obj.Item4, default (T4Serializer).Comparer);
        return ((HashCode) ref hashCode).ToHashCode();
      }
    }
  }
}

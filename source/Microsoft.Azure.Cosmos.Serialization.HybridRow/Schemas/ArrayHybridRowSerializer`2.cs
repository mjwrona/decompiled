// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.ArrayHybridRowSerializer`2
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
  public struct ArrayHybridRowSerializer<T, TSerializer> : IHybridRowSerializer<List<T>> where TSerializer : struct, IHybridRowSerializer<T>
  {
    public IEqualityComparer<List<T>> Comparer => (IEqualityComparer<List<T>>) ArrayHybridRowSerializer<T, TSerializer>.ArrayComparer.Default;

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      List<T> value)
    {
      RowCursor rowCursor;
      Result result1 = LayoutType.Array.WriteScope(ref row, ref scope, new TypeArgumentList(), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      foreach (T obj in value)
      {
        Result result2 = default (TSerializer).Write(ref row, ref rowCursor, false, typeArgs[0].TypeArgs, obj);
        if (result2 != Result.Success)
          return result2;
        rowCursor.MoveNext(ref row);
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    public Result Read(ref RowBuffer row, ref RowCursor scope, bool isRoot, out List<T> value)
    {
      RowCursor rowCursor;
      Result result1 = LayoutType.Array.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (List<T>) null;
        return result1;
      }
      List<T> objList = new List<T>();
      while (rowCursor.MoveNext(ref row))
      {
        T obj;
        Result result2 = default (TSerializer).Read(ref row, ref rowCursor, isRoot, out obj);
        if (result2 != Result.Success)
        {
          value = (List<T>) null;
          return result2;
        }
        objList.Add(obj);
      }
      value = objList;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    public sealed class ArrayComparer : EqualityComparer<List<T>>
    {
      public static readonly ArrayHybridRowSerializer<T, TSerializer>.ArrayComparer Default = new ArrayHybridRowSerializer<T, TSerializer>.ArrayComparer();

      public override bool Equals(List<T> x, List<T> y)
      {
        if (x == y)
          return true;
        if (x == null || y == null || x.Count != y.Count)
          return false;
        int index = 0;
        while (index < x.Count)
        {
          if (!default (TSerializer).Comparer.Equals(x[index], y[index]))
            return false;
          checked { ++index; }
        }
        return true;
      }

      public override int GetHashCode(List<T> obj)
      {
        HashCode hashCode = new HashCode();
        IEqualityComparer<T> comparer = default (TSerializer).Comparer;
        foreach (T obj1 in obj)
          ((HashCode) ref hashCode).Add<T>(obj1, comparer);
        return ((HashCode) ref hashCode).ToHashCode();
      }
    }
  }
}

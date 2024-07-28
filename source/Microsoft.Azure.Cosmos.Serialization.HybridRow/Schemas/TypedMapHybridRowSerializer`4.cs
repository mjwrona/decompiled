// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.TypedMapHybridRowSerializer`4
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
  public struct TypedMapHybridRowSerializer<TKey, TKeySerializer, TValue, TValueSerializer> : 
    IHybridRowSerializer<Dictionary<TKey, TValue>>
    where TKeySerializer : struct, IHybridRowSerializer<TKey>
    where TValueSerializer : struct, IHybridRowSerializer<TValue>
  {
    public IEqualityComparer<Dictionary<TKey, TValue>> Comparer => (IEqualityComparer<Dictionary<TKey, TValue>>) TypedMapHybridRowSerializer<TKey, TKeySerializer, TValue, TValueSerializer>.TypedMapComparer.Default;

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      Dictionary<TKey, TValue> value)
    {
      RowCursor src;
      Result result1 = LayoutType.TypedMap.WriteScope(ref row, ref scope, typeArgs, out src, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      RowCursor dest;
      src.Clone(out dest);
      dest.deferUniqueIndex = true;
      foreach (KeyValuePair<TKey, TValue> keyValuePair in value)
      {
        Result result2 = new TypedTupleHybridRowSerializer<TKey, TKeySerializer, TValue, TValueSerializer>().Write(ref row, ref dest, false, typeArgs, (keyValuePair.Key, keyValuePair.Value));
        if (result2 != Result.Success)
          return result2;
        dest.MoveNext(ref row);
      }
      src.count = dest.count;
      Result result3 = row.TypedCollectionUniqueIndexRebuild(ref src);
      if (result3 != Result.Success)
        return result3;
      scope.Skip(ref row, ref dest);
      return Result.Success;
    }

    public Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      out Dictionary<TKey, TValue> value)
    {
      RowCursor rowCursor;
      Result result1 = LayoutType.TypedMap.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (Dictionary<TKey, TValue>) null;
        return result1;
      }
      Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(default (TKeySerializer).Comparer);
      while (rowCursor.MoveNext(ref row))
      {
        (TKey Item1, TValue Item2) tuple;
        Result result2 = new TypedTupleHybridRowSerializer<TKey, TKeySerializer, TValue, TValueSerializer>().Read(ref row, ref rowCursor, isRoot, out tuple);
        if (result2 != Result.Success)
        {
          value = (Dictionary<TKey, TValue>) null;
          return result2;
        }
        dictionary.Add(tuple.Item1, tuple.Item2);
      }
      value = dictionary;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    public sealed class TypedMapComparer : EqualityComparer<Dictionary<TKey, TValue>>
    {
      public static readonly TypedMapHybridRowSerializer<TKey, TKeySerializer, TValue, TValueSerializer>.TypedMapComparer Default = new TypedMapHybridRowSerializer<TKey, TKeySerializer, TValue, TValueSerializer>.TypedMapComparer();

      public override bool Equals(Dictionary<TKey, TValue> x, Dictionary<TKey, TValue> y)
      {
        if (x == y)
          return true;
        if (x == null || y == null || x.Count != y.Count)
          return false;
        foreach (KeyValuePair<TKey, TValue> keyValuePair in x)
        {
          TValue y1;
          if (!y.TryGetValue(keyValuePair.Key, out y1) || !default (TValueSerializer).Comparer.Equals(keyValuePair.Value, y1))
            return false;
        }
        return true;
      }

      public override int GetHashCode(Dictionary<TKey, TValue> obj)
      {
        HashCode hashCode = new HashCode();
        IEqualityComparer<TKey> comparer1 = default (TKeySerializer).Comparer;
        IEqualityComparer<TValue> comparer2 = default (TValueSerializer).Comparer;
        foreach (KeyValuePair<TKey, TValue> keyValuePair in obj)
        {
          ((HashCode) ref hashCode).Add<TKey>(keyValuePair.Key, comparer1);
          ((HashCode) ref hashCode).Add<TValue>(keyValuePair.Value, comparer2);
        }
        return ((HashCode) ref hashCode).ToHashCode();
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.NullableHybridRowSerializer`3
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
  public struct NullableHybridRowSerializer<TNullable, T, TSerializer> : 
    IHybridRowSerializer<TNullable>
    where TSerializer : struct, IHybridRowSerializer<T>
  {
    public IEqualityComparer<TNullable> Comparer => (IEqualityComparer<TNullable>) NullableHybridRowSerializer<TNullable, T, TSerializer>.NullableComparer.Default;

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      TNullable value)
    {
      bool hasValue = NullableHybridRowSerializer<TNullable, T, TSerializer>.HasValue(value);
      RowCursor rowCursor;
      Result result1 = LayoutType.Nullable.WriteScope(ref row, ref scope, typeArgs, hasValue, out rowCursor);
      if (result1 != Result.Success)
        return result1;
      if (hasValue)
      {
        Result result2 = default (TSerializer).Write(ref row, ref rowCursor, false, typeArgs[0].TypeArgs, NullableHybridRowSerializer<TNullable, T, TSerializer>.AsValue(value));
        if (result2 != Result.Success)
          return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    public Result Read(ref RowBuffer row, ref RowCursor scope, bool isRoot, out TNullable value)
    {
      RowCursor rowCursor;
      Result result1 = LayoutType.Nullable.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = default (TNullable);
        return result1;
      }
      if (rowCursor.MoveNext(ref row))
      {
        T obj;
        Result result2 = default (TSerializer).Read(ref row, ref rowCursor, isRoot, out obj);
        if (result2 != Result.Success)
        {
          value = default (TNullable);
          return result2;
        }
        value = NullableHybridRowSerializer<TNullable, T, TSerializer>.AsNullable(obj);
      }
      else
        value = default (TNullable);
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasValue(TNullable value) => (object) value != null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static TNullable AsNullable(T value) => (TNullable) (object) value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T AsValue(TNullable value) => (T) (object) value;

    public sealed class NullableComparer : EqualityComparer<TNullable>
    {
      public static readonly NullableHybridRowSerializer<TNullable, T, TSerializer>.NullableComparer Default = new NullableHybridRowSerializer<TNullable, T, TSerializer>.NullableComparer();

      public override bool Equals(TNullable x, TNullable y)
      {
        bool flag = NullableHybridRowSerializer<TNullable, T, TSerializer>.HasValue(x);
        if (flag != NullableHybridRowSerializer<TNullable, T, TSerializer>.HasValue(y))
          return false;
        return !flag || default (TSerializer).Comparer.Equals(NullableHybridRowSerializer<TNullable, T, TSerializer>.AsValue(x), NullableHybridRowSerializer<TNullable, T, TSerializer>.AsValue(y));
      }

      public override int GetHashCode(TNullable obj)
      {
        HashCode hashCode = new HashCode();
        if (NullableHybridRowSerializer<TNullable, T, TSerializer>.HasValue(obj))
          ((HashCode) ref hashCode).Add<T>(NullableHybridRowSerializer<TNullable, T, TSerializer>.AsValue(obj), default (TSerializer).Comparer);
        return ((HashCode) ref hashCode).ToHashCode();
      }
    }
  }
}

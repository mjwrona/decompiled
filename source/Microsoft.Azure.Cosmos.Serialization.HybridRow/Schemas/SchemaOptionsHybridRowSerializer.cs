// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.SchemaOptionsHybridRowSerializer
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
  public readonly struct SchemaOptionsHybridRowSerializer : IHybridRowSerializer<SchemaOptions>
  {
    public const int SchemaId = 2147473653;
    public const int Size = 0;
    private static readonly Utf8String DisallowUnschematizedName = Utf8String.TranscodeUtf16("disallowUnschematized");
    private static readonly Utf8String EnablePropertyLevelTimestampName = Utf8String.TranscodeUtf16("enablePropertyLevelTimestamp");
    private static readonly Utf8String DisableSystemPrefixName = Utf8String.TranscodeUtf16("disableSystemPrefix");
    private static readonly Utf8String AbstractName = Utf8String.TranscodeUtf16("abstract");
    private static readonly LayoutColumn DisallowUnschematizedColumn;
    private static readonly LayoutColumn EnablePropertyLevelTimestampColumn;
    private static readonly LayoutColumn DisableSystemPrefixColumn;
    private static readonly LayoutColumn AbstractColumn;
    private static readonly StringToken DisallowUnschematizedToken;
    private static readonly StringToken EnablePropertyLevelTimestampToken;
    private static readonly StringToken DisableSystemPrefixToken;
    private static readonly StringToken AbstractToken;

    public IEqualityComparer<SchemaOptions> Comparer => (IEqualityComparer<SchemaOptions>) SchemaOptionsHybridRowSerializer.SchemaOptionsComparer.Default;

    static SchemaOptionsHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473653));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaOptionsHybridRowSerializer.DisallowUnschematizedName), out SchemaOptionsHybridRowSerializer.DisallowUnschematizedColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaOptionsHybridRowSerializer.EnablePropertyLevelTimestampName), out SchemaOptionsHybridRowSerializer.EnablePropertyLevelTimestampColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaOptionsHybridRowSerializer.DisableSystemPrefixName), out SchemaOptionsHybridRowSerializer.DisableSystemPrefixColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaOptionsHybridRowSerializer.AbstractName), out SchemaOptionsHybridRowSerializer.AbstractColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SchemaOptionsHybridRowSerializer.DisallowUnschematizedColumn.Path), out SchemaOptionsHybridRowSerializer.DisallowUnschematizedToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SchemaOptionsHybridRowSerializer.EnablePropertyLevelTimestampColumn.Path), out SchemaOptionsHybridRowSerializer.EnablePropertyLevelTimestampToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SchemaOptionsHybridRowSerializer.DisableSystemPrefixColumn.Path), out SchemaOptionsHybridRowSerializer.DisableSystemPrefixToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SchemaOptionsHybridRowSerializer.AbstractColumn.Path), out SchemaOptionsHybridRowSerializer.AbstractToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      SchemaOptions value)
    {
      if (isRoot)
        return SchemaOptionsHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473653), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = SchemaOptionsHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, SchemaOptions value)
    {
      if (value.DisallowUnschematized)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SchemaOptionsHybridRowSerializer.DisallowUnschematizedColumn.Path));
        Result result = LayoutType.Boolean.WriteSparse(ref row, ref scope, value.DisallowUnschematized, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      if (value.EnablePropertyLevelTimestamp)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SchemaOptionsHybridRowSerializer.EnablePropertyLevelTimestampColumn.Path));
        Result result = LayoutType.Boolean.WriteSparse(ref row, ref scope, value.EnablePropertyLevelTimestamp, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      if (value.DisableSystemPrefix)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SchemaOptionsHybridRowSerializer.DisableSystemPrefixColumn.Path));
        Result result = LayoutType.Boolean.WriteSparse(ref row, ref scope, value.DisableSystemPrefix, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      if (value.Abstract)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SchemaOptionsHybridRowSerializer.AbstractColumn.Path));
        Result result = LayoutType.Boolean.WriteSparse(ref row, ref scope, value.Abstract, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      return Result.Success;
    }

    public Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      out SchemaOptions value)
    {
      if (isRoot)
      {
        value = new SchemaOptions();
        return SchemaOptionsHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (SchemaOptions) null;
        return result1;
      }
      value = new SchemaOptions();
      Result result2 = SchemaOptionsHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (SchemaOptions) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref SchemaOptions value)
    {
      while (scope.MoveNext(ref row))
      {
        if ((long) scope.Token == (long) SchemaOptionsHybridRowSerializer.DisallowUnschematizedToken.Id)
        {
          bool flag;
          Result result = LayoutType.Boolean.ReadSparse(ref row, ref scope, out flag);
          if (result != Result.Success)
            return result;
          value.DisallowUnschematized = flag;
        }
        else if ((long) scope.Token == (long) SchemaOptionsHybridRowSerializer.EnablePropertyLevelTimestampToken.Id)
        {
          bool flag;
          Result result = LayoutType.Boolean.ReadSparse(ref row, ref scope, out flag);
          if (result != Result.Success)
            return result;
          value.EnablePropertyLevelTimestamp = flag;
        }
        else if ((long) scope.Token == (long) SchemaOptionsHybridRowSerializer.DisableSystemPrefixToken.Id)
        {
          bool flag;
          Result result = LayoutType.Boolean.ReadSparse(ref row, ref scope, out flag);
          if (result != Result.Success)
            return result;
          value.DisableSystemPrefix = flag;
        }
        else if ((long) scope.Token == (long) SchemaOptionsHybridRowSerializer.AbstractToken.Id)
        {
          bool flag;
          Result result = LayoutType.Boolean.ReadSparse(ref row, ref scope, out flag);
          if (result != Result.Success)
            return result;
          value.Abstract = flag;
        }
      }
      return Result.Success;
    }

    public sealed class SchemaOptionsComparer : EqualityComparer<SchemaOptions>
    {
      public static readonly SchemaOptionsHybridRowSerializer.SchemaOptionsComparer Default = new SchemaOptionsHybridRowSerializer.SchemaOptionsComparer();

      public override bool Equals(SchemaOptions x, SchemaOptions y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<SchemaOptions>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        BooleanHybridRowSerializer hybridRowSerializer = new BooleanHybridRowSerializer();
        if (hybridRowSerializer.Comparer.Equals(x.DisallowUnschematized, y.DisallowUnschematized))
        {
          hybridRowSerializer = new BooleanHybridRowSerializer();
          if (hybridRowSerializer.Comparer.Equals(x.EnablePropertyLevelTimestamp, y.EnablePropertyLevelTimestamp))
          {
            hybridRowSerializer = new BooleanHybridRowSerializer();
            if (hybridRowSerializer.Comparer.Equals(x.DisableSystemPrefix, y.DisableSystemPrefix))
            {
              hybridRowSerializer = new BooleanHybridRowSerializer();
              return hybridRowSerializer.Comparer.Equals(x.Abstract, y.Abstract);
            }
          }
        }
        return false;
      }

      public override int GetHashCode(SchemaOptions obj)
      {
        BooleanHybridRowSerializer hybridRowSerializer = new BooleanHybridRowSerializer();
        int hashCode1 = hybridRowSerializer.Comparer.GetHashCode(obj.DisallowUnschematized);
        hybridRowSerializer = new BooleanHybridRowSerializer();
        int hashCode2 = hybridRowSerializer.Comparer.GetHashCode(obj.EnablePropertyLevelTimestamp);
        hybridRowSerializer = new BooleanHybridRowSerializer();
        int hashCode3 = hybridRowSerializer.Comparer.GetHashCode(obj.DisableSystemPrefix);
        hybridRowSerializer = new BooleanHybridRowSerializer();
        int hashCode4 = hybridRowSerializer.Comparer.GetHashCode(obj.Abstract);
        return HashCode.Combine<int, int, int, int>(hashCode1, hashCode2, hashCode3, hashCode4);
      }
    }
  }
}

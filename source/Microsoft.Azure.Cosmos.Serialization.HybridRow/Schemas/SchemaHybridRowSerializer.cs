// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.SchemaHybridRowSerializer
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
  public readonly struct SchemaHybridRowSerializer : IHybridRowSerializer<Schema>
  {
    public const int SchemaId = 2147473652;
    public const int Size = 7;
    private static readonly Utf8String VersionName = Utf8String.TranscodeUtf16("version");
    private static readonly Utf8String TypeName = Utf8String.TranscodeUtf16("type");
    private static readonly Utf8String SchemaIdName = Utf8String.TranscodeUtf16("id");
    private static readonly Utf8String NameName = Utf8String.TranscodeUtf16("name");
    private static readonly Utf8String CommentName = Utf8String.TranscodeUtf16("comment");
    private static readonly Utf8String OptionsName = Utf8String.TranscodeUtf16("options");
    private static readonly Utf8String PartitionKeysName = Utf8String.TranscodeUtf16("partitionKeys");
    private static readonly Utf8String PrimaryKeysName = Utf8String.TranscodeUtf16("primaryKeys");
    private static readonly Utf8String StaticKeysName = Utf8String.TranscodeUtf16("staticKeys");
    private static readonly Utf8String PropertiesName = Utf8String.TranscodeUtf16("properties");
    private static readonly Utf8String BaseNameName = Utf8String.TranscodeUtf16("baseName");
    private static readonly Utf8String BaseSchemaIdName = Utf8String.TranscodeUtf16("baseId");
    private static readonly LayoutColumn VersionColumn;
    private static readonly LayoutColumn TypeColumn;
    private static readonly LayoutColumn SchemaIdColumn;
    private static readonly LayoutColumn NameColumn;
    private static readonly LayoutColumn CommentColumn;
    private static readonly LayoutColumn OptionsColumn;
    private static readonly LayoutColumn PartitionKeysColumn;
    private static readonly LayoutColumn PrimaryKeysColumn;
    private static readonly LayoutColumn StaticKeysColumn;
    private static readonly LayoutColumn PropertiesColumn;
    private static readonly LayoutColumn BaseNameColumn;
    private static readonly LayoutColumn BaseSchemaIdColumn;
    private static readonly StringToken CommentToken;
    private static readonly StringToken OptionsToken;
    private static readonly StringToken PartitionKeysToken;
    private static readonly StringToken PrimaryKeysToken;
    private static readonly StringToken StaticKeysToken;
    private static readonly StringToken PropertiesToken;
    private static readonly StringToken BaseNameToken;
    private static readonly StringToken BaseSchemaIdToken;

    public IEqualityComparer<Schema> Comparer => (IEqualityComparer<Schema>) SchemaHybridRowSerializer.SchemaComparer.Default;

    static SchemaHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473652));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaHybridRowSerializer.VersionName), out SchemaHybridRowSerializer.VersionColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaHybridRowSerializer.TypeName), out SchemaHybridRowSerializer.TypeColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaHybridRowSerializer.SchemaIdName), out SchemaHybridRowSerializer.SchemaIdColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaHybridRowSerializer.NameName), out SchemaHybridRowSerializer.NameColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaHybridRowSerializer.CommentName), out SchemaHybridRowSerializer.CommentColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaHybridRowSerializer.OptionsName), out SchemaHybridRowSerializer.OptionsColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaHybridRowSerializer.PartitionKeysName), out SchemaHybridRowSerializer.PartitionKeysColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaHybridRowSerializer.PrimaryKeysName), out SchemaHybridRowSerializer.PrimaryKeysColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaHybridRowSerializer.StaticKeysName), out SchemaHybridRowSerializer.StaticKeysColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaHybridRowSerializer.PropertiesName), out SchemaHybridRowSerializer.PropertiesColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaHybridRowSerializer.BaseNameName), out SchemaHybridRowSerializer.BaseNameColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SchemaHybridRowSerializer.BaseSchemaIdName), out SchemaHybridRowSerializer.BaseSchemaIdColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SchemaHybridRowSerializer.CommentColumn.Path), out SchemaHybridRowSerializer.CommentToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SchemaHybridRowSerializer.OptionsColumn.Path), out SchemaHybridRowSerializer.OptionsToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SchemaHybridRowSerializer.PartitionKeysColumn.Path), out SchemaHybridRowSerializer.PartitionKeysToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SchemaHybridRowSerializer.PrimaryKeysColumn.Path), out SchemaHybridRowSerializer.PrimaryKeysToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SchemaHybridRowSerializer.StaticKeysColumn.Path), out SchemaHybridRowSerializer.StaticKeysToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SchemaHybridRowSerializer.PropertiesColumn.Path), out SchemaHybridRowSerializer.PropertiesToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SchemaHybridRowSerializer.BaseNameColumn.Path), out SchemaHybridRowSerializer.BaseNameToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SchemaHybridRowSerializer.BaseSchemaIdColumn.Path), out SchemaHybridRowSerializer.BaseSchemaIdToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      Schema value)
    {
      if (isRoot)
        return SchemaHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473652), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = SchemaHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, Schema value)
    {
      if (value.Version != SchemaLanguageVersion.V1)
      {
        Result result = LayoutType.UInt8.WriteFixed(ref row, ref scope, SchemaHybridRowSerializer.VersionColumn, (byte) value.Version);
        if (result != Result.Success)
          return result;
      }
      if (value.Type != TypeKind.Invalid)
      {
        Result result = LayoutType.UInt8.WriteFixed(ref row, ref scope, SchemaHybridRowSerializer.TypeColumn, (byte) value.Type);
        if (result != Result.Success)
          return result;
      }
      if (value.SchemaId != new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId())
      {
        Result result = LayoutType.Int32.WriteFixed(ref row, ref scope, SchemaHybridRowSerializer.SchemaIdColumn, (int) value.SchemaId);
        if (result != Result.Success)
          return result;
      }
      if (value.Name != null)
      {
        Result result = LayoutType.Utf8.WriteVariable(ref row, ref scope, SchemaHybridRowSerializer.NameColumn, value.Name);
        if (result != Result.Success)
          return result;
      }
      if (value.Comment != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SchemaHybridRowSerializer.CommentColumn.Path));
        Result result = LayoutType.Utf8.WriteSparse(ref row, ref scope, value.Comment, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      if (value.Options != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SchemaHybridRowSerializer.OptionsColumn.Path));
        Result result = new SchemaOptionsHybridRowSerializer().Write(ref row, ref scope, false, SchemaHybridRowSerializer.OptionsColumn.TypeArgs, value.Options);
        if (result != Result.Success)
          return result;
      }
      if (value.PartitionKeys != null && value.PartitionKeys.Count > 0)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SchemaHybridRowSerializer.PartitionKeysColumn.Path));
        Result result = new TypedArrayHybridRowSerializer<PartitionKey, PartitionKeyHybridRowSerializer>().Write(ref row, ref scope, false, SchemaHybridRowSerializer.PartitionKeysColumn.TypeArgs, value.PartitionKeys);
        if (result != Result.Success)
          return result;
      }
      if (value.PrimaryKeys != null && value.PrimaryKeys.Count > 0)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SchemaHybridRowSerializer.PrimaryKeysColumn.Path));
        Result result = new TypedArrayHybridRowSerializer<PrimarySortKey, PrimarySortKeyHybridRowSerializer>().Write(ref row, ref scope, false, SchemaHybridRowSerializer.PrimaryKeysColumn.TypeArgs, value.PrimaryKeys);
        if (result != Result.Success)
          return result;
      }
      if (value.StaticKeys != null && value.StaticKeys.Count > 0)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SchemaHybridRowSerializer.StaticKeysColumn.Path));
        Result result = new TypedArrayHybridRowSerializer<StaticKey, StaticKeyHybridRowSerializer>().Write(ref row, ref scope, false, SchemaHybridRowSerializer.StaticKeysColumn.TypeArgs, value.StaticKeys);
        if (result != Result.Success)
          return result;
      }
      if (value.Properties != null && value.Properties.Count > 0)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SchemaHybridRowSerializer.PropertiesColumn.Path));
        Result result = new TypedArrayHybridRowSerializer<Property, PropertyHybridRowSerializer>().Write(ref row, ref scope, false, SchemaHybridRowSerializer.PropertiesColumn.TypeArgs, value.Properties);
        if (result != Result.Success)
          return result;
      }
      if (value.BaseName != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SchemaHybridRowSerializer.BaseNameColumn.Path));
        Result result = LayoutType.Utf8.WriteSparse(ref row, ref scope, value.BaseName, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      if (value.BaseSchemaId != new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId())
      {
        scope.Find(ref row, Utf8String.op_Implicit(SchemaHybridRowSerializer.BaseSchemaIdColumn.Path));
        Result result = LayoutType.Int32.WriteSparse(ref row, ref scope, (int) value.BaseSchemaId, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      return Result.Success;
    }

    public Result Read(ref RowBuffer row, ref RowCursor scope, bool isRoot, out Schema value)
    {
      if (isRoot)
      {
        value = new Schema();
        return SchemaHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (Schema) null;
        return result1;
      }
      value = new Schema();
      Result result2 = SchemaHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (Schema) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref Schema value)
    {
      byte num1;
      Result result1 = LayoutType.UInt8.ReadFixed(ref row, ref scope, SchemaHybridRowSerializer.VersionColumn, out num1);
      switch (result1)
      {
        case Result.Success:
          value.Version = (SchemaLanguageVersion) num1;
          goto case Result.NotFound;
        case Result.NotFound:
          byte num2;
          Result result2 = LayoutType.UInt8.ReadFixed(ref row, ref scope, SchemaHybridRowSerializer.TypeColumn, out num2);
          switch (result2)
          {
            case Result.Success:
              value.Type = (TypeKind) num2;
              goto case Result.NotFound;
            case Result.NotFound:
              int num3;
              Result result3 = LayoutType.Int32.ReadFixed(ref row, ref scope, SchemaHybridRowSerializer.SchemaIdColumn, out num3);
              switch (result3)
              {
                case Result.Success:
                  value.SchemaId = (Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId) num3;
                  goto case Result.NotFound;
                case Result.NotFound:
                  string str1;
                  Result result4 = LayoutType.Utf8.ReadVariable(ref row, ref scope, SchemaHybridRowSerializer.NameColumn, out str1);
                  switch (result4)
                  {
                    case Result.Success:
                      value.Name = str1;
                      goto case Result.NotFound;
                    case Result.NotFound:
                      while (scope.MoveNext(ref row))
                      {
                        if ((long) scope.Token == (long) SchemaHybridRowSerializer.CommentToken.Id)
                        {
                          string str2;
                          Result result5 = LayoutType.Utf8.ReadSparse(ref row, ref scope, out str2);
                          if (result5 != Result.Success)
                            return result5;
                          value.Comment = str2;
                        }
                        else if ((long) scope.Token == (long) SchemaHybridRowSerializer.OptionsToken.Id)
                        {
                          SchemaOptions schemaOptions;
                          Result result6 = new SchemaOptionsHybridRowSerializer().Read(ref row, ref scope, false, out schemaOptions);
                          if (result6 != Result.Success)
                            return result6;
                          value.Options = schemaOptions;
                        }
                        else if ((long) scope.Token == (long) SchemaHybridRowSerializer.PartitionKeysToken.Id)
                        {
                          List<PartitionKey> partitionKeyList;
                          Result result7 = new TypedArrayHybridRowSerializer<PartitionKey, PartitionKeyHybridRowSerializer>().Read(ref row, ref scope, false, out partitionKeyList);
                          if (result7 != Result.Success)
                            return result7;
                          value.PartitionKeys = partitionKeyList;
                        }
                        else if ((long) scope.Token == (long) SchemaHybridRowSerializer.PrimaryKeysToken.Id)
                        {
                          List<PrimarySortKey> primarySortKeyList;
                          Result result8 = new TypedArrayHybridRowSerializer<PrimarySortKey, PrimarySortKeyHybridRowSerializer>().Read(ref row, ref scope, false, out primarySortKeyList);
                          if (result8 != Result.Success)
                            return result8;
                          value.PrimaryKeys = primarySortKeyList;
                        }
                        else if ((long) scope.Token == (long) SchemaHybridRowSerializer.StaticKeysToken.Id)
                        {
                          List<StaticKey> staticKeyList;
                          Result result9 = new TypedArrayHybridRowSerializer<StaticKey, StaticKeyHybridRowSerializer>().Read(ref row, ref scope, false, out staticKeyList);
                          if (result9 != Result.Success)
                            return result9;
                          value.StaticKeys = staticKeyList;
                        }
                        else if ((long) scope.Token == (long) SchemaHybridRowSerializer.PropertiesToken.Id)
                        {
                          List<Property> propertyList;
                          Result result10 = new TypedArrayHybridRowSerializer<Property, PropertyHybridRowSerializer>().Read(ref row, ref scope, false, out propertyList);
                          if (result10 != Result.Success)
                            return result10;
                          value.Properties = propertyList;
                        }
                        else if ((long) scope.Token == (long) SchemaHybridRowSerializer.BaseNameToken.Id)
                        {
                          string str3;
                          Result result11 = LayoutType.Utf8.ReadSparse(ref row, ref scope, out str3);
                          if (result11 != Result.Success)
                            return result11;
                          value.BaseName = str3;
                        }
                        else if ((long) scope.Token == (long) SchemaHybridRowSerializer.BaseSchemaIdToken.Id)
                        {
                          int num4;
                          Result result12 = LayoutType.Int32.ReadSparse(ref row, ref scope, out num4);
                          if (result12 != Result.Success)
                            return result12;
                          value.BaseSchemaId = (Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId) num4;
                        }
                      }
                      return Result.Success;
                    default:
                      return result4;
                  }
                default:
                  return result3;
              }
            default:
              return result2;
          }
        default:
          return result1;
      }
    }

    public sealed class SchemaComparer : EqualityComparer<Schema>
    {
      public static readonly SchemaHybridRowSerializer.SchemaComparer Default = new SchemaHybridRowSerializer.SchemaComparer();

      public override bool Equals(Schema x, Schema y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<Schema>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        UInt8HybridRowSerializer hybridRowSerializer1 = new UInt8HybridRowSerializer();
        if (hybridRowSerializer1.Comparer.Equals((byte) x.Version, (byte) y.Version))
        {
          hybridRowSerializer1 = new UInt8HybridRowSerializer();
          if (hybridRowSerializer1.Comparer.Equals((byte) x.Type, (byte) y.Type))
          {
            Int32HybridRowSerializer hybridRowSerializer2 = new Int32HybridRowSerializer();
            if (hybridRowSerializer2.Comparer.Equals((int) x.SchemaId, (int) y.SchemaId))
            {
              Utf8HybridRowSerializer hybridRowSerializer3 = new Utf8HybridRowSerializer();
              if (hybridRowSerializer3.Comparer.Equals(x.Name, y.Name))
              {
                hybridRowSerializer3 = new Utf8HybridRowSerializer();
                if (hybridRowSerializer3.Comparer.Equals(x.Comment, y.Comment) && new SchemaOptionsHybridRowSerializer().Comparer.Equals(x.Options, y.Options) && new TypedArrayHybridRowSerializer<PartitionKey, PartitionKeyHybridRowSerializer>().Comparer.Equals(x.PartitionKeys, y.PartitionKeys) && new TypedArrayHybridRowSerializer<PrimarySortKey, PrimarySortKeyHybridRowSerializer>().Comparer.Equals(x.PrimaryKeys, y.PrimaryKeys) && new TypedArrayHybridRowSerializer<StaticKey, StaticKeyHybridRowSerializer>().Comparer.Equals(x.StaticKeys, y.StaticKeys) && new TypedArrayHybridRowSerializer<Property, PropertyHybridRowSerializer>().Comparer.Equals(x.Properties, y.Properties))
                {
                  hybridRowSerializer3 = new Utf8HybridRowSerializer();
                  if (hybridRowSerializer3.Comparer.Equals(x.BaseName, y.BaseName))
                  {
                    hybridRowSerializer2 = new Int32HybridRowSerializer();
                    return hybridRowSerializer2.Comparer.Equals((int) x.BaseSchemaId, (int) y.BaseSchemaId);
                  }
                }
              }
            }
          }
        }
        return false;
      }

      public override int GetHashCode(Schema obj)
      {
        HashCode hashCode = new HashCode();
        ref HashCode local1 = ref hashCode;
        int version = (int) obj.Version;
        UInt8HybridRowSerializer hybridRowSerializer1 = new UInt8HybridRowSerializer();
        IEqualityComparer<byte> comparer1 = hybridRowSerializer1.Comparer;
        ((HashCode) ref local1).Add<byte>((byte) version, comparer1);
        ref HashCode local2 = ref hashCode;
        int type = (int) obj.Type;
        hybridRowSerializer1 = new UInt8HybridRowSerializer();
        IEqualityComparer<byte> comparer2 = hybridRowSerializer1.Comparer;
        ((HashCode) ref local2).Add<byte>((byte) type, comparer2);
        ref HashCode local3 = ref hashCode;
        int schemaId = (int) obj.SchemaId;
        Int32HybridRowSerializer hybridRowSerializer2 = new Int32HybridRowSerializer();
        IEqualityComparer<int> comparer3 = hybridRowSerializer2.Comparer;
        ((HashCode) ref local3).Add<int>(schemaId, comparer3);
        ref HashCode local4 = ref hashCode;
        string name = obj.Name;
        Utf8HybridRowSerializer hybridRowSerializer3 = new Utf8HybridRowSerializer();
        IEqualityComparer<string> comparer4 = hybridRowSerializer3.Comparer;
        ((HashCode) ref local4).Add<string>(name, comparer4);
        ref HashCode local5 = ref hashCode;
        string comment = obj.Comment;
        hybridRowSerializer3 = new Utf8HybridRowSerializer();
        IEqualityComparer<string> comparer5 = hybridRowSerializer3.Comparer;
        ((HashCode) ref local5).Add<string>(comment, comparer5);
        ((HashCode) ref hashCode).Add<SchemaOptions>(obj.Options, new SchemaOptionsHybridRowSerializer().Comparer);
        ((HashCode) ref hashCode).Add<List<PartitionKey>>(obj.PartitionKeys, new TypedArrayHybridRowSerializer<PartitionKey, PartitionKeyHybridRowSerializer>().Comparer);
        ((HashCode) ref hashCode).Add<List<PrimarySortKey>>(obj.PrimaryKeys, new TypedArrayHybridRowSerializer<PrimarySortKey, PrimarySortKeyHybridRowSerializer>().Comparer);
        ((HashCode) ref hashCode).Add<List<StaticKey>>(obj.StaticKeys, new TypedArrayHybridRowSerializer<StaticKey, StaticKeyHybridRowSerializer>().Comparer);
        ((HashCode) ref hashCode).Add<List<Property>>(obj.Properties, new TypedArrayHybridRowSerializer<Property, PropertyHybridRowSerializer>().Comparer);
        ref HashCode local6 = ref hashCode;
        string baseName = obj.BaseName;
        hybridRowSerializer3 = new Utf8HybridRowSerializer();
        IEqualityComparer<string> comparer6 = hybridRowSerializer3.Comparer;
        ((HashCode) ref local6).Add<string>(baseName, comparer6);
        ref HashCode local7 = ref hashCode;
        int baseSchemaId = (int) obj.BaseSchemaId;
        hybridRowSerializer2 = new Int32HybridRowSerializer();
        IEqualityComparer<int> comparer7 = hybridRowSerializer2.Comparer;
        ((HashCode) ref local7).Add<int>(baseSchemaId, comparer7);
        return ((HashCode) ref hashCode).ToHashCode();
      }
    }
  }
}

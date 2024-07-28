// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutCompiler
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  internal sealed class LayoutCompiler
  {
    private const string BasePropertyName = "__base";

    public static Layout Compile(Namespace ns, Schema schema)
    {
      Contract.Requires(ns != null);
      Contract.Requires(schema != null);
      Contract.Requires(schema.Type == TypeKind.Schema);
      Contract.Requires(!string.IsNullOrWhiteSpace(schema.Name));
      Contract.Requires(ns.Schemas.Contains(schema));
      SchemaLanguageVersion effectiveSdlVersion = schema.GetEffectiveSdlVersion(ns);
      LayoutBuilder builder = new LayoutBuilder(schema.Name, schema.SchemaId);
      if (schema.BaseName != null)
        LayoutCompiler.AddBase(builder, ns, schema);
      LayoutCompiler.AddProperties(builder, effectiveSdlVersion, ns, LayoutCode.Schema, schema.Properties);
      return builder.Build();
    }

    private static void AddBase(LayoutBuilder builder, Namespace ns, Schema s)
    {
      Schema schema;
      if (s.BaseSchemaId == SchemaId.Invalid)
      {
        schema = ns.Schemas.Find((Predicate<Schema>) (q => q.Name == s.BaseName));
      }
      else
      {
        schema = ns.Schemas.Find((Predicate<Schema>) (q => q.SchemaId == s.BaseSchemaId));
        if (schema.Name != s.BaseName)
          throw new LayoutCompilationException(string.Format("Ambiguous schema reference: '{0}:{1}'", (object) s.BaseName, (object) s.BaseSchemaId));
      }
      if (schema == null)
        throw new LayoutCompilationException(string.Format("Cannot resolve schema reference '{0}:{1}'", (object) s.BaseName, (object) s.BaseSchemaId));
      builder.AddTypedScope("__base", (LayoutType) LayoutType.UDT, new TypeArgumentList(schema.SchemaId));
    }

    private static void AddProperties(
      LayoutBuilder builder,
      SchemaLanguageVersion v,
      Namespace ns,
      LayoutCode scope,
      List<Property> properties)
    {
      foreach (Property property in properties)
      {
        if (property.PropertyType == null)
          throw new LayoutCompilationException("Property missing type");
        TypeArgumentList typeArgs;
        LayoutType physicalType = LayoutCompiler.LogicalToPhysicalType(v, ns, property.PropertyType, out typeArgs);
        switch (LayoutCodeTraits.ClearImmutableBit(physicalType.LayoutCode))
        {
          case LayoutCode.ObjectScope:
            if (!property.PropertyType.Nullable)
              throw new LayoutCompilationException("Non-nullable sparse column are not supported.");
            ObjectPropertyType propertyType1 = (ObjectPropertyType) property.PropertyType;
            builder.AddObjectScope(property.Path, physicalType);
            LayoutCompiler.AddProperties(builder, v, ns, physicalType.LayoutCode, propertyType1.Properties);
            builder.EndObjectScope();
            continue;
          case LayoutCode.ArrayScope:
          case LayoutCode.TypedArrayScope:
          case LayoutCode.TupleScope:
          case LayoutCode.TypedTupleScope:
          case LayoutCode.MapScope:
          case LayoutCode.TypedMapScope:
          case LayoutCode.SetScope:
          case LayoutCode.TypedSetScope:
          case LayoutCode.TaggedScope:
          case LayoutCode.Tagged2Scope:
          case LayoutCode.Schema:
            if (!property.PropertyType.Nullable)
              throw new LayoutCompilationException("Non-nullable sparse column are not supported.");
            builder.AddTypedScope(property.Path, physicalType, typeArgs);
            continue;
          case LayoutCode.NullableScope:
            throw new LayoutCompilationException("Nullables cannot be explicitly declared as columns.");
          default:
            if (!(property.PropertyType is PrimitivePropertyType propertyType2))
              throw new LayoutCompilationException("Unknown property type: " + physicalType.Name);
            if (propertyType2.Type == TypeKind.Enum && v < SchemaLanguageVersion.V2)
              throw new LayoutCompilationException("Enums require SDL v2 or higher.");
            switch (propertyType2.Storage)
            {
              case StorageKind.Sparse:
                if (!propertyType2.Nullable)
                  throw new LayoutCompilationException("Non-nullable sparse columns are not supported.");
                builder.AddSparseColumn(property.Path, physicalType);
                continue;
              case StorageKind.Fixed:
                if (LayoutCodeTraits.ClearImmutableBit(scope) != LayoutCode.Schema)
                  throw new LayoutCompilationException("Cannot have fixed storage within a sparse scope.");
                if (physicalType.IsNull && !propertyType2.Nullable)
                  throw new LayoutCompilationException("Non-nullable null columns are not supported.");
                builder.AddFixedColumn(property.Path, physicalType, propertyType2.Nullable, propertyType2.Length);
                continue;
              case StorageKind.Variable:
                if (propertyType2.Type == TypeKind.Enum)
                  throw new LayoutCompilationException(string.Format("Enums cannot have storage specification: {0}", (object) propertyType2.Storage));
                if (LayoutCodeTraits.ClearImmutableBit(scope) != LayoutCode.Schema)
                  throw new LayoutCompilationException("Cannot have variable storage within a sparse scope.");
                if (!propertyType2.Nullable)
                  throw new LayoutCompilationException("Non-nullable variable columns are not supported.");
                builder.AddVariableColumn(property.Path, physicalType, propertyType2.Length);
                continue;
              default:
                throw new LayoutCompilationException(string.Format("Unknown storage specification: {0}", (object) propertyType2.Storage));
            }
        }
      }
    }

    private static LayoutType LogicalToPhysicalType(
      SchemaLanguageVersion v,
      Namespace ns,
      PropertyType logicalType,
      out TypeArgumentList typeArgs)
    {
      typeArgs = TypeArgumentList.Empty;
      bool flag = logicalType is ScopePropertyType scopePropertyType && scopePropertyType.Immutable;
      switch (logicalType.Type)
      {
        case TypeKind.Null:
        case TypeKind.Boolean:
        case TypeKind.Int8:
        case TypeKind.Int16:
        case TypeKind.Int32:
        case TypeKind.Int64:
        case TypeKind.UInt8:
        case TypeKind.UInt16:
        case TypeKind.UInt32:
        case TypeKind.UInt64:
        case TypeKind.VarInt:
        case TypeKind.VarUInt:
        case TypeKind.Float32:
        case TypeKind.Float64:
        case TypeKind.Float128:
        case TypeKind.Decimal:
        case TypeKind.DateTime:
        case TypeKind.UnixDateTime:
        case TypeKind.Guid:
        case TypeKind.MongoDbObjectId:
        case TypeKind.Utf8:
        case TypeKind.Binary:
          return LayoutCompiler.PrimitiveToPhysicalType(logicalType.Type);
        case TypeKind.Object:
          return !flag ? (LayoutType) LayoutType.Object : (LayoutType) LayoutType.ImmutableObject;
        case TypeKind.Array:
          ArrayPropertyType arrayPropertyType = (ArrayPropertyType) logicalType;
          if (arrayPropertyType.Items != null && arrayPropertyType.Items.Type != TypeKind.Any)
          {
            TypeArgumentList typeArgs1;
            LayoutType type = LayoutCompiler.LogicalToPhysicalType(v, ns, arrayPropertyType.Items, out typeArgs1);
            if (arrayPropertyType.Items.Nullable)
            {
              typeArgs1 = new TypeArgumentList(new TypeArgument[1]
              {
                new TypeArgument(type, typeArgs1)
              });
              type = type.Immutable ? (LayoutType) LayoutType.ImmutableNullable : (LayoutType) LayoutType.Nullable;
            }
            typeArgs = new TypeArgumentList(new TypeArgument[1]
            {
              new TypeArgument(type, typeArgs1)
            });
            return !flag ? (LayoutType) LayoutType.TypedArray : (LayoutType) LayoutType.ImmutableTypedArray;
          }
          return !flag ? (LayoutType) LayoutType.Array : (LayoutType) LayoutType.ImmutableArray;
        case TypeKind.Set:
          SetPropertyType setPropertyType = (SetPropertyType) logicalType;
          if (setPropertyType.Items == null || setPropertyType.Items.Type == TypeKind.Any)
            throw new LayoutCompilationException(string.Format("Unknown property type: {0}", (object) logicalType.Type));
          TypeArgumentList typeArgs2;
          LayoutType type1 = LayoutCompiler.LogicalToPhysicalType(v, ns, setPropertyType.Items, out typeArgs2);
          if (setPropertyType.Items.Nullable)
          {
            typeArgs2 = new TypeArgumentList(new TypeArgument[1]
            {
              new TypeArgument(type1, typeArgs2)
            });
            type1 = type1.Immutable ? (LayoutType) LayoutType.ImmutableNullable : (LayoutType) LayoutType.Nullable;
          }
          typeArgs = new TypeArgumentList(new TypeArgument[1]
          {
            new TypeArgument(type1, typeArgs2)
          });
          return !flag ? (LayoutType) LayoutType.TypedSet : (LayoutType) LayoutType.ImmutableTypedSet;
        case TypeKind.Map:
          MapPropertyType mapPropertyType = (MapPropertyType) logicalType;
          if (mapPropertyType.Keys == null || mapPropertyType.Keys.Type == TypeKind.Any || mapPropertyType.Values == null || mapPropertyType.Values.Type == TypeKind.Any)
            throw new LayoutCompilationException(string.Format("Unknown property type: {0}", (object) logicalType.Type));
          TypeArgumentList typeArgs3;
          LayoutType type2 = LayoutCompiler.LogicalToPhysicalType(v, ns, mapPropertyType.Keys, out typeArgs3);
          if (mapPropertyType.Keys.Nullable)
          {
            typeArgs3 = new TypeArgumentList(new TypeArgument[1]
            {
              new TypeArgument(type2, typeArgs3)
            });
            type2 = type2.Immutable ? (LayoutType) LayoutType.ImmutableNullable : (LayoutType) LayoutType.Nullable;
          }
          TypeArgumentList typeArgs4;
          LayoutType type3 = LayoutCompiler.LogicalToPhysicalType(v, ns, mapPropertyType.Values, out typeArgs4);
          if (mapPropertyType.Values.Nullable)
          {
            typeArgs4 = new TypeArgumentList(new TypeArgument[1]
            {
              new TypeArgument(type3, typeArgs4)
            });
            type3 = type3.Immutable ? (LayoutType) LayoutType.ImmutableNullable : (LayoutType) LayoutType.Nullable;
          }
          typeArgs = new TypeArgumentList(new TypeArgument[2]
          {
            new TypeArgument(type2, typeArgs3),
            new TypeArgument(type3, typeArgs4)
          });
          return !flag ? (LayoutType) LayoutType.TypedMap : (LayoutType) LayoutType.ImmutableTypedMap;
        case TypeKind.Tuple:
          TuplePropertyType tuplePropertyType = (TuplePropertyType) logicalType;
          TypeArgument[] args1 = new TypeArgument[tuplePropertyType.Items.Count];
          int index1 = 0;
          while (index1 < tuplePropertyType.Items.Count)
          {
            TypeArgumentList typeArgs5;
            LayoutType type4 = LayoutCompiler.LogicalToPhysicalType(v, ns, tuplePropertyType.Items[index1], out typeArgs5);
            if (tuplePropertyType.Items[index1].Nullable)
            {
              typeArgs5 = new TypeArgumentList(new TypeArgument[1]
              {
                new TypeArgument(type4, typeArgs5)
              });
              type4 = type4.Immutable ? (LayoutType) LayoutType.ImmutableNullable : (LayoutType) LayoutType.Nullable;
            }
            args1[index1] = new TypeArgument(type4, typeArgs5);
            checked { ++index1; }
          }
          typeArgs = new TypeArgumentList(args1);
          return !flag ? (LayoutType) LayoutType.TypedTuple : (LayoutType) LayoutType.ImmutableTypedTuple;
        case TypeKind.Tagged:
          TaggedPropertyType taggedPropertyType = (TaggedPropertyType) logicalType;
          if (taggedPropertyType.Items.Count < 1 || taggedPropertyType.Items.Count > 2)
            throw new LayoutCompilationException(string.Format("Invalid number of arguments in Tagged: {0} <= {1} <= {2}", (object) 1, (object) taggedPropertyType.Items.Count, (object) 2));
          TypeArgument[] args2 = new TypeArgument[checked (taggedPropertyType.Items.Count + 1)];
          args2[0] = new TypeArgument((LayoutType) LayoutType.UInt8, TypeArgumentList.Empty);
          int index2 = 0;
          while (index2 < taggedPropertyType.Items.Count)
          {
            TypeArgumentList typeArgs6;
            LayoutType type5 = LayoutCompiler.LogicalToPhysicalType(v, ns, taggedPropertyType.Items[index2], out typeArgs6);
            if (taggedPropertyType.Items[index2].Nullable)
            {
              typeArgs6 = new TypeArgumentList(new TypeArgument[1]
              {
                new TypeArgument(type5, typeArgs6)
              });
              type5 = type5.Immutable ? (LayoutType) LayoutType.ImmutableNullable : (LayoutType) LayoutType.Nullable;
            }
            args2[checked (index2 + 1)] = new TypeArgument(type5, typeArgs6);
            checked { ++index2; }
          }
          typeArgs = new TypeArgumentList(args2);
          switch (taggedPropertyType.Items.Count)
          {
            case 1:
              return !flag ? (LayoutType) LayoutType.Tagged : (LayoutType) LayoutType.ImmutableTagged;
            case 2:
              return !flag ? (LayoutType) LayoutType.Tagged2 : (LayoutType) LayoutType.ImmutableTagged2;
            default:
              throw new LayoutCompilationException("Unexpected tagged arity");
          }
        case TypeKind.Schema:
          UdtPropertyType up = (UdtPropertyType) logicalType;
          Schema schema;
          if (up.SchemaId == SchemaId.Invalid)
          {
            schema = ns.Schemas.Find((Predicate<Schema>) (s => s.Name == up.Name));
          }
          else
          {
            schema = ns.Schemas.Find((Predicate<Schema>) (s => s.SchemaId == up.SchemaId));
            if (schema.Name != up.Name)
              throw new LayoutCompilationException(string.Format("Ambiguous schema reference: '{0}:{1}'", (object) up.Name, (object) up.SchemaId));
          }
          typeArgs = schema != null ? new TypeArgumentList(schema.SchemaId) : throw new LayoutCompilationException(string.Format("Cannot resolve schema reference '{0}:{1}'", (object) up.Name, (object) up.SchemaId));
          return !flag ? (LayoutType) LayoutType.UDT : (LayoutType) LayoutType.ImmutableUDT;
        case TypeKind.Enum:
          if (v < SchemaLanguageVersion.V2)
            throw new LayoutCompilationException("Enums require SDL v2 or higher.");
          PrimitivePropertyType ep = (PrimitivePropertyType) logicalType;
          return LayoutCompiler.PrimitiveToPhysicalType((ns.Enums.Find((Predicate<EnumSchema>) (es => es.Name == ep.Enum)) ?? throw new LayoutCompilationException("Cannot resolve enum schema reference '" + ep.Enum + "'")).Type);
        default:
          throw new LayoutCompilationException(string.Format("Unknown property type: {0}", (object) logicalType.Type));
      }
    }

    private static LayoutType PrimitiveToPhysicalType(TypeKind type)
    {
      switch (type)
      {
        case TypeKind.Null:
          return (LayoutType) LayoutType.Null;
        case TypeKind.Boolean:
          return (LayoutType) LayoutType.Boolean;
        case TypeKind.Int8:
          return (LayoutType) LayoutType.Int8;
        case TypeKind.Int16:
          return (LayoutType) LayoutType.Int16;
        case TypeKind.Int32:
          return (LayoutType) LayoutType.Int32;
        case TypeKind.Int64:
          return (LayoutType) LayoutType.Int64;
        case TypeKind.UInt8:
          return (LayoutType) LayoutType.UInt8;
        case TypeKind.UInt16:
          return (LayoutType) LayoutType.UInt16;
        case TypeKind.UInt32:
          return (LayoutType) LayoutType.UInt32;
        case TypeKind.UInt64:
          return (LayoutType) LayoutType.UInt64;
        case TypeKind.VarInt:
          return (LayoutType) LayoutType.VarInt;
        case TypeKind.VarUInt:
          return (LayoutType) LayoutType.VarUInt;
        case TypeKind.Float32:
          return (LayoutType) LayoutType.Float32;
        case TypeKind.Float64:
          return (LayoutType) LayoutType.Float64;
        case TypeKind.Float128:
          return (LayoutType) LayoutType.Float128;
        case TypeKind.Decimal:
          return (LayoutType) LayoutType.Decimal;
        case TypeKind.DateTime:
          return (LayoutType) LayoutType.DateTime;
        case TypeKind.UnixDateTime:
          return (LayoutType) LayoutType.UnixDateTime;
        case TypeKind.Guid:
          return (LayoutType) LayoutType.Guid;
        case TypeKind.MongoDbObjectId:
          return (LayoutType) LayoutType.MongoDbObjectId;
        case TypeKind.Utf8:
          return (LayoutType) LayoutType.Utf8;
        case TypeKind.Binary:
          return (LayoutType) LayoutType.Binary;
        default:
          throw new LayoutCompilationException(string.Format("Unknown property type: {0}", (object) type));
      }
    }
  }
}

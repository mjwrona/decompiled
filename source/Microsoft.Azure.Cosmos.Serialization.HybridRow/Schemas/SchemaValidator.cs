// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.SchemaValidator
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  public static class SchemaValidator
  {
    public static void Validate(Namespace ns)
    {
      Dictionary<string, int> dictionary1 = new Dictionary<string, int>(ns.Schemas.Count);
      Dictionary<(string, SchemaId), Schema> dictionary2 = new Dictionary<(string, SchemaId), Schema>(ns.Schemas.Count);
      Dictionary<SchemaId, Schema> dictionary3 = new Dictionary<SchemaId, Schema>(ns.Schemas.Count);
      Dictionary<string, EnumSchema> dictionary4 = new Dictionary<string, EnumSchema>(ns.Enums.Count);
      foreach (Schema schema in ns.Schemas)
      {
        SchemaValidator.ValidateAssert.IsValidSchemaId(schema.SchemaId, "Schema id");
        SchemaValidator.ValidateAssert.IsValidIdentifier(schema.Name, "Schema name");
        SchemaValidator.ValidateAssert.DuplicateCheck<SchemaId, Schema>(schema.SchemaId, schema, dictionary3, "Schema id", "Namespace");
        SchemaValidator.ValidateAssert.DuplicateCheck<(string, SchemaId), Schema>((schema.Name, schema.SchemaId), schema, dictionary2, "Schema reference", "Namespace");
        int num;
        dictionary1.TryGetValue(schema.Name, out num);
        dictionary1[schema.Name] = checked (num + 1);
      }
      foreach (Schema schema in ns.Schemas)
      {
        if (dictionary1[schema.Name] == 1)
          SchemaValidator.ValidateAssert.DuplicateCheck<(string, SchemaId), Schema>((schema.Name, SchemaId.Invalid), schema, dictionary2, "Schema reference", "Namespace");
      }
      foreach (EnumSchema enumSchema in ns.Enums)
      {
        SchemaValidator.ValidateAssert.IsValidIdentifier(enumSchema.Name, "EnumSchema name");
        SchemaValidator.ValidateAssert.DuplicateCheck<string, EnumSchema>(enumSchema.Name, enumSchema, dictionary4, "EnumSchema reference", "Namespace");
      }
      SchemaValidator.Visit(ns, dictionary2, dictionary3, dictionary4);
    }

    private static void Visit(
      Namespace ns,
      Dictionary<(string, SchemaId), Schema> schemas,
      Dictionary<SchemaId, Schema> ids,
      Dictionary<string, EnumSchema> enums)
    {
      foreach (Schema schema in ns.Schemas)
        SchemaValidator.Visit(schema.GetEffectiveSdlVersion(ns), schema, schemas, ids, enums);
      foreach (EnumSchema es in ns.Enums)
        SchemaValidator.Visit(ns.GetEffectiveSdlVersion(), es);
    }

    private static void Visit(SchemaLanguageVersion v, EnumSchema es)
    {
      SchemaValidator.ValidateAssert.IsTrue(v >= SchemaLanguageVersion.V2, string.Format("Enums require SDL v2 or higher : {0}", (object) v));
      Dictionary<string, EnumValue> scope = new Dictionary<string, EnumValue>(es.Values.Count);
      foreach (EnumValue enumValue in es.Values)
      {
        SchemaValidator.ValidateAssert.IsValidIdentifier(enumValue.Name, "EnumSchema name");
        SchemaValidator.ValidateAssert.DuplicateCheck<string, EnumValue>(enumValue.Name, enumValue, scope, "EnumValue", "EnumSchema");
        switch (es.Type)
        {
          case TypeKind.Int8:
            SchemaValidator.ValidateAssert.IsValidEnumSize((long) sbyte.MinValue, (long) sbyte.MaxValue, enumValue, es);
            continue;
          case TypeKind.Int16:
            SchemaValidator.ValidateAssert.IsValidEnumSize((long) short.MinValue, (long) short.MaxValue, enumValue, es);
            continue;
          case TypeKind.Int32:
            SchemaValidator.ValidateAssert.IsValidEnumSize((long) int.MinValue, (long) int.MaxValue, enumValue, es);
            continue;
          case TypeKind.Int64:
          case TypeKind.VarInt:
            SchemaValidator.ValidateAssert.IsValidEnumSize(long.MinValue, long.MaxValue, enumValue, es);
            continue;
          case TypeKind.UInt8:
            SchemaValidator.ValidateAssert.IsValidEnumUSize(0UL, (ulong) byte.MaxValue, enumValue, es);
            continue;
          case TypeKind.UInt16:
            SchemaValidator.ValidateAssert.IsValidEnumUSize(0UL, (ulong) ushort.MaxValue, enumValue, es);
            continue;
          case TypeKind.UInt32:
            SchemaValidator.ValidateAssert.IsValidEnumUSize(0UL, (ulong) uint.MaxValue, enumValue, es);
            continue;
          case TypeKind.UInt64:
          case TypeKind.VarUInt:
            SchemaValidator.ValidateAssert.IsValidEnumUSize(0UL, ulong.MaxValue, enumValue, es);
            continue;
          default:
            throw new SchemaException(string.Format("{0} is not a valid enumeration type.", (object) es.Type));
        }
      }
    }

    private static void Visit(
      SchemaLanguageVersion v,
      Schema s,
      Dictionary<(string, SchemaId), Schema> schemas,
      Dictionary<SchemaId, Schema> ids,
      Dictionary<string, EnumSchema> enums)
    {
      SchemaValidator.ValidateAssert.AreEqual<TypeKind>(s.Type, TypeKind.Schema, string.Format("The type of a schema MUST be {0}: {1}", (object) TypeKind.Schema, (object) s.Type));
      Dictionary<string, Property> scope1 = new Dictionary<string, Property>(s.Properties.Count);
      foreach (Property property in s.Properties)
        SchemaValidator.ValidateAssert.DuplicateCheck<string, Property>(property.Path, property, scope1, "Property path", "Schema");
      foreach (PartitionKey partitionKey in s.PartitionKeys)
        SchemaValidator.ValidateAssert.Exists<string, Property>(partitionKey.Path, scope1, "Partition key column", "Schema");
      foreach (PrimarySortKey primaryKey in s.PrimaryKeys)
        SchemaValidator.ValidateAssert.Exists<string, Property>(primaryKey.Path, scope1, "Primary sort key column", "Schema");
      foreach (StaticKey staticKey in s.StaticKeys)
        SchemaValidator.ValidateAssert.Exists<string, Property>(staticKey.Path, scope1, "Static key column", "Schema");
      foreach (Property property in s.Properties)
        SchemaValidator.Visit(property, s, schemas, ids, enums);
      List<SchemaId> scope2 = new List<SchemaId>();
      Schema schema1;
      for (Schema schema2 = s; schema2.BaseName != null; schema2 = schema1)
      {
        SchemaValidator.ValidateAssert.IsTrue(v >= SchemaLanguageVersion.V2, string.Format("Inheritance requires SDL v2 or higher : {0}", (object) v));
        schema1 = SchemaValidator.ValidateAssert.Exists<(string, SchemaId), Schema>((schema2.BaseName, schema2.BaseSchemaId), schemas, "Schema reference", "Namespace");
        if (schema2.BaseSchemaId != SchemaId.Invalid)
        {
          Schema schema3 = SchemaValidator.ValidateAssert.Exists<SchemaId, Schema>(schema2.BaseSchemaId, ids, "Schema id", "Namespace");
          SchemaValidator.ValidateAssert.AreEqual<string>(schema2.BaseName, schema3.Name, string.Format("Schema name '{0}' does not match the name of schema with id '{1}': {2}", (object) schema2.BaseName, (object) schema2.BaseSchemaId, (object) schema3.Name));
        }
        SchemaValidator.ValidateAssert.NotExists<SchemaId>(schema1.SchemaId, scope2, string.Format("Inheritance cycle with '{0}': {1}", (object) s.Name, (object) s.SchemaId), "Namespace");
        scope2.Add(schema1.SchemaId);
      }
    }

    private static void Visit(
      Property p,
      Schema s,
      Dictionary<(string, SchemaId), Schema> schemas,
      Dictionary<SchemaId, Schema> ids,
      Dictionary<string, EnumSchema> enums)
    {
      SchemaValidator.ValidateAssert.IsValidIdentifier(p.Path, "Property path");
      SchemaValidator.Visit(p.PropertyType, (PropertyType) null, schemas, ids, enums);
    }

    private static void Visit(
      PropertyType p,
      PropertyType parent,
      Dictionary<(string, SchemaId), Schema> schemas,
      Dictionary<SchemaId, Schema> ids,
      Dictionary<string, EnumSchema> enums)
    {
      switch (p)
      {
        case PrimitivePropertyType primitivePropertyType:
          SchemaValidator.ValidateAssert.IsTrue(primitivePropertyType.Length >= 0, "Length MUST be positive");
          if (parent != null)
            SchemaValidator.ValidateAssert.AreEqual<StorageKind>(primitivePropertyType.Storage, StorageKind.Sparse, string.Format("Nested fields MUST have storage {0}", (object) StorageKind.Sparse));
          if (primitivePropertyType.Type == TypeKind.Enum)
            SchemaValidator.ValidateAssert.Exists<string, EnumSchema>(primitivePropertyType.Enum, enums, "Enum reference", "Namespace");
          if (!primitivePropertyType.RowBufferSize)
            break;
          SchemaValidator.ValidateAssert.AreEqual<StorageKind>(primitivePropertyType.Storage, StorageKind.Fixed, string.Format("RowBufferSize fields MUST have storage {0}", (object) StorageKind.Fixed));
          SchemaValidator.ValidateAssert.AreEqual<TypeKind>(primitivePropertyType.Type, TypeKind.Int32, "RowBufferSize fields MUST be type int32");
          break;
        case ArrayPropertyType arrayPropertyType:
          if (arrayPropertyType.Items == null)
            break;
          SchemaValidator.Visit(arrayPropertyType.Items, p, schemas, ids, enums);
          break;
        case MapPropertyType mapPropertyType:
          SchemaValidator.Visit(mapPropertyType.Keys, p, schemas, ids, enums);
          SchemaValidator.Visit(mapPropertyType.Values, p, schemas, ids, enums);
          break;
        case SetPropertyType setPropertyType:
          SchemaValidator.Visit(setPropertyType.Items, p, schemas, ids, enums);
          break;
        case TaggedPropertyType taggedPropertyType:
          using (List<PropertyType>.Enumerator enumerator = taggedPropertyType.Items.GetEnumerator())
          {
            while (enumerator.MoveNext())
              SchemaValidator.Visit(enumerator.Current, p, schemas, ids, enums);
            break;
          }
        case TuplePropertyType tuplePropertyType:
          using (List<PropertyType>.Enumerator enumerator = tuplePropertyType.Items.GetEnumerator())
          {
            while (enumerator.MoveNext())
              SchemaValidator.Visit(enumerator.Current, p, schemas, ids, enums);
            break;
          }
        case ObjectPropertyType objectPropertyType:
          Dictionary<string, Property> scope = new Dictionary<string, Property>(objectPropertyType.Properties.Count);
          using (List<Property>.Enumerator enumerator = objectPropertyType.Properties.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Property current = enumerator.Current;
              SchemaValidator.ValidateAssert.DuplicateCheck<string, Property>(current.Path, current, scope, "Property path", "Object");
              SchemaValidator.Visit(current.PropertyType, p, schemas, ids, enums);
            }
            break;
          }
        case UdtPropertyType udtPropertyType:
          SchemaValidator.ValidateAssert.Exists<(string, SchemaId), Schema>((udtPropertyType.Name, udtPropertyType.SchemaId), schemas, "Schema reference", "Namespace");
          if (!(udtPropertyType.SchemaId != SchemaId.Invalid))
            break;
          Schema schema = SchemaValidator.ValidateAssert.Exists<SchemaId, Schema>(udtPropertyType.SchemaId, ids, "Schema id", "Namespace");
          SchemaValidator.ValidateAssert.AreEqual<string>(udtPropertyType.Name, schema.Name, string.Format("Schema name '{0}' does not match the name of schema with id '{1}': {2}", (object) udtPropertyType.Name, (object) udtPropertyType.SchemaId, (object) schema.Name));
          break;
        default:
          Contract.Fail("Unknown property type");
          break;
      }
    }

    private static class ValidateAssert
    {
      public static void AreEqual<T>(T left, T right, string message)
      {
        if (!left.Equals((object) right))
          throw new SchemaException(message);
      }

      public static void IsTrue(bool predicate, string message)
      {
        if (!predicate)
          throw new SchemaException(message);
      }

      public static void IsValidIdentifier(string identifier, string label)
      {
        if (string.IsNullOrWhiteSpace(identifier))
          throw new SchemaException(label + " must be a valid identifier: " + identifier);
      }

      public static void IsValidSchemaId(SchemaId id, string label)
      {
        if (id == SchemaId.Invalid)
          throw new SchemaException(label + " cannot be 0");
      }

      public static void DuplicateCheck<TKey, TValue>(
        TKey key,
        TValue value,
        Dictionary<TKey, TValue> scope,
        string label,
        string scopeLabel)
      {
        if (scope.ContainsKey(key))
          throw new SchemaException(string.Format("{0} must be unique within a {1}: {2}", (object) label, (object) scopeLabel, (object) key));
        scope.Add(key, value);
      }

      public static TValue Exists<TKey, TValue>(
        TKey key,
        Dictionary<TKey, TValue> scope,
        string label,
        string scopeLabel)
      {
        TValue obj;
        if (!scope.TryGetValue(key, out obj))
          throw new SchemaException(string.Format("{0} must exist within a {1}: {2}", (object) label, (object) scopeLabel, (object) key));
        return obj;
      }

      public static void NotExists<TKey>(
        TKey key,
        List<TKey> scope,
        string label,
        string scopeLabel)
      {
        if (scope.Contains(key))
          throw new SchemaException(string.Format("{0} must not exist within a {1}: {2}", (object) label, (object) scopeLabel, (object) key));
      }

      public static void IsValidEnumSize(
        long minValue,
        long maxValue,
        EnumValue value,
        EnumSchema es)
      {
        if (value.Value < minValue || value.Value > maxValue)
          throw new SchemaException(string.Format("{0} ({1}) cannot fit in {2} of type {3}.", (object) value.Name, (object) value.Value, (object) es.Name, (object) es.Type));
      }

      public static void IsValidEnumUSize(
        ulong minValue,
        ulong maxValue,
        EnumValue value,
        EnumSchema es)
      {
        ulong num = (ulong) value.Value;
        if (num < minValue || num > maxValue)
          throw new SchemaException(string.Format("{0} ({1}) cannot fit in {2} of type {3}.", (object) value.Name, (object) value.Value, (object) es.Name, (object) es.Type));
      }
    }
  }
}

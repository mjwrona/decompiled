// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.SchemaHash
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Internal;
using System;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  public static class SchemaHash
  {
    public static (ulong low, ulong high) ComputeHash(
      Namespace ns,
      Schema schema,
      (ulong low, ulong high) seed = default ((ulong low, ulong high)))
    {
      (ulong low, ulong high) seed1 = seed;
      SchemaLanguageVersion effectiveSdlVersion = schema.GetEffectiveSdlVersion(ns);
      (ulong, ulong) seed2 = MurmurHash3.Hash128<SchemaId>(schema.SchemaId, seed1);
      (ulong low, ulong high) hash1 = SchemaHash.ComputeHash(effectiveSdlVersion, ns, schema.Type, seed2);
      (ulong, ulong) seed3 = MurmurHash3.Hash128<SchemaLanguageVersion>(effectiveSdlVersion, hash1);
      (ulong, ulong) hash2 = SchemaHash.ComputeHash(effectiveSdlVersion, schema.Options, seed3);
      (ulong, ulong) seed4 = MurmurHash3.Hash128<int>(schema.PartitionKeys.Count, hash2);
      foreach (PartitionKey partitionKey in schema.PartitionKeys)
        seed4 = SchemaHash.ComputeHash(effectiveSdlVersion, ns, partitionKey, seed4);
      (ulong, ulong) seed5 = MurmurHash3.Hash128<int>(schema.PrimaryKeys.Count, seed4);
      foreach (PrimarySortKey primaryKey in schema.PrimaryKeys)
        seed5 = SchemaHash.ComputeHash(effectiveSdlVersion, ns, primaryKey, seed5);
      (ulong, ulong) seed6 = MurmurHash3.Hash128<int>(schema.StaticKeys.Count, seed5);
      foreach (StaticKey staticKey in schema.StaticKeys)
        seed6 = SchemaHash.ComputeHash(effectiveSdlVersion, ns, staticKey, seed6);
      (ulong, ulong) seed7 = MurmurHash3.Hash128<int>(schema.Properties.Count, seed6);
      foreach (Property property in schema.Properties)
        seed7 = SchemaHash.ComputeHash(effectiveSdlVersion, ns, property, seed7);
      return seed7;
    }

    private static (ulong low, ulong high) ComputeHash(
      SchemaLanguageVersion v,
      SchemaOptions options,
      (ulong low, ulong high) seed = default ((ulong low, ulong high)))
    {
      (ulong, ulong) seed1 = seed;
      (ulong, ulong) seed2 = MurmurHash3.Hash128(options != null && options.DisallowUnschematized, seed1);
      (ulong, ulong) seed3 = MurmurHash3.Hash128(options != null && options.EnablePropertyLevelTimestamp, seed2);
      return MurmurHash3.Hash128(options != null && options.DisableSystemPrefix, seed3);
    }

    private static (ulong low, ulong high) ComputeHash(
      SchemaLanguageVersion v,
      Namespace ns,
      Property p,
      (ulong low, ulong high) seed = default ((ulong low, ulong high)))
    {
      Contract.Requires(p != null);
      (ulong, ulong) seed1 = seed;
      (ulong, ulong) seed2 = MurmurHash3.Hash128(p.Path, seed1);
      return SchemaHash.ComputeHash(v, ns, p.PropertyType, seed2);
    }

    private static (ulong low, ulong high) ComputeHash(
      SchemaLanguageVersion v,
      Namespace ns,
      PropertyType p,
      (ulong low, ulong high) seed = default ((ulong low, ulong high)))
    {
      Contract.Requires(p != null);
      (ulong, ulong) seed1 = seed;
      (ulong, ulong) hash = SchemaHash.ComputeHash(v, ns, p.Type, seed1);
      (ulong, ulong) seed2 = MurmurHash3.Hash128(p.Nullable, hash);
      if (p.ApiType != null)
        seed2 = MurmurHash3.Hash128(p.ApiType, seed2);
      PrimitivePropertyType primitivePropertyType = p as PrimitivePropertyType;
      if (primitivePropertyType == null)
      {
        if (p is ScopePropertyType scopePropertyType)
        {
          seed2 = MurmurHash3.Hash128(scopePropertyType.Immutable, seed2);
          if (!(p is ArrayPropertyType arrayPropertyType))
          {
            if (!(p is ObjectPropertyType objectPropertyType))
            {
              if (!(p is MapPropertyType mapPropertyType))
              {
                if (!(p is SetPropertyType setPropertyType))
                {
                  if (!(p is TaggedPropertyType taggedPropertyType))
                  {
                    if (!(p is TuplePropertyType tuplePropertyType))
                    {
                      UdtPropertyType udtPropertyType = p as UdtPropertyType;
                      if (udtPropertyType != null)
                      {
                        Schema schema;
                        if (udtPropertyType.SchemaId == SchemaId.Invalid)
                        {
                          schema = ns.Schemas.Find((Predicate<Schema>) (s => s.Name == udtPropertyType.Name));
                        }
                        else
                        {
                          schema = ns.Schemas.Find((Predicate<Schema>) (s => s.SchemaId == udtPropertyType.SchemaId));
                          if (schema.Name != udtPropertyType.Name)
                            throw new SchemaException(string.Format("Ambiguous schema reference: '{0}:{1}'", (object) udtPropertyType.Name, (object) udtPropertyType.SchemaId));
                        }
                        if (schema == null)
                          throw new SchemaException(string.Format("Cannot resolve schema reference '{0}:{1}'", (object) udtPropertyType.Name, (object) udtPropertyType.SchemaId));
                        seed2 = SchemaHash.ComputeHash(ns, schema, seed2);
                      }
                    }
                    else if (tuplePropertyType.Items != null)
                    {
                      foreach (PropertyType p1 in tuplePropertyType.Items)
                        seed2 = SchemaHash.ComputeHash(v, ns, p1, seed2);
                    }
                  }
                  else if (taggedPropertyType.Items != null)
                  {
                    foreach (PropertyType p2 in taggedPropertyType.Items)
                      seed2 = SchemaHash.ComputeHash(v, ns, p2, seed2);
                  }
                }
                else if (setPropertyType.Items != null)
                  seed2 = SchemaHash.ComputeHash(v, ns, setPropertyType.Items, seed2);
              }
              else
              {
                if (mapPropertyType.Keys != null)
                  seed2 = SchemaHash.ComputeHash(v, ns, mapPropertyType.Keys, seed2);
                if (mapPropertyType.Values != null)
                  seed2 = SchemaHash.ComputeHash(v, ns, mapPropertyType.Values, seed2);
              }
            }
            else if (objectPropertyType.Properties != null)
            {
              foreach (Property property in objectPropertyType.Properties)
                seed2 = SchemaHash.ComputeHash(v, ns, property, seed2);
            }
          }
          else if (arrayPropertyType.Items != null)
            seed2 = SchemaHash.ComputeHash(v, ns, arrayPropertyType.Items, seed2);
        }
      }
      else
      {
        (ulong, ulong) seed3 = MurmurHash3.Hash128<StorageKind>(primitivePropertyType.Storage, seed2);
        seed2 = MurmurHash3.Hash128<int>(primitivePropertyType.Length, seed3);
        if (primitivePropertyType.Type == TypeKind.Enum)
        {
          if (v < SchemaLanguageVersion.V2)
            throw new SchemaException(string.Format("Enums require SDL v2 or higher: {0}.", (object) v));
          seed2 = SchemaHash.ComputeHash(v, ns, ns.Enums.Find((Predicate<EnumSchema>) (es => es.Name == primitivePropertyType.Enum)) ?? throw new SchemaException("Cannot resolve enum schema reference '" + primitivePropertyType.Enum + "'"), seed2);
        }
      }
      return seed2;
    }

    private static (ulong low, ulong high) ComputeHash(
      SchemaLanguageVersion v,
      Namespace ns,
      PartitionKey key,
      (ulong low, ulong high) seed = default ((ulong low, ulong high)))
    {
      (ulong, ulong) seed1 = seed;
      if (key != null)
        seed1 = MurmurHash3.Hash128(key.Path, seed1);
      return seed1;
    }

    private static (ulong low, ulong high) ComputeHash(
      SchemaLanguageVersion v,
      Namespace ns,
      PrimarySortKey key,
      (ulong low, ulong high) seed = default ((ulong low, ulong high)))
    {
      (ulong, ulong) seed1 = seed;
      if (key != null)
      {
        (ulong, ulong) seed2 = MurmurHash3.Hash128(key.Path, seed1);
        seed1 = SchemaHash.ComputeHash(v, ns, key.Direction, seed2);
      }
      return seed1;
    }

    private static (ulong low, ulong high) ComputeHash(
      SchemaLanguageVersion v,
      Namespace ns,
      StaticKey key,
      (ulong low, ulong high) seed = default ((ulong low, ulong high)))
    {
      (ulong, ulong) seed1 = seed;
      if (key != null)
        seed1 = MurmurHash3.Hash128(key.Path, seed1);
      return seed1;
    }

    private static (ulong low, ulong high) ComputeHash(
      SchemaLanguageVersion v,
      Namespace ns,
      EnumSchema es,
      (ulong low, ulong high) seed = default ((ulong low, ulong high)))
    {
      (ulong, ulong) seed1 = seed;
      (ulong, ulong) hash = SchemaHash.ComputeHash(v, ns, es.Type, seed1);
      (ulong, ulong) seed2 = MurmurHash3.Hash128<int>(es.Values.Count, hash);
      foreach (EnumValue ev in es.Values)
        seed2 = SchemaHash.ComputeHash(v, ns, ev, seed2);
      return seed2;
    }

    private static (ulong low, ulong high) ComputeHash(
      SchemaLanguageVersion v,
      Namespace ns,
      EnumValue ev,
      (ulong low, ulong high) seed = default ((ulong low, ulong high)))
    {
      (ulong, ulong) seed1 = seed;
      return MurmurHash3.Hash128<long>(ev.Value, seed1);
    }

    private static (ulong low, ulong high) ComputeHash(
      SchemaLanguageVersion v,
      Namespace ns,
      TypeKind type,
      (ulong low, ulong high) seed = default ((ulong low, ulong high)))
    {
      return MurmurHash3.Hash128<int>((int) type, seed);
    }

    private static (ulong low, ulong high) ComputeHash(
      SchemaLanguageVersion v,
      Namespace ns,
      SortDirection direction,
      (ulong low, ulong high) seed = default ((ulong low, ulong high)))
    {
      return MurmurHash3.Hash128<int>((int) direction, seed);
    }
  }
}

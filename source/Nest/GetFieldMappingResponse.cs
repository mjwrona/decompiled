// Decompiled with JetBrains decompiler
// Type: Nest.GetFieldMappingResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Nest
{
  [JsonFormatter(typeof (ResolvableDictionaryResponseFormatter<GetFieldMappingResponse, IndexName, TypeFieldMappings>))]
  public class GetFieldMappingResponse : DictionaryResponseBase<IndexName, TypeFieldMappings>
  {
    [IgnoreDataMember]
    public IReadOnlyDictionary<IndexName, TypeFieldMappings> Indices => this.Self.BackingDictionary;

    public override bool IsValid => base.IsValid && this.Indices.HasAny<KeyValuePair<IndexName, TypeFieldMappings>>();

    public IFieldMapping GetMapping(IndexName index, Field property)
    {
      if (property == (Field) null)
        return (IFieldMapping) null;
      IReadOnlyDictionary<Field, FieldMapping> readOnlyDictionary = this.MappingsFor(index);
      if (readOnlyDictionary == null)
        return (IFieldMapping) null;
      FieldMapping fieldMapping1;
      if (!readOnlyDictionary.TryGetValue(property, out fieldMapping1) || fieldMapping1.Mapping == null)
        return (IFieldMapping) null;
      IFieldMapping fieldMapping2;
      return !fieldMapping1.Mapping.TryGetValue(property, out fieldMapping2) ? (IFieldMapping) null : fieldMapping2;
    }

    public IFieldMapping MappingFor<T>(Field property) => this.MappingFor<T>(property, (IndexName) null);

    public IFieldMapping MappingFor<T>(Field property, IndexName index)
    {
      IndexName index1 = index;
      if ((object) index1 == null)
        index1 = Infer.Index<T>();
      return this.GetMapping(index1, property);
    }

    public IFieldMapping MappingFor<T, TValue>(
      Expression<Func<T, TValue>> objectPath,
      IndexName index = null)
      where T : class
    {
      IndexName index1 = index;
      if ((object) index1 == null)
        index1 = Infer.Index<T>();
      return this.GetMapping(index1, Infer.Field<T, TValue>(objectPath));
    }

    public IFieldMapping MappingFor<T>(Expression<Func<T, object>> objectPath, IndexName index = null) where T : class
    {
      IndexName index1 = index;
      if ((object) index1 == null)
        index1 = Infer.Index<T>();
      return this.GetMapping(index1, Infer.Field<T>(objectPath));
    }

    private IReadOnlyDictionary<Field, FieldMapping> MappingsFor(IndexName index)
    {
      TypeFieldMappings typeFieldMappings;
      return !this.Indices.TryGetValue(index, out typeFieldMappings) || typeFieldMappings.Mappings == null ? (IReadOnlyDictionary<Field, FieldMapping>) null : typeFieldMappings.Mappings;
    }
  }
}

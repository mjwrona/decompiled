// Decompiled with JetBrains decompiler
// Type: Nest.FieldMappingFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class FieldMappingFormatter : 
    IJsonFormatter<IReadOnlyDictionary<Field, IFieldMapping>>,
    IJsonFormatter
  {
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "_all",
        0
      },
      {
        "_source",
        1
      },
      {
        "_routing",
        2
      },
      {
        "_index",
        3
      },
      {
        "_size",
        4
      }
    };

    public IReadOnlyDictionary<Field, IFieldMapping> Deserialize(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      Dictionary<Field, IFieldMapping> backingDictionary = new Dictionary<Field, IFieldMapping>();
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNext();
        return (IReadOnlyDictionary<Field, IFieldMapping>) backingDictionary;
      }
      int count = 0;
      IFieldMapping fieldMapping = (IFieldMapping) null;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (FieldMappingFormatter.Fields.TryGetValue(segment, out num))
        {
          switch (num)
          {
            case 0:
              fieldMapping = (IFieldMapping) formatterResolver.GetFormatter<AllField>().Deserialize(ref reader, formatterResolver);
              break;
            case 1:
              fieldMapping = (IFieldMapping) formatterResolver.GetFormatter<SourceField>().Deserialize(ref reader, formatterResolver);
              break;
            case 2:
              fieldMapping = (IFieldMapping) formatterResolver.GetFormatter<RoutingField>().Deserialize(ref reader, formatterResolver);
              break;
            case 3:
              fieldMapping = (IFieldMapping) formatterResolver.GetFormatter<IndexField>().Deserialize(ref reader, formatterResolver);
              break;
            case 4:
              fieldMapping = (IFieldMapping) formatterResolver.GetFormatter<SizeField>().Deserialize(ref reader, formatterResolver);
              break;
          }
        }
        else
        {
          IProperty property = formatterResolver.GetFormatter<IProperty>().Deserialize(ref reader, formatterResolver);
          if (property != null)
            property.Name = (PropertyName) segment.Utf8String();
          fieldMapping = (IFieldMapping) property;
        }
        if (fieldMapping != null)
        {
          string key = segment.Utf8String();
          backingDictionary.Add((Field) key, fieldMapping);
        }
      }
      return (IReadOnlyDictionary<Field, IFieldMapping>) new ResolvableDictionaryProxy<Field, IFieldMapping>((IConnectionConfigurationValues) formatterResolver.GetConnectionSettings(), (IReadOnlyDictionary<Field, IFieldMapping>) backingDictionary);
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      IReadOnlyDictionary<Field, IFieldMapping> value,
      IJsonFormatterResolver formatterResolver)
    {
      DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<IReadOnlyDictionary<Field, IFieldMapping>>().Serialize(ref writer, value, formatterResolver);
    }
  }
}

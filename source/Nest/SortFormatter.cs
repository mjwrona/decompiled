// Decompiled with JetBrains decompiler
// Type: Nest.SortFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class SortFormatter : IJsonFormatter<ISort>, IJsonFormatter
  {
    private static readonly AutomataDictionary SortFields = new AutomataDictionary()
    {
      {
        "_geo_distance",
        0
      },
      {
        "_script",
        1
      }
    };

    public ISort Deserialize(ref Elasticsearch.Net.Utf8Json.JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      ISort sort = (ISort) null;
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.BeginObject:
          int count1 = 0;
          while (reader.ReadIsInObject(ref count1))
          {
            ArraySegment<byte> segment1 = reader.ReadPropertyNameSegmentRaw();
            int num;
            if (SortFormatter.SortFields.TryGetValue(segment1, out num))
            {
              switch (num)
              {
                case 0:
                  int count2 = 0;
                  string str1 = (string) null;
                  ArraySegment<byte> arraySegment = reader.ReadNextBlockSegment();
                  Elasticsearch.Net.Utf8Json.JsonReader reader1 = new Elasticsearch.Net.Utf8Json.JsonReader(arraySegment.Array, arraySegment.Offset);
                  IEnumerable<GeoLocation> geoLocations = (IEnumerable<GeoLocation>) null;
                  while (reader1.ReadIsInObject(ref count2))
                  {
                    ArraySegment<byte> segment2 = reader1.ReadPropertyNameSegmentRaw();
                    if (reader1.GetCurrentJsonToken() == JsonToken.BeginArray)
                    {
                      str1 = segment2.Utf8String();
                      geoLocations = formatterResolver.GetFormatter<IEnumerable<GeoLocation>>().Deserialize(ref reader1, formatterResolver);
                      break;
                    }
                    reader1.ReadNextBlock();
                  }
                  reader1 = new Elasticsearch.Net.Utf8Json.JsonReader(arraySegment.Array, arraySegment.Offset);
                  GeoDistanceSort geoDistanceSort = formatterResolver.GetFormatter<GeoDistanceSort>().Deserialize(ref reader1, formatterResolver);
                  geoDistanceSort.Field = (Field) str1;
                  geoDistanceSort.Points = geoLocations;
                  sort = (ISort) geoDistanceSort;
                  continue;
                case 1:
                  sort = (ISort) formatterResolver.GetFormatter<ScriptSort>().Deserialize(ref reader, formatterResolver);
                  continue;
                default:
                  continue;
              }
            }
            else
            {
              string str2 = segment1.Utf8String();
              FieldSort fieldSort1;
              if (reader.GetCurrentJsonToken() == JsonToken.String)
              {
                SortOrder sortOrder = formatterResolver.GetFormatter<SortOrder>().Deserialize(ref reader, formatterResolver);
                FieldSort fieldSort2 = new FieldSort();
                fieldSort2.Field = (Field) str2;
                fieldSort2.Order = new SortOrder?(sortOrder);
                fieldSort1 = fieldSort2;
              }
              else
              {
                fieldSort1 = formatterResolver.GetFormatter<FieldSort>().Deserialize(ref reader, formatterResolver);
                fieldSort1.Field = (Field) str2;
              }
              sort = (ISort) fieldSort1;
            }
          }
          break;
        case JsonToken.String:
          string str = reader.ReadString();
          sort = (ISort) new FieldSort()
          {
            Field = (Field) str
          };
          break;
        default:
          throw new JsonParsingException(string.Format("Cannot deserialize {0} from {1}", (object) "ISort", (object) reader.GetCurrentJsonToken()));
      }
      return sort;
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      ISort value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value?.SortKey == (Field) null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
        switch (value.SortKey.Name ?? string.Empty)
        {
          case "_script":
            writer.WritePropertyName("_script");
            IScriptSort scriptSort = (IScriptSort) value;
            formatterResolver.GetFormatter<IScriptSort>().Serialize(ref writer, scriptSort, formatterResolver);
            break;
          case "_geo_distance":
            IGeoDistanceSort geoDistanceSort = value as IGeoDistanceSort;
            writer.WritePropertyName(geoDistanceSort.SortKey.Name);
            Elasticsearch.Net.Utf8Json.JsonWriter writer1 = new Elasticsearch.Net.Utf8Json.JsonWriter();
            DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<IGeoDistanceSort>().Serialize(ref writer1, geoDistanceSort, formatterResolver);
            ArraySegment<byte> buffer = writer1.GetBuffer();
            for (int offset = buffer.Offset; offset < buffer.Count - 1; ++offset)
              writer.WriteRawUnsafe(buffer.Array[offset]);
            if (buffer.Count > 2)
              writer.WriteValueSeparator();
            writer.WritePropertyName(connectionSettings.Inferrer.Field(geoDistanceSort.Field));
            formatterResolver.GetFormatter<IEnumerable<GeoLocation>>().Serialize(ref writer, geoDistanceSort.Points, formatterResolver);
            writer.WriteEndObject();
            break;
          default:
            writer.WritePropertyName(connectionSettings.Inferrer.Field(value.SortKey));
            DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<IFieldSort>().Serialize(ref writer, value as IFieldSort, formatterResolver);
            break;
        }
        writer.WriteEndObject();
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.FieldNameQueryFormatter`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Extensions;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class FieldNameQueryFormatter<T, TInterface> : ReadAsFormatter<T, TInterface>
    where T : class, TInterface, IFieldNameQuery, new()
    where TInterface : class, IFieldNameQuery
  {
    public override void Serialize(
      ref JsonWriter writer,
      TInterface value,
      IJsonFormatterResolver formatterResolver)
    {
      if ((object) value == null)
      {
        writer.WriteNull();
      }
      else
      {
        Field field = value.Field;
        if (field == (Field) null)
          return;
        string propertyName = formatterResolver.GetConnectionSettings().Inferrer.Field(field);
        if (propertyName.IsNullOrEmpty())
          return;
        writer.WriteBeginObject();
        writer.WritePropertyName(propertyName);
        base.Serialize(ref writer, value, formatterResolver);
        writer.WriteEndObject();
      }
    }

    public override TInterface Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      reader.ReadIsBeginObjectWithVerify();
      if (reader.ReadIsEndObject())
        return default (TInterface);
      TInterface @interface = default (TInterface);
      string str = reader.ReadPropertyName();
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.BeginObject:
          @interface = base.Deserialize(ref reader, formatterResolver);
          reader.ReadIsEndObjectWithVerify();
          break;
        case JsonToken.Null:
          reader.ReadNext();
          break;
        default:
          @interface = (TInterface) new T();
          switch (@interface)
          {
            case ITermQuery termQuery:
              switch (currentJsonToken)
              {
                case JsonToken.Number:
                  ArraySegment<byte> arraySegment = reader.ReadNumberSegment();
                  termQuery.Value = !arraySegment.IsLong() ? (object) NumberConverter.ReadDouble(arraySegment.Array, arraySegment.Offset, out int _) : (object) NumberConverter.ReadInt64(arraySegment.Array, arraySegment.Offset, out int _);
                  break;
                case JsonToken.String:
                  termQuery.Value = (object) reader.ReadString();
                  break;
                case JsonToken.True:
                case JsonToken.False:
                  termQuery.Value = (object) reader.ReadBoolean();
                  break;
              }
              reader.ReadIsEndObjectWithVerify();
              break;
            case IMatchQuery matchQuery:
              matchQuery.Query = reader.ReadString();
              reader.ReadIsEndObjectWithVerify();
              break;
            case IMatchPhraseQuery matchPhraseQuery:
              matchPhraseQuery.Query = reader.ReadString();
              reader.ReadIsEndObjectWithVerify();
              break;
            case IMatchPhrasePrefixQuery phrasePrefixQuery:
              phrasePrefixQuery.Query = reader.ReadString();
              reader.ReadIsEndObjectWithVerify();
              break;
            case IMatchBoolPrefixQuery matchBoolPrefixQuery:
              matchBoolPrefixQuery.Query = reader.ReadString();
              reader.ReadIsEndObjectWithVerify();
              break;
          }
          break;
      }
      if ((object) @interface == null)
        return default (TInterface);
      @interface.Field = (Field) str;
      return @interface;
    }
  }
}

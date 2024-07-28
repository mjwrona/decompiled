// Decompiled with JetBrains decompiler
// Type: Nest.TermsQueryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class TermsQueryFormatter : IJsonFormatter<ITermsQuery>, IJsonFormatter
  {
    private static readonly AutomataDictionary FieldLookups = new AutomataDictionary()
    {
      {
        "id",
        0
      },
      {
        "index",
        1
      },
      {
        "path",
        2
      },
      {
        "routing",
        3
      }
    };
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "boost",
        0
      },
      {
        "_name",
        1
      }
    };
    private static readonly Nest.SourceWriteFormatter<object> SourceWriteFormatter = new Nest.SourceWriteFormatter<object>();

    public ITermsQuery Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return (ITermsQuery) null;
      }
      ITermsQuery termsQuery = (ITermsQuery) new TermsQuery();
      int count = 0;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (TermsQueryFormatter.Fields.TryGetValue(segment, out num))
        {
          switch (num)
          {
            case 0:
              termsQuery.Boost = new double?(reader.ReadDouble());
              continue;
            case 1:
              termsQuery.Name = reader.ReadString();
              continue;
            default:
              continue;
          }
        }
        else
        {
          termsQuery.Field = (Field) segment.Utf8String();
          this.ReadTerms(ref reader, termsQuery, formatterResolver);
        }
      }
      return termsQuery;
    }

    public void Serialize(
      ref JsonWriter writer,
      ITermsQuery value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        string propertyName = formatterResolver.GetConnectionSettings().Inferrer.Field(value.Field);
        bool flag = false;
        writer.WriteBeginObject();
        if (!value.Name.IsNullOrEmpty())
        {
          writer.WritePropertyName("_name");
          writer.WriteString(value.Name);
          flag = true;
        }
        double? boost = value.Boost;
        if (boost.HasValue)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("boost");
          ref JsonWriter local = ref writer;
          boost = value.Boost;
          double num = boost.Value;
          local.WriteDouble(num);
          flag = true;
        }
        if (flag)
          writer.WriteValueSeparator();
        if (value.IsVerbatim)
        {
          if (value.TermsLookup != null)
          {
            writer.WritePropertyName(propertyName);
            formatterResolver.GetFormatter<IFieldLookup>().Serialize(ref writer, value.TermsLookup, formatterResolver);
          }
          else if (value.Terms != null)
          {
            writer.WritePropertyName(propertyName);
            writer.WriteBeginArray();
            int num = 0;
            foreach (object term in value.Terms)
            {
              if (num > 0)
                writer.WriteValueSeparator();
              TermsQueryFormatter.SourceWriteFormatter.Serialize(ref writer, term, formatterResolver);
              ++num;
            }
            writer.WriteEndArray();
          }
        }
        else if (value.Terms.HasAny<object>())
        {
          writer.WritePropertyName(propertyName);
          writer.WriteBeginArray();
          int num = 0;
          foreach (object term in value.Terms)
          {
            if (num > 0)
              writer.WriteValueSeparator();
            TermsQueryFormatter.SourceWriteFormatter.Serialize(ref writer, term, formatterResolver);
            ++num;
          }
          writer.WriteEndArray();
        }
        else if (value.TermsLookup != null)
        {
          writer.WritePropertyName(propertyName);
          formatterResolver.GetFormatter<IFieldLookup>().Serialize(ref writer, value.TermsLookup, formatterResolver);
        }
        writer.WriteEndObject();
      }
    }

    private void ReadTerms(
      ref JsonReader reader,
      ITermsQuery termsQuery,
      IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.BeginObject:
          FieldLookup fieldLookup = new FieldLookup();
          int count = 0;
          while (reader.ReadIsInObject(ref count))
          {
            ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
            int num;
            if (TermsQueryFormatter.FieldLookups.TryGetValue(bytes, out num))
            {
              switch (num)
              {
                case 0:
                  fieldLookup.Id = formatterResolver.GetFormatter<Id>().Deserialize(ref reader, formatterResolver);
                  continue;
                case 1:
                  fieldLookup.Index = formatterResolver.GetFormatter<IndexName>().Deserialize(ref reader, formatterResolver);
                  continue;
                case 2:
                  fieldLookup.Path = formatterResolver.GetFormatter<Field>().Deserialize(ref reader, formatterResolver);
                  continue;
                case 3:
                  fieldLookup.Routing = formatterResolver.GetFormatter<Routing>().Deserialize(ref reader, formatterResolver);
                  continue;
                default:
                  continue;
              }
            }
          }
          termsQuery.TermsLookup = (IFieldLookup) fieldLookup;
          break;
        case JsonToken.BeginArray:
          IEnumerable<object> objects = formatterResolver.GetFormatter<IEnumerable<object>>().Deserialize(ref reader, formatterResolver);
          termsQuery.Terms = objects;
          break;
      }
    }
  }
}

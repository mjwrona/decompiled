// Decompiled with JetBrains decompiler
// Type: Nest.TotalHitsFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Extensions;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Formatters;
using System;

namespace Nest
{
  internal class TotalHitsFormatter : IJsonFormatter<TotalHits>, IJsonFormatter
  {
    private static readonly byte[] ValueField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("value");
    private static readonly byte[] RelationField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("relation");
    private static readonly EnumFormatter<TotalHitsRelation> RelationFormatter = new EnumFormatter<TotalHitsRelation>(true);

    public TotalHits Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.BeginObject:
          int count = 0;
          long num = -1;
          TotalHitsRelation? nullable = new TotalHitsRelation?();
          while (reader.ReadIsInObject(ref count))
          {
            ArraySegment<byte> arraySegment = reader.ReadPropertyNameSegmentRaw();
            if (arraySegment.EqualsBytes(TotalHitsFormatter.ValueField))
              num = reader.ReadInt64();
            else if (arraySegment.EqualsBytes(TotalHitsFormatter.RelationField))
              nullable = new TotalHitsRelation?(TotalHitsFormatter.RelationFormatter.Deserialize(ref reader, formatterResolver));
            else
              reader.ReadNextBlock();
          }
          return new TotalHits()
          {
            Value = num,
            Relation = nullable
          };
        case JsonToken.Number:
          return new TotalHits()
          {
            Value = reader.ReadInt64()
          };
        default:
          reader.ReadNextBlock();
          return (TotalHits) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      TotalHits value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        TotalHitsRelation? relation = value.Relation;
        if (relation.HasValue)
        {
          writer.WriteBeginObject();
          writer.WritePropertyName(nameof (value));
          writer.WriteInt64(value.Value);
          writer.WriteValueSeparator();
          writer.WritePropertyName("relation");
          EnumFormatter<TotalHitsRelation> relationFormatter = TotalHitsFormatter.RelationFormatter;
          ref JsonWriter local = ref writer;
          relation = value.Relation;
          int num = (int) relation.Value;
          IJsonFormatterResolver formatterResolver1 = formatterResolver;
          relationFormatter.Serialize(ref local, (TotalHitsRelation) num, formatterResolver1);
          writer.WriteEndObject();
        }
        else
          writer.WriteInt64(value.Value);
      }
    }
  }
}

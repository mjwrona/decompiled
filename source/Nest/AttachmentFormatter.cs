// Decompiled with JetBrains decompiler
// Type: Nest.AttachmentFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class AttachmentFormatter : IJsonFormatter<Attachment>, IJsonFormatter
  {
    private static readonly AutomataDictionary AutomataDictionary = new AutomataDictionary()
    {
      {
        "_content",
        0
      },
      {
        "content",
        0
      },
      {
        "_name",
        1
      },
      {
        "name",
        1
      },
      {
        "author",
        2
      },
      {
        "keywords",
        3
      },
      {
        "date",
        4
      },
      {
        "_content_type",
        5
      },
      {
        "content_type",
        5
      },
      {
        "_content_length",
        6
      },
      {
        "content_length",
        6
      },
      {
        "contentlength",
        6
      },
      {
        "_language",
        7
      },
      {
        "language",
        7
      },
      {
        "_detect_language",
        8
      },
      {
        "detect_language",
        8
      },
      {
        "_indexed_chars",
        9
      },
      {
        "indexed_chars",
        9
      },
      {
        "title",
        10
      }
    };

    public Attachment Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.BeginObject:
          Attachment attachment = new Attachment();
          int count = 0;
          while (reader.ReadIsInObject(ref count))
          {
            ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
            int num;
            if (AttachmentFormatter.AutomataDictionary.TryGetValue(bytes, out num))
            {
              switch (num)
              {
                case 0:
                  attachment.Content = reader.ReadString();
                  continue;
                case 1:
                  attachment.Name = reader.ReadString();
                  continue;
                case 2:
                  attachment.Author = reader.ReadString();
                  continue;
                case 3:
                  attachment.Keywords = reader.ReadString();
                  continue;
                case 4:
                  attachment.Date = formatterResolver.GetFormatter<DateTime?>().Deserialize(ref reader, formatterResolver);
                  continue;
                case 5:
                  attachment.ContentType = reader.ReadString();
                  continue;
                case 6:
                  attachment.ContentLength = reader.ReadNullableLong();
                  continue;
                case 7:
                  attachment.Language = reader.ReadString();
                  continue;
                case 8:
                  attachment.DetectLanguage = reader.ReadNullableBoolean();
                  continue;
                case 9:
                  attachment.IndexedCharacters = reader.ReadNullableLong();
                  continue;
                case 10:
                  attachment.Title = reader.ReadString();
                  continue;
                default:
                  continue;
              }
            }
          }
          return attachment;
        case JsonToken.String:
          return new Attachment()
          {
            Content = reader.ReadString()
          };
        default:
          return (Attachment) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      Attachment value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else if (!value.ContainsMetadata)
      {
        writer.WriteString(value.Content);
      }
      else
      {
        writer.WriteBeginObject();
        writer.WritePropertyName("content");
        writer.WriteString(value.Content);
        if (!string.IsNullOrEmpty(value.Author))
        {
          writer.WriteValueSeparator();
          writer.WritePropertyName("author");
          writer.WriteString(value.Author);
        }
        long? nullable = value.ContentLength;
        if (nullable.HasValue)
        {
          writer.WriteValueSeparator();
          writer.WritePropertyName("content_length");
          ref JsonWriter local = ref writer;
          nullable = value.ContentLength;
          long num = nullable.Value;
          local.WriteInt64(num);
        }
        if (!string.IsNullOrEmpty(value.ContentType))
        {
          writer.WriteValueSeparator();
          writer.WritePropertyName("content_type");
          writer.WriteString(value.ContentType);
        }
        if (value.Date.HasValue)
        {
          writer.WriteValueSeparator();
          writer.WritePropertyName("date");
          formatterResolver.GetFormatter<DateTime?>().Serialize(ref writer, value.Date, formatterResolver);
        }
        bool? detectLanguage = value.DetectLanguage;
        if (detectLanguage.HasValue)
        {
          writer.WriteValueSeparator();
          writer.WritePropertyName("detect_language");
          ref JsonWriter local = ref writer;
          detectLanguage = value.DetectLanguage;
          int num = detectLanguage.Value ? 1 : 0;
          local.WriteBoolean(num != 0);
        }
        nullable = value.IndexedCharacters;
        if (nullable.HasValue)
        {
          writer.WriteValueSeparator();
          writer.WritePropertyName("indexed_chars");
          ref JsonWriter local = ref writer;
          nullable = value.IndexedCharacters;
          long num = nullable.Value;
          local.WriteInt64(num);
        }
        if (!string.IsNullOrEmpty(value.Keywords))
        {
          writer.WriteValueSeparator();
          writer.WritePropertyName("keywords");
          writer.WriteString(value.Keywords);
        }
        if (!string.IsNullOrEmpty(value.Language))
        {
          writer.WriteValueSeparator();
          writer.WritePropertyName("language");
          writer.WriteString(value.Language);
        }
        if (!string.IsNullOrEmpty(value.Name))
        {
          writer.WriteValueSeparator();
          writer.WritePropertyName("name");
          writer.WriteString(value.Name);
        }
        if (!string.IsNullOrEmpty(value.Title))
        {
          writer.WriteValueSeparator();
          writer.WritePropertyName("title");
          writer.WriteString(value.Title);
        }
        writer.WriteEndObject();
      }
    }
  }
}

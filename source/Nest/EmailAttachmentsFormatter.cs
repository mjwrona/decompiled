// Decompiled with JetBrains decompiler
// Type: Nest.EmailAttachmentsFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class EmailAttachmentsFormatter : IJsonFormatter<IEmailAttachments>, IJsonFormatter
  {
    private static readonly AutomataDictionary AttachmentType = new AutomataDictionary()
    {
      {
        "http",
        0
      },
      {
        "data",
        1
      }
    };

    public IEmailAttachments Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (IEmailAttachments) null;
      Dictionary<string, IEmailAttachment> attachments = new Dictionary<string, IEmailAttachment>();
      int count1 = 0;
      while (reader.ReadIsInObject(ref count1))
      {
        string key = reader.ReadPropertyName();
        int count2 = 0;
        while (reader.ReadIsInObject(ref count2))
        {
          int num;
          if (EmailAttachmentsFormatter.AttachmentType.TryGetValue(reader.ReadPropertyNameSegmentRaw(), out num))
          {
            switch (num)
            {
              case 0:
                IEmailAttachment emailAttachment1 = (IEmailAttachment) formatterResolver.GetFormatter<HttpAttachment>().Deserialize(ref reader, formatterResolver);
                attachments.Add(key, emailAttachment1);
                continue;
              case 1:
                IEmailAttachment emailAttachment2 = (IEmailAttachment) formatterResolver.GetFormatter<DataAttachment>().Deserialize(ref reader, formatterResolver);
                attachments.Add(key, emailAttachment2);
                continue;
              default:
                continue;
            }
          }
        }
      }
      return (IEmailAttachments) new EmailAttachments((IDictionary<string, IEmailAttachment>) attachments);
    }

    public void Serialize(
      ref JsonWriter writer,
      IEmailAttachments value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WriteBeginObject();
      IDictionary<string, IEmailAttachment> dictionary = (IDictionary<string, IEmailAttachment>) value;
      if (dictionary != null)
      {
        int num = 0;
        foreach (KeyValuePair<string, IEmailAttachment> keyValuePair in (IEnumerable<KeyValuePair<string, IEmailAttachment>>) dictionary)
        {
          if (num > 0)
            writer.WriteValueSeparator();
          writer.WritePropertyName(keyValuePair.Key);
          writer.WriteBeginObject();
          IEmailAttachment emailAttachment = keyValuePair.Value;
          switch (emailAttachment)
          {
            case IHttpAttachment httpAttachment:
              writer.WritePropertyName("http");
              formatterResolver.GetFormatter<IHttpAttachment>().Serialize(ref writer, httpAttachment, formatterResolver);
              break;
            case IDataAttachment dataAttachment:
              writer.WritePropertyName("data");
              formatterResolver.GetFormatter<IDataAttachment>().Serialize(ref writer, dataAttachment, formatterResolver);
              break;
            default:
              throw new ArgumentException(emailAttachment.GetType().FullName + " is not a supported email attachment type");
          }
          writer.WriteEndObject();
          ++num;
        }
      }
      writer.WriteEndObject();
    }
  }
}

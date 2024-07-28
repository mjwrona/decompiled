// Decompiled with JetBrains decompiler
// Type: Nest.StartBasicLicenseFeatureAcknowledgementsFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Formatters;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nest
{
  internal class StartBasicLicenseFeatureAcknowledgementsFormatter : 
    IJsonFormatter<StartBasicLicenseFeatureAcknowledgements>,
    IJsonFormatter
  {
    private static readonly ArrayFormatter<string> StringArrayFormatter = new ArrayFormatter<string>();

    public StartBasicLicenseFeatureAcknowledgements Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return (StartBasicLicenseFeatureAcknowledgements) null;
      int count = 0;
      string str = (string) null;
      Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
      while (reader.ReadIsInObject(ref count))
      {
        string key = reader.ReadPropertyName();
        if (key == "message")
          str = reader.ReadString();
        else
          dictionary.Add(key, StartBasicLicenseFeatureAcknowledgementsFormatter.StringArrayFormatter.Deserialize(ref reader, formatterResolver));
      }
      return new StartBasicLicenseFeatureAcknowledgements((IDictionary<string, string[]>) dictionary)
      {
        Message = str
      };
    }

    public void Serialize(
      ref JsonWriter writer,
      StartBasicLicenseFeatureAcknowledgements value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        int num = 0;
        if (!string.IsNullOrEmpty(value.Message))
        {
          writer.WritePropertyName("message");
          writer.WriteString(value.Message);
          ++num;
        }
        foreach (KeyValuePair<string, string[]> keyValuePair in (ReadOnlyDictionary<string, string[]>) value)
        {
          if (num > 0)
            writer.WriteValueSeparator();
          writer.WritePropertyName(keyValuePair.Key);
          StartBasicLicenseFeatureAcknowledgementsFormatter.StringArrayFormatter.Serialize(ref writer, keyValuePair.Value, formatterResolver);
          ++num;
        }
        writer.WriteEndObject();
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.JsonNetCompatibleUriFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class JsonNetCompatibleUriFormatter : IJsonFormatter<Uri>, IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      Uri value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == (Uri) null)
        writer.WriteNull();
      else
        writer.WriteString(value.OriginalString);
    }

    public Uri Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.String:
          return new Uri(reader.ReadString(), UriKind.RelativeOrAbsolute);
        case JsonToken.Null:
          reader.ReadNext();
          return (Uri) null;
        default:
          throw new Exception(string.Format("Cannot deserialize {0} from {1}", (object) typeof (Uri).FullName, (object) currentJsonToken));
      }
    }
  }
}

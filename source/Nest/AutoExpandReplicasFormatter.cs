// Decompiled with JetBrains decompiler
// Type: Nest.AutoExpandReplicasFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class AutoExpandReplicasFormatter : IJsonFormatter<AutoExpandReplicas>, IJsonFormatter
  {
    public AutoExpandReplicas Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.String:
          return AutoExpandReplicas.Create(reader.ReadString());
        case JsonToken.False:
          return AutoExpandReplicas.Disabled;
        default:
          throw new Exception(string.Format("Cannot deserialize {0} from {1}", (object) typeof (AutoExpandReplicas), (object) currentJsonToken));
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      AutoExpandReplicas value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null || !value.Enabled)
        writer.WriteBoolean(false);
      else
        writer.WriteString(value.ToString());
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.DistanceFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class DistanceFormatter : IJsonFormatter<Distance>, IJsonFormatter
  {
    public Distance Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.String)
      {
        reader.ReadNextBlock();
        return (Distance) null;
      }
      string distanceUnit = reader.ReadString();
      return distanceUnit != null ? new Distance(distanceUnit) : (Distance) null;
    }

    public void Serialize(
      ref JsonWriter writer,
      Distance value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else
        writer.WriteString(value.ToString());
    }
  }
}

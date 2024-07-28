// Decompiled with JetBrains decompiler
// Type: Nest.GeoCoordinateFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class GeoCoordinateFormatter : IJsonFormatter<GeoCoordinate>, IJsonFormatter
  {
    public GeoCoordinate Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginArray)
        return (GeoCoordinate) null;
      double[] numArray = formatterResolver.GetFormatter<double[]>().Deserialize(ref reader, formatterResolver);
      switch (numArray.Length)
      {
        case 2:
          return new GeoCoordinate(numArray[1], numArray[0]);
        case 3:
          return new GeoCoordinate(numArray[1], numArray[0], numArray[2]);
        default:
          return (GeoCoordinate) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      GeoCoordinate value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginArray();
        writer.WriteDouble(value.Longitude);
        writer.WriteValueSeparator();
        writer.WriteDouble(value.Latitude);
        if (value.Z.HasValue)
        {
          writer.WriteValueSeparator();
          writer.WriteDouble(value.Z.Value);
        }
        writer.WriteEndArray();
      }
    }
  }
}

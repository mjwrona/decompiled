// Decompiled with JetBrains decompiler
// Type: Nest.NullableGeoOrientationFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class NullableGeoOrientationFormatter : IJsonFormatter<GeoOrientation?>, IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      GeoOrientation? value,
      IJsonFormatterResolver formatterResolver)
    {
      if (!value.HasValue)
      {
        writer.WriteNull();
      }
      else
      {
        if (!value.HasValue)
          return;
        switch (value.GetValueOrDefault())
        {
          case GeoOrientation.ClockWise:
            writer.WriteString("cw");
            break;
          case GeoOrientation.CounterClockWise:
            writer.WriteString("ccw");
            break;
        }
      }
    }

    public GeoOrientation? Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return new GeoOrientation?();
      switch (reader.ReadString().ToUpperInvariant())
      {
        case "LEFT":
        case "CW":
        case "CLOCKWISE":
          return new GeoOrientation?(GeoOrientation.ClockWise);
        case "RIGHT":
        case "CCW":
        case "COUNTERCLOCKWISE":
          return new GeoOrientation?(GeoOrientation.CounterClockWise);
        default:
          return new GeoOrientation?();
      }
    }
  }
}

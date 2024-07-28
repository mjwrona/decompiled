// Decompiled with JetBrains decompiler
// Type: Nest.GeoOrientationFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class GeoOrientationFormatter : IJsonFormatter<GeoOrientation>, IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      GeoOrientation value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value != GeoOrientation.ClockWise)
      {
        if (value != GeoOrientation.CounterClockWise)
          return;
        writer.WriteString("ccw");
      }
      else
        writer.WriteString("cw");
    }

    public GeoOrientation Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return GeoOrientation.CounterClockWise;
      string upperInvariant = reader.ReadString().ToUpperInvariant();
      return upperInvariant == "LEFT" || upperInvariant == "CW" || upperInvariant == "CLOCKWISE" ? GeoOrientation.ClockWise : GeoOrientation.CounterClockWise;
    }
  }
}

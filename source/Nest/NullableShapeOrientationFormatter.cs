// Decompiled with JetBrains decompiler
// Type: Nest.NullableShapeOrientationFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class NullableShapeOrientationFormatter : 
    IJsonFormatter<ShapeOrientation?>,
    IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      ShapeOrientation? value,
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
          case ShapeOrientation.ClockWise:
            writer.WriteString("clockwise");
            break;
          case ShapeOrientation.CounterClockWise:
            writer.WriteString("counterclockwise");
            break;
        }
      }
    }

    public ShapeOrientation? Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return new ShapeOrientation?();
      switch (reader.ReadString().ToUpperInvariant())
      {
        case "COUNTERCLOCKWISE":
        case "RIGHT":
        case "CCW":
          return new ShapeOrientation?(ShapeOrientation.CounterClockWise);
        case "CLOCKWISE":
        case "LEFT":
        case "CW":
          return new ShapeOrientation?(ShapeOrientation.ClockWise);
        default:
          return new ShapeOrientation?();
      }
    }
  }
}

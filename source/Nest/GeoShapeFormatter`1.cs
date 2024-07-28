// Decompiled with JetBrains decompiler
// Type: Nest.GeoShapeFormatter`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class GeoShapeFormatter<TShape> : IJsonFormatter<TShape>, IJsonFormatter where TShape : IGeoShape
  {
    public TShape Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) => (TShape) GeoShapeFormatter.Instance.Deserialize(ref reader, formatterResolver);

    public void Serialize(
      ref JsonWriter writer,
      TShape value,
      IJsonFormatterResolver formatterResolver)
    {
      GeoShapeFormatter.Instance.Serialize(ref writer, (IGeoShape) value, formatterResolver);
    }
  }
}

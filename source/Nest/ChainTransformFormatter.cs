// Decompiled with JetBrains decompiler
// Type: Nest.ChainTransformFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class ChainTransformFormatter : IJsonFormatter<IChainTransform>, IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      IChainTransform value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WriteBeginArray();
      if (value != null)
      {
        IJsonFormatter<TransformContainer> formatter = formatterResolver.GetFormatter<TransformContainer>();
        int num = 0;
        foreach (TransformContainer transform in (IEnumerable<TransformContainer>) value.Transforms)
        {
          if (num > 0)
            writer.WriteValueSeparator();
          formatter.Serialize(ref writer, transform, formatterResolver);
          ++num;
        }
      }
      writer.WriteEndArray();
    }

    public IChainTransform Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return reader.GetCurrentJsonToken() != JsonToken.BeginArray ? (IChainTransform) null : (IChainTransform) new ChainTransform((IEnumerable<TransformContainer>) formatterResolver.GetFormatter<ICollection<TransformContainer>>().Deserialize(ref reader, formatterResolver));
    }
  }
}
